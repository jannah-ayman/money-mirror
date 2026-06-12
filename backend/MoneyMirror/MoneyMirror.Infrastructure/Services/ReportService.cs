using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Report;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Infrastructure.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MoneyMirror.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReportService> _logger;

        public ReportService(ApplicationDbContext context, ILogger<ReportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private async Task<bool> IsParentLinkedToChildAsync(int parentId, int childId) =>
            await _context.ParentChildren.AnyAsync(pc => pc.ParentID == parentId && pc.ChildID == childId);

        // ==================== 1. SUMMARY ====================

        public async Task<(bool success, SpendingSummaryDto? data, string errorMessage)>
            GetSpendingSummaryAsync(int parentId, int childId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (!await IsParentLinkedToChildAsync(parentId, childId))
                    return (false, null, "Not authorized for this child.");

                var query = _context.Expenses
                    .Include(e => e.ExpenseCategory)
                    .Where(e => e.ChildID == childId);

                if (startDate.HasValue) query = query.Where(e => e.LogDate >= startDate.Value);
                if (endDate.HasValue) query = query.Where(e => e.LogDate <= endDate.Value);

                var expenses = await query.ToListAsync();

                if (!expenses.Any())
                    return (true, new SpendingSummaryDto { StartDate = startDate, EndDate = endDate }, string.Empty);

                var totalSpent = expenses.Sum(e => e.MoneyAmount);
                var biggest = expenses.OrderByDescending(e => e.MoneyAmount).First();

                int days = startDate.HasValue && endDate.HasValue
                    ? Math.Max(1, (endDate.Value - startDate.Value).Days + 1)
                    : Math.Max(1, (expenses.Max(e => e.LogDate) - expenses.Min(e => e.LogDate)).Days + 1);

                return (true, new SpendingSummaryDto
                {
                    TotalSpent = totalSpent,
                    TotalExpenseCount = expenses.Count,
                    AverageDailySpend = Math.Round(totalSpent / days, 2),
                    BiggestSingleExpense = biggest.MoneyAmount,
                    BiggestExpenseCategory = biggest.ExpenseCategory?.Name,
                    StartDate = startDate,
                    EndDate = endDate
                }, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetSpendingSummaryAsync for child {ChildId}", childId);
                return (false, null, "An error occurred.");
            }
        }

        // ==================== 2. CATEGORY BREAKDOWN ====================

        public async Task<(bool success, CategoryBreakdownDto? data, string errorMessage)>
            GetCategoryBreakdownAsync(int parentId, int childId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (!await IsParentLinkedToChildAsync(parentId, childId))
                    return (false, null, "Not authorized for this child.");

                var query = _context.Expenses
                    .Include(e => e.ExpenseCategory)
                    .Where(e => e.ChildID == childId);

                if (startDate.HasValue) query = query.Where(e => e.LogDate >= startDate.Value);
                if (endDate.HasValue) query = query.Where(e => e.LogDate <= endDate.Value);

                var expenses = await query.ToListAsync();
                var grandTotal = expenses.Sum(e => e.MoneyAmount);

                var categories = expenses
                    .GroupBy(e => e.ExpenseCategory?.Name ?? "Uncategorized")
                    .Select(g => new CategoryBreakdownItemDto
                    {
                        CategoryName = g.Key,
                        TotalAmount = g.Sum(e => e.MoneyAmount),
                        ExpenseCount = g.Count(),
                        Percentage = grandTotal > 0
                            ? Math.Round((g.Sum(e => e.MoneyAmount) / grandTotal) * 100, 1)
                            : 0
                    })
                    .OrderByDescending(c => c.TotalAmount)
                    .ToList();

                return (true, new CategoryBreakdownDto { GrandTotal = grandTotal, Categories = categories }, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCategoryBreakdownAsync for child {ChildId}", childId);
                return (false, null, "An error occurred.");
            }
        }

        // ==================== 3. MOOD SPENDING ====================

        public async Task<(bool success, MoodSpendingDto? data, string errorMessage)>
            GetMoodSpendingAsync(int parentId, int childId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (!await IsParentLinkedToChildAsync(parentId, childId))
                    return (false, null, "Not authorized for this child.");

                var query = _context.Expenses
                    .Include(e => e.Mood)
                    .Where(e => e.ChildID == childId);

                if (startDate.HasValue) query = query.Where(e => e.LogDate >= startDate.Value);
                if (endDate.HasValue) query = query.Where(e => e.LogDate <= endDate.Value);

                var expenses = await query.ToListAsync();
                var grandTotal = expenses.Sum(e => e.MoneyAmount);

                var moods = expenses
                    .GroupBy(e => e.Mood?.Description ?? "Unknown")
                    .Select(g => new MoodSpendingItemDto
                    {
                        MoodDescription = g.Key,
                        TotalAmount = g.Sum(e => e.MoneyAmount),
                        ExpenseCount = g.Count(),
                        Percentage = grandTotal > 0
                            ? Math.Round((g.Sum(e => e.MoneyAmount) / grandTotal) * 100, 1)
                            : 0
                    })
                    .OrderByDescending(m => m.TotalAmount)
                    .ToList();

                return (true, new MoodSpendingDto { GrandTotal = grandTotal, Moods = moods }, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetMoodSpendingAsync for child {ChildId}", childId);
                return (false, null, "An error occurred.");
            }
        }

        // ==================== 4. TIME PATTERNS ====================

        public async Task<(bool success, TimePatternDto? data, string errorMessage)>
            GetTimePatternAsync(int parentId, int childId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (!await IsParentLinkedToChildAsync(parentId, childId))
                    return (false, null, "Not authorized for this child.");

                var query = _context.Expenses.Where(e => e.ChildID == childId);

                if (startDate.HasValue) query = query.Where(e => e.LogDate >= startDate.Value);
                if (endDate.HasValue) query = query.Where(e => e.LogDate <= endDate.Value);

                var expenses = await query.ToListAsync();

                var weekendDays = new[] { DayOfWeek.Friday, DayOfWeek.Saturday };

                var weekdayTotal = expenses
                    .Where(e => !weekendDays.Contains(e.LogDate.DayOfWeek))
                    .Sum(e => e.MoneyAmount);

                var weekendTotal = expenses
                    .Where(e => weekendDays.Contains(e.LogDate.DayOfWeek))
                    .Sum(e => e.MoneyAmount);

                var grandTotal = weekdayTotal + weekendTotal;

                var orderedDays = new[]
                {
                    DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday,
                    DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday
                };

                var daily = orderedDays.Select(day => new DayOfWeekSpendingDto
                {
                    DayName = day.ToString(),
                    TotalAmount = expenses.Where(e => e.LogDate.DayOfWeek == day).Sum(e => e.MoneyAmount),
                    ExpenseCount = expenses.Count(e => e.LogDate.DayOfWeek == day)
                }).ToList();

                return (true, new TimePatternDto
                {
                    WeekdayTotal = weekdayTotal,
                    WeekendTotal = weekendTotal,
                    WeekdayPercentage = grandTotal > 0 ? Math.Round((weekdayTotal / grandTotal) * 100, 1) : 0,
                    WeekendPercentage = grandTotal > 0 ? Math.Round((weekendTotal / grandTotal) * 100, 1) : 0,
                    DailyBreakdown = daily
                }, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTimePatternAsync for child {ChildId}", childId);
                return (false, null, "An error occurred.");
            }
        }

        // ==================== 5. TOP CATEGORIES ====================

        public async Task<(bool success, TopCategoriesDto? data, string errorMessage)>
            GetTopCategoriesAsync(int parentId, int childId, DateTime? startDate, DateTime? endDate, int topN = 3)
        {
            try
            {
                if (!await IsParentLinkedToChildAsync(parentId, childId))
                    return (false, null, "Not authorized for this child.");

                var query = _context.Expenses
                    .Include(e => e.ExpenseCategory)
                    .Where(e => e.ChildID == childId);

                if (startDate.HasValue) query = query.Where(e => e.LogDate >= startDate.Value);
                if (endDate.HasValue) query = query.Where(e => e.LogDate <= endDate.Value);

                var expenses = await query.ToListAsync();

                var grouped = expenses
                    .GroupBy(e => e.ExpenseCategory?.Name ?? "Uncategorized")
                    .Select(g => new { Name = g.Key, Total = g.Sum(e => e.MoneyAmount), Count = g.Count() })
                    .OrderByDescending(g => g.Total)
                    .Take(topN)
                    .ToList();

                var topAmount = grouped.FirstOrDefault()?.Total ?? 1;

                var result = grouped.Select((g, i) => new TopCategoryItemDto
                {
                    Rank = i + 1,
                    CategoryName = g.Name,
                    TotalAmount = g.Total,
                    ExpenseCount = g.Count,
                    PercentageOfTop = Math.Round((g.Total / topAmount) * 100, 1)
                }).ToList();

                return (true, new TopCategoriesDto { Categories = result }, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTopCategoriesAsync for child {ChildId}", childId);
                return (false, null, "An error occurred.");
            }
        }

        // ==================== 6. BALANCE HISTORY ====================

        public async Task<(bool success, BalanceHistoryDto? data, string errorMessage)>
            GetBalanceHistoryAsync(int parentId, int childId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (!await IsParentLinkedToChildAsync(parentId, childId))
                    return (false, null, "Not authorized for this child.");

                var query = _context.Transactions.Where(t => t.ChildID == childId);

                if (startDate.HasValue) query = query.Where(t => t.TransactionDate >= startDate.Value);
                if (endDate.HasValue) query = query.Where(t => t.TransactionDate <= endDate.Value);

                var transactions = await query
                    .OrderBy(t => t.TransactionDate)
                    .ToListAsync();

                // Opening balance = BalanceAfter of the transaction just before the range
                decimal openingBalance = 0;
                if (startDate.HasValue)
                {
                    var beforeRange = await _context.Transactions
                        .Where(t => t.ChildID == childId && t.TransactionDate < startDate.Value)
                        .OrderByDescending(t => t.TransactionDate)
                        .Select(t => (decimal?)t.BalanceAfter)
                        .FirstOrDefaultAsync();

                    openingBalance = beforeRange ?? 0;
                }

                var points = transactions.Select(t => new BalanceHistoryPointDto
                {
                    Date = t.TransactionDate,
                    BalanceAfter = t.BalanceAfter,
                    TransactionType = t.Type,
                    Amount = t.Amount,
                    Description = t.Description
                }).ToList();

                var closingBalance = points.LastOrDefault()?.BalanceAfter ?? openingBalance;

                return (true, new BalanceHistoryDto
                {
                    OpeningBalance = openingBalance,
                    ClosingBalance = closingBalance,
                    Points = points
                }, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetBalanceHistoryAsync for child {ChildId}", childId);
                return (false, null, "An error occurred.");
            }
        }

        // ==================== 7. GOAL REPORT ====================

        public async Task<(bool success, GoalReportDto? data, string errorMessage)>
            GetGoalReportAsync(int parentId, int childId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (!await IsParentLinkedToChildAsync(parentId, childId))
                    return (false, null, "Not authorized for this child.");

                var query = _context.SavingsGoals.Where(g => g.ChildID == childId);

                if (startDate.HasValue) query = query.Where(g => g.StartDate >= startDate.Value);
                if (endDate.HasValue) query = query.Where(g => g.StartDate <= endDate.Value);

                var goals = await query.ToListAsync();

                var completed = goals.Where(g => g.Status == "Success").ToList();

                var avgCompletion = goals.Any()
                    ? Math.Round(goals.Average(g => g.TargetAmount > 0
                        ? Math.Min((g.CurrentAmount / g.TargetAmount) * 100, 100)
                        : 0), 1)
                    : 0;

                var completedSummaries = completed.Select(g => new CompletedGoalSummaryDto
                {
                    Title = g.Title,
                    TargetAmount = g.TargetAmount,
                    RewardEarned = g.RewardValue,
                    WasChallenge = g.IsChallenge
                }).ToList();

                return (true, new GoalReportDto
                {
                    TotalCreated = goals.Count,
                    TotalCompleted = completed.Count,
                    TotalFailed = goals.Count(g => g.Status == "Failure"),
                    TotalActive = goals.Count(g => g.Status == "Active"),
                    TotalSavedAcrossAllGoals = goals.Sum(g => g.CurrentAmount),
                    AverageCompletionPercentage = avgCompletion,
                    TotalRewardsEarned = completed.Sum(g => g.RewardValue ?? 0),
                    CompletedGoals = completedSummaries
                }, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetGoalReportAsync for child {ChildId}", childId);
                return (false, null, "An error occurred.");
            }
        }

        // ==================== 8. PDF GENERATION ====================

        public async Task<(bool success, byte[]? pdfBytes, string errorMessage)>
            GeneratePdfAsync(int parentId, int childId, PdfDownloadRequestDto dto)
        {
            try
            {
                if (!await IsParentLinkedToChildAsync(parentId, childId))
                    return (false, null, "Not authorized for this child.");

                var child = await _context.Children.FindAsync(childId);
                if (child == null)
                    return (false, null, "Child not found.");

                var sections = dto.Sections ?? new List<string>();

                // Pre-fetch only requested sections
                SpendingSummaryDto? summary = null;
                CategoryBreakdownDto? categoryBreakdown = null;
                MoodSpendingDto? moodSpending = null;
                TimePatternDto? timePattern = null;
                TopCategoriesDto? topCategories = null;
                BalanceHistoryDto? balanceHistory = null;
                GoalReportDto? goalReport = null;

                if (sections.Contains("summary"))
                    (_, summary, _) = await GetSpendingSummaryAsync(parentId, childId, dto.StartDate, dto.EndDate);

                if (sections.Contains("category-breakdown"))
                    (_, categoryBreakdown, _) = await GetCategoryBreakdownAsync(parentId, childId, dto.StartDate, dto.EndDate);

                if (sections.Contains("mood-spending"))
                    (_, moodSpending, _) = await GetMoodSpendingAsync(parentId, childId, dto.StartDate, dto.EndDate);

                if (sections.Contains("time-patterns"))
                    (_, timePattern, _) = await GetTimePatternAsync(parentId, childId, dto.StartDate, dto.EndDate);

                if (sections.Contains("top-categories"))
                    (_, topCategories, _) = await GetTopCategoriesAsync(parentId, childId, dto.StartDate, dto.EndDate, dto.TopN);

                if (sections.Contains("balance-history"))
                    (_, balanceHistory, _) = await GetBalanceHistoryAsync(parentId, childId, dto.StartDate, dto.EndDate);

                if (sections.Contains("goal-report"))
                    (_, goalReport, _) = await GetGoalReportAsync(parentId, childId, dto.StartDate, dto.EndDate);

                QuestPDF.Settings.License = LicenseType.Community;

                var pdfBytes = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(40);
                        page.DefaultTextStyle(t => t.FontSize(11));

                        page.Header().Column(col =>
                        {
                            col.Item().Text($"Money Mirror — {child.FName} {child.LName}")
                                .FontSize(18).Bold();

                            string dateLabel = dto.StartDate.HasValue && dto.EndDate.HasValue
                                ? $"{dto.StartDate.Value:dd MMM yyyy} – {dto.EndDate.Value:dd MMM yyyy}"
                                : "All Time";

                            col.Item().Text($"Report Period: {dateLabel}").FontSize(10).FontColor("#666666");
                            col.Item().PaddingTop(4).LineHorizontal(1).LineColor("#CCCCCC");
                        });

                        page.Content().PaddingTop(16).Column(col =>
                        {
                            // --- Summary ---
                            if (summary != null)
                            {
                                col.Item().Text("Spending Summary").FontSize(14).Bold();
                                col.Item().PaddingTop(6).Table(table =>
                                {
                                    table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                                    AddTableRow(table, "Total Spent", $"{summary.TotalSpent:F2} EGP");
                                    AddTableRow(table, "Total Expenses", summary.TotalExpenseCount.ToString());
                                    AddTableRow(table, "Avg Daily Spend", $"{summary.AverageDailySpend:F2} EGP");
                                    AddTableRow(table, "Biggest Expense", $"{summary.BiggestSingleExpense:F2} EGP ({summary.BiggestExpenseCategory})");
                                });
                                col.Item().PaddingVertical(12).LineHorizontal(1).LineColor("#EEEEEE");
                            }

                            // --- Category Breakdown ---
                            if (categoryBreakdown != null)
                            {
                                col.Item().Text("Category Breakdown").FontSize(14).Bold();
                                col.Item().PaddingTop(6).Table(table =>
                                {
                                    table.ColumnsDefinition(c =>
                                    {
                                        c.RelativeColumn(3);
                                        c.RelativeColumn(2);
                                        c.RelativeColumn(2);
                                        c.RelativeColumn(1);
                                    });
                                    AddTableHeader(table, "Category", "Amount (EGP)", "Percentage", "Count");
                                    foreach (var item in categoryBreakdown.Categories)
                                        AddTableRow(table, item.CategoryName, $"{item.TotalAmount:F2}", $"{item.Percentage}%", item.ExpenseCount.ToString());
                                });
                                col.Item().PaddingVertical(12).LineHorizontal(1).LineColor("#EEEEEE");
                            }

                            // --- Mood Spending ---
                            if (moodSpending != null)
                            {
                                col.Item().Text("Mood Spending").FontSize(14).Bold();
                                col.Item().PaddingTop(6).Table(table =>
                                {
                                    table.ColumnsDefinition(c =>
                                    {
                                        c.RelativeColumn(3);
                                        c.RelativeColumn(2);
                                        c.RelativeColumn(2);
                                        c.RelativeColumn(1);
                                    });
                                    AddTableHeader(table, "Mood", "Amount (EGP)", "Percentage", "Count");
                                    foreach (var item in moodSpending.Moods)
                                        AddTableRow(table, item.MoodDescription, $"{item.TotalAmount:F2}", $"{item.Percentage}%", item.ExpenseCount.ToString());
                                });
                                col.Item().PaddingVertical(12).LineHorizontal(1).LineColor("#EEEEEE");
                            }

                            // --- Time Patterns ---
                            if (timePattern != null)
                            {
                                col.Item().Text("Time Patterns").FontSize(14).Bold();
                                col.Item().PaddingTop(6).Table(table =>
                                {
                                    table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                                    AddTableRow(table, "Weekday Total", $"{timePattern.WeekdayTotal:F2} EGP ({timePattern.WeekdayPercentage}%)");
                                    AddTableRow(table, "Weekend Total", $"{timePattern.WeekendTotal:F2} EGP ({timePattern.WeekendPercentage}%)");
                                });
                                col.Item().PaddingTop(8).Table(table =>
                                {
                                    table.ColumnsDefinition(c => { c.RelativeColumn(3); c.RelativeColumn(2); c.RelativeColumn(1); });
                                    AddTableHeader(table, "Day", "Amount (EGP)", "Count");
                                    foreach (var d in timePattern.DailyBreakdown)
                                        AddTableRow(table, d.DayName, $"{d.TotalAmount:F2}", d.ExpenseCount.ToString());
                                });
                                col.Item().PaddingVertical(12).LineHorizontal(1).LineColor("#EEEEEE");
                            }

                            // --- Top Categories ---
                            if (topCategories != null)
                            {
                                col.Item().Text("Top Categories").FontSize(14).Bold();
                                col.Item().PaddingTop(6).Table(table =>
                                {
                                    table.ColumnsDefinition(c =>
                                    {
                                        c.ConstantColumn(30);
                                        c.RelativeColumn(3);
                                        c.RelativeColumn(2);
                                        c.RelativeColumn(1);
                                    });
                                    AddTableHeader(table, "#", "Category", "Amount (EGP)", "Count");
                                    foreach (var item in topCategories.Categories)
                                        AddTableRow(table, item.Rank.ToString(), item.CategoryName, $"{item.TotalAmount:F2}", item.ExpenseCount.ToString());
                                });
                                col.Item().PaddingVertical(12).LineHorizontal(1).LineColor("#EEEEEE");
                            }

                            // --- Balance History ---
                            if (balanceHistory != null)
                            {
                                col.Item().Text("Balance History").FontSize(14).Bold();
                                col.Item().PaddingTop(4).Table(table =>
                                {
                                    table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                                    AddTableRow(table, "Opening Balance", $"{balanceHistory.OpeningBalance:F2} EGP");
                                    AddTableRow(table, "Closing Balance", $"{balanceHistory.ClosingBalance:F2} EGP");
                                });
                                col.Item().PaddingTop(8).Table(table =>
                                {
                                    table.ColumnsDefinition(c =>
                                    {
                                        c.RelativeColumn(2);
                                        c.RelativeColumn(2);
                                        c.RelativeColumn(2);
                                        c.RelativeColumn(3);
                                    });
                                    AddTableHeader(table, "Date", "Type", "Amount", "Balance After");
                                    foreach (var p in balanceHistory.Points)
                                        AddTableRow(table,
                                            p.Date.ToString("dd MMM yyyy"),
                                            p.TransactionType,
                                            $"{p.Amount:F2}",
                                            $"{p.BalanceAfter:F2} EGP");
                                });
                                col.Item().PaddingVertical(12).LineHorizontal(1).LineColor("#EEEEEE");
                            }

                            // --- Goal Report ---
                            if (goalReport != null)
                            {
                                col.Item().Text("Goal Report").FontSize(14).Bold();
                                col.Item().PaddingTop(6).Table(table =>
                                {
                                    table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                                    AddTableRow(table, "Total Created", goalReport.TotalCreated.ToString());
                                    AddTableRow(table, "Completed", goalReport.TotalCompleted.ToString());
                                    AddTableRow(table, "Failed", goalReport.TotalFailed.ToString());
                                    AddTableRow(table, "Active", goalReport.TotalActive.ToString());
                                    AddTableRow(table, "Total Saved", $"{goalReport.TotalSavedAcrossAllGoals:F2} EGP");
                                    AddTableRow(table, "Avg Completion", $"{goalReport.AverageCompletionPercentage}%");
                                    AddTableRow(table, "Total Rewards Earned", $"{goalReport.TotalRewardsEarned:F2} EGP");
                                });

                                if (goalReport.CompletedGoals.Any())
                                {
                                    col.Item().PaddingTop(8).Text("Completed Goals").Bold();
                                    col.Item().PaddingTop(4).Table(table =>
                                    {
                                        table.ColumnsDefinition(c =>
                                        {
                                            c.RelativeColumn(3);
                                            c.RelativeColumn(2);
                                            c.RelativeColumn(2);
                                            c.RelativeColumn(1);
                                        });
                                        AddTableHeader(table, "Title", "Target (EGP)", "Reward (EGP)", "Challenge");
                                        foreach (var g in goalReport.CompletedGoals)
                                            AddTableRow(table,
                                                g.Title,
                                                $"{g.TargetAmount:F2}",
                                                g.RewardEarned.HasValue ? $"{g.RewardEarned:F2}" : "-",
                                                g.WasChallenge ? "Yes" : "No");
                                    });
                                }
                            }
                        });

                        page.Footer().AlignCenter().Text(text =>
                        {
                            text.Span("Generated by Money Mirror — ").FontSize(9).FontColor("#999999");
                            text.Span(DateTime.UtcNow.ToString("dd MMM yyyy HH:mm") + " UTC").FontSize(9).FontColor("#999999");
                        });
                    });
                }).GeneratePdf();

                return (true, pdfBytes, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF for child {ChildId}", childId);
                return (false, null, "An error occurred while generating the PDF.");
            }
        }

        // ==================== PDF HELPERS ====================

        private static void AddTableHeader(TableDescriptor table, params string[] headers)
        {
            table.Header(header =>
            {
                foreach (var h in headers)
                    header.Cell().Background("#F0F0F0").Padding(4).Text(h).Bold();
            });
        }

        private static void AddTableRow(TableDescriptor table, params string[] values)
        {
            foreach (var v in values)
                table.Cell().BorderBottom(1).BorderColor("#EEEEEE").Padding(4).Text(v);
        }
    }
}