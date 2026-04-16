using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PiggyBank.Application.DTOs;
using PiggyBank.Core.Entities;
using PiggyBank.Core.Interfaces;

namespace PiggyBank.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private const string BANK_ACCOUNTS_CACHE_PREFIX = "BankAccounts_";

        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<PagedResultDto<TransactionDto>> GetPagedAsync(string userId, int page, int pageSize)
        {
            var (items, totalCount) = await _unitOfWork.Transactions
                .GetPagedAsync(userId, page, pageSize);

            return new PagedResultDto<TransactionDto>
            {
                Items = _mapper.Map<IEnumerable<TransactionDto>>(items),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<TransactionDto> GetByIdAsync(int id, string userId)
        {
            var transaction = await _unitOfWork.Transactions.GetByIdAsync(id) ?? throw new KeyNotFoundException("Transakcija nije pronađena");
            var bankAccount = await _unitOfWork.BankAccounts.GetByIdAsync(transaction.BankAccountId);
            if (bankAccount?.UserId != userId)
            {
                throw new UnauthorizedAccessException("Nemate pristup ovoj transakciji");
            }

            return _mapper.Map<TransactionDto>(transaction);
        }

        public async Task<TransactionDto> CreateAsync(CreateTransactionDto dto, string userId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Load account and authorize
                var account = await _unitOfWork.BankAccounts.GetByIdAsync(dto.BankAccountId);
                if (account == null || account.UserId != userId)
                    throw new KeyNotFoundException("Račun nije pronađen.");

                // Optional: prevent overdraft
                if (dto.Type == TransactionType.Expense && account.Balance - dto.Amount < 0)
                    throw new InvalidOperationException("Nema dovoljno sredstava na računu.");

                var tx = new Transaction
                {
                    BankAccountId = dto.BankAccountId,
                    CategoryId = dto.CategoryId,
                    Amount = dto.Amount,
                    Description = dto.Description,
                    Date = dto.Date ?? DateTime.UtcNow,
                    Type = dto.Type,
                    Notes = dto.Notes
                };

                // Update balance
                if (dto.Type == TransactionType.Expense) account.Balance -= dto.Amount;
                else account.Balance += dto.Amount;

                // Persist both
                await _unitOfWork.Transactions.AddAsync(tx);
                await _unitOfWork.BankAccounts.UpdateAsync(account);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Clear the bank accounts cache so balance updates are reflected
                _cache.Remove($"{BANK_ACCOUNTS_CACHE_PREFIX}{userId}");

                return _mapper.Map<TransactionDto>(tx);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }


        public async Task DeleteAsync(int id, string userId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var tx = await _unitOfWork.Transactions.GetByIdAsync(id) ?? throw new KeyNotFoundException("Transakcija nije pronađena.");
                var account = await _unitOfWork.BankAccounts.GetByIdAsync(tx.BankAccountId);
                if (account == null || account.UserId != userId)
                    throw new UnauthorizedAccessException();

                // Reverse balance
                if (tx.Type == TransactionType.Expense) account.Balance += tx.Amount;
                else account.Balance -= tx.Amount;

                await _unitOfWork.BankAccounts.UpdateAsync(account);
                await _unitOfWork.Transactions.DeleteAsync(tx);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Clear the bank accounts cache so balance updates are reflected
                _cache.Remove($"{BANK_ACCOUNTS_CACHE_PREFIX}{userId}");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }


        public async Task<IEnumerable<TransactionDto>> GetByDateRangeAsync(
            string userId, DateTime from, DateTime to)
        {
            var transactions = await _unitOfWork.Transactions
                .GetByDateRangeAsync(userId, from, to);
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        }
    }
}