// ─── Auth ───────────────────────────────────────────────────────────────────
export interface LoginRequest { email: string; password: string; }
export interface RegisterRequest { email: string; password: string; firstName: string; lastName: string; }
export interface AuthResponse { token: string; email: string; firstName: string; lastName: string; }

// ─── Bank Accounts ───────────────────────────────────────────────────────────
export interface BankAccount {
  id: number;
  name: string;
  balance: number;
  currency: string;
  includeInSharedBudget: boolean;
}
export interface CreateBankAccountRequest { name: string; initialBalance: number; currency: string; includeInSharedBudget?: boolean; }
export interface UpdateBankAccountRequest { name: string; }

// ─── Transactions ────────────────────────────────────────────────────────────
export enum TransactionType { Expense = 0, Income = 1 }
export interface Transaction {
  id: number;
  bankAccountId: number;
  bankAccountName: string;
  amount: number;
  type: TransactionType;
  categoryId?: number;
  categoryName?: string;
  categoryIcon?: string;
  description: string;
  date: string;
  notes?: string;
}
export interface CreateTransactionRequest {
  bankAccountId: number;
  amount: number;
  type: TransactionType;
  categoryId?: number;
  description: string;
  date: string;
  notes?: string;
}

// ─── Categories ──────────────────────────────────────────────────────────────
export interface Category { id: number; name: string; icon: string; }
export interface CreateCategoryRequest { name: string; icon: string; }

// ─── Groups ──────────────────────────────────────────────────────────────────
export interface Group {
  id: number;
  name: string;
  description?: string;
  createdBy: { id: string; firstName: string; lastName: string; email: string };
  memberCount: number;
  totalExpenses: number;
  inviteCode: string;
  // populated only when loading a single group (detail view)
  members: GroupMember[];
}
export interface GroupMember {
  id: number;
  user: { id: string; firstName: string; lastName: string; email: string };
  status: number;
  isAdmin: boolean;
  joinedAt: string;
}
export interface GroupExpense {
  id: number;
  groupId: number;
  description: string;
  amount: number;
  date: string;
  paidBy: { id: string; firstName: string; lastName: string; email: string };
  shares: ExpenseShare[];
}
export interface ExpenseShare {
  id: number;
  groupMemberId: number;
  user: { id: string; firstName: string; lastName: string; email: string };
  shareAmount: number;
  isPaid: boolean;
}
export interface GroupSettlement {
  id: number;
  groupId: number;
  fromUserId: string;
  fromUserName: string;
  toUserId: string;
  toUserName: string;
  amount: number;
  date: string;
}
export interface CreateGroupRequest { name: string; description?: string; }
export interface CreateGroupExpenseRequest {
  groupId: number;
  description: string;
  amount: number;
  date: string;
  paidByUserId: string;
  memberIds?: number[];
  manualShares?: { groupMemberId: number; shareAmount: number }[];
}
export interface CreateSettlementRequest {
  groupId: number;
  toUserId: string;
  amount: number;
}

// ─── Shared Accounts ─────────────────────────────────────────────────────────
export interface SharedAccount {
  id: number;
  name: string;
  status: number;
  inviteCode: string;
  createdAt: string;
  acceptedAt?: string;
  user1: UserSummary;
  user2?: UserSummary;
}
export interface UserSummary { id: string; email: string; firstName: string; lastName: string; }
export interface CreateSharedAccountRequest { name: string; }

// ─── Savings Goals ───────────────────────────────────────────────────────────
export interface SavingsGoal {
  id: number;
  sharedAccountId: number;
  name: string;
  description?: string;
  targetAmount: number;
  currentAmount: number;
  targetDate?: string;
  isArchived: boolean;
  createdAt: string;
  contributions: SavingsContribution[];
}
export interface SavingsContribution {
  id: number;
  amount: number;
  note?: string;
  date: string;
  contributedBy: UserSummary;
}
export interface CreateSavingsGoalRequest {
  name: string;
  description?: string;
  targetAmount: number;
  icon?: string;
  targetDate?: string;
}
export interface AddContributionRequest { amount: number; note?: string; }

// ─── Recurring Bills ─────────────────────────────────────────────────────────
export enum BillFrequency { Weekly = 0, Monthly = 1, Yearly = 2 }
export enum BillPaidBy { User1 = 0, User2 = 1, Alternating = 2, Split50 = 3 }
export interface RecurringBill {
  id: number;
  sharedAccountId: number;
  name: string;
  amount: number;
  frequency: BillFrequency;
  paidBy: BillPaidBy;
  dayOfMonth: number;
  isActive: boolean;
  createdAt: string;
  payments: RecurringBillPayment[];
}
export interface RecurringBillPayment {
  id: number;
  month: number;
  year: number;
  isPaid: boolean;
  paidAt?: string;
  note?: string;
  paidByUser?: UserSummary;
}
export interface CreateRecurringBillRequest {
  name: string;
  amount: number;
  frequency: BillFrequency;
  paidBy: BillPaidBy;
  dayOfMonth: number;
}

// ─── Shared Budget ───────────────────────────────────────────────────────────
export interface SharedBudget {
  id: number;
  sharedAccountId: number;
  month: number;
  year: number;
  notes?: string;
  totalAllocated: number;
  totalSpent: number;
  categories: SharedBudgetCategory[];
}
export interface SharedBudgetCategory {
  id: number;
  categoryName: string;
  icon: string;
  allocatedAmount: number;
  spentAmount: number;
}
export interface CreateSharedBudgetRequest {
  month: number;
  year: number;
  notes?: string;
  categories: { categoryName: string; icon: string; allocatedAmount: number }[];
}
