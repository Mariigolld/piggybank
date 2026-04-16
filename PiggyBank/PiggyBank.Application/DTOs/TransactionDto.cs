using System.ComponentModel.DataAnnotations;
using PiggyBank.Core.Entities;

namespace PiggyBank.Application.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int BankAccountId { get; set; }
        public string BankAccountName { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryIcon { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateTransactionDto
    {
        [Required]
        public int BankAccountId { get; set; }

        public int? CategoryId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public DateTime? Date { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        public string? Notes { get; set; }
    }

    public class PagedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}