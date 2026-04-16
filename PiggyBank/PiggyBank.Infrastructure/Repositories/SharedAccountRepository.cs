using Microsoft.EntityFrameworkCore;
using PiggyBank.Core.Entities;
using PiggyBank.Core.Interfaces;
using PiggyBank.Infrastructure.Data;

namespace PiggyBank.Infrastructure.Repositories
{
    public class SharedAccountRepository : Repository<SharedAccount>, ISharedAccountRepository
    {
        public SharedAccountRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SharedAccount>> GetByUserIdAsync(string userId)
        {
            return await _dbSet
                .Include(s => s.User1)
                .Include(s => s.User2)
                .Where(s => (s.User1Id == userId || s.User2Id == userId) &&
                           s.Status == SharedAccountStatus.Accepted)
                .ToListAsync();
        }

        public async Task<SharedAccount?> GetSharedAccountBetweenUsersAsync(string user1Id, string user2Id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s =>
                    (s.User1Id == user1Id && s.User2Id == user2Id) ||
                    (s.User1Id == user2Id && s.User2Id == user1Id));
        }

        public async Task<IEnumerable<SharedAccount>> GetPendingInvitationsAsync(string userId)
        {
            return await _dbSet
                .Include(s => s.User1)
                .Include(s => s.User2)
                .Where(s => s.User2Id == userId && s.Status == SharedAccountStatus.Pending)
                .ToListAsync();
        }
    }
}