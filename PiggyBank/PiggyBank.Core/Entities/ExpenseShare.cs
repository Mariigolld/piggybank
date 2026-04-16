namespace PiggyBank.Core.Entities
{
    public class ExpenseShare
    {
        public int Id { get; set; }
        public int GroupExpenseId { get; set; }
        public int GroupMemberId { get; set; }
        public decimal ShareAmount { get; set; }
        public bool IsPaid { get; set; } = false;
        public DateTime? PaidAt { get; set; }

        // Navigation properties
        public virtual GroupExpense GroupExpense { get; set; } = null!;
        public virtual GroupMember GroupMember { get; set; } = null!;
    }
}
