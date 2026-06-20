namespace MoneyMirror.Core.DTOs.Report
{
    // ==================== SHARED ====================

    public class ReportFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    // ==================== 1. SUMMARY ====================

    public class SpendingSummaryDto
    {
        public decimal TotalSpent { get; set; }
        public int TotalExpenseCount { get; set; }
        public decimal AverageDailySpend { get; set; }
        public decimal BiggestSingleExpense { get; set; }
        public string? BiggestExpenseCategory { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    // ==================== 2. CATEGORY BREAKDOWN ====================

    public class CategoryBreakdownItemDto
    {
        public string CategoryName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Percentage { get; set; }
        public int ExpenseCount { get; set; }
    }

    public class CategoryBreakdownDto
    {
        public string Description { get; set; }
        public decimal GrandTotal { get; set; }
        public List<CategoryBreakdownItemDto> Categories { get; set; }
    }

    // ==================== 3. MOOD SPENDING ====================

    public class MoodSpendingItemDto
    {
        public string MoodDescription { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Percentage { get; set; }
        public int ExpenseCount { get; set; }
    }

    public class MoodSpendingDto
    {
        public string Description { get; set; }
        public decimal GrandTotal { get; set; }
        public List<MoodSpendingItemDto> Moods { get; set; }
    }

    // ==================== 4. TIME PATTERNS ====================

    public class DayOfWeekSpendingDto
    {
        public string DayName { get; set; }   // "Monday", "Tuesday", etc.
        public decimal TotalAmount { get; set; }
        public int ExpenseCount { get; set; }
    }

    public class TimePatternDto
    {
        public string Description { get; set; }
        public decimal WeekdayTotal { get; set; }
        public decimal WeekendTotal { get; set; }
        public decimal WeekdayPercentage { get; set; }
        public decimal WeekendPercentage { get; set; }
        public List<DayOfWeekSpendingDto> DailyBreakdown { get; set; }
    }

    // ==================== 6. BALANCE HISTORY ====================

    public class BalanceHistoryPointDto
    {
        public DateTime Date { get; set; }
        public decimal BalanceAfter { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }

    public class BalanceHistoryDto
    {
        public string Description { get; set; }
        public decimal OpeningBalance { get; set; }   // balance before first transaction in range
        public decimal ClosingBalance { get; set; }   // balance after last transaction in range
        public List<BalanceHistoryPointDto> Points { get; set; }
    }

    // ==================== 7. GOAL REPORT ====================

    public class CompletedGoalSummaryDto
    {
        public string Title { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal? RewardEarned { get; set; }
        public bool WasChallenge { get; set; }
    }

    public class GoalReportDto
    {
        public string Description { get; set; }
        public int TotalCreated { get; set; }
        public int TotalCompleted { get; set; }
        public int TotalFailed { get; set; }
        public int TotalActive { get; set; }
        public decimal TotalSavedAcrossAllGoals { get; set; }
        public decimal AverageCompletionPercentage { get; set; }
        public decimal TotalRewardsEarned { get; set; }
        public List<CompletedGoalSummaryDto> CompletedGoals { get; set; }
    }

    // ==================== 8. PDF REQUEST ====================

    public class PdfDownloadRequestDto
    {
        // Valid values: "summary", "category-breakdown", "mood-spending",
        //               "time-patterns", "top-categories", "balance-history", "goal-report"
        public List<string> Sections { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}