using System.ComponentModel.DataAnnotations;
using PiggyBank.Core.Entities;

namespace PiggyBank.Application.DTOs
{
    public class RecurringBillDto
    {
        public int Id { get; set; }
        public int SharedAccountId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public BillFrequency Frequency { get; set; }
        public BillPaidBy PaidBy { get; set; }
        public int DayOfMonth { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<RecurringBillPaymentDto> Payments { get; set; } = new();
    }

    public class RecurringBillPaymentDto
    {
        public int Id { get; set; }
        public int RecurringBillId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public bool IsPaid { get; set; }
        public UserInfoDto? PaidByUser { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? Note { get; set; }
    }

    public class CreateRecurringBillDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public BillFrequency Frequency { get; set; } = BillFrequency.Monthly;
        public BillPaidBy PaidBy { get; set; } = BillPaidBy.Split50;

        [Range(1, 31)]
        public int DayOfMonth { get; set; } = 1;
    }

    public class MarkBillPaidDto
    {
        public string? Note { get; set; }
    }
}
