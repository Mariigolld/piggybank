namespace PiggyBank.Core.Entities
{
    public class GroupMember
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public GroupMemberStatus Status { get; set; } = GroupMemberStatus.Pending;
        public bool IsAdmin { get; set; } = false;

        // Navigation properties
        public virtual Group Group { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<ExpenseShare> ExpenseShares { get; set; } = new List<ExpenseShare>();
    }

    public enum GroupMemberStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2,
        Left = 3
    }
}
