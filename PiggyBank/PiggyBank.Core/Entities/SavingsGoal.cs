namespace PiggyBank.Core.Entities
{
    public class SavingsGoal
    {
        public int Id { get; set; }
        public int SharedAccountId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal TargetAmount { get; set; }
        public string Icon { get; set; } = "🎯";
        public DateTime? TargetDate { get; set; }
        public bool IsArchived { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual SharedAccount SharedAccount { get; set; } = null!;
        public virtual ICollection<SavingsContribution> Contributions { get; set; } = new List<SavingsContribution>();
    }

    public class SavingsContribution
    {
        public int Id { get; set; }
        public int SavingsGoalId { get; set; }
        public string ContributedByUserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Note { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual SavingsGoal SavingsGoal { get; set; } = null!;
        public virtual User ContributedBy { get; set; } = null!;
    }
}
