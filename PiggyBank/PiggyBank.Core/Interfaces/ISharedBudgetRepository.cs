using PiggyBank.Core.Entities;

namespace PiggyBank.Core.Interfaces
{
    public interface ISharedBudgetRepository : IRepository<SharedBudget>
    {
        Task<IEnumerable<SharedBudget>> GetBySharedAccountIdAsync(int sharedAccountId);
        Task<SharedBudget?> GetByMonthYearAsync(int sharedAccountId, int month, int year);
    }
}
