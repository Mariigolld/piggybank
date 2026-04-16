using System.ComponentModel.DataAnnotations;

namespace PiggyBank.Application.DTOs
{
    public class SharedBudgetDto
    {
        public int Id { get; set; }
        public int SharedAccountId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<SharedBudgetCategoryDto> Categories { get; set; } = new();
        public decimal TotalAllocated { get; set; }
        public decimal TotalSpent { get; set; }
    }

    public class SharedBudgetCategoryDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Icon { get; set; } = "📦";
        public decimal AllocatedAmount { get; set; }
        public decimal SpentAmount { get; set; }
    }

    public class CreateSharedBudgetDto
    {
        [Required]
        [Range(1, 12)]
        public int Month { get; set; }

        [Required]
        [Range(2020, 2100)]
        public int Year { get; set; }

        public string? Notes { get; set; }

        public List<CreateSharedBudgetCategoryDto> Categories { get; set; } = new();
    }

    public class CreateSharedBudgetCategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        public string Icon { get; set; } = "📦";

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Allocated amount must be greater than 0")]
        public decimal AllocatedAmount { get; set; }
    }
}
