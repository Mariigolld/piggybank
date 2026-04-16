namespace PiggyBank.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBankAccountRepository BankAccounts { get; }
        ICategoryRepository Categories { get; }
        ITransactionRepository Transactions { get; }
        ISharedAccountRepository SharedAccounts { get; }
        ISavingsGoalRepository SavingsGoals { get; }
        IRecurringBillRepository RecurringBills { get; }
        ISharedBudgetRepository SharedBudgets { get; }
        IGroupRepository Groups { get; }
        IGroupExpenseRepository GroupExpenses { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
