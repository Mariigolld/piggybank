using PiggyBank.Application.DTOs;

namespace PiggyBank.Application.Services
{
    public interface ITransactionService
    {
        Task<PagedResultDto<TransactionDto>> GetPagedAsync(string userId, int page, int pageSize);
        Task<TransactionDto> GetByIdAsync(int id, string userId);
        Task<TransactionDto> CreateAsync(CreateTransactionDto dto, string userId);
        Task DeleteAsync(int id, string userId);
        Task<IEnumerable<TransactionDto>> GetByDateRangeAsync(string userId, DateTime from, DateTime to);
    }
}