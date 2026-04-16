using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PiggyBank.Core.Entities;
using PiggyBank.Core.Interfaces;
using PiggyBank.Application.DTOs;
using PiggyBank.Infrastructure.Data;

namespace PiggyBank.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public GroupService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IEnumerable<GroupDto>> GetUserGroupsAsync(string userId)
        {
            var groups = await _unitOfWork.Groups.GetUserGroupsAsync(userId);
            return _mapper.Map<IEnumerable<GroupDto>>(groups);
        }

        public async Task<GroupDetailsDto> GetGroupDetailsAsync(int groupId, string userId)
        {
            var group = await _unitOfWork.Groups.GetWithMembersAsync(groupId);

            if (group == null)
            {
                throw new KeyNotFoundException("Grupa nije pronađena");
            }

            var isMember = group.CreatedById == userId ||
                          group.Members.Any(m => m.UserId == userId &&
                                               m.Status == GroupMemberStatus.Accepted);

            if (!isMember)
            {
                throw new UnauthorizedAccessException("Niste član ove grupe");
            }

            var expenses = await _unitOfWork.GroupExpenses.GetByGroupIdAsync(groupId);
            var groupDetails = _mapper.Map<GroupDetailsDto>(group);
            groupDetails.RecentExpenses = _mapper.Map<List<GroupExpenseDto>>(
                expenses.OrderByDescending(e => e.Date).Take(10));

            return groupDetails;
        }

        public async Task<GroupDto> CreateAsync(CreateGroupDto dto, string userId)
        {
            var group = _mapper.Map<Group>(dto);
            group.CreatedById = userId;
            group.CreatedAt = DateTime.UtcNow;
            group.InviteCode = GenerateInviteCode();

            await _unitOfWork.Groups.AddAsync(group);
            await _unitOfWork.SaveChangesAsync();

            // Add creator as admin member
            var creatorMember = new GroupMember
            {
                GroupId = group.Id,
                UserId = userId,
                Status = GroupMemberStatus.Accepted,
                IsAdmin = true,
                JoinedAt = DateTime.UtcNow
            };

            group.Members.Add(creatorMember);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<GroupDto>(group);
        }

        public async Task DeleteAsync(int groupId, string userId)
        {
            var group = await _unitOfWork.Groups.GetWithMembersAsync(groupId);

            if (group == null)
            {
                throw new KeyNotFoundException("Grupa nije pronađena");
            }

            // Only the creator or admin can delete the group
            var isCreator = group.CreatedById == userId;
            var isAdmin = group.Members.Any(m => m.UserId == userId && m.IsAdmin && m.Status == GroupMemberStatus.Accepted);

            if (!isCreator && !isAdmin)
            {
                throw new UnauthorizedAccessException("Samo kreator ili admin može obrisati grupu");
            }

            // Delete associated settlements
            var settlements = await _context.GroupSettlements
                .Where(s => s.GroupId == groupId)
                .ToListAsync();
            _context.GroupSettlements.RemoveRange(settlements);

            // Delete associated expenses and their shares (cascade should handle shares)
            var expenses = await _context.Set<GroupExpense>()
                .Where(e => e.GroupId == groupId)
                .ToListAsync();
            _context.Set<GroupExpense>().RemoveRange(expenses);

            // Delete the group (cascade should handle members)
            await _unitOfWork.Groups.DeleteAsync(group);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<GroupDto> JoinByCodeAsync(string inviteCode, string userId)
        {
            var groups = await _unitOfWork.Groups.FindAsync(g => g.InviteCode == inviteCode && g.IsActive);
            var group = groups.FirstOrDefault();

            if (group == null)
            {
                throw new KeyNotFoundException("Nevažeći pozivni kod");
            }

            // Load members
            group = await _unitOfWork.Groups.GetWithMembersAsync(group.Id);

            // Check if already a member
            var existingMember = group!.Members.FirstOrDefault(m => m.UserId == userId);
            if (existingMember != null)
            {
                if (existingMember.Status == GroupMemberStatus.Accepted)
                {
                    throw new InvalidOperationException("Već ste član ove grupe");
                }
                // If pending, accept
                existingMember.Status = GroupMemberStatus.Accepted;
            }
            else
            {
                // Add as new member
                var newMember = new GroupMember
                {
                    GroupId = group.Id,
                    UserId = userId,
                    Status = GroupMemberStatus.Accepted,
                    IsAdmin = false,
                    JoinedAt = DateTime.UtcNow
                };
                group.Members.Add(newMember);
            }

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<GroupDto>(group);
        }

        public async Task<GroupSettlementsDto> CalculateSettlementsAsync(int groupId, string userId)
        {
            var group = await _unitOfWork.Groups.GetWithExpensesAsync(groupId);

            if (group == null)
            {
                throw new KeyNotFoundException("Grupa nije pronađena");
            }

            var isMember = group.CreatedById == userId ||
                          group.Members.Any(m => m.UserId == userId &&
                                               m.Status == GroupMemberStatus.Accepted);

            if (!isMember)
            {
                throw new UnauthorizedAccessException("Niste član ove grupe");
            }

            // Calculate balances
            var memberBalances = new Dictionary<string, decimal>();
            var members = await _unitOfWork.Groups.GetWithMembersAsync(groupId);

            foreach (var member in members!.Members.Where(m => m.Status == GroupMemberStatus.Accepted))
            {
                memberBalances[member.UserId] = 0;
            }

            foreach (var expense in group.Expenses)
            {
                // Payer paid the full amount
                if (memberBalances.ContainsKey(expense.PaidByUserId))
                {
                    memberBalances[expense.PaidByUserId] += expense.Amount;
                }

                // Each member owes their share
                foreach (var share in expense.Shares)
                {
                    var shareUserId = share.GroupMember.UserId;
                    if (memberBalances.ContainsKey(shareUserId))
                    {
                        memberBalances[shareUserId] -= share.ShareAmount;
                    }
                }
            }

            // Factor in recorded settlements
            var recordedSettlements = await _context.GroupSettlements
                .Where(s => s.GroupId == groupId)
                .ToListAsync();

            foreach (var settlement in recordedSettlements)
            {
                // FromUser paid ToUser, so FromUser's balance decreases (they owe less)
                // and ToUser's balance decreases (they're owed less)
                if (memberBalances.ContainsKey(settlement.FromUserId))
                {
                    memberBalances[settlement.FromUserId] += settlement.Amount;
                }
                if (memberBalances.ContainsKey(settlement.ToUserId))
                {
                    memberBalances[settlement.ToUserId] -= settlement.Amount;
                }
            }

            // Create settlements (simplified debt resolution)
            var settlements = new List<SettlementDto>();
            var creditors = memberBalances.Where(b => b.Value > 0).OrderByDescending(b => b.Value).ToList();
            var debtors = memberBalances.Where(b => b.Value < 0).OrderBy(b => b.Value).ToList();

            int i = 0, j = 0;
            while (i < creditors.Count && j < debtors.Count)
            {
                var creditor = creditors[i];
                var debtor = debtors[j];
                var amount = Math.Min(creditor.Value, Math.Abs(debtor.Value));

                if (amount > 0.01m)
                {
                    var creditorUser = await _userManager.FindByIdAsync(creditor.Key);
                    var debtorUser = await _userManager.FindByIdAsync(debtor.Key);

                    settlements.Add(new SettlementDto
                    {
                        From = _mapper.Map<UserInfoDto>(debtorUser),
                        To = _mapper.Map<UserInfoDto>(creditorUser),
                        Amount = amount
                    });
                }

                creditors[i] = new KeyValuePair<string, decimal>(creditor.Key, creditor.Value - amount);
                debtors[j] = new KeyValuePair<string, decimal>(debtor.Key, debtor.Value + amount);

                if (creditors[i].Value < 0.01m) i++;
                if (Math.Abs(debtors[j].Value) < 0.01m) j++;
            }

            return new GroupSettlementsDto
            {
                GroupId = groupId,
                GroupName = group.Name,
                Settlements = settlements
            };
        }

        public async Task<GroupSettlementDto> RecordSettlementAsync(CreateGroupSettlementDto dto, string userId)
        {
            var group = await _unitOfWork.Groups.GetWithMembersAsync(dto.GroupId);

            if (group == null)
            {
                throw new KeyNotFoundException("Grupa nije pronađena");
            }

            var isMember = group.CreatedById == userId ||
                          group.Members.Any(m => m.UserId == userId &&
                                               m.Status == GroupMemberStatus.Accepted);

            if (!isMember)
            {
                throw new UnauthorizedAccessException("Niste član ove grupe");
            }

            // Verify ToUser is also a member
            var toUserIsMember = group.CreatedById == dto.ToUserId ||
                                group.Members.Any(m => m.UserId == dto.ToUserId &&
                                                     m.Status == GroupMemberStatus.Accepted);

            if (!toUserIsMember)
            {
                throw new InvalidOperationException("Korisnik kome plaćate nije član grupe");
            }

            if (userId == dto.ToUserId)
            {
                throw new InvalidOperationException("Ne možete platiti sami sebi");
            }

            var settlement = new GroupSettlement
            {
                GroupId = dto.GroupId,
                FromUserId = userId,
                ToUserId = dto.ToUserId,
                Amount = dto.Amount,
                Date = DateTime.UtcNow,
                Notes = dto.Notes
            };

            await _context.GroupSettlements.AddAsync(settlement);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            settlement = await _context.GroupSettlements
                .Include(s => s.FromUser)
                .Include(s => s.ToUser)
                .FirstOrDefaultAsync(s => s.Id == settlement.Id);

            return _mapper.Map<GroupSettlementDto>(settlement!);
        }

        public async Task<IEnumerable<GroupSettlementDto>> GetGroupSettlementsAsync(int groupId, string userId)
        {
            var group = await _unitOfWork.Groups.GetWithMembersAsync(groupId);

            if (group == null)
            {
                throw new KeyNotFoundException("Grupa nije pronađena");
            }

            var isMember = group.CreatedById == userId ||
                          group.Members.Any(m => m.UserId == userId &&
                                               m.Status == GroupMemberStatus.Accepted);

            if (!isMember)
            {
                throw new UnauthorizedAccessException("Niste član ove grupe");
            }

            var settlements = await _context.GroupSettlements
                .Include(s => s.FromUser)
                .Include(s => s.ToUser)
                .Where(s => s.GroupId == groupId)
                .OrderByDescending(s => s.Date)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GroupSettlementDto>>(settlements);
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
