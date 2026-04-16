using Microsoft.EntityFrameworkCore;
using PiggyBank.Core.Entities;
using PiggyBank.Core.Interfaces;
using PiggyBank.Infrastructure.Data;

namespace PiggyBank.Infrastructure.Repositories
{
    public class BankAccountRepository : Repository<BankAccount>, IBankAccountRepository
    {
        public BankAccountRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BankAccount>> GetByUserIdAsync(string userId)
        {
            return await _dbSet
                .Where(b => b.UserId == userId && b.IsActive)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<BankAccount?> GetByAccountNumberAsync(string accountNumber)
        {
            return await _dbSet
                .FirstOrDefaultAsync(b => b.AccountNumber == accountNumber);
        }

        public async Task<decimal> GetTotalBalanceAsync(string userId)
        {
            return await _dbSet
                .Where(b => b.UserId == userId && b.IsActive)
                .SumAsync(b => b.Balance);
        }
    }
}