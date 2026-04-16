using PiggyBank.Application.DTOs;

namespace PiggyBank.Application.Services
{
    public interface ISharedAccountService
    {
        Task<IEnumerable<SharedAccountDto>> GetUserSharedAccountsAsync(string userId);
        Task<IEnumerable<SharedAccountDto>> GetPendingInvitationsAsync(string userId);
        Task<SharedAccountDto> CreateAsync(CreateSharedAccountDto dto, string userId);
        Task<SharedAccountDto> JoinByCodeAsync(string inviteCode, string userId);
        Task DeleteAsync(int sharedAccountId, string userId);
    }
}
