namespace PiggyBank.Core.Entities
{
    public class RecurringBill
    {
        public int Id { get; set; }
        public int SharedAccountId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public BillFrequency Frequency { get; set; } = BillFrequency.Monthly;
        public BillPaidBy PaidBy { get; set; } = BillPaidBy.Split50;
        public int DayOfMonth { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual SharedAccount SharedAccount { get; set; } = null!;
        public virtual ICollection<RecurringBillPayment> Payments { get; set; } = new List<RecurringBillPayment>();
    }

    public class RecurringBillPayment
    {
        public int Id { get; set; }
        public int RecurringBillId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public bool IsPaid { get; set; } = false;
        public string? PaidByUserId { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? Note { get; set; }

        // Navigation properties
        public virtual RecurringBill RecurringBill { get; set; } = null!;
        public virtual User? PaidByUser { get; set; }
    }

    public enum BillFrequency
    {
        Weekly = 0,
        Monthly = 1,
        Yearly = 2
    }

    public enum BillPaidBy
    {
        User1 = 0,
        User2 = 1,
        Alternating = 2,
        Split50 = 3
    }
}
