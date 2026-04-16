using PiggyBank.Core.Entities;

namespace PiggyBank.Core.Interfaces
{
    public interface ISharedAccountRepository : IRepository<SharedAccount>
    {
        Task<IEnumerable<SharedAccount>> GetByUserIdAsync(string userId);
        Task<SharedAccount?> GetSharedAccountBetweenUsersAsync(string user1Id, string user2Id);
        Task<IEnumerable<SharedAccount>> GetPendingInvitationsAsync(string userId);
    }
}