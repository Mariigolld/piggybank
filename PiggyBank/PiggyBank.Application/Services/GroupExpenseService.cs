using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PiggyBank.Core.Entities;
using PiggyBank.Core.Interfaces;
using PiggyBank.Application.DTOs;
using PiggyBank.Infrastructure.Data;

namespace PiggyBank.Application.Services
{
    public class GroupExpenseService : IGroupExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public GroupExpenseService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<GroupExpenseDto>> GetGroupExpensesAsync(int groupId, string userId)
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
            return _mapper.Map<IEnumerable<GroupExpenseDto>>(expenses);
        }

        public async Task<GroupExpenseDto> GetByIdAsync(int expenseId, string userId)
        {
            var expense = await _unitOfWork.GroupExpenses.GetWithSharesAsync(expenseId);

            if (expense == null)
            {
                throw new KeyNotFoundException("Trošak nije pronađen");
            }

            var group = await _unitOfWork.Groups.GetWithMembersAsync(expense.GroupId);
            var isMember = group!.CreatedById == userId ||
                          group.Members.Any(m => m.UserId == userId &&
                                               m.Status == GroupMemberStatus.Accepted);

            if (!isMember)
            {
                throw new UnauthorizedAccessException("Niste član ove grupe");
            }

            return _mapper.Map<GroupExpenseDto>(expense);
        }

        public async Task<GroupExpenseDto> CreateAsync(CreateGroupExpenseDto dto, string userId)
        {
            var group = await _unitOfWork.Groups.GetWithMembersAsync(dto.GroupId);

            if (group == null)
            {
                throw new KeyNotFoundException("Grupa nije pronađena");
            }

            var userMember = group.Members.FirstOrDefault(m => m.UserId == userId &&
                                                              m.Status == GroupMemberStatus.Accepted);

            if (userMember == null && group.CreatedById != userId)
            {
                throw new UnauthorizedAccessException("Niste član ove grupe");
            }

            var expense = _mapper.Map<GroupExpense>(dto);
            expense.PaidByUserId = userId;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.GroupExpenses.AddAsync(expense);
                await _unitOfWork.SaveChangesAsync();

                // Create shares - include creator and accepted members
                var activeMembers = group.Members
                    .Where(m => m.Status == GroupMemberStatus.Accepted)
                    .ToList();

                // Check if creator is in members list, if not we need to handle this
                var creatorMember = group.Members.FirstOrDefault(m => m.UserId == group.CreatedById);
                if (creatorMember == null)
                {
                    // Creator is not in members table, create a temporary member entry for them
                    var creatorAsMember = new GroupMember
                    {
                        GroupId = group.Id,
                        UserId = group.CreatedById,
                        Status = GroupMemberStatus.Accepted,
                        IsAdmin = true,
                        JoinedAt = group.CreatedAt
                    };
                    await _context.GroupMembers.AddAsync(creatorAsMember);
                    await _unitOfWork.SaveChangesAsync();
                    activeMembers.Add(creatorAsMember);
                }
                else if (creatorMember.Status == GroupMemberStatus.Accepted && !activeMembers.Contains(creatorMember))
                {
                    activeMembers.Add(creatorMember);
                }

                if (activeMembers.Count == 0)
                {
                    throw new InvalidOperationException("Grupa nema aktivnih članova za podelu troškova");
                }

                if (dto.ManualShares != null && dto.ManualShares.Any())
                {
                    // Use manual shares
                    var totalManualShares = dto.ManualShares.Sum(s => s.ShareAmount);
                    if (Math.Abs(totalManualShares - dto.Amount) > 0.01m)
                    {
                        throw new InvalidOperationException(
                            "Suma manuelnih delova mora biti jednaka ukupnom iznosu");
                    }

                    foreach (var manualShare in dto.ManualShares)
                    {
                        var share = new ExpenseShare
                        {
                            GroupExpenseId = expense.Id,
                            GroupMemberId = manualShare.GroupMemberId,
                            ShareAmount = manualShare.ShareAmount,
                            IsPaid = false
                        };

                        await _context.ExpenseShares.AddAsync(share);
                    }
                }
                else
                {
                    // Split equally
                    var shareAmount = dto.Amount / activeMembers.Count;

                    foreach (var member in activeMembers)
                    {
                        var share = new ExpenseShare
                        {
                            GroupExpenseId = expense.Id,
                            GroupMemberId = member.Id,
                            ShareAmount = shareAmount,
                            IsPaid = member.UserId == userId // Payer is already paid
                        };

                        await _context.ExpenseShares.AddAsync(share);
                    }
                }

                await _unitOfWork.CommitTransactionAsync();

                // Reload with shares
                expense = await _unitOfWork.GroupExpenses.GetWithSharesAsync(expense.Id);
                return _mapper.Map<GroupExpenseDto>(expense!);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<GroupExpenseDto> CreateWithSelectiveSplitAsync(CreateGroupExpenseWithSplitDto dto, string userId)
        {
            var group = await _unitOfWork.Groups.GetWithMembersAsync(dto.GroupId);

            if (group == null)
            {
                throw new KeyNotFoundException("Grupa nije pronađena");
            }

            var userMember = group.Members.FirstOrDefault(m => m.UserId == userId &&
                                                              m.Status == GroupMemberStatus.Accepted);

            if (userMember == null && group.CreatedById != userId)
            {
                throw new UnauthorizedAccessException("Niste član ove grupe");
            }

            if (dto.MemberIds == null || !dto.MemberIds.Any())
            {
                throw new InvalidOperationException("Mora biti izabran barem jedan član za podelu");
            }

            var expense = new GroupExpense
            {
                GroupId = dto.GroupId,
                PaidByUserId = userId,
                Amount = dto.Amount,
                Description = dto.Description,
                Date = dto.Date ?? DateTime.UtcNow,
                Notes = dto.Notes
            };

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.GroupExpenses.AddAsync(expense);
                await _unitOfWork.SaveChangesAsync();

                // Get selected members
                var selectedMembers = group.Members
                    .Where(m => dto.MemberIds.Contains(m.Id) && m.Status == GroupMemberStatus.Accepted)
                    .ToList();

                if (selectedMembers.Count == 0)
                {
                    throw new InvalidOperationException("Nijedan od izabranih članova nije aktivan član grupe");
                }

                // Split equally among selected members
                var shareAmount = dto.Amount / selectedMembers.Count;

                foreach (var member in selectedMembers)
                {
                    var share = new ExpenseShare
                    {
                        GroupExpenseId = expense.Id,
                        GroupMemberId = member.Id,
                        ShareAmount = shareAmount,
                        IsPaid = member.UserId == userId // Payer is already paid
                    };

                    await _context.ExpenseShares.AddAsync(share);
                }

                await _unitOfWork.CommitTransactionAsync();

                // Reload with shares
                expense = await _unitOfWork.GroupExpenses.GetWithSharesAsync(expense.Id);
                return _mapper.Map<GroupExpenseDto>(expense!);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteAsync(int expenseId, string userId)
        {
            var expense = await _unitOfWork.GroupExpenses.GetWithSharesAsync(expenseId);

            if (expense == null)
            {
                throw new KeyNotFoundException("Trošak nije pronađen");
            }

            var group = await _unitOfWork.Groups.GetWithMembersAsync(expense.GroupId);
            var isAdmin = group!.CreatedById == userId ||
                         group.Members.Any(m => m.UserId == userId && m.IsAdmin);

            if (!isAdmin && expense.PaidByUserId != userId)
            {
                throw new UnauthorizedAccessException(
                    "Samo kreator troška ili admin mogu obrisati trošak");
            }

            await _unitOfWork.GroupExpenses.DeleteAsync(expense);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task MarkShareAsPaidAsync(int shareId, string userId)
        {
            var share = await _context.ExpenseShares
                .Include(s => s.GroupMember)
                .Include(s => s.GroupExpense)
                .FirstOrDefaultAsync(s => s.Id == shareId);

            if (share == null)
            {
                throw new KeyNotFoundException("Deo troška nije pronađen");
            }

            var isShareOwner = share.GroupMember.UserId == userId;
            var isExpensePayer = share.GroupExpense.PaidByUserId == userId;
            if (!isShareOwner && !isExpensePayer)
            {
                throw new UnauthorizedAccessException("Ne možete označiti tuđi deo kao plaćen");
            }

            share.IsPaid = true;
            share.PaidAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}