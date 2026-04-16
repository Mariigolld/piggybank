using Microsoft.AspNetCore.Identity;

namespace PiggyBank.Core.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
        public virtual ICollection<SharedAccount> SharedAccountsAsUser1 { get; set; } = new List<SharedAccount>();
        public virtual ICollection<SharedAccount> SharedAccountsAsUser2 { get; set; } = new List<SharedAccount>();
        public virtual ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
        public virtual ICollection<Group> CreatedGroups { get; set; } = new List<Group>();

        // Group settlement navigation properties
        public virtual ICollection<GroupSettlement> GroupSettlementsFrom { get; set; } = new List<GroupSettlement>();
        public virtual ICollection<GroupSettlement> GroupSettlementsTo { get; set; } = new List<GroupSettlement>();
    }
}