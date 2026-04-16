using System.ComponentModel.DataAnnotations;

namespace PiggyBank.Application.DTOs
{
    public class SavingsGoalDto
    {
        public int Id { get; set; }
        public int SharedAccountId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal TargetAmount { get; set; }
        public string Icon { get; set; } = "🎯";
        public DateTime? TargetDate { get; set; }
        public bool IsArchived { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal CurrentAmount { get; set; }
        public List<SavingsContributionDto> Contributions { get; set; } = new();
    }

    public class SavingsContributionDto
    {
        public int Id { get; set; }
        public int SavingsGoalId { get; set; }
        public UserInfoDto ContributedBy { get; set; } = null!;
        public decimal Amount { get; set; }
        public string? Note { get; set; }
        public DateTime Date { get; set; }
    }

    public class CreateSavingsGoalDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Target amount must be greater than 0")]
        public decimal TargetAmount { get; set; }

        public string Icon { get; set; } = "🎯";
        public DateTime? TargetDate { get; set; }
    }

    public class AddContributionDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public string? Note { get; set; }
        public DateTime? Date { get; set; }
    }
}
