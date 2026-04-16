using Microsoft.EntityFrameworkCore;
using PiggyBank.Core.Entities;
using PiggyBank.Core.Interfaces;
using PiggyBank.Infrastructure.Data;

namespace PiggyBank.Infrastructure.Repositories
{
    public class SharedBudgetRepository : Repository<SharedBudget>, ISharedBudgetRepository
    {
        public SharedBudgetRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SharedBudget>> GetBySharedAccountIdAsync(int sharedAccountId)
        {
            return await _dbSet
                .Include(b => b.Categories)
                .Where(b => b.SharedAccountId == sharedAccountId)
                .OrderByDescending(b => b.Year)
                .ThenByDescending(b => b.Month)
                .ToListAsync();
        }

        public async Task<SharedBudget?> GetByMonthYearAsync(int sharedAccountId, int month, int year)
        {
            return await _dbSet
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b =>
                    b.SharedAccountId == sharedAccountId &&
                    b.Month == month &&
                    b.Year == year);
        }
    }
}
