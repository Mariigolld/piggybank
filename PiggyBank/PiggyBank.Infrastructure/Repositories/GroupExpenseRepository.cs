using Microsoft.EntityFrameworkCore;
using PiggyBank.Core.Entities;
using PiggyBank.Core.Interfaces;
using PiggyBank.Infrastructure.Data;

namespace PiggyBank.Infrastructure.Repositories
{
    public class GroupExpenseRepository : Repository<GroupExpense>, IGroupExpenseRepository
    {
        public GroupExpenseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<GroupExpense>> GetByGroupIdAsync(int groupId)
        {
            return await _dbSet
                .Include(e => e.PaidBy)
                .Include(e => e.Shares)
                    .ThenInclude(s => s.GroupMember)
                        .ThenInclude(m => m.User)
                .Where(e => e.GroupId == groupId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<GroupExpense?> GetWithSharesAsync(int expenseId)
        {
            return await _dbSet
                .Include(e => e.PaidBy)
                .Include(e => e.Shares)
                    .ThenInclude(s => s.GroupMember)
                        .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(e => e.Id == expenseId);
        }

        public async Task<decimal> GetTotalExpensesForGroupAsync(int groupId)
        {
            return await _dbSet
                .Where(e => e.GroupId == groupId)
                .SumAsync(e => e.Amount);
        }
    }
}
