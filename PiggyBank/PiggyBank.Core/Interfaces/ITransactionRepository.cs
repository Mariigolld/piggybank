using PiggyBank.Core.Entities;

namespace PiggyBank.Core.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<(IEnumerable<Transaction> Items, int TotalCount)> GetPagedAsync(
            string userId, int page, int pageSize);
        Task<IEnumerable<Transaction>> GetByBankAccountIdAsync(int bankAccountId);
        Task<IEnumerable<Transaction>> GetByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Transaction>> GetByDateRangeAsync(
            string userId, DateTime from, DateTime to);
        Task<decimal> GetTotalExpensesByCategoryAsync(int categoryId, DateTime? from = null);
    }
}
