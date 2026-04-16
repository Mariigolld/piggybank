using PiggyBank.Application.DTOs;

namespace PiggyBank.Application.Services
{
    public interface IGroupExpenseService
    {
        Task<IEnumerable<GroupExpenseDto>> GetGroupExpensesAsync(int groupId, string userId);
        Task<GroupExpenseDto> GetByIdAsync(int expenseId, string userId);
        Task<GroupExpenseDto> CreateAsync(CreateGroupExpenseDto dto, string userId);
        Task<GroupExpenseDto> CreateWithSelectiveSplitAsync(CreateGroupExpenseWithSplitDto dto, string userId);
        Task DeleteAsync(int expenseId, string userId);
        Task MarkShareAsPaidAsync(int shareId, string userId);
    }
}