namespace PiggyBank.Core.Entities
{
    public class SharedBudget
    {
        public int Id { get; set; }
        public int SharedAccountId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual SharedAccount SharedAccount { get; set; } = null!;
        public virtual ICollection<SharedBudgetCategory> Categories { get; set; } = new List<SharedBudgetCategory>();
    }

    public class SharedBudgetCategory
    {
        public int Id { get; set; }
        public int SharedBudgetId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Icon { get; set; } = "📦";
        public decimal AllocatedAmount { get; set; }

        // Navigation properties
        public virtual SharedBudget SharedBudget { get; set; } = null!;
    }
}
