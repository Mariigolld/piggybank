import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Sidebar } from '../../shared/sidebar/sidebar';
import { AuthService } from '../../services/auth';
import { SavingsGoalService } from '../../services/savings-goal';
import { RecurringBillService } from '../../services/recurring-bill';
import { SharedBudgetService } from '../../services/shared-budget';
import {
  SharedAccount, SavingsGoal, RecurringBill, SharedBudget,
  BillFrequency, BillPaidBy
} from '../../models';
import { HttpClient } from '@angular/common/http';

const API = 'http://localhost:5235/api';

@Component({
  selector: 'app-partner',
  imports: [CommonModule, FormsModule, Sidebar],
  templateUrl: './partner.html',
  styleUrl: './partner.css'
})
export class Partner implements OnInit {
  private auth = inject(AuthService);
  private http = inject(HttpClient);
  private goalSvc = inject(SavingsGoalService);
  private billSvc = inject(RecurringBillService);
  private budgetSvc = inject(SharedBudgetService);

  user = this.auth.getUser();
  accounts = signal<SharedAccount[]>([]);
  currentAccount = signal<SharedAccount | null>(null);
  pendingAccounts = signal<SharedAccount[]>([]);

  activeTab = signal<'savings' | 'bills' | 'budget'>('savings');

  // Savings goals
  goals = signal<SavingsGoal[]>([]);
  showArchived = false;

  // Recurring bills
  bills = signal<RecurringBill[]>([]);

  // Budget
  budget = signal<SharedBudget | null>(null);
  budgetMonth = new Date().getMonth() + 1;
  budgetYear = new Date().getFullYear();

  // Modals
  showCreateModal = signal(false);
  showJoinModal = signal(false);
  showAddGoalModal = signal(false);
  showContribModal = signal(false);
  showAddBillModal = signal(false);
  showBudgetModal = signal(false);

  // Create account
  createName = '';
  generatedCode = signal('');
  showCode = signal(false);

  // Join
  joinCode = '';

  // Add goal
  goalName = ''; goalDesc = ''; goalTarget = 0; goalDate = '';

  // Contribution
  contribGoalId = 0; contribAmount = 0; contribNote = '';

  // Add bill
  billName = ''; billAmount = 0; billDay = 1;
  billFrequency = BillFrequency.Monthly;
  billPaidBy = BillPaidBy.Split50;

  // Budget
  budgetNotes = '';
  budgetCategories: { name: string; icon: string; amount: number }[] = [];

  BillFrequency = BillFrequency;
  BillPaidBy = BillPaidBy;

  readonly freqLabels = ['Weekly', 'Monthly', 'Yearly'];

  get paidByLabels(): string[] {
    const acc = this.currentAccount();
    const u1 = acc?.user1 ? `${acc.user1.firstName} ${acc.user1.lastName}` : 'User 1';
    const u2 = acc?.user2 ? `${acc.user2.firstName} ${acc.user2.lastName}` : 'User 2';
    return [`${u1} pays`, `${u2} pays`, 'Alternating', '50/50 Split'];
  }

  get user1Name(): string {
    const acc = this.currentAccount();
    return acc?.user1 ? `${acc.user1.firstName} ${acc.user1.lastName}` : 'User 1';
  }

  get user2Name(): string {
    const acc = this.currentAccount();
    return acc?.user2 ? `${acc.user2.firstName} ${acc.user2.lastName}` : 'User 2';
  }

  readonly months = ['January','February','March','April','May','June','July','August','September','October','November','December'];
  Math = Math;

  ngOnInit() { this.init(); }

  async init() {
    this.http.get<SharedAccount[]>(`${API}/SharedAccounts`).subscribe(list => {
      const active = list.filter(a => a.status === 1);
      this.accounts.set(active);
      if (active.length > 0) {
        this.currentAccount.set(active[0]);
        this.loadAll();
      }
    });
    this.http.get<SharedAccount[]>(`${API}/SharedAccounts/pending`).subscribe(p => this.pendingAccounts.set(p));
  }

  loadAll() {
    this.loadGoals();
    this.loadBills();
    this.loadBudget();
  }

  onAccountChange(id: string) {
    const acc = this.accounts().find(a => a.id === +id);
    if (acc) { this.currentAccount.set(acc); this.loadAll(); }
  }

  get accountId() { return this.currentAccount()?.id || 0; }

  get myName(): string {
    return this.user ? `${this.user.firstName} ${this.user.lastName}` : '';
  }

  get partnerName(): string {
    const acc = this.currentAccount();
    if (!acc || !acc.user2) return 'Waiting for partner';
    const me = acc.user1?.email === this.user?.email;
    return me ? `${acc.user2.firstName} ${acc.user2.lastName}` : `${acc.user1.firstName} ${acc.user1.lastName}`;
  }

  // ─── Savings Goals ─────────────────────────────────────────────────────────
  loadGoals() {
    this.goalSvc.getAll(this.accountId, this.showArchived).subscribe(g => this.goals.set(g));
  }

  createGoal() {
    if (!this.goalName.trim() || this.goalTarget <= 0) return;
    this.goalSvc.create(this.accountId, {
      name: this.goalName, description: this.goalDesc || undefined,
      targetAmount: this.goalTarget, targetDate: this.goalDate || undefined, icon: ''
    }).subscribe(() => { this.showAddGoalModal.set(false); this.loadGoals(); });
  }

  openContrib(goalId: number) { this.contribGoalId = goalId; this.contribAmount = 0; this.contribNote = ''; this.showContribModal.set(true); }

  addContrib() {
    if (this.contribAmount <= 0) return;
    this.goalSvc.addContribution(this.accountId, this.contribGoalId, { amount: this.contribAmount, note: this.contribNote || undefined })
      .subscribe(() => { this.showContribModal.set(false); this.loadGoals(); });
  }

  archiveGoal(id: number) {
    if (!confirm('Archive this goal?')) return;
    this.goalSvc.archive(this.accountId, id).subscribe(() => this.loadGoals());
  }

  deleteGoal(id: number) {
    if (!confirm('Delete this goal?')) return;
    this.goalSvc.delete(this.accountId, id).subscribe(() => this.loadGoals());
  }

  // ─── Recurring Bills ───────────────────────────────────────────────────────
  loadBills() {
    this.billSvc.getAll(this.accountId).subscribe(b => this.bills.set(b));
  }

  createBill() {
    if (!this.billName.trim() || this.billAmount <= 0) return;
    this.billSvc.create(this.accountId, {
      name: this.billName,
      amount: Number(this.billAmount),
      frequency: Number(this.billFrequency),
      paidBy: Number(this.billPaidBy),
      dayOfMonth: Number(this.billDay) || 1
    }).subscribe({ next: () => { this.showAddBillModal.set(false); this.loadBills(); }, error: e => alert(e.error?.message || 'Failed to create bill') });
  }

  markPaid(billId: number, month: number, year: number) {
    this.billSvc.markPaid(this.accountId, billId, year, month).subscribe(() => this.loadBills());
  }

  markUnpaid(billId: number, month: number, year: number) {
    this.billSvc.markUnpaid(this.accountId, billId, year, month).subscribe(() => this.loadBills());
  }

  deactivateBill(id: number) {
    if (!confirm('Deactivate this bill?')) return;
    this.billSvc.deactivate(this.accountId, id).subscribe(() => this.loadBills());
  }

  deleteBill(id: number) {
    if (!confirm('Delete this bill and all its payment history?')) return;
    this.billSvc.delete(this.accountId, id).subscribe(() => this.loadBills());
  }

  currentPayment(bill: RecurringBill) {
    const now = new Date();
    return bill.payments.find(p => p.month === now.getMonth() + 1 && p.year === now.getFullYear()) || null;
  }

  upcomingPayments(bill: RecurringBill) {
    const now = new Date();
    const m = now.getMonth() + 1, y = now.getFullYear();
    return bill.payments.filter(p => !p.isPaid && (p.year > y || (p.year === y && p.month > m))).slice(0, 2);
  }

  // ─── Budget ────────────────────────────────────────────────────────────────
  loadBudget() {
    this.budgetSvc.getByMonth(this.accountId, this.budgetYear, this.budgetMonth)
      .subscribe({ next: b => this.budget.set(b), error: () => this.budget.set(null) });
  }

  changeMonth(delta: number) {
    this.budgetMonth += delta;
    if (this.budgetMonth > 12) { this.budgetMonth = 1; this.budgetYear++; }
    if (this.budgetMonth < 1) { this.budgetMonth = 12; this.budgetYear--; }
    this.loadBudget();
  }

  get budgetMonthLabel() { return `${this.months[this.budgetMonth - 1]} ${this.budgetYear}`; }

  openBudgetModal() {
    const b = this.budget();
    if (b) {
      this.budgetNotes = b.notes || '';
      this.budgetCategories = b.categories.map(c => ({ name: c.categoryName, icon: c.icon, amount: c.allocatedAmount }));
    } else {
      this.budgetNotes = '';
      this.budgetCategories = [
        { name: 'Groceries', icon: '🛒', amount: 0 },
        { name: 'Bills', icon: '💡', amount: 0 },
        { name: 'Transport', icon: '🚗', amount: 0 },
        { name: 'Restaurant', icon: '🍽️', amount: 0 }
      ];
    }
    this.showBudgetModal.set(true);
  }

  deleteBudget() {
    const b = this.budget();
    if (!b || !confirm('Delete this budget?')) return;
    this.budgetSvc.delete(this.accountId, b.id).subscribe(() => { this.budget.set(null); });
  }

  addBudgetCategory() {
    this.budgetCategories.push({ name: '', icon: '📦', amount: 0 });
  }

  removeBudgetCategory(i: number) { this.budgetCategories.splice(i, 1); }

  saveBudget() {
    const cats = this.budgetCategories.filter(c => c.name.trim() && c.amount > 0);
    if (!cats.length) return;
    this.budgetSvc.save(this.accountId, {
      month: this.budgetMonth, year: this.budgetYear,
      notes: this.budgetNotes || undefined, categories: cats.map(c => ({ categoryName: c.name, icon: c.icon || '📦', allocatedAmount: c.amount }))
    }).subscribe(() => { this.showBudgetModal.set(false); this.loadBudget(); });
  }

  // ─── Shared Account Management ─────────────────────────────────────────────
  createAccount() {
    if (!this.createName.trim()) return;
    this.http.post<SharedAccount>(`${API}/SharedAccounts`, { name: this.createName }).subscribe(acc => {
      this.generatedCode.set(acc.inviteCode);
      this.showCode.set(true);
    });
  }

  joinAccount() {
    const code = this.joinCode.trim().toUpperCase();
    if (!code) return;
    this.http.post<SharedAccount>(`${API}/SharedAccounts/join/${code}`, {}).subscribe(() => {
      this.showJoinModal.set(false);
      this.init();
    });
  }

  // ─── Helpers ───────────────────────────────────────────────────────────────
  fmt(n: number) { return new Intl.NumberFormat('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(n || 0) + ' RSD'; }
  fmtDate(d: string) { return new Date(d).toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' }); }
  monthName(m: number) { return ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'][m - 1]; }
  goalPct(g: SavingsGoal) { return Math.min(100, g.targetAmount > 0 ? (g.currentAmount / g.targetAmount) * 100 : 0); }

  ordinal(n: number): string {
    const s = ['th', 'st', 'nd', 'rd'];
    const v = n % 100;
    return n + (s[(v - 20) % 10] || s[v] || s[0]);
  }

  recentPaidPayments(bill: RecurringBill) {
    const now = new Date();
    const cm = now.getMonth() + 1, cy = now.getFullYear();
    return bill.payments
      .filter(p => p.isPaid && !(p.month === cm && p.year === cy))
      .sort((a, b) => b.year !== a.year ? b.year - a.year : b.month - a.month)
      .slice(0, 3);
  }

  pauseBill(id: number) {
    if (!confirm('Pause this bill? You can resume it any time.')) return;
    this.billSvc.deactivate(this.accountId, id).subscribe(() => this.loadBills());
  }

  resumeBill(id: number) {
    this.billSvc.reactivate(this.accountId, id).subscribe(() => this.loadBills());
  }
}
