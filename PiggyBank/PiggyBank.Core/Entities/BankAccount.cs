namespace PiggyBank.Core.Entities
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "RSD";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public bool IncludeInSharedBudget { get; set; } = false;

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
