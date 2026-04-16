using PiggyBank.Application.DTOs;

namespace PiggyBank.Application.Services
{
    public interface ISavingsGoalService
    {
        Task<IEnumerable<SavingsGoalDto>> GetBySharedAccountAsync(int sharedAccountId, string userId, bool includeArchived = false);
        Task<SavingsGoalDto> GetByIdAsync(int goalId, string userId);
        Task<SavingsGoalDto> CreateAsync(int sharedAccountId, CreateSavingsGoalDto dto, string userId);
        Task<SavingsGoalDto> AddContributionAsync(int goalId, AddContributionDto dto, string userId);
        Task ArchiveAsync(int goalId, string userId);
        Task DeleteAsync(int goalId, string userId);
    }
}
