using Microsoft.EntityFrameworkCore;
using PiggyBank.Core.Entities;
using PiggyBank.Core.Interfaces;
using PiggyBank.Infrastructure.Data;

namespace PiggyBank.Infrastructure.Repositories
{
    public class SavingsGoalRepository : Repository<SavingsGoal>, ISavingsGoalRepository
    {
        public SavingsGoalRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SavingsGoal>> GetBySharedAccountIdAsync(int sharedAccountId, bool includeArchived = false)
        {
            var query = _dbSet
                .Include(g => g.Contributions)
                    .ThenInclude(c => c.ContributedBy)
                .Where(g => g.SharedAccountId == sharedAccountId);

            if (!includeArchived)
                query = query.Where(g => !g.IsArchived);

            return await query.OrderByDescending(g => g.CreatedAt).ToListAsync();
        }

        public async Task<SavingsGoal?> GetWithContributionsAsync(int goalId)
        {
            return await _dbSet
                .Include(g => g.Contributions)
                    .ThenInclude(c => c.ContributedBy)
                .FirstOrDefaultAsync(g => g.Id == goalId);
        }
    }
}
