using PiggyBank.Core.Entities;

namespace PiggyBank.Core.Interfaces
{
    public interface IRecurringBillRepository : IRepository<RecurringBill>
    {
        Task<IEnumerable<RecurringBill>> GetBySharedAccountIdAsync(int sharedAccountId);
        Task<RecurringBill?> GetWithPaymentsAsync(int billId);
    }
}
