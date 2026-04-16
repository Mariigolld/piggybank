using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using PiggyBank.Core.Entities;
using PiggyBank.Core.Interfaces;
using PiggyBank.Application.DTOs;

namespace PiggyBank.Application.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private const string CACHE_KEY_PREFIX = "BankAccounts_";

        public BankAccountService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<BankAccountDto>> GetUserAccountsAsync(string userId)
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}{userId}";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<BankAccountDto>? cachedAccounts))
            {
                var accounts = await _unitOfWork.BankAccounts.GetByUserIdAsync(userId);
                cachedAccounts = _mapper.Map<IEnumerable<BankAccountDto>>(accounts);

                _cache.Set(cacheKey, cachedAccounts, TimeSpan.FromMinutes(10));
            }

            return cachedAccounts!;
        }

        public async Task<BankAccountDto> GetByIdAsync(int id, string userId)
        {
            var account = await _unitOfWork.BankAccounts.GetByIdAsync(id);

            if (account == null || account.UserId != userId)
            {
                throw new UnauthorizedAccessException("Račun nije pronađen ili nemate pristup");
            }

            return _mapper.Map<BankAccountDto>(account);
        }

        public async Task<BankAccountDto> CreateAsync(CreateBankAccountDto dto, string userId)
        {
            var account = new BankAccount
            {
                UserId = userId,
                BankName = dto.Name,
                AccountNumber = dto.Name,
                Balance = dto.InitialBalance,
                Currency = dto.Currency,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IncludeInSharedBudget = dto.IncludeInSharedBudget
            };

            await _unitOfWork.BankAccounts.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();

            _cache.Remove($"{CACHE_KEY_PREFIX}{userId}");

            return _mapper.Map<BankAccountDto>(account);
        }

        public async Task<BankAccountDto> UpdateAsync(int id, UpdateBankAccountDto dto, string userId)
        {
            var account = await _unitOfWork.BankAccounts.GetByIdAsync(id);

            if (account == null || account.UserId != userId)
            {
                throw new UnauthorizedAccessException("Račun nije pronađen ili nemate pristup");
            }

            account.BankName = dto.Name;
            account.AccountNumber = dto.Name;
            await _unitOfWork.BankAccounts.UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();

            _cache.Remove($"{CACHE_KEY_PREFIX}{userId}");

            return _mapper.Map<BankAccountDto>(account);
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var account = await _unitOfWork.BankAccounts.GetByIdAsync(id);

            if (account == null || account.UserId != userId)
            {
                throw new UnauthorizedAccessException("Račun nije pronađen ili nemate pristup");
            }

            account.IsActive = false;
            await _unitOfWork.BankAccounts.UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();

            _cache.Remove($"{CACHE_KEY_PREFIX}{userId}");
        }

        public async Task<decimal> GetTotalBalanceAsync(string userId)
        {
            return await _unitOfWork.BankAccounts.GetTotalBalanceAsync(userId);
        }

        public async Task<BankAccountDto> ToggleSharedBudgetAsync(int id, string userId)
        {
            var account = await _unitOfWork.BankAccounts.GetByIdAsync(id);
            if (account == null || account.UserId != userId)
                throw new UnauthorizedAccessException("Račun nije pronađen ili nemate pristup");

            account.IncludeInSharedBudget = !account.IncludeInSharedBudget;
            await _unitOfWork.BankAccounts.UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();

            _cache.Remove($"{CACHE_KEY_PREFIX}{userId}");
            return _mapper.Map<BankAccountDto>(account);
        }
    }
}