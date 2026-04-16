using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PiggyBank.Core.Entities;

namespace PiggyBank.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<SharedAccount> SharedAccounts { get; set; }
        public DbSet<SavingsGoal> SavingsGoals { get; set; }
        public DbSet<SavingsContribution> SavingsContributions { get; set; }
        public DbSet<RecurringBill> RecurringBills { get; set; }
        public DbSet<RecurringBillPayment> RecurringBillPayments { get; set; }
        public DbSet<SharedBudget> SharedBudgets { get; set; }
        public DbSet<SharedBudgetCategory> SharedBudgetCategories { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<GroupExpense> GroupExpenses { get; set; }
        public DbSet<ExpenseShare> ExpenseShares { get; set; }
        public DbSet<GroupSettlement> GroupSettlements { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // BankAccount configuration
            builder.Entity<BankAccount>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.BankName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Currency).HasMaxLength(10);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.BankAccounts)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.AccountNumber).IsUnique();
            });

            // Category configuration
            builder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Icon).HasMaxLength(10);
                entity.Property(e => e.Color).HasMaxLength(20);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Categories)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Transaction configuration
            builder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);

                entity.HasOne(e => e.BankAccount)
                    .WithMany(b => b.Transactions)
                    .HasForeignKey(e => e.BankAccountId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Transactions)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Date);
            });

            // SharedAccount configuration
            builder.Entity<SharedAccount>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

                entity.HasOne(e => e.User1)
                    .WithMany(u => u.SharedAccountsAsUser1)
                    .HasForeignKey(e => e.User1Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User2)
                    .WithMany(u => u.SharedAccountsAsUser2)
                    .HasForeignKey(e => e.User2Id)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Group configuration
            builder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

                entity.HasOne(e => e.CreatedBy)
                    .WithMany(u => u.CreatedGroups)
                    .HasForeignKey(e => e.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // GroupMember configuration
            builder.Entity<GroupMember>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Group)
                    .WithMany(g => g.Members)
                    .HasForeignKey(e => e.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.GroupMemberships)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.GroupId, e.UserId }).IsUnique();
            });

            // GroupExpense configuration
            builder.Entity<GroupExpense>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);

                entity.HasOne(e => e.Group)
                    .WithMany(g => g.Expenses)
                    .HasForeignKey(e => e.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.PaidBy)
                    .WithMany()
                    .HasForeignKey(e => e.PaidByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ExpenseShare configuration
            builder.Entity<ExpenseShare>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ShareAmount).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.GroupExpense)
                    .WithMany(ge => ge.Shares)
                    .HasForeignKey(e => e.GroupExpenseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.GroupMember)
                    .WithMany(gm => gm.ExpenseShares)
                    .HasForeignKey(e => e.GroupMemberId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // GroupSettlement configuration
            builder.Entity<GroupSettlement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.HasOne(e => e.Group)
                    .WithMany(g => g.Settlements)
                    .HasForeignKey(e => e.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.FromUser)
                    .WithMany(u => u.GroupSettlementsFrom)
                    .HasForeignKey(e => e.FromUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ToUser)
                    .WithMany(u => u.GroupSettlementsTo)
                    .HasForeignKey(e => e.ToUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // SavingsGoal configuration
            builder.Entity<SavingsGoal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.TargetAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Icon).HasMaxLength(10);

                entity.HasOne(e => e.SharedAccount)
                    .WithMany(sa => sa.SavingsGoals)
                    .HasForeignKey(e => e.SharedAccountId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // SavingsContribution configuration
            builder.Entity<SavingsContribution>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Note).HasMaxLength(500);

                entity.HasOne(e => e.SavingsGoal)
                    .WithMany(g => g.Contributions)
                    .HasForeignKey(e => e.SavingsGoalId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ContributedBy)
                    .WithMany()
                    .HasForeignKey(e => e.ContributedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // RecurringBill configuration
            builder.Entity<RecurringBill>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.SharedAccount)
                    .WithMany(sa => sa.RecurringBills)
                    .HasForeignKey(e => e.SharedAccountId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // RecurringBillPayment configuration
            builder.Entity<RecurringBillPayment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Note).HasMaxLength(500);

                entity.HasOne(e => e.RecurringBill)
                    .WithMany(b => b.Payments)
                    .HasForeignKey(e => e.RecurringBillId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.PaidByUser)
                    .WithMany()
                    .HasForeignKey(e => e.PaidByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.RecurringBillId, e.Month, e.Year }).IsUnique();
            });

            // SharedBudget configuration
            builder.Entity<SharedBudget>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.HasOne(e => e.SharedAccount)
                    .WithMany(sa => sa.SharedBudgets)
                    .HasForeignKey(e => e.SharedAccountId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.SharedAccountId, e.Month, e.Year }).IsUnique();
            });

            // SharedBudgetCategory configuration
            builder.Entity<SharedBudgetCategory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Icon).HasMaxLength(10);
                entity.Property(e => e.AllocatedAmount).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.SharedBudget)
                    .WithMany(b => b.Categories)
                    .HasForeignKey(e => e.SharedBudgetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed default categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Entertainment", Icon = "🎉", Color = "#EC4899", IsShared = true },
                new Category { Id = 2, Name = "Groceries", Icon = "🛒", Color = "#10B981", IsShared = true },
                new Category { Id = 3, Name = "Bills", Icon = "💡", Color = "#F59E0B", IsShared = true },
                new Category { Id = 4, Name = "Transport", Icon = "🚗", Color = "#3B82F6", IsShared = true },
                new Category { Id = 5, Name = "Health", Icon = "🏥", Color = "#EF4444", IsShared = true },
                new Category { Id = 6, Name = "Restaurant", Icon = "🍽️", Color = "#8B5CF6", IsShared = true },
                new Category { Id = 7, Name = "Other", Icon = "📦", Color = "#6B7280", IsShared = true }
            );
        }
    }
}