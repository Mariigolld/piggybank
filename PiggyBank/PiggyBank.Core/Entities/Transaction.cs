namespace PiggyBank.Core.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public int BankAccountId { get; set; }
        public int? CategoryId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public TransactionType Type { get; set; }
        public string? Notes { get; set; }

        // Navigation properties
        public virtual BankAccount BankAccount { get; set; } = null!;
        public virtual Category? Category { get; set; }
    }

    public enum TransactionType
    {
        Expense = 0,
        Income = 1
    }
}

