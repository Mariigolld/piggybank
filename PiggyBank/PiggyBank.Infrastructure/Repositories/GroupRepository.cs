using Microsoft.EntityFrameworkCore;
using PiggyBank.Core.Entities;
using PiggyBank.Core.Interfaces;
using PiggyBank.Infrastructure.Data;

namespace PiggyBank.Infrastructure.Repositories
{
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        public GroupRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Group>> GetUserGroupsAsync(string userId)
        {
            return await _dbSet
                .Include(g => g.CreatedBy)
                .Include(g => g.Members)
                    .ThenInclude(m => m.User)
                .Where(g => g.IsActive &&
                           (g.CreatedById == userId ||
                            g.Members.Any(m => m.UserId == userId &&
                                             m.Status == GroupMemberStatus.Accepted)))
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<Group?> GetWithMembersAsync(int groupId)
        {
            return await _dbSet
                .Include(g => g.CreatedBy)
                .Include(g => g.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(g => g.Id == groupId);
        }

        public async Task<Group?> GetWithExpensesAsync(int groupId)
        {
            return await _dbSet
                .Include(g => g.Members)
                    .ThenInclude(m => m.User)
                .Include(g => g.Expenses)
                    .ThenInclude(e => e.PaidBy)
                .Include(g => g.Expenses)
                    .ThenInclude(e => e.Shares)
                        .ThenInclude(s => s.GroupMember)
                .FirstOrDefaultAsync(g => g.Id == groupId);
        }
    }
}