namespace PiggyBank.Core.Entities
{
    public class GroupExpense
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string PaidByUserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
        public string? Receipt { get; set; }

        // Navigation properties
        public virtual Group Group { get; set; } = null!;
        public virtual User PaidBy { get; set; } = null!;
        public virtual ICollection<ExpenseShare> Shares { get; set; } = new List<ExpenseShare>();
    }
}
