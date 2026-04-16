using PiggyBank.Core.Entities;

namespace PiggyBank.Core.Interfaces
{
    public interface ISavingsGoalRepository : IRepository<SavingsGoal>
    {
        Task<IEnumerable<SavingsGoal>> GetBySharedAccountIdAsync(int sharedAccountId, bool includeArchived = false);
        Task<SavingsGoal?> GetWithContributionsAsync(int goalId);
    }
}
