using PiggyBank.Application.DTOs;

namespace PiggyBank.Application.Services
{
    public interface IRecurringBillService
    {
        Task<IEnumerable<RecurringBillDto>> GetBySharedAccountAsync(int sharedAccountId, string userId);
        Task<RecurringBillDto> GetByIdAsync(int billId, string userId);
        Task<RecurringBillDto> CreateAsync(int sharedAccountId, CreateRecurringBillDto dto, string userId);
        Task<RecurringBillPaymentDto> MarkPaymentPaidAsync(int billId, int month, int year, string userId, MarkBillPaidDto dto);
        Task<RecurringBillPaymentDto> MarkPaymentUnpaidAsync(int billId, int month, int year, string userId);
        Task DeactivateAsync(int billId, string userId);
        Task ReactivateAsync(int billId, string userId);
        Task DeleteAsync(int billId, string userId);
    }
}
