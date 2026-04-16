namespace PiggyBank.Core.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CreatedById { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public string? ImageUrl { get; set; }
        public string InviteCode { get; set; } = string.Empty;

        // Navigation properties
        public virtual User CreatedBy { get; set; } = null!;
        public virtual ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
        public virtual ICollection<GroupExpense> Expenses { get; set; } = new List<GroupExpense>();
        public virtual ICollection<GroupSettlement> Settlements { get; set; } = new List<GroupSettlement>();
    }
}