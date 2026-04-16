using PiggyBank.Core.Entities;

namespace PiggyBank.Core.Interfaces
{
    public interface IBankAccountRepository : IRepository<BankAccount>
    {
        Task<IEnumerable<BankAccount>> GetByUserIdAsync(string userId);
        Task<BankAccount?> GetByAccountNumberAsync(string accountNumber);
        Task<decimal> GetTotalBalanceAsync(string userId);
    }
}
