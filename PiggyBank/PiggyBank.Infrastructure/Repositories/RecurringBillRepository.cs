using Microsoft.EntityFrameworkCore;
using PiggyBank.Core.Entities;
using PiggyBank.Core.Interfaces;
using PiggyBank.Infrastructure.Data;

namespace PiggyBank.Infrastructure.Repositories
{
    public class RecurringBillRepository : Repository<RecurringBill>, IRecurringBillRepository
    {
        public RecurringBillRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RecurringBill>> GetBySharedAccountIdAsync(int sharedAccountId)
        {
            return await _dbSet
                .Include(b => b.Payments)
                    .ThenInclude(p => p.PaidByUser)
                .Where(b => b.SharedAccountId == sharedAccountId)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<RecurringBill?> GetWithPaymentsAsync(int billId)
        {
            return await _dbSet
                .Include(b => b.Payments)
                    .ThenInclude(p => p.PaidByUser)
                .FirstOrDefaultAsync(b => b.Id == billId);
        }
    }
}
