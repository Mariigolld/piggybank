using System.ComponentModel.DataAnnotations;

namespace PiggyBank.Application.DTOs
{
    public class GroupExpenseDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public UserInfoDto PaidBy { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
        public List<ExpenseShareDto> Shares { get; set; } = new();
    }

    public class CreateGroupExpenseDto
    {
        [Required]
        public int GroupId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public DateTime? Date { get; set; }

        public string? Notes { get; set; }

        // If null, split equally among all members
        public List<ManualShareDto>? ManualShares { get; set; }
    }

    public class ManualShareDto
    {
        [Required]
        public int GroupMemberId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal ShareAmount { get; set; }
    }

    public class ExpenseShareDto
    {
        public int Id { get; set; }
        public UserInfoDto User { get; set; } = null!;
        public decimal ShareAmount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }
    }

    public class SettlementDto
    {
        public UserInfoDto From { get; set; } = null!;
        public UserInfoDto To { get; set; } = null!;
        public decimal Amount { get; set; }
    }

    public class GroupSettlementsDto
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public List<MemberBalanceDto> MemberBalances { get; set; } = new();
        public List<SettlementDto> Settlements { get; set; } = new();
    }

    public class MemberBalanceDto
    {
        public UserInfoDto User { get; set; } = null!;
        public decimal TotalPaid { get; set; }
        public decimal TotalOwed { get; set; }
        public decimal Balance { get; set; }
    }

    public class GroupSettlementDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public UserInfoDto FromUser { get; set; } = null!;
        public UserInfoDto ToUser { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateGroupSettlementDto
    {
        [Required]
        public int GroupId { get; set; }

        [Required]
        public string ToUserId { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    public class CreateGroupExpenseWithSplitDto
    {
        [Required]
        public int GroupId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public DateTime? Date { get; set; }

        public string? Notes { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one member must be selected")]
        public List<int> MemberIds { get; set; } = new();
    }
}
