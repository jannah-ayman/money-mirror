using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Allowance;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Core.Models;
using MoneyMirror.Infrastructure.Data;

namespace MoneyMirror.Infrastructure.Services
{
    /// <summary>
    /// Service implementing allowance and balance management logic.
    /// Handles recurring allowances, one-time bonuses, balance updates, and transaction history.
    /// </summary>
    public class AllowanceService : IAllowanceService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AllowanceService> _logger;

        public AllowanceService(
            ApplicationDbContext context,
            ILogger<AllowanceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ==================== HELPER METHOD: VERIFY PARENT-CHILD RELATIONSHIP ====================

        /// <summary>
        /// Verifies that a parent is linked to a child.
        /// Prevents parents from accessing other people's children's data.
        /// </summary>
        private async Task<bool> IsParentLinkedToChildAsync(int parentId, int childId)
        {
            return await _context.ParentChildren
                .AnyAsync(pc => pc.ParentID == parentId && pc.ChildID == childId);
        }

        // ==================== ALLOWANCE CONFIGURATION ====================

        public async Task<(bool success, string message)> UpdateAllowanceAsync(
            int parentId,
            int childId,
            UpdateAllowanceDto dto)
        {
            try
            {
                // STEP 1: Verify parent-child relationship
                bool isLinked = await IsParentLinkedToChildAsync(parentId, childId);

                if (!isLinked)
                {
                    _logger.LogWarning($"Parent {parentId} attempted to modify allowance for non-linked child {childId}");
                    return (false, "You are not authorized to manage this child's allowance");
                }

                // STEP 2: Find existing active recurring allowance (if any)
                var existingAllowance = await _context.Allowances
                    .FirstOrDefaultAsync(a => a.ChildID == childId && a.IsRecurring && a.IsActive);

                // STEP 3: Deactivate old allowance (only one active recurring allowance at a time)
                if (existingAllowance != null)
                {
                    existingAllowance.IsActive = false;
                    _context.Allowances.Update(existingAllowance);

                    _logger.LogInformation($"Deactivated old allowance {existingAllowance.AllowanceID} for child {childId}");
                }

                // STEP 4: Create new allowance record
                var newAllowance = new Allowance
                {
                    Type = dto.Type,
                    Amount = dto.Amount,
                    IsRecurring = true,
                    IsActive = dto.IsActive,
                    DailyHour = dto.Type == "Daily" ? dto.DailyHour : null,
                    WeeklyDay = dto.Type == "Weekly" ? dto.WeeklyDay : null,
                    MonthlyDay = dto.Type == "Monthly" ? dto.MonthlyDay : null,
                    SetDate = DateTime.UtcNow,
                    LastCreditedDate = null, // Not credited yet
                    ChildID = childId,
                    ParentID = parentId
                };

                _context.Allowances.Add(newAllowance);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Created new {dto.Type} allowance {newAllowance.AllowanceID} for child {childId}: {dto.Amount}");

                return (true, "Allowance settings updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating allowance for child {childId}: {ex.Message}");
                return (false, "An error occurred while updating allowance settings");
            }
        }

        public async Task<(bool success, AllowanceDetailsDto? allowance, string errorMessage)> GetAllowanceAsync(
            int parentId,
            int childId)
        {
            try
            {
                // STEP 1: Verify parent-child relationship
                bool isLinked = await IsParentLinkedToChildAsync(parentId, childId);

                if (!isLinked)
                {
                    _logger.LogWarning($"Parent {parentId} attempted to view allowance for non-linked child {childId}");
                    return (false, null, "You are not authorized to view this child's allowance");
                }

                // STEP 2: Find active recurring allowance
                var allowance = await _context.Allowances
                    .FirstOrDefaultAsync(a => a.ChildID == childId && a.IsRecurring && a.IsActive);

                // No allowance set up - return null (not an error)
                if (allowance == null)
                {
                    _logger.LogInformation($"No active allowance found for child {childId}");
                    return (true, null, string.Empty);
                }

                // STEP 3: Build response DTO
                var dto = new AllowanceDetailsDto
                {
                    AllowanceId = allowance.AllowanceID,
                    Type = allowance.Type,
                    Amount = allowance.Amount,
                    IsActive = allowance.IsActive,
                    DailyHour = allowance.DailyHour,
                    WeeklyDay = allowance.WeeklyDay,
                    MonthlyDay = allowance.MonthlyDay,
                    LastCreditedDate = allowance.LastCreditedDate,
                    SetDate = allowance.SetDate
                };

                return (true, dto, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting allowance for child {childId}: {ex.Message}");
                return (false, null, "An error occurred while retrieving allowance settings");
            }
        }

        // ==================== BONUS ====================

        public async Task<(bool success, decimal newBalance, string errorMessage)> GiveBonusAsync(
            int parentId,
            int childId,
            GiveBonusDto dto)
        {
            // Use database transaction to ensure consistency
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // STEP 1: Verify parent-child relationship
                bool isLinked = await IsParentLinkedToChildAsync(parentId, childId);

                if (!isLinked)
                {
                    _logger.LogWarning($"Parent {parentId} attempted to give bonus to non-linked child {childId}");
                    return (false, 0, "You are not authorized to give bonuses to this child");
                }

                // STEP 2: Get child's current balance
                var child = await _context.Children.FindAsync(childId);

                if (child == null)
                {
                    return (false, 0, "Child not found");
                }

                // STEP 3: Update child's balance
                child.CurrentBalance += dto.Amount;
                _context.Children.Update(child);

                // STEP 4: Create transaction record
                var transactionRecord = new Transaction
                {
                    Type = "BonusCredit",
                    Amount = dto.Amount,
                    BalanceAfter = child.CurrentBalance,
                    Description = dto.Reason, // Use parent's reason as description
                    TransactionDate = DateTime.UtcNow,
                    ChildID = childId,
                    ParentID = parentId,
                    AllowanceID = null // Bonuses are not linked to allowance schedules
                };

                _context.Transactions.Add(transactionRecord);

                // STEP 5: Save changes and commit transaction
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Bonus of {dto.Amount} given to child {childId} by parent {parentId}. New balance: {child.CurrentBalance}");

                return (true, child.CurrentBalance, string.Empty);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Error giving bonus to child {childId}: {ex.Message}");
                return (false, 0, "An error occurred while giving the bonus");
            }
        }

        // ==================== BALANCE (PARENT VIEW) ====================

        public async Task<(bool success, BalanceResponseDto? balance, string errorMessage)> GetChildBalanceAsync(
            int parentId,
            int childId)
        {
            try
            {
                // STEP 1: Verify parent-child relationship
                bool isLinked = await IsParentLinkedToChildAsync(parentId, childId);

                if (!isLinked)
                {
                    _logger.LogWarning($"Parent {parentId} attempted to view balance for non-linked child {childId}");
                    return (false, null, "You are not authorized to view this child's balance");
                }

                // STEP 2: Get child's balance
                var child = await _context.Children
                    .Where(c => c.ChildID == childId)
                    .Select(c => new BalanceResponseDto
                    {
                        CurrentBalance = c.CurrentBalance,
                        ChildId = c.ChildID,
                        ChildName = $"{c.FName} {c.LName}"
                    })
                    .FirstOrDefaultAsync();

                if (child == null)
                {
                    return (false, null, "Child not found");
                }

                return (true, child, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting balance for child {childId}: {ex.Message}");
                return (false, null, "An error occurred while retrieving balance");
            }
        }

        // ==================== BALANCE (CHILD VIEW) ====================

        public async Task<(bool success, BalanceResponseDto? balance, string errorMessage)> GetMyBalanceAsync(
            int childId)
        {
            try
            {
                // No authorization check needed - child can only access their own balance (enforced by JWT)

                var child = await _context.Children
                    .Where(c => c.ChildID == childId)
                    .Select(c => new BalanceResponseDto
                    {
                        CurrentBalance = c.CurrentBalance,
                        ChildId = c.ChildID,
                        ChildName = $"{c.FName} {c.LName}"
                    })
                    .FirstOrDefaultAsync();

                if (child == null)
                {
                    return (false, null, "Child not found");
                }

                return (true, child, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting balance for child {childId}: {ex.Message}");
                return (false, null, "An error occurred while retrieving your balance");
            }
        }

        // ==================== TRANSACTION HISTORY (PARENT VIEW) ====================

        public async Task<(bool success, TransactionHistoryDto? history, string errorMessage)> GetTransactionHistoryAsync(
            int parentId,
            int childId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string type = "All")
        {
            try
            {
                // STEP 1: Verify parent-child relationship
                bool isLinked = await IsParentLinkedToChildAsync(parentId, childId);

                if (!isLinked)
                {
                    _logger.LogWarning($"Parent {parentId} attempted to view transactions for non-linked child {childId}");
                    return (false, null, "You are not authorized to view this child's transactions");
                }

                // STEP 2: Build query
                var query = _context.Transactions
                    .Where(t => t.ChildID == childId);

                // Apply filters
                if (startDate.HasValue)
                {
                    query = query.Where(t => t.TransactionDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(t => t.TransactionDate <= endDate.Value);
                }

                if (type != "All")
                {
                    query = query.Where(t => t.Type == type);
                }

                // STEP 3: Execute query and build response
                var transactions = await query
                    .OrderByDescending(t => t.TransactionDate) // Newest first
                    .Select(t => new TransactionDto
                    {
                        TransactionId = t.TransactionID,
                        Type = t.Type,
                        Amount = t.Amount,
                        BalanceAfter = t.BalanceAfter,
                        Description = t.Description,
                        TransactionDate = t.TransactionDate
                    })
                    .ToListAsync();

                // STEP 4: Calculate summary statistics
                var totalCredits = transactions
                    .Where(t => t.Amount > 0) // Only count positive amounts (credits)
                    .Sum(t => t.Amount);

                var history = new TransactionHistoryDto
                {
                    Transactions = transactions,
                    TotalCredits = totalCredits,
                    TotalCount = transactions.Count
                };

                return (true, history, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting transaction history for child {childId}: {ex.Message}");
                return (false, null, "An error occurred while retrieving transaction history");
            }
        }

        // ==================== TRANSACTION HISTORY (CHILD VIEW) ====================

        public async Task<(bool success, TransactionHistoryDto? history, string errorMessage)> GetMyTransactionsAsync(
            int childId,
            DateTime? startDate = null)
        {
            try
            {
                // No authorization check needed - child can only access their own transactions (enforced by JWT)

                // Build query
                var query = _context.Transactions
                    .Where(t => t.ChildID == childId);

                if (startDate.HasValue)
                {
                    query = query.Where(t => t.TransactionDate >= startDate.Value);
                }

                // Execute query
                var transactions = await query
                    .OrderByDescending(t => t.TransactionDate) // Newest first
                    .Select(t => new TransactionDto
                    {
                        TransactionId = t.TransactionID,
                        Type = t.Type,
                        Amount = t.Amount,
                        BalanceAfter = t.BalanceAfter,
                        Description = t.Description,
                        TransactionDate = t.TransactionDate
                    })
                    .ToListAsync();

                var totalCredits = transactions
                    .Where(t => t.Amount > 0)
                    .Sum(t => t.Amount);

                var history = new TransactionHistoryDto
                {
                    Transactions = transactions,
                    TotalCredits = totalCredits,
                    TotalCount = transactions.Count
                };

                return (true, history, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting transactions for child {childId}: {ex.Message}");
                return (false, null, "An error occurred while retrieving your transactions");
            }
        }

        // ==================== CHILD ALLOWANCE INFO ====================

        public async Task<(bool success, ChildAllowanceInfoDto? info, string errorMessage)> GetMyAllowanceInfoAsync(
            int childId)
        {
            try
            {
                // Find active recurring allowance
                var allowance = await _context.Allowances
                    .FirstOrDefaultAsync(a => a.ChildID == childId && a.IsRecurring && a.IsActive);

                if (allowance == null)
                {
                    // No allowance set up - return friendly message
                    return (true, new ChildAllowanceInfoDto
                    {
                        Message = "You don't have an allowance set up yet. Ask your parent!",
                        NextPaymentDate = null,
                        IsActive = false,
                        Amount = 0
                    }, string.Empty);
                }

                // Build child-friendly message based on frequency
                string message = allowance.Type switch
                {
                    "Daily" => $"You get {allowance.Amount:F2} every day at {FormatHour(allowance.DailyHour!.Value)}! 🎉",
                    "Weekly" => $"You get {allowance.Amount:F2} every {allowance.WeeklyDay}! 🎉",
                    "Monthly" => $"You get {allowance.Amount:F2} on the {FormatDayOfMonth(allowance.MonthlyDay!.Value)} of each month! 🎉",
                    _ => $"You get {allowance.Amount:F2}! 🎉"
                };

                // Calculate next payment date (approximate)
                DateTime? nextPaymentDate = CalculateNextPaymentDate(allowance);

                var info = new ChildAllowanceInfoDto
                {
                    Message = message,
                    NextPaymentDate = nextPaymentDate,
                    IsActive = allowance.IsActive,
                    Amount = allowance.Amount
                };

                return (true, info, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting allowance info for child {childId}: {ex.Message}");
                return (false, null, "An error occurred while retrieving your allowance info");
            }
        }

        // ==================== BACKGROUND JOB ====================

        public async Task<int> CreditScheduledAllowancesAsync()
        {
            int creditedCount = 0;

            try
            {
                _logger.LogInformation("Starting scheduled allowance credit job");

                // STEP 1: Get all active recurring allowances
                var dueAllowances = await _context.Allowances
                    .Include(a => a.Child) // Need to update child's balance
                    .Where(a => a.IsRecurring && a.IsActive)
                    .ToListAsync();

                _logger.LogInformation($"Found {dueAllowances.Count} active recurring allowances to check");

                // STEP 2: Check each allowance to see if it's due
                foreach (var allowance in dueAllowances)
                {
                    bool isDue = IsAllowanceDue(allowance);

                    if (!isDue)
                    {
                        continue; // Skip this allowance - not due yet
                    }

                    // STEP 3: Credit this allowance
                    try
                    {
                        await CreditAllowanceAsync(allowance);
                        creditedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error crediting allowance {allowance.AllowanceID}: {ex.Message}");
                        // Continue with other allowances even if one fails
                    }
                }

                _logger.LogInformation($"Scheduled allowance credit job completed. Credited {creditedCount} allowances");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in scheduled allowance credit job: {ex.Message}");
            }

            return creditedCount;
        }

        // ==================== HELPER METHODS ====================

        /// <summary>
        /// Checks if an allowance is due to be credited now.
        /// </summary>
        private bool IsAllowanceDue(Allowance allowance)
        {
            DateTime now = DateTime.UtcNow;

            switch (allowance.Type)
            {
                case "Daily":
                    // Due if:
                    // 1. Current hour matches DailyHour
                    // 2. Last credited date is null OR was before today
                    if (now.Hour != allowance.DailyHour)
                        return false;

                    if (allowance.LastCreditedDate == null)
                        return true; // Never credited - credit now

                    // Already credited today?
                    return allowance.LastCreditedDate.Value.Date < now.Date;

                case "Weekly":
                    // Due if:
                    // 1. Current day of week matches WeeklyDay
                    // 2. Last credited date is null OR was before the start of this week
                    string currentDay = now.DayOfWeek.ToString();
                    if (currentDay != allowance.WeeklyDay)
                        return false;

                    if (allowance.LastCreditedDate == null)
                        return true;

                    // Already credited this week?
                    DateTime startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek);
                    return allowance.LastCreditedDate.Value < startOfWeek;

                case "Monthly":
                    // Due if:
                    // 1. Current day of month matches MonthlyDay (or closest valid day)
                    // 2. Last credited date is null OR was before the start of this month
                    int targetDay = allowance.MonthlyDay!.Value;
                    int daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
                    int effectiveDay = Math.Min(targetDay, daysInMonth); // Handle months with fewer days

                    if (now.Day != effectiveDay)
                        return false;

                    if (allowance.LastCreditedDate == null)
                        return true;

                    // Already credited this month?
                    DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
                    return allowance.LastCreditedDate.Value < startOfMonth;

                default:
                    _logger.LogWarning($"Unknown allowance type: {allowance.Type}");
                    return false;
            }
        }

        /// <summary>
        /// Credits an allowance to a child's balance and creates a transaction record.
        /// </summary>
        private async Task CreditAllowanceAsync(Allowance allowance)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // STEP 1: Update child's balance
                allowance.Child.CurrentBalance += allowance.Amount;
                _context.Children.Update(allowance.Child);

                // STEP 2: Create transaction record
                var transactionRecord = new Transaction
                {
                    Type = "AllowanceCredit",
                    Amount = allowance.Amount,
                    BalanceAfter = allowance.Child.CurrentBalance,
                    Description = $"{allowance.Type} allowance credited",
                    TransactionDate = DateTime.UtcNow,
                    ChildID = allowance.ChildID,
                    ParentID = allowance.ParentID,
                    AllowanceID = allowance.AllowanceID
                };

                _context.Transactions.Add(transactionRecord);

                // STEP 3: Update allowance LastCreditedDate
                allowance.LastCreditedDate = DateTime.UtcNow;
                allowance.GivenDate = DateTime.UtcNow; // Also update GivenDate
                _context.Allowances.Update(allowance);

                // STEP 4: Save and commit
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    $"Credited {allowance.Amount} to child {allowance.ChildID} from {allowance.Type} allowance {allowance.AllowanceID}. New balance: {allowance.Child.CurrentBalance}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Error crediting allowance {allowance.AllowanceID}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Formats hour (0-23) as human-readable time.
        /// </summary>
        private string FormatHour(int hour)
        {
            if (hour == 0)
                return "midnight";
            if (hour == 12)
                return "noon";
            if (hour < 12)
                return $"{hour} AM";
            return $"{hour - 12} PM";
        }

        /// <summary>
        /// Formats day of month with ordinal suffix (1st, 2nd, 3rd, etc.)
        /// </summary>
        private string FormatDayOfMonth(int day)
        {
            string suffix = day switch
            {
                1 or 21 or 31 => "st",
                2 or 22 => "nd",
                3 or 23 => "rd",
                _ => "th"
            };

            return $"{day}{suffix}";
        }

        /// <summary>
        /// Calculates approximate next payment date based on allowance schedule.
        /// </summary>
        private DateTime? CalculateNextPaymentDate(Allowance allowance)
        {
            DateTime now = DateTime.UtcNow;

            switch (allowance.Type)
            {
                case "Daily":
                    // Next occurrence of DailyHour
                    var nextDaily = new DateTime(now.Year, now.Month, now.Day, allowance.DailyHour!.Value, 0, 0);
                    if (nextDaily <= now)
                        nextDaily = nextDaily.AddDays(1);
                    return nextDaily;

                case "Weekly":
                    // Next occurrence of WeeklyDay
                    DayOfWeek targetDay = Enum.Parse<DayOfWeek>(allowance.WeeklyDay!);
                    int daysUntilTarget = ((int)targetDay - (int)now.DayOfWeek + 7) % 7;
                    if (daysUntilTarget == 0 && allowance.LastCreditedDate?.Date == now.Date)
                        daysUntilTarget = 7; // Already credited today, so next week
                    return now.Date.AddDays(daysUntilTarget);

                case "Monthly":
                    // Next occurrence of MonthlyDay
                    int targetDayOfMonth = allowance.MonthlyDay!.Value;
                    int daysInCurrentMonth = DateTime.DaysInMonth(now.Year, now.Month);
                    int effectiveDay = Math.Min(targetDayOfMonth, daysInCurrentMonth);

                    if (now.Day < effectiveDay)
                    {
                        // This month
                        return new DateTime(now.Year, now.Month, effectiveDay);
                    }
                    else
                    {
                        // Next month
                        var nextMonth = now.AddMonths(1);
                        int daysInNextMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
                        int effectiveDayNextMonth = Math.Min(targetDayOfMonth, daysInNextMonth);
                        return new DateTime(nextMonth.Year, nextMonth.Month, effectiveDayNextMonth);
                    }

                default:
                    return null;
            }
        }
    }
}
