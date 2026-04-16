import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Sidebar } from '../../shared/sidebar/sidebar';
import { GroupService } from '../../services/group';
import { AuthService } from '../../services/auth';
import { Group, GroupExpense, GroupMember, GroupSettlement, ExpenseShare } from '../../models';

@Component({
  selector: 'app-groups',
  imports: [CommonModule, FormsModule, Sidebar],
  templateUrl: './groups.html',
  styleUrl: './groups.css'
})
export class Groups implements OnInit {
  private groupSvc = inject(GroupService);
  private auth = inject(AuthService);

  user = this.auth.getUser();
  groups = signal<Group[]>([]);
  selectedGroup = signal<Group | null>(null);
  expenses = signal<GroupExpense[]>([]);
  settlements = signal<GroupSettlement[]>([]);

  // Create group modal
  showCreateModal = signal(false);
  newGroupName = ''; newGroupDesc = '';

  // Add expense modal
  showExpenseModal = signal(false);
  expDesc = ''; expAmount = 0; expDate = ''; expPaidBy = '';
  expSplitType: 'all' | 'selected' | 'custom' = 'all';
  selectedMemberIds = signal<number[]>([]);
  manualShares: { memberId: number; amount: number }[] = [];

  // Join group modal
  showJoinModal = signal(false);
  joinCode = '';

  // Invite member modal
  showInviteModal = signal(false);

  // Settle modal
  showSettleModal = signal(false);
  settleFrom = ''; settleTo = ''; settleAmount = 0;

  error = signal('');

  ngOnInit() {
    this.loadGroups();
    this.expDate = new Date().toISOString().split('T')[0];
  }

  loadGroups() {
    this.groupSvc.getAll().subscribe(list => this.groups.set(list));
  }

  openGroup(g: Group) {
    this.selectedGroup.set(g);
    this.loadGroupData(g.id);
  }

  loadGroupData(id: number) {
    this.groupSvc.getExpenses(id).subscribe(list => this.expenses.set(list));
    this.groupSvc.getSettlements(id).subscribe(list => this.settlements.set(list));
    // reload to get fresh balance data
    this.groupSvc.getById(id).subscribe(g => this.selectedGroup.set(g));
  }

  back() { this.selectedGroup.set(null); }

  // ─── Create Group ──────────────────────────────────────────────────────────
  createGroup() {
    if (!this.newGroupName.trim()) return;
    this.groupSvc.create({ name: this.newGroupName.trim(), description: this.newGroupDesc || undefined })
      .subscribe({ next: () => { this.showCreateModal.set(false); this.newGroupName = ''; this.newGroupDesc = ''; this.loadGroups(); }, error: e => this.error.set(e.error?.message || 'Error') });
  }

  // ─── Join Group ───────────────────────────────────────────────────────────
  joinGroup() {
    if (!this.joinCode.trim()) return;
    this.error.set('');
    this.groupSvc.joinByCode(this.joinCode.trim().toUpperCase())
      .subscribe({
        next: () => { this.showJoinModal.set(false); this.joinCode = ''; this.loadGroups(); },
        error: e => this.error.set(e.error?.message || 'Invalid invite code')
      });
  }

  // ─── Add Expense ───────────────────────────────────────────────────────────
  openExpenseModal() {
    this.expDesc = ''; this.expAmount = 0;
    this.expDate = new Date().toISOString().split('T')[0];
    this.expSplitType = 'all';
    this.expPaidBy = this.currentUserId;
    this.selectedMemberIds.set(this.acceptedMembers.map(m => m.id));
    this.manualShares = this.acceptedMembers.map(m => ({ memberId: m.id, amount: 0 }));
    this.error.set('');
    this.showExpenseModal.set(true);
  }

  updateManualShare(memberId: number, value: string) {
    const share = this.manualShares.find(s => s.memberId === memberId);
    if (share) share.amount = parseFloat(value) || 0;
  }

  get manualTotal() { return this.manualShares.reduce((s, m) => s + m.amount, 0); }

  addExpense() {
    const g = this.selectedGroup();
    if (!g || !this.expDesc.trim() || this.expAmount <= 0) { this.error.set('Fill in description and amount'); return; }
    if (this.expSplitType === 'custom' && Math.abs(this.manualTotal - this.expAmount) > 0.01) { this.error.set('Manual shares must sum to total amount'); return; }

    const base = { groupId: g.id, description: this.expDesc, amount: this.expAmount, date: this.expDate, paidByUserId: this.expPaidBy };

    let obs;
    if (this.expSplitType === 'all') {
      obs = this.groupSvc.createExpense(base);
    } else if (this.expSplitType === 'selected') {
      obs = this.groupSvc.createExpenseSplit({ ...base, memberIds: this.selectedMemberIds() });
    } else {
      const manualShares = this.manualShares.filter(s => s.amount > 0).map(s => ({ groupMemberId: s.memberId, shareAmount: s.amount }));
      obs = this.groupSvc.createExpense({ ...base, manualShares });
    }

    obs.subscribe({ next: () => { this.showExpenseModal.set(false); this.loadGroupData(g.id); }, error: e => this.error.set(e.error?.message || 'Error') });
  }

  deleteExpense(id: number) {
    const g = this.selectedGroup();
    if (!g || !confirm('Delete this expense?')) return;
    this.groupSvc.deleteExpense(id).subscribe(() => this.loadGroupData(g.id));
  }

  // ─── Settle ────────────────────────────────────────────────────────────────
  openSettleModal() {
    this.settleFrom = this.currentUserId;
    this.settleTo = '';
    this.settleAmount = 0;
    this.showSettleModal.set(true);
  }

  addSettlement() {
    const g = this.selectedGroup();
    if (!g || !this.settleTo || this.settleAmount <= 0) return;
    this.groupSvc.createSettlement({ groupId: g.id, toUserId: this.settleTo, amount: this.settleAmount })
      .subscribe({ next: () => { this.showSettleModal.set(false); this.loadGroupData(g.id); }, error: e => this.error.set(e.error?.message || 'Error') });
  }

  deleteGroup() {
    const g = this.selectedGroup();
    if (!g || !confirm(`Delete "${g.name}"? This cannot be undone.`)) return;
    this.groupSvc.delete(g.id).subscribe(() => { this.selectedGroup.set(null); this.loadGroups(); });
  }

  quickSettleShare(exp: GroupExpense, share: ExpenseShare) {
    const g = this.selectedGroup();
    if (!g) return;
    this.groupSvc.markShareAsPaid(share.id)
      .subscribe({ next: () => this.loadGroupData(g.id), error: e => this.error.set(e.error?.message || 'Error') });
  }

  // ─── Helpers ───────────────────────────────────────────────────────────────
  get currentUserId(): string {
    const g = this.selectedGroup();
    if (!g) return '';
    const me = (g.members || []).find(m => m.user?.email === this.user?.email);
    return me?.user?.id || '';
  }

  get acceptedMembers(): GroupMember[] {
    return ((this.selectedGroup()?.members) || []).filter(m => m.status === 1);
  }

  get myBalance(): number {
    return 0; // balance not returned by API in member list
  }

  get groupBalances(): { fromId: string; fromName: string; toId: string; toName: string; amount: number }[] {
    const owed = new Map<string, Map<string, number>>();
    const names = new Map<string, string>();

    for (const exp of this.expenses()) {
      if (exp.paidBy?.id) names.set(exp.paidBy.id, `${exp.paidBy.firstName} ${exp.paidBy.lastName}`);
      for (const s of exp.shares) {
        if (!s.user?.id || s.isPaid || s.user.id === exp.paidBy?.id) continue;
        names.set(s.user.id, `${s.user.firstName} ${s.user.lastName}`);
        if (!owed.has(s.user.id)) owed.set(s.user.id, new Map());
        const cur = owed.get(s.user.id)!.get(exp.paidBy.id!) || 0;
        owed.get(s.user.id)!.set(exp.paidBy.id!, cur + s.shareAmount);
      }
    }

    const result: { fromId: string; fromName: string; toId: string; toName: string; amount: number }[] = [];
    const processed = new Set<string>();

    for (const [fromId, toMap] of owed) {
      for (const [toId, amount] of toMap) {
        const key = [fromId, toId].sort().join('|');
        if (processed.has(key)) continue;
        processed.add(key);
        const reverse = owed.get(toId)?.get(fromId) || 0;
        const net = amount - reverse;
        if (net > 0.01) {
          result.push({ fromId, fromName: names.get(fromId)!, toId, toName: names.get(toId)!, amount: net });
        } else if (net < -0.01) {
          result.push({ fromId: toId, fromName: names.get(toId)!, toId: fromId, toName: names.get(fromId)!, amount: -net });
        }
      }
    }
    return result;
  }

  memberName(m: GroupMember) { return `${m.user?.firstName || ''} ${m.user?.lastName || ''}`.trim(); }
  memberInitials(m: GroupMember) { return ((m.user?.firstName || '')[0] || '') + ((m.user?.lastName || '')[0] || ''); }
  isMemberSelected(memberId: number) { return this.selectedMemberIds().includes(memberId); }
  toggleMember(memberId: number) {
    const cur = this.selectedMemberIds();
    this.selectedMemberIds.set(cur.includes(memberId) ? cur.filter(id => id !== memberId) : [...cur, memberId]);
  }

  copyCode(code: string, event: Event) {
    event.stopPropagation();
    navigator.clipboard.writeText(code).then(() => {
      const el = event.target as HTMLElement;
      const chip = el.closest('.group-chip') as HTMLElement;
      if (chip) { chip.classList.add('copied'); setTimeout(() => chip.classList.remove('copied'), 1800); }
    });
  }

  copyInviteCode(code: string) {
    navigator.clipboard.writeText(code).then(() => {
      const box = document.querySelector('.invite-code-box') as HTMLElement;
      if (box) { box.classList.add('copied'); setTimeout(() => box.classList.remove('copied'), 1800); }
    });
  }

  Math = Math;
  fmt(n: number) { return new Intl.NumberFormat('sr-RS').format(n || 0); }

  fmtDate(d: string) {
    return new Date(d).toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' });
  }
}
