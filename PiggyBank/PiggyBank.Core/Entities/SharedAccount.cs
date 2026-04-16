namespace PiggyBank.Core.Entities
{
    public class SharedAccount
    {
        public int Id { get; set; }
        public string User1Id { get; set; } = string.Empty;
        public string? User2Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public SharedAccountStatus Status { get; set; } = SharedAccountStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? AcceptedAt { get; set; }
        public string InviteCode { get; set; } = string.Empty;

        // Navigation properties
        public virtual User User1 { get; set; } = null!;
        public virtual User? User2 { get; set; }
        public virtual ICollection<SavingsGoal> SavingsGoals { get; set; } = new List<SavingsGoal>();
        public virtual ICollection<RecurringBill> RecurringBills { get; set; } = new List<RecurringBill>();
        public virtual ICollection<SharedBudget> SharedBudgets { get; set; } = new List<SharedBudget>();
    }

    public enum SharedAccountStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2
    }
}