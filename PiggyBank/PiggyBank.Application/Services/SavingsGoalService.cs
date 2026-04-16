using AutoMapper;
using PiggyBank.Core.Entities;
using PiggyBank.Core.Interfaces;
using PiggyBank.Application.DTOs;

namespace PiggyBank.Application.Services
{
    public class SavingsGoalService : ISavingsGoalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SavingsGoalService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SavingsGoalDto>> GetBySharedAccountAsync(int sharedAccountId, string userId, bool includeArchived = false)
        {
            await VerifySharedAccountAccessAsync(sharedAccountId, userId);
            var goals = await _unitOfWork.SavingsGoals.GetBySharedAccountIdAsync(sharedAccountId, includeArchived);
            return goals.Select(MapGoalToDto).ToList();
        }

        public async Task<SavingsGoalDto> GetByIdAsync(int goalId, string userId)
        {
            var goal = await _unitOfWork.SavingsGoals.GetWithContributionsAsync(goalId)
                ?? throw new KeyNotFoundException("Savings goal not found");
            await VerifySharedAccountAccessAsync(goal.SharedAccountId, userId);
            return MapGoalToDto(goal);
        }

        public async Task<SavingsGoalDto> CreateAsync(int sharedAccountId, CreateSavingsGoalDto dto, string userId)
        {
            await VerifySharedAccountAccessAsync(sharedAccountId, userId);

            var goal = new SavingsGoal
            {
                SharedAccountId = sharedAccountId,
                Name = dto.Name,
                Description = dto.Description,
                TargetAmount = dto.TargetAmount,
                Icon = dto.Icon,
                TargetDate = dto.TargetDate,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.SavingsGoals.AddAsync(goal);
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.SavingsGoals.GetWithContributionsAsync(goal.Id);
            return MapGoalToDto(created!);
        }

        public async Task<SavingsGoalDto> AddContributionAsync(int goalId, AddContributionDto dto, string userId)
        {
            var goal = await _unitOfWork.SavingsGoals.GetWithContributionsAsync(goalId)
                ?? throw new KeyNotFoundException("Savings goal not found");
            await VerifySharedAccountAccessAsync(goal.SharedAccountId, userId);

            if (goal.IsArchived)
                throw new InvalidOperationException("Cannot add contribution to an archived goal");

            var contribution = new SavingsContribution
            {
                SavingsGoalId = goalId,
                ContributedByUserId = userId,
                Amount = dto.Amount,
                Note = dto.Note,
                Date = dto.Date ?? DateTime.UtcNow
            };

            await _unitOfWork.SavingsGoals.GetWithContributionsAsync(goalId); // ensure loaded
            goal.Contributions.Add(contribution);
            await _unitOfWork.SaveChangesAsync();

            var updated = await _unitOfWork.SavingsGoals.GetWithContributionsAsync(goalId);
            return MapGoalToDto(updated!);
        }

        public async Task ArchiveAsync(int goalId, string userId)
        {
            var goal = await _unitOfWork.SavingsGoals.GetByIdAsync(goalId)
                ?? throw new KeyNotFoundException("Savings goal not found");
            await VerifySharedAccountAccessAsync(goal.SharedAccountId, userId);

            goal.IsArchived = true;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int goalId, string userId)
        {
            var goal = await _unitOfWork.SavingsGoals.GetWithContributionsAsync(goalId)
                ?? throw new KeyNotFoundException("Savings goal not found");
            await VerifySharedAccountAccessAsync(goal.SharedAccountId, userId);

            if (goal.Contributions.Any())
                throw new InvalidOperationException("Cannot delete a goal that has contributions. Archive it instead.");

            await _unitOfWork.SavingsGoals.DeleteAsync(goal);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task VerifySharedAccountAccessAsync(int sharedAccountId, string userId)
        {
            var account = await _unitOfWork.SharedAccounts.GetByIdAsync(sharedAccountId)
                ?? throw new KeyNotFoundException("Shared account not found");
            if (account.User1Id != userId && account.User2Id != userId)
                throw new UnauthorizedAccessException("You do not have access to this shared account");
        }

        private static SavingsGoalDto MapGoalToDto(SavingsGoal goal)
        {
            var currentAmount = goal.Contributions.Sum(c => c.Amount);
            return new SavingsGoalDto
            {
                Id = goal.Id,
                SharedAccountId = goal.SharedAccountId,
                Name = goal.Name,
                Description = goal.Description,
                TargetAmount = goal.TargetAmount,
                Icon = goal.Icon,
                TargetDate = goal.TargetDate,
                IsArchived = goal.IsArchived,
                CreatedAt = goal.CreatedAt,
                CurrentAmount = currentAmount,
                Contributions = goal.Contributions.OrderByDescending(c => c.Date).Select(c => new SavingsContributionDto
                {
                    Id = c.Id,
                    SavingsGoalId = c.SavingsGoalId,
                    ContributedBy = new UserInfoDto
                    {
                        Id = c.ContributedBy?.Id ?? c.ContributedByUserId,
                        Email = c.ContributedBy?.Email ?? string.Empty,
                        FirstName = c.ContributedBy?.FirstName ?? string.Empty,
                        LastName = c.ContributedBy?.LastName ?? string.Empty
                    },
                    Amount = c.Amount,
                    Note = c.Note,
                    Date = c.Date
                }).ToList()
            };
        }
    }
}
