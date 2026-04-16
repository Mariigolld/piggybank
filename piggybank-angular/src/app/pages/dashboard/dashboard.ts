import { Component, OnInit, inject, signal, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Sidebar } from '../../shared/sidebar/sidebar';
import { AuthService } from '../../services/auth';
import { BankAccountService } from '../../services/bank-account';
import { TransactionService } from '../../services/transaction';
import { BankAccount, Category, Transaction, TransactionType } from '../../models';

declare const Chart: any;

const COLORS = ['#7c3aed','#f97316','#10b981','#ef4444','#3b82f6','#ec4899','#f59e0b','#8b5cf6','#06b6d4','#84cc16'];

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, FormsModule, Sidebar],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit, AfterViewInit {
  private auth = inject(AuthService);
  private accountSvc = inject(BankAccountService);
  private txSvc = inject(TransactionService);

  user = this.auth.getUser();
  accounts = signal<BankAccount[]>([]);
  transactions = signal<Transaction[]>([]);
  categories = signal<Category[]>([]);

  totalBalance = signal(0);
  monthlyIncome = signal(0);
  monthlyExpenses = signal(0);
  categoryBreakdown = signal<{name:string;icon:string;amount:number;color:string}[]>([]);

  // Account modal
  showAccountModal = signal(false);
  editingAccountId = signal<number|null>(null);
  accName = ''; accBalance = 0; accCurrency = 'RSD'; accShared = false;
  accountError = signal('');

  // Transaction modal
  showTxModal = signal(false);
  txType = signal<TransactionType>(TransactionType.Expense);
  txAmount = 0; txAccountId = 0; txCategoryId: number|null = null;
  txDescription = ''; txDate = ''; txNotes = '';
  txError = signal('');

  // New category
  newCategoryName = ''; newCategoryIcon = '';

  // Confirm delete
  showConfirmModal = signal(false);
  confirmMessage = signal('');
  private confirmCallback: (() => void) | null = null;

  private activityChart: any;
  private paymentsChart: any;
  private balanceDonut: any;

  @ViewChild('activityCanvas') activityCanvas!: ElementRef;
  @ViewChild('paymentsCanvas') paymentsCanvas!: ElementRef;
  @ViewChild('donutCanvas') donutCanvas!: ElementRef;

  TransactionType = TransactionType;
  Math = Math;

  get initials() {
    return ((this.user?.firstName || '')[0] || '') + ((this.user?.lastName || '')[0] || '') || '?';
  }

  get monthlyNet() { return this.monthlyIncome() - this.monthlyExpenses(); }

  ngOnInit() {
    const today = new Date();
    this.txDate = today.toISOString().split('T')[0];
    this.loadAll();
  }

  ngAfterViewInit() {
    this.initCharts();
  }

  loadAll() {
    this.accountSvc.getAll().subscribe(list => {
      this.accounts.set(list);
      this.totalBalance.set(list.reduce((s, a) => s + a.balance, 0));
      this.loadMonthly();
    });
    this.txSvc.getCategories().subscribe(cats => this.categories.set(cats));
    this.txSvc.getRecent(20).subscribe(items => {
      const list = Array.isArray(items) ? items : (items as any)?.items || [];
      this.transactions.set(list);
    });
  }

  loadMonthly() {
    const now = new Date();
    const from = new Date(now.getFullYear(), now.getMonth(), 1).toISOString();
    const to = new Date(now.getFullYear(), now.getMonth() + 1, 1).toISOString();
    const qs = `from=${encodeURIComponent(from)}&to=${encodeURIComponent(to)}`;
    // Load monthly transactions via date range
    const http = (this.txSvc as any)['http'] as import('@angular/common/http').HttpClient;
    http
      .get<Transaction[]>(`http://localhost:5235/api/Transactions/by-date-range?${qs}`)
      .subscribe((txs: Transaction[]) => {
        const isIncome = (t: Transaction) => (t.type as any) === 1 || (t.type as any) === 'Income';
        const income = txs.filter(isIncome).reduce((s, t) => s + t.amount, 0);
        const expenses = txs.filter(t => !isIncome(t)).reduce((s, t) => s + t.amount, 0);
        this.monthlyIncome.set(income);
        this.monthlyExpenses.set(expenses);

        const byCat: Record<string, {amount:number;icon:string}> = {};
        txs.filter(t => !isIncome(t)).forEach(t => {
          const k = t.categoryName || 'Other';
          if (!byCat[k]) byCat[k] = { amount: 0, icon: t.categoryIcon || '📁' };
          byCat[k].amount += t.amount;
        });
        const breakdown = Object.entries(byCat)
          .sort((a,b) => b[1].amount - a[1].amount)
          .slice(0, 6)
          .map(([name, d], i) => ({ name, icon: d.icon, amount: d.amount, color: COLORS[i % COLORS.length] }));
        this.categoryBreakdown.set(breakdown);

        this.updateCharts(txs);
      });
  }

  // ─── Charts ────────────────────────────────────────────────────────────────
  initCharts() {
    if (!this.activityCanvas) return;
    this.activityChart = new Chart(this.activityCanvas.nativeElement, {
      type: 'line',
      data: { labels: [], datasets: [
        { label: 'Income', data: [], borderColor: '#10b981', backgroundColor: 'rgba(16,185,129,0.1)', fill: true, tension: 0.4, pointRadius: 0, pointHoverRadius: 6 },
        { label: 'Expenses', data: [], borderColor: '#ef4444', backgroundColor: 'rgba(239,68,68,0.1)', fill: true, tension: 0.4, pointRadius: 0, pointHoverRadius: 6 }
      ]},
      options: { responsive: true, maintainAspectRatio: false,
        plugins: { legend: { display: false } },
        scales: {
          x: { grid: { display: false }, ticks: { color: '#94a3b8', font: { size: 11 } } },
          y: { grid: { color: '#f1f5f9' }, ticks: { color: '#94a3b8', font: { size: 11 }, callback: (v: number) => this.shortNum(v) } }
        },
        interaction: { intersect: false, mode: 'index' }
      }
    });

    this.paymentsChart = new Chart(this.paymentsCanvas.nativeElement, {
      type: 'bar',
      data: { labels: [], datasets: [{ data: [], backgroundColor: COLORS, borderRadius: 6, barThickness: 24 }] },
      options: { responsive: true, maintainAspectRatio: false, indexAxis: 'y',
        plugins: { legend: { display: false } },
        scales: {
          x: { grid: { color: '#f1f5f9' }, ticks: { color: '#94a3b8', callback: (v: number) => this.shortNum(v) } },
          y: { grid: { display: false }, ticks: { color: '#64748b', font: { size: 12 } } }
        }
      }
    });

    this.balanceDonut = new Chart(this.donutCanvas.nativeElement, {
      type: 'doughnut',
      data: { labels: [], datasets: [{ data: [], backgroundColor: COLORS, borderWidth: 0 }] },
      options: { responsive: true, maintainAspectRatio: false, cutout: '75%',
        plugins: {
          legend: { display: false },
          tooltip: { callbacks: { label: (ctx: any) => ` ${ctx.label}: ${new Intl.NumberFormat('sr-RS').format(ctx.raw)}` } }
        }
      }
    });
  }

  updateCharts(txs: Transaction[]) {
    const isIncome = (t: Transaction) => (t.type as any) === 1 || (t.type as any) === 'Income';
    const now = new Date();
    const daysInMonth = new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();
    const daily: Record<number, {income:number;expenses:number}> = {};
    for (let i = 1; i <= daysInMonth; i++) daily[i] = { income: 0, expenses: 0 };
    txs.forEach(t => {
      const d = new Date(t.date).getDate();
      if (daily[d]) {
        if (isIncome(t)) daily[d].income += t.amount;
        else daily[d].expenses += t.amount;
      }
    });
    if (this.activityChart) {
      this.activityChart.data.labels = Object.keys(daily).map(d => `${d}.`);
      this.activityChart.data.datasets[0].data = Object.values(daily).map(d => d.income);
      this.activityChart.data.datasets[1].data = Object.values(daily).map(d => d.expenses);
      this.activityChart.update();
    }
    const top5 = this.categoryBreakdown().slice(0, 5);
    if (this.paymentsChart) {
      this.paymentsChart.data.labels = top5.map(c => c.name);
      this.paymentsChart.data.datasets[0].data = top5.map(c => c.amount);
      this.paymentsChart.update();
    }
    if (this.balanceDonut) {
      const accs = this.accounts();
      this.balanceDonut.data.labels = accs.map(a => a.name);
      this.balanceDonut.data.datasets[0].data = accs.map(a => a.balance);
      this.balanceDonut.data.datasets[0].backgroundColor = accs.map((_, i) => COLORS[i % COLORS.length]);
      this.balanceDonut.update();
    }
  }

  shortNum(n: number) {
    if (n >= 1000000) return (n/1000000).toFixed(1) + 'M';
    if (n >= 1000) return (n/1000).toFixed(0) + 'K';
    return n.toString();
  }

  fmt(n: number) { return new Intl.NumberFormat('sr-RS').format(n || 0); }

  fmtDate(d: string) {
    const dt = new Date(d);
    const months = ['jan','feb','mar','apr','maj','jun','jul','avg','sep','okt','nov','dec'];
    return `${dt.getDate()}. ${months[dt.getMonth()]}`;
  }

  // ─── Account Modal ─────────────────────────────────────────────────────────
  openAddAccount() {
    this.editingAccountId.set(null);
    this.accName = ''; this.accBalance = 0; this.accCurrency = 'RSD'; this.accShared = false;
    this.accountError.set('');
    this.showAccountModal.set(true);
  }

  openEditAccount(a: BankAccount) {
    this.editingAccountId.set(a.id);
    this.accName = a.name; this.accBalance = a.balance; this.accCurrency = a.currency;
    this.accountError.set('');
    this.showAccountModal.set(true);
  }

  saveAccount() {
    if (!this.accName.trim()) { this.accountError.set('Account name is required'); return; }
    const id = this.editingAccountId();
    const obs = id
      ? this.accountSvc.update(id, { name: this.accName })
      : this.accountSvc.create({ name: this.accName, initialBalance: this.accBalance, currency: this.accCurrency, includeInSharedBudget: this.accShared });
    obs.subscribe({ next: () => { this.showAccountModal.set(false); this.loadAll(); }, error: e => this.accountError.set(e.error?.message || 'Error') });
  }

  toggleSharedBudget(event: Event, id: number) {
    event.stopPropagation();
    this.accountSvc.toggleSharedBudget(id).subscribe(updated => {
      this.accounts.update(list => list.map(a => a.id === id ? { ...a, includeInSharedBudget: updated.includeInSharedBudget } : a));
    });
  }

  confirmDeleteAccount(id: number) {
    this.confirmMessage.set('Delete this account and all its transactions?');
    this.confirmCallback = () => this.accountSvc.delete(id).subscribe(() => { this.showConfirmModal.set(false); this.loadAll(); });
    this.showConfirmModal.set(true);
  }

  // ─── Transaction Modal ─────────────────────────────────────────────────────
  openAddTransaction() {
    this.txType.set(TransactionType.Expense);
    this.txAmount = 0; this.txDescription = ''; this.txNotes = '';
    this.txAccountId = this.accounts()[0]?.id || 0;
    this.txCategoryId = this.categories()[0]?.id || null;
    this.txDate = new Date().toISOString().split('T')[0];
    this.txError.set('');
    this.showTxModal.set(true);
  }

  saveTransaction() {
    if (!this.txAmount || this.txAmount <= 0) { this.txError.set('Amount is required'); return; }
    if (!this.txDescription.trim()) { this.txError.set('Description is required'); return; }
    this.txSvc.create({
      bankAccountId: this.txAccountId,
      amount: this.txAmount,
      type: this.txType(),
      categoryId: this.txType() === TransactionType.Expense ? (this.txCategoryId || undefined) : undefined,
      description: this.txDescription,
      date: this.txDate,
      notes: this.txNotes || undefined
    }).subscribe({ next: () => { this.showTxModal.set(false); this.loadAll(); }, error: e => this.txError.set(e.error?.message || 'Error') });
  }

  confirmDeleteTx(id: number) {
    this.confirmMessage.set('Delete this transaction?');
    this.confirmCallback = () => this.txSvc.delete(id).subscribe(() => { this.showConfirmModal.set(false); this.loadAll(); });
    this.showConfirmModal.set(true);
  }

  runConfirm() { this.confirmCallback?.(); }

  // ─── Categories ────────────────────────────────────────────────────────────
  addCategory() {
    if (!this.newCategoryName.trim()) return;
    this.txSvc.createCategory({ name: this.newCategoryName.trim(), icon: this.newCategoryIcon || '📁' })
      .subscribe(() => { this.newCategoryName = ''; this.newCategoryIcon = ''; this.txSvc.getCategories().subscribe(c => this.categories.set(c)); });
  }

  deleteCategory(id: number) {
    this.txSvc.deleteCategory(id).subscribe(() => this.txSvc.getCategories().subscribe(c => this.categories.set(c)));
  }
}
