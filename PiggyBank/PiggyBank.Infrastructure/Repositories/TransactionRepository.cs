using Microsoft.EntityFrameworkCore;
using PiggyBank.Core.Entities;
using PiggyBank.Core.Interfaces;
using PiggyBank.Infrastructure.Data;

namespace PiggyBank.Infrastructure.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Transaction> Items, int TotalCount)> GetPagedAsync(
            string userId, int page, int pageSize)
        {
            var query = _dbSet
                .Include(t => t.Category)
                .Include(t => t.BankAccount)
                .Where(t => t.BankAccount.UserId == userId)
                .OrderByDescending(t => t.Date);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<Transaction>> GetByBankAccountIdAsync(int bankAccountId)
        {
            return await _dbSet
                .Include(t => t.Category)
                .Where(t => t.BankAccountId == bankAccountId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByCategoryIdAsync(int categoryId)
        {
            return await _dbSet
                .Include(t => t.BankAccount)
                .Where(t => t.CategoryId == categoryId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(
            string userId, DateTime from, DateTime to)
        {
            return await _dbSet
                .Include(t => t.Category)
                .Include(t => t.BankAccount)
                .Where(t => t.BankAccount.UserId == userId && t.Date >= from && t.Date <= to)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalExpensesByCategoryAsync(int categoryId, DateTime? from = null)
        {
            var query = _dbSet.Where(t => t.CategoryId == categoryId && t.Type == TransactionType.Expense);

            if (from.HasValue)
                query = query.Where(t => t.Date >= from.Value);

            return await query.SumAsync(t => t.Amount);
        }
    }
}