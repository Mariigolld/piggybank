using System.ComponentModel.DataAnnotations;
using PiggyBank.Core.Entities;

namespace PiggyBank.Application.DTOs
{
    public class SharedAccountDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public UserInfoDto User1 { get; set; } = null!;
        public UserInfoDto? User2 { get; set; }
        public SharedAccountStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string InviteCode { get; set; } = string.Empty;
    }

    public class CreateSharedAccountDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
    }

    public class UserInfoDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}