using PiggyBank.Core.Entities;

namespace PiggyBank.Core.Interfaces
{
    public interface IGroupExpenseRepository : IRepository<GroupExpense>
    {
        Task<IEnumerable<GroupExpense>> GetByGroupIdAsync(int groupId);
        Task<GroupExpense?> GetWithSharesAsync(int expenseId);
        Task<decimal> GetTotalExpensesForGroupAsync(int groupId);
    }
}
