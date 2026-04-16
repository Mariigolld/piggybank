using PiggyBank.Core.Entities;

namespace PiggyBank.Core.Interfaces
{
    public interface IGroupRepository : IRepository<Group>
    {
        Task<IEnumerable<Group>> GetUserGroupsAsync(string userId);
        Task<Group?> GetWithMembersAsync(int groupId);
        Task<Group?> GetWithExpensesAsync(int groupId);
    }
}