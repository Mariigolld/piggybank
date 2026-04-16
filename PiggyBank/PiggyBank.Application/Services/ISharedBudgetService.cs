using PiggyBank.Application.DTOs;

namespace PiggyBank.Application.Services
{
    public interface ISharedBudgetService
    {
        Task<IEnumerable<SharedBudgetDto>> GetBySharedAccountAsync(int sharedAccountId, string userId);
        Task<SharedBudgetDto?> GetByMonthYearAsync(int sharedAccountId, int month, int year, string userId);
        Task<SharedBudgetDto> CreateOrUpdateAsync(int sharedAccountId, CreateSharedBudgetDto dto, string userId);
        Task DeleteAsync(int budgetId, string userId);
    }
}
