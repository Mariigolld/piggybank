using AutoMapper;
using PiggyBank.Core.Entities;
using PiggyBank.Application.DTOs;
using PiggyBank.Application.DTOs.Auth;

namespace PiggyBank.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserInfoDto>();

            // BankAccount mappings
            CreateMap<BankAccount, BankAccountDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.BankName));

            // Category mappings
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();

            // Transaction mappings
            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.BankAccountName,
                    opt => opt.MapFrom(src => src.BankAccount.BankName))
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.CategoryIcon,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.Icon : null));

            CreateMap<CreateTransactionDto, Transaction>()
                .ForMember(dest => dest.Date,
                    opt => opt.MapFrom(src => src.Date ?? DateTime.UtcNow));

            // SharedAccount mappings
            CreateMap<SharedAccount, SharedAccountDto>()
                .ForMember(dest => dest.User1, opt => opt.MapFrom(src => src.User1))
                .ForMember(dest => dest.User2, opt => opt.MapFrom(src => src.User2));

            // Group mappings
            CreateMap<Group, GroupDto>()
                .ForMember(dest => dest.MemberCount,
                    opt => opt.MapFrom(src => src.Members.Count(m => m.Status == GroupMemberStatus.Accepted)))
                .ForMember(dest => dest.TotalExpenses,
                    opt => opt.MapFrom(src => src.Expenses.Sum(e => e.Amount)));

            CreateMap<Group, GroupDetailsDto>()
                .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Members))
                .ForMember(dest => dest.RecentExpenses,
                    opt => opt.MapFrom(src => src.Expenses.OrderByDescending(e => e.Date).Take(10)))
                .ForMember(dest => dest.TotalExpenses,
                    opt => opt.MapFrom(src => src.Expenses.Sum(e => e.Amount)));

            CreateMap<CreateGroupDto, Group>();

            // GroupMember mappings
            CreateMap<GroupMember, GroupMemberDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            // GroupExpense mappings
            CreateMap<GroupExpense, GroupExpenseDto>()
                .ForMember(dest => dest.PaidBy, opt => opt.MapFrom(src => src.PaidBy))
                .ForMember(dest => dest.Shares, opt => opt.MapFrom(src => src.Shares));

            CreateMap<CreateGroupExpenseDto, GroupExpense>()
                .ForMember(dest => dest.Date,
                    opt => opt.MapFrom(src => src.Date ?? DateTime.UtcNow));

            // ExpenseShare mappings
            CreateMap<ExpenseShare, ExpenseShareDto>()
                .ForMember(dest => dest.User,
                    opt => opt.MapFrom(src => src.GroupMember.User));

            // GroupSettlement mappings
            CreateMap<GroupSettlement, GroupSettlementDto>()
                .ForMember(dest => dest.FromUser, opt => opt.MapFrom(src => src.FromUser))
                .ForMember(dest => dest.ToUser, opt => opt.MapFrom(src => src.ToUser));
        }
    }
}