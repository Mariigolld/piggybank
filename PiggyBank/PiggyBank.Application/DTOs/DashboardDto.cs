namespace PiggyBank.Application.DTOs
{
    public class DashboardDto
    {
        public decimal TotalBalance { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public int ActiveAccounts { get; set; }
        public List<CategoryExpenseSummaryDto> CategoryBreakdown { get; set; } = new();
        public List<TransactionDto> RecentTransactions { get; set; } = new();
        public List<GroupDto> ActiveGroups { get; set; } = new();
    }

    public class CategoryExpenseSummaryDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryIcon { get; set; } = string.Empty;
        public string CategoryColor { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
    }
}