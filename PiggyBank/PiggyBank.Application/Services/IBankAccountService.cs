using PiggyBank.Application.DTOs;

namespace PiggyBank.Application.Services
{
    public interface IBankAccountService
    {
        Task<IEnumerable<BankAccountDto>> GetUserAccountsAsync(string userId);
        Task<BankAccountDto> GetByIdAsync(int id, string userId);
        Task<BankAccountDto> CreateAsync(CreateBankAccountDto dto, string userId);
        Task<BankAccountDto> UpdateAsync(int id, UpdateBankAccountDto dto, string userId);
        Task DeleteAsync(int id, string userId);
        Task<decimal> GetTotalBalanceAsync(string userId);
        Task<BankAccountDto> ToggleSharedBudgetAsync(int id, string userId);
    }
}