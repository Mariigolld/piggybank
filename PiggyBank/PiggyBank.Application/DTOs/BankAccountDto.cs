using System.ComponentModel.DataAnnotations;

namespace PiggyBank.Application.DTOs
{
    public class BankAccountDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "RSD";
        public DateTime CreatedAt { get; set; }
        public bool IncludeInSharedBudget { get; set; }
    }

    public class CreateBankAccountDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal InitialBalance { get; set; }

        [MaxLength(10)]
        public string Currency { get; set; } = "RSD";

        public bool IncludeInSharedBudget { get; set; } = false;
    }

    public class UpdateBankAccountDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
