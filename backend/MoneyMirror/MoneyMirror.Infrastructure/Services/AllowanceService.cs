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
        private readonly INotificationService _notificationService;

        public AllowanceService(
            ApplicationDbContext context,
            ILogger<AllowanceService> logger,
            INotificationService notificationService)
        {
            _context = context;
            _logger = logger;
            _notificationService = notificationService;
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
                // This makes sure the parent is allowed to manage this child's allowance
                bool isLinked = await IsParentLinkedToChildAsync(parentId, childId);

                if (!isLinked)
                {
                    _logger.LogWarning($"Parent {parentId} attempted to modify allowance for non-linked child {childId}");
                    return (false, "You are not authorized to manage this child's allowance");
                }

                // STEP 2: Find existing active recurring allowance (if any)
                var existingAllowance = await _context.Allowances
                    .FirstOrDefaultAsync(a => a.ChildID == childId && a.IsRecurring && a.IsActive);

                // STEP 3: Decide what to do based on whether allowance exists
                if (existingAllowance != null)
                {
                    // ✅ ALLOWANCE EXISTS → UPDATE IT
                    // Instead of creating a new one, we update the existing record
                    _logger.LogInformation($"Updating existing allowance {existingAllowance.AllowanceID} for child {childId}");

                    // Update all the fields with new values from the DTO
                    existingAllowance.Type = dto.Type;
                    existingAllowance.Amount = dto.Amount;
                    existingAllowance.IsActive = dto.IsActive;

                    // Update schedule fields based on the new type
                    // Important: Clear old schedule fields that don't apply to new type
                    existingAllowance.DailyHour = dto.Type == "Daily" ? dto.DailyHour : null;
                    existingAllowance.WeeklyDay = dto.Type == "Weekly" ? dto.WeeklyDay : null;
                    existingAllowance.MonthlyDay = dto.Type == "Monthly" ? dto.MonthlyDay : null;

                    // Update the SetDate to show when it was last modified
                    existingAllowance.SetDate = DateTime.UtcNow;

                    // Don't reset LastCreditedDate - keep the history of when it was last credited

                    // Tell Entity Framework to update this record
                    _context.Allowances.Update(existingAllowance);

                    _logger.LogInformation($"Updated {dto.Type} allowance for child {childId}: {dto.Amount}");
                }
                else
                {
                    // ✅ NO ALLOWANCE EXISTS → CREATE NEW ONE
                    // This is the first time setting up allowance for this child
                    _logger.LogInformation($"Creating new allowance for child {childId} (first time setup)");

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

                    // Tell Entity Framework to add this new record
                    _context.Allowances.Add(newAllowance);

                    _logger.LogInformation($"Created new {dto.Type} allowance for child {childId}: {dto.Amount}");
                }

                // STEP 4: Save changes to database
                // This actually executes the UPDATE or INSERT in the database
                await _context.SaveChangesAsync();
                string schedule = dto.Type switch
                {
                    "Daily" => $"every day at {dto.DailyHour:D2}:00",
                    "Weekly" => $"every {dto.WeeklyDay}",
                    "Monthly" => $"on the {dto.MonthlyDay} of each month",
                    _ => dto.Type
                };

                await _notificationService.NotifyChildAsync(
                    childId,
                    "Allowance Updated! 📅",
                    $"Your allowance has been updated to {dto.Amount:F2} EGP {schedule}.",
                    "/balance"
                );
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
            // ✅ STEP 1: Get the execution strategy from EF Core
            // This is REQUIRED when EnableRetryOnFailure is enabled
            var strategy = _context.Database.CreateExecutionStrategy();

            // ✅ STEP 2: Execute everything inside the strategy
            return await strategy.ExecuteAsync(async () =>
            {
                // NOW we can safely use transactions inside the strategy
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
                        Description = dto.Reason?.Trim(), // Trims out accidental whitespace if they just typed spaces
                        TransactionDate = DateTime.UtcNow,
                        ChildID = childId,
                        ParentID = parentId,
                        AllowanceID = null // Bonuses are not linked to allowance schedules
                    };

                    _context.Transactions.Add(transactionRecord);

                    // STEP 5: Save changes and commit transaction
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    await _notificationService.NotifyChildAsync(
                        childId,
                        "Bonus Received! 🎁",
                        string.IsNullOrWhiteSpace(dto.Reason)
                            ? $"Your parent just gave you {dto.Amount:F2} EGP bonus!"
                            : $"Your parent just gave you {dto.Amount:F2} EGP bonus: {dto.Reason}",
                        "/balance"
                    );
                    _logger.LogInformation($"Bonus of {dto.Amount} given to child {childId} by parent {parentId}. New balance: {child.CurrentBalance}");

                    return (true, child.CurrentBalance, string.Empty);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError($"Error giving bonus to child {childId}: {ex.Message}");
                    return (false, 0, "An error occurred while giving the bonus");
                }
            });
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
                bool isLinked = await IsParentLinkedToChildAsync(parentId, childId);
                if (!isLinked)
                {
                    _logger.LogWarning($"Parent {parentId} attempted to view transactions for non-linked child {childId}");
                    return (false, null, "You are not authorized to view this child's transactions");
                }

                var validTypes = new HashSet<string>
        {
            "All", "AllowanceAndBonus",
            "AllowanceCredit", "BonusCredit", "Expense", "GoalTransfer", "GoalRefund",
            "ExpenseAdjustment", "BonusAdjustment"
        };


                if (!validTypes.Contains(type))
                    return (false, null, "Invalid transaction type filter.");

                var now = DateTime.UtcNow;

                if (startDate.HasValue && startDate.Value > now)
                    return (false, null, "Start date cannot be in the future.");

                if (endDate.HasValue && endDate.Value > now)
                    return (false, null, "End date cannot be in the future.");

                if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
                    return (false, null, "Start date cannot be after end date.");

                var query = _context.Transactions.Where(t => t.ChildID == childId);

                if (startDate.HasValue)
                    query = query.Where(t => t.TransactionDate >= startDate.Value);

                if (endDate.HasValue)
                {
                    // FIX: Include the entire end day up to 23:59:59
                    var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(t => t.TransactionDate <= endOfDay);
                }

                if (type == "AllowanceAndBonus")
                    query = query.Where(t => t.Type == "AllowanceCredit" || t.Type == "BonusCredit");
                else if (type != "All")
                    query = query.Where(t => t.Type == type);

                var transactions = await query
                    .OrderByDescending(t => t.TransactionDate)
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

                var totalCredits = transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);

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
            DateTime? startDate = null,
            DateTime? endDate = null, // ADDED: Match parent functionality
            string type = "All")       // ADDED: Match parent functionality
        {
            try
            {
                // ADDED: Input validations matching the parent rules
                var validTypes = new HashSet<string>
        {
            "All", "AllowanceAndBonus",
            "AllowanceCredit", "BonusCredit", "Expense", "GoalTransfer", "GoalRefund",
            "ExpenseAdjustment", "BonusAdjustment"
        };


                if (!validTypes.Contains(type))
                    return (false, null, "Invalid transaction type filter.");

                var now = DateTime.UtcNow;

                if (startDate.HasValue && startDate.Value > now)
                    return (false, null, "Start date cannot be in the future.");

                if (endDate.HasValue && endDate.Value > now)
                    return (false, null, "End date cannot be in the future.");

                if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
                    return (false, null, "Start date cannot be after end date.");

                // Build query
                var query = _context.Transactions.Where(t => t.ChildID == childId);

                if (startDate.HasValue)
                    query = query.Where(t => t.TransactionDate >= startDate.Value);

                if (endDate.HasValue)
                {
                    // FIX: Include the entire end day
                    var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(t => t.TransactionDate <= endOfDay);
                }

                if (type == "AllowanceAndBonus")
                    query = query.Where(t => t.Type == "AllowanceCredit" || t.Type == "BonusCredit");
                else if (type != "All")
                    query = query.Where(t => t.Type == type);

                // Execute query
                var transactions = await query
                    .OrderByDescending(t => t.TransactionDate)
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

                var totalCredits = transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);

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
                _logger.LogInformation("🔍 Starting scheduled allowance credit job");
                Console.WriteLine("🔍 Starting scheduled allowance credit job"); // Also print to console

                // STEP 1: Get all active recurring allowances
                var dueAllowances = await _context.Allowances
                    .Include(a => a.Child)
                    .Where(a => a.IsRecurring && a.IsActive)
                    .ToListAsync();

                _logger.LogInformation($"📊 Found {dueAllowances.Count} active recurring allowances to check");
                Console.WriteLine($"📊 Found {dueAllowances.Count} active recurring allowances to check");

                // Log each allowance found
                foreach (var allowance in dueAllowances)
                {
                    var logMsg = $"  - Allowance {allowance.AllowanceID}: Type={allowance.Type}, Amount={allowance.Amount}, DailyHour={allowance.DailyHour}, LastCredited={allowance.LastCreditedDate?.ToString("yyyy-MM-dd HH:mm") ?? "NEVER"}";
                    _logger.LogInformation(logMsg);
                    Console.WriteLine(logMsg);
                }

                // STEP 2: Check each allowance to see if it's due
                foreach (var allowance in dueAllowances)
                {
                    var checkMsg = $"🔍 Checking allowance {allowance.AllowanceID} for child {allowance.ChildID}";
                    _logger.LogInformation(checkMsg);
                    Console.WriteLine(checkMsg);

                    bool isDue = IsAllowanceDue(allowance);

                    var resultMsg = $"  → Result: {(isDue ? "✅ DUE - will credit" : "❌ NOT DUE - skipping")}";
                    _logger.LogInformation(resultMsg);
                    Console.WriteLine(resultMsg);

                    if (!isDue)
                    {
                        continue;
                    }

                    // STEP 3: Credit this allowance
                    try
                    {
                        var creditMsg = $"💰 Attempting to credit allowance {allowance.AllowanceID}";
                        _logger.LogInformation(creditMsg);
                        Console.WriteLine(creditMsg);

                        await CreditAllowanceAsync(allowance);
                        creditedCount++;

                        var successMsg = $"✅ Successfully credited allowance {allowance.AllowanceID}";
                        _logger.LogInformation(successMsg);
                        Console.WriteLine(successMsg);
                    }
                    catch (Exception ex)
                    {
                        var errorMsg = $"❌ Error crediting allowance {allowance.AllowanceID}: {ex.Message}";
                        _logger.LogError(errorMsg);
                        Console.WriteLine(errorMsg);
                    }
                }

                var finalMsg = $"✅ Job completed. Credited {creditedCount} allowances";
                _logger.LogInformation(finalMsg);
                Console.WriteLine(finalMsg);
            }
            catch (Exception ex)
            {
                var errorMsg = $"❌ Error in scheduled allowance credit job: {ex.Message}";
                _logger.LogError(errorMsg);
                Console.WriteLine(errorMsg);
            }

            return creditedCount;
        }

        // ==================== HELPER METHODS ====================

        /// <summary>
        /// Checks if an allowance is due to be credited now.
        /// IMPROVED: Checks if we're PAST the target hour, not just AT it.
        /// </summary>
        private bool IsAllowanceDue(Allowance allowance)
        {
            DateTime now = DateTime.UtcNow;

            // ✅ ADD DETAILED LOGGING
            _logger.LogInformation($"Checking allowance {allowance.AllowanceID} for child {allowance.ChildID}");
            _logger.LogInformation($"  Type: {allowance.Type}");
            _logger.LogInformation($"  Current UTC time: {now:yyyy-MM-dd HH:mm:ss}");
            _logger.LogInformation($"  LastCreditedDate: {allowance.LastCreditedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Never"}");

            switch (allowance.Type)
            {
                case "Daily":
                    _logger.LogInformation($"  DailyHour setting: {allowance.DailyHour}");
                    _logger.LogInformation($"  Current hour: {now.Hour}");

                    if (allowance.LastCreditedDate == null)
                    {
                        _logger.LogInformation($"  ✅ DECISION: Credit now (never credited before)");
                        return true;
                    }

                    if (allowance.LastCreditedDate.Value.Date == now.Date)
                    {
                        _logger.LogInformation($"  ❌ DECISION: Skip (already credited today)");
                        return false;
                    }

                    if (now.Hour < allowance.DailyHour)
                    {
                        _logger.LogInformation($"  ❌ DECISION: Skip (current hour {now.Hour} < target hour {allowance.DailyHour})");
                        return false;
                    }

                    _logger.LogInformation($"  ✅ DECISION: Credit now (new day + past target hour)");
                    return true;

                case "Weekly":
                    string currentDay = now.DayOfWeek.ToString();
                    _logger.LogInformation($"  WeeklyDay setting: {allowance.WeeklyDay}");
                    _logger.LogInformation($"  Current day: {currentDay}");

                    if (currentDay != allowance.WeeklyDay)
                    {
                        _logger.LogInformation($"  ❌ DECISION: Skip (wrong day)");
                        return false;
                    }

                    if (allowance.LastCreditedDate == null)
                    {
                        _logger.LogInformation($"  ✅ DECISION: Credit now (never credited before)");
                        return true;
                    }

                    DateTime startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek);
                    bool shouldCredit = allowance.LastCreditedDate.Value < startOfWeek;

                    _logger.LogInformation($"  Start of week: {startOfWeek:yyyy-MM-dd}");
                    _logger.LogInformation($"  {(shouldCredit ? "✅" : "❌")} DECISION: {(shouldCredit ? "Credit now" : "Skip (already credited this week)")}");

                    return shouldCredit;

                case "Monthly":
                    int targetDay = allowance.MonthlyDay!.Value;
                    int daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
                    int effectiveDay = Math.Min(targetDay, daysInMonth);

                    _logger.LogInformation($"  MonthlyDay setting: {allowance.MonthlyDay}");
                    _logger.LogInformation($"  Effective day this month: {effectiveDay}");
                    _logger.LogInformation($"  Current day: {now.Day}");

                    if (now.Day != effectiveDay)
                    {
                        _logger.LogInformation($"  ❌ DECISION: Skip (wrong day)");
                        return false;
                    }

                    if (allowance.LastCreditedDate == null)
                    {
                        _logger.LogInformation($"  ✅ DECISION: Credit now (never credited before)");
                        return true;
                    }

                    DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
                    bool shouldCreditMonth = allowance.LastCreditedDate.Value < startOfMonth;

                    _logger.LogInformation($"  Start of month: {startOfMonth:yyyy-MM-dd}");
                    _logger.LogInformation($"  {(shouldCreditMonth ? "✅" : "❌")} DECISION: {(shouldCreditMonth ? "Credit now" : "Skip (already credited this month)")}");

                    return shouldCreditMonth;

                default:
                    _logger.LogWarning($"Unknown allowance type: {allowance.Type}");
                    return false;
            }
        }

        /// <summary>
        /// Credits an allowance to a child's balance and creates a transaction record.
        /// Uses EF's execution strategy for retry compatibility.
        /// </summary>
        private async Task CreditAllowanceAsync(Allowance allowance)
        {
            // ✅ Use EF's execution strategy (works with EnableRetryOnFailure)
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                // Now we can safely use a transaction inside the strategy
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
                    allowance.GivenDate = DateTime.UtcNow;
                    _context.Allowances.Update(allowance);

                    // STEP 4: Save and commit
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    await _notificationService.NotifyChildAsync(
                            allowance.ChildID,
                            "Allowance Received! 🎉",
                            $"You just received {allowance.Amount:F2} EGP! Check your balance.",
                            "/balance"
                        );
                    await _notificationService.NotifyAllParentsOfChildAsync(
                        allowance.ChildID,
                        "Allowance Sent 💰",
                        $"{allowance.Child.FName} received their {allowance.Type.ToLower()} allowance of {allowance.Amount:F2} EGP.",
                        $"/children/{allowance.ChildID}"
                    );
                    _logger.LogInformation(
                        $"✅ Credited {allowance.Amount} to child {allowance.ChildID}. New balance: {allowance.Child.CurrentBalance}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError($"❌ Error crediting allowance {allowance.AllowanceID}: {ex.Message}");
                    throw; // Re-throw so the strategy knows it failed
                }
            });
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
        public async Task<(bool success, decimal newBalance, string errorMessage)> EditBonusAsync(
    int parentId,
    int transactionId,
    EditBonusDto dto)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // 1. Fetch the bonus transaction
                    var bonusTx = await _context.Transactions
                        .FirstOrDefaultAsync(t => t.TransactionID == transactionId && t.Type == "BonusCredit");

                    if (bonusTx == null)
                    {
                        return (false, 0, "Bonus transaction not found");
                    }

                    // 2. Verify parent is linked to the child who received this bonus
                    bool isLinked = await IsParentLinkedToChildAsync(parentId, bonusTx.ChildID);
                    if (!isLinked)
                    {
                        _logger.LogWarning($"Parent {parentId} unauthorized attempt to edit bonus transaction {transactionId}");
                        return (false, 0, "You are not authorized to edit this child's bonus");
                    }

                    // 3. Get child and check balance
                    var child = await _context.Children.FindAsync(bonusTx.ChildID);
                    if (child == null)
                    {
                        return (false, 0, "Child not found");
                    }

                    decimal oldAmount = bonusTx.Amount;
                    decimal newAmount = dto.Amount;
                    decimal difference = newAmount - oldAmount; // positive if increased, negative if decreased

                    // Restriction: child must not have spent the money (balance check)
                    if (difference < 0 && child.CurrentBalance + difference < 0)
                    {
                        return (false, 0,
                            $"Cannot reduce bonus. Child's current balance ({child.CurrentBalance:F2} EGP) is less than the deduction amount ({-difference:F2} EGP), meaning they have already spent it.");
                    }

                    // 4. Update child's balance
                    child.CurrentBalance += difference;
                    _context.Children.Update(child);

                    // 5. Update original transaction details
                    bonusTx.Amount = newAmount;
                    bonusTx.Description = dto.Reason?.Trim() ?? bonusTx.Description;
                    _context.Transactions.Update(bonusTx);

                    // 6. Record corrective transaction if amount changed
                    if (difference != 0)
                    {
                        string directionDesc = difference > 0 ? "credited" : "deducted";
                        _context.Transactions.Add(new Transaction
                        {
                            Type = "BonusAdjustment",
                            Amount = difference,
                            BalanceAfter = child.CurrentBalance,
                            Description = $"Corrective transaction for bonus adjustment. Adjusted from {oldAmount:F2} to {newAmount:F2} EGP. {Math.Abs(difference):F2} EGP {directionDesc}.",
                            TransactionDate = DateTime.UtcNow,
                            ChildID = bonusTx.ChildID,
                            ParentID = parentId,
                            AllowanceID = null
                        });
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // Notify child of the adjustment
                    string actionText = difference > 0 ? "increased" : "decreased";
                    await _notificationService.NotifyChildAsync(
                        bonusTx.ChildID,
                        "Bonus Adjusted 🎁",
                        $"Your parent adjusted your bonus. It was changed from {oldAmount:F2} to {newAmount:F2} EGP.",
                        "/balance"
                    );

                    _logger.LogInformation($"Bonus transaction {transactionId} edited. Amount changed from {oldAmount} to {newAmount}. New child balance: {child.CurrentBalance}");

                    return (true, child.CurrentBalance, string.Empty);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error editing bonus transaction {TransactionId} by parent {ParentId}", transactionId, parentId);
                    return (false, 0, $"Error: {ex.Message}");
                }
            });
        }

    }
}
