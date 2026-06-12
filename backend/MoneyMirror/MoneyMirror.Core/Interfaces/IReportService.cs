using MoneyMirror.Core.DTOs.Report;

namespace MoneyMirror.Core.Interfaces
{
    public interface IReportService
    {
        Task<(bool success, SpendingSummaryDto? data, string errorMessage)>
            GetSpendingSummaryAsync(int parentId, int childId, DateTime? startDate, DateTime? endDate);

        Task<(bool success, CategoryBreakdownDto? data, string errorMessage)>
            GetCategoryBreakdownAsync(int parentId, int childId, DateTime? startDate, DateTime? endDate);

        Task<(bool success, MoodSpendingDto? data, string errorMessage)>
            GetMoodSpendingAsync(int parentId, int childId, DateTime? startDate, DateTime? endDate);

        Task<(bool success, TimePatternDto? data, string errorMessage)>
            GetTimePatternAsync(int parentId, int childId, DateTime? startDate, DateTime? endDate);

        Task<(bool success, TopCategoriesDto? data, string errorMessage)>
            GetTopCategoriesAsync(int parentId, int childId, DateTime? startDate, DateTime? endDate, int topN = 3);

        Task<(bool success, BalanceHistoryDto? data, string errorMessage)>
            GetBalanceHistoryAsync(int parentId, int childId, DateTime? startDate, DateTime? endDate);

        Task<(bool success, GoalReportDto? data, string errorMessage)>
            GetGoalReportAsync(int parentId, int childId, DateTime? startDate, DateTime? endDate);

        Task<(bool success, byte[]? pdfBytes, string errorMessage)>
            GeneratePdfAsync(int parentId, int childId, PdfDownloadRequestDto dto);
    }
}