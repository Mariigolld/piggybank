using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PiggyBank.Core.Entities;
using PiggyBank.Core.Interfaces;
using PiggyBank.Application.DTOs;

namespace PiggyBank.Application.Services
{
    public class SharedAccountService : ISharedAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public SharedAccountService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IEnumerable<SharedAccountDto>> GetUserSharedAccountsAsync(string userId)
        {
            var sharedAccounts = await _unitOfWork.SharedAccounts.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<SharedAccountDto>>(sharedAccounts);
        }

        public async Task<IEnumerable<SharedAccountDto>> GetPendingInvitationsAsync(string userId)
        {
            // Get shared accounts created by this user that are still pending (no partner joined yet)
            var all = await _unitOfWork.SharedAccounts.FindAsync(s =>
                s.User1Id == userId &&
                s.Status == SharedAccountStatus.Pending &&
                s.User2Id == null);
            return _mapper.Map<IEnumerable<SharedAccountDto>>(all);
        }

        public async Task<SharedAccountDto> CreateAsync(CreateSharedAccountDto dto, string userId)
        {
            var inviteCode = GenerateInviteCode();

            var sharedAccount = new SharedAccount
            {
                User1Id = userId,
                User2Id = null,
                Name = dto.Name,
                Status = SharedAccountStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                InviteCode = inviteCode
            };

            await _unitOfWork.SharedAccounts.AddAsync(sharedAccount);
            await _unitOfWork.SaveChangesAsync();

            // Reload to get navigation properties
            var created = await _unitOfWork.SharedAccounts.GetByIdAsync(sharedAccount.Id);
            return _mapper.Map<SharedAccountDto>(created);
        }

        public async Task<SharedAccountDto> JoinByCodeAsync(string inviteCode, string userId)
        {
            var sharedAccounts = await _unitOfWork.SharedAccounts.FindAsync(s => s.InviteCode == inviteCode);
            var sharedAccount = sharedAccounts.FirstOrDefault();

            if (sharedAccount == null)
            {
                throw new KeyNotFoundException("Invalid invite code");
            }

            if (sharedAccount.User1Id == userId)
            {
                throw new InvalidOperationException("You cannot join your own shared account");
            }

            if (sharedAccount.Status == SharedAccountStatus.Accepted)
            {
                throw new InvalidOperationException("This shared account already has a partner");
            }

            if (sharedAccount.User2Id != null && sharedAccount.User2Id != userId)
            {
                throw new InvalidOperationException("This shared account already has a partner");
            }

            sharedAccount.User2Id = userId;
            sharedAccount.Status = SharedAccountStatus.Accepted;
            sharedAccount.AcceptedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            // Reload to get navigation properties
            var updated = await _unitOfWork.SharedAccounts.GetByIdAsync(sharedAccount.Id);
            return _mapper.Map<SharedAccountDto>(updated);
        }

        public async Task DeleteAsync(int sharedAccountId, string userId)
        {
            var sharedAccount = await _unitOfWork.SharedAccounts.GetByIdAsync(sharedAccountId);

            if (sharedAccount == null)
            {
                throw new KeyNotFoundException("Shared account not found");
            }

            if (sharedAccount.User1Id != userId && sharedAccount.User2Id != userId)
            {
                throw new UnauthorizedAccessException("You do not have access to this shared account");
            }

            await _unitOfWork.SharedAccounts.DeleteAsync(sharedAccount);
            await _unitOfWork.SaveChangesAsync();
        }

        private static string GenerateInviteCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
