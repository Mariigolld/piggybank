using Microsoft.EntityFrameworkCore;
using PiggyBank.Core.Entities;
using PiggyBank.Core.Interfaces;
using PiggyBank.Application.DTOs;
using PiggyBank.Infrastructure.Data;
using AutoMapper;

namespace PiggyBank.Application.Services
{
    public class SharedBudgetService : ISharedBudgetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;

        public SharedBudgetService(IUnitOfWork unitOfWork, ApplicationDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<SharedBudgetDto>> GetBySharedAccountAsync(int sharedAccountId, string userId)
        {
            var account = await VerifySharedAccountAccessAsync(sharedAccountId, userId);
            var budgets = await _unitOfWork.SharedBudgets.GetBySharedAccountIdAsync(sharedAccountId);

            var dtos = new List<SharedBudgetDto>();
            foreach (var budget in budgets)
            {
                dtos.Add(await MapBudgetToDtoAsync(budget, account));
            }
            return dtos;
        }

        public async Task<SharedBudgetDto?> GetByMonthYearAsync(int sharedAccountId, int month, int year, string userId)
        {
            var account = await VerifySharedAccountAccessAsync(sharedAccountId, userId);
            var budget = await _unitOfWork.SharedBudgets.GetByMonthYearAsync(sharedAccountId, month, year);
            if (budget == null) return null;
            return await MapBudgetToDtoAsync(budget, account);
        }

        public async Task<SharedBudgetDto> CreateOrUpdateAsync(int sharedAccountId, CreateSharedBudgetDto dto, string userId)
        {
            var account = await VerifySharedAccountAccessAsync(sharedAccountId, userId);

            var existing = await _unitOfWork.SharedBudgets.GetByMonthYearAsync(sharedAccountId, dto.Month, dto.Year);

            if (existing != null)
            {
                // Update: remove old categories and replace
                existing.Notes = dto.Notes;
                existing.Categories.Clear();
                foreach (var cat in dto.Categories)
                {
                    existing.Categories.Add(new SharedBudgetCategory
                    {
                        CategoryName = cat.CategoryName,
                        Icon = cat.Icon,
                        AllocatedAmount = cat.AllocatedAmount
                    });
                }
                await _unitOfWork.SaveChangesAsync();

                var updated = await _unitOfWork.SharedBudgets.GetByMonthYearAsync(sharedAccountId, dto.Month, dto.Year);
                return await MapBudgetToDtoAsync(updated!, account);
            }
            else
            {
                var budget = new SharedBudget
                {
                    SharedAccountId = sharedAccountId,
                    Month = dto.Month,
                    Year = dto.Year,
                    Notes = dto.Notes,
                    CreatedAt = DateTime.UtcNow,
                    Categories = dto.Categories.Select(cat => new SharedBudgetCategory
                    {
                        CategoryName = cat.CategoryName,
                        Icon = cat.Icon,
                        AllocatedAmount = cat.AllocatedAmount
                    }).ToList()
                };

                await _unitOfWork.SharedBudgets.AddAsync(budget);
                await _unitOfWork.SaveChangesAsync();

                var created = await _unitOfWork.SharedBudgets.GetByMonthYearAsync(sharedAccountId, dto.Month, dto.Year);
                return await MapBudgetToDtoAsync(created!, account);
            }
        }

        public async Task DeleteAsync(int budgetId, string userId)
        {
            var budget = await _unitOfWork.SharedBudgets.GetByIdAsync(budgetId)
                ?? throw new KeyNotFoundException("Budget not found");
            await VerifySharedAccountAccessAsync(budget.SharedAccountId, userId);

            await _unitOfWork.SharedBudgets.DeleteAsync(budget);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<SharedAccount> VerifySharedAccountAccessAsync(int sharedAccountId, string userId)
        {
            var account = await _unitOfWork.SharedAccounts.GetByIdAsync(sharedAccountId)
                ?? throw new KeyNotFoundException("Shared account not found");
            if (account.User1Id != userId && account.User2Id != userId)
                throw new UnauthorizedAccessException("You do not have access to this shared account");
            return account;
        }

        private async Task<SharedBudgetDto> MapBudgetToDtoAsync(SharedBudget budget, SharedAccount account)
        {
            // Compute actual spending from both users' transactions for this month/year
            var userIds = new[] { account.User1Id, account.User2Id }.Where(id => id != null).ToList();

            var startDate = new DateTime(budget.Year, budget.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endDate = startDate.AddMonths(1);

            // Get transactions only from accounts explicitly included in shared budget
            var transactions = await _dbContext.Transactions
                .Include(t => t.BankAccount)
                .Include(t => t.Category)
                .Where(t => t.BankAccount != null &&
                            t.BankAccount.IncludeInSharedBudget &&
                            userIds.Contains(t.BankAccount.UserId) &&
                            t.Type == TransactionType.Expense &&
                            t.Date >= startDate && t.Date < endDate)
                .ToListAsync();

            var categoryDtos = budget.Categories.Select(cat =>
            {
                // Sum spending for this category name (match by category name)
                var spent = transactions
                    .Where(t => t.Category != null &&
                                string.Equals(t.Category.Name, cat.CategoryName, StringComparison.OrdinalIgnoreCase))
                    .Sum(t => t.Amount);

                return new SharedBudgetCategoryDto
                {
                    Id = cat.Id,
                    CategoryName = cat.CategoryName,
                    Icon = cat.Icon,
                    AllocatedAmount = cat.AllocatedAmount,
                    SpentAmount = spent
                };
            }).ToList();

            return new SharedBudgetDto
            {
                Id = budget.Id,
                SharedAccountId = budget.SharedAccountId,
                Month = budget.Month,
                Year = budget.Year,
                Notes = budget.Notes,
                CreatedAt = budget.CreatedAt,
                Categories = categoryDtos,
                TotalAllocated = categoryDtos.Sum(c => c.AllocatedAmount),
                TotalSpent = categoryDtos.Sum(c => c.SpentAmount)
            };
        }
    }
}
