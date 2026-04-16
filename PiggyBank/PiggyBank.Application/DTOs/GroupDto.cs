using System.ComponentModel.DataAnnotations;
using PiggyBank.Core.Entities;

namespace PiggyBank.Application.DTOs
{
    public class GroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public UserInfoDto CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int MemberCount { get; set; }
        public decimal TotalExpenses { get; set; }
        public string? ImageUrl { get; set; }
        public string InviteCode { get; set; } = string.Empty;
    }

    public class GroupDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public UserInfoDto CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public List<GroupMemberDto> Members { get; set; } = new();
        public List<GroupExpenseDto> RecentExpenses { get; set; } = new();
        public decimal TotalExpenses { get; set; }
        public string? ImageUrl { get; set; }
        public string InviteCode { get; set; } = string.Empty;
    }

    public class CreateGroupDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
    }

    public class GroupMemberDto
    {
        public int Id { get; set; }
        public UserInfoDto User { get; set; } = null!;
        public DateTime JoinedAt { get; set; }
        public GroupMemberStatus Status { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class InviteToGroupDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
