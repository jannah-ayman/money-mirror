using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Expense;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Core.Models;
using MoneyMirror.Infrastructure.Data;

namespace MoneyMirror.Infrastructure.Services
{
    /// <summary>
    /// Service implementing expense logging and retrieval logic.
    /// Handles balance validation, expense creation, and transaction recording.
    /// </summary>
    public class ExpenseService : IExpenseService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ExpenseService> _logger;
        private readonly IAchievementService _achievementService;
        private readonly INotificationService _notificationService;

        public ExpenseService(ApplicationDbContext context, ILogger<ExpenseService> logger, IAchievementService achievementService, INotificationService notificationService
)
        {
            _context = context;
            _logger = logger;
            _achievementService = achievementService;
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
      
        /// Resolves the log date of an expense: uses the requested date at midnight UTC if provided, 
        /// otherwise defaults to the current UTC time.
       private DateTime ResolveLogDate(DateTime? requestedDate)
        {
            return requestedDate.HasValue ? requestedDate.Value.Date : DateTime.UtcNow;
        }


        // ==================== EXPENSE LOGGING ====================

        public async Task<(bool success, ExpenseResponseDto? expense, decimal newBalance, string errorMessage)>
            LogExpenseAsync(int childId, LogExpenseDto dto)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var child = await _context.Children.FindAsync(childId);

                    if (child == null)
                        return (false, null, 0, "Child not found");

                    if (child.CurrentBalance < dto.MoneyAmount)
                        return (false, null, child.CurrentBalance,
                            $"Insufficient balance. You have {child.CurrentBalance:F2} but tried to spend {dto.MoneyAmount:F2}");

                    var category = await _context.ExpenseCategories.FindAsync(dto.CategoryID);
                    if (category == null)
                        return (false, null, child.CurrentBalance, "Invalid category selected");

                    bool isOtherCategory = category.Name.Equals("Other", StringComparison.OrdinalIgnoreCase);

                    if (isOtherCategory && string.IsNullOrWhiteSpace(dto.ItemName))
                        return (false, null, child.CurrentBalance, "Please provide an item name when category is 'Other'");

                    string? finalItemName = isOtherCategory ? dto.ItemName?.Trim() : null;

                    var mood = await _context.Moods.FindAsync(dto.MoodID);
                    if (mood == null)
                        return (false, null, child.CurrentBalance, "Invalid mood selected");

                    var expense = new Core.Models.Expense
                    {
                        ItemName = finalItemName,
                        MoneyAmount = dto.MoneyAmount,
                        LogDate = ResolveLogDate(dto.LogDate),
                        Note = string.IsNullOrWhiteSpace(dto.Note) ? null : dto.Note.Trim(),
                        ChildID = childId,
                        CategoryID = dto.CategoryID,
                        MoodID = dto.MoodID
                    };

                    _context.Expenses.Add(expense);

                    child.CurrentBalance -= dto.MoneyAmount;
                    child.ExpenseCount++;
                    _context.Children.Update(child);

                    string transactionDesc = isOtherCategory && !string.IsNullOrWhiteSpace(finalItemName)
                        ? $"Spent on {finalItemName}"
                        : $"Spent on {category.Name}";

                    _context.Transactions.Add(new Transaction
                    {
                        Type = "Expense",
                        Amount = dto.MoneyAmount,
                        BalanceAfter = child.CurrentBalance,
                        Description = transactionDesc,
                        TransactionDate = DateTime.UtcNow,
                        ChildID = childId,
                        ParentID = null,
                        AllowanceID = null
                    });

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    await _notificationService.CheckAndNotifyLowBalanceAsync(childId);
                    await _achievementService.CheckAndUnlockAsync(childId, "Expense");
                    await _notificationService.NotifyAllParentsOfChildAsync(
                            childId,
                            "New Expense Logged 💸",
                            $"{child.FName} just spent {dto.MoneyAmount:F2} EGP on {category.Name}.",
                            $"/children/{childId}/expenses"
                        );
                    _logger.LogInformation(
                        "Child {ChildId} logged expense: Category={Category}, Amount={Amount}. New balance: {Balance}",
                        childId, category.Name, dto.MoneyAmount, child.CurrentBalance);

                    var expenseResponse = new ExpenseResponseDto
                    {
                        ExpenseID = expense.ExpenseID,
                        ItemName = isOtherCategory ? finalItemName : null,
                        CategoryName = category.Name,
                        MoodDescription = mood.Description,
                        Amount = expense.MoneyAmount,
                        LogDate = expense.LogDate,
                        Note = expense.Note
                    };

                    return (true, expenseResponse, child.CurrentBalance, string.Empty);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error logging expense for child {ChildId}", childId);
                    return (false, null, 0, $"Error: {ex.Message}");
                }
            });
        }

        // ==================== EXPENSE HISTORY (CHILD VIEW) ====================

        public async Task<(bool success, ExpenseHistoryDto? history, string errorMessage)>
            GetMyExpensesAsync(
                int childId,
                DateTime? startDate = null,
                DateTime? endDate = null,
                int? categoryId = null,
                int? moodId = null)
        {
            try
            {
                var now = DateTime.UtcNow;

                // Validate dates in the future
                if (startDate.HasValue && startDate.Value > now)
                    return (false, null, "Start date cannot be in the future.");

                if (endDate.HasValue && endDate.Value > now)
                    return (false, null, "End date cannot be in the future.");

                // Validate start date after end date
                if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
                    return (false, null, "Start date cannot be after end date.");

                // Validate category existence
                if (categoryId.HasValue)
                {
                    bool categoryExists = await _context.ExpenseCategories.AnyAsync(c => c.CategoryID == categoryId.Value);
                    if (!categoryExists)
                        return (false, null, "Category not found.");
                }

                // Validate mood existence
                if (moodId.HasValue)
                {
                    bool moodExists = await _context.Moods.AnyAsync(m => m.MoodID == moodId.Value);
                    if (!moodExists)
                        return (false, null, "Mood not found.");
                }

                // STEP 1: Build query for child's expenses
                var query = _context.Expenses
                    .Include(e => e.ExpenseCategory)
                    .Include(e => e.Mood)
                    .Where(e => e.ChildID == childId);

                // STEP 2: Apply filters if provided

                // Filter by start date
                if (startDate.HasValue)
                {
                    query = query.Where(e => e.LogDate >= startDate.Value);
                }

                // Filter by end date (including the entire end day)
                if (endDate.HasValue)
                {
                    var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(e => e.LogDate <= endOfDay);
                }

                // Filter by category
                if (categoryId.HasValue)
                {
                    query = query.Where(e => e.CategoryID == categoryId.Value);
                }

                // Filter by mood
                if (moodId.HasValue)
                {
                    query = query.Where(e => e.MoodID == moodId.Value);
                }


                // STEP 3: Execute query and build response
                var expenses = await query
                    .OrderByDescending(e => e.LogDate) // Newest first
                    .Select(e => new ExpenseResponseDto
                    {
                        ExpenseID = e.ExpenseID,
                        ItemName = e.ExpenseCategory.Name.ToLower() == "other" ? e.ItemName : null,
                        CategoryName = e.ExpenseCategory.Name,
                        MoodDescription = e.Mood.Description,
                        Amount = e.MoneyAmount,
                        LogDate = e.LogDate,
                        Note = e.Note
                    })
                    .ToListAsync();

                // STEP 4: Calculate summary
                var totalCount = expenses.Count;
                var totalSpent = expenses.Sum(e => e.Amount);

                var history = new ExpenseHistoryDto
                {
                    Expenses = expenses,
                    TotalCount = totalCount,
                    TotalSpent = totalSpent
                };

                return (true, history, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting expenses for child {childId}: {ex.Message}");
                return (false, null, "An error occurred while retrieving your expenses");
            }
        }

        // ==================== EXPENSE HISTORY (PARENT VIEW) ====================

        public async Task<(bool success, ExpenseHistoryDto? history, string errorMessage)>
            GetChildExpensesAsync(
                int parentId,
                int childId,
                DateTime? startDate = null,
                DateTime? endDate = null,
                int? categoryId = null,
                int? moodId = null)
        {
            try
            {
                // STEP 1: Verify parent-child relationship
                bool isLinked = await IsParentLinkedToChildAsync(parentId, childId);

                if (!isLinked)
                {
                    _logger.LogWarning($"Parent {parentId} attempted to view expenses for non-linked child {childId}");
                    return (false, null, "You are not authorized to view this child's expenses");
                }

                var now = DateTime.UtcNow;

                // Validate dates in the future
                if (startDate.HasValue && startDate.Value > now)
                    return (false, null, "Start date cannot be in the future.");

                if (endDate.HasValue && endDate.Value > now)
                    return (false, null, "End date cannot be in the future.");

                // Validate start date after end date
                if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
                    return (false, null, "Start date cannot be after end date.");

                // Validate category existence
                if (categoryId.HasValue)
                {
                    bool categoryExists = await _context.ExpenseCategories.AnyAsync(c => c.CategoryID == categoryId.Value);
                    if (!categoryExists)
                        return (false, null, "Category not found.");
                }

                // Validate mood existence
                if (moodId.HasValue)
                {
                    bool moodExists = await _context.Moods.AnyAsync(m => m.MoodID == moodId.Value);
                    if (!moodExists)
                        return (false, null, "Mood not found.");
                }

                // STEP 2: Build query (same as child view)
                var query = _context.Expenses
                    .Include(e => e.ExpenseCategory)
                    .Include(e => e.Mood)
                    .Where(e => e.ChildID == childId);

                // Apply filters
                if (startDate.HasValue)
                {
                    query = query.Where(e => e.LogDate >= startDate.Value);
                }


                // Filter by end date (including the entire end day)
                if (endDate.HasValue)
                {
                    var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(e => e.LogDate <= endOfDay);
                }

                if (categoryId.HasValue)
                {
                    query = query.Where(e => e.CategoryID == categoryId.Value);
                }

                if (moodId.HasValue)
                {
                    query = query.Where(e => e.MoodID == moodId.Value);
                }

                // STEP 3: Execute query
                var expenses = await query
                    .OrderByDescending(e => e.LogDate)
                    .Select(e => new ExpenseResponseDto
                    {
                        ExpenseID = e.ExpenseID,
                        ItemName = e.ExpenseCategory.Name.ToLower() == "other" ? e.ItemName : null,
                        CategoryName = e.ExpenseCategory.Name,
                        MoodDescription = e.Mood.Description,
                        Amount = e.MoneyAmount,
                        LogDate = e.LogDate,
                        Note = e.Note
                    })
                    .ToListAsync();

                // STEP 4: Calculate summary
                var totalCount = expenses.Count;
                var totalSpent = expenses.Sum(e => e.Amount);

                var history = new ExpenseHistoryDto
                {
                    Expenses = expenses,
                    TotalCount = totalCount,
                    TotalSpent = totalSpent
                };

                return (true, history, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting expenses for child {childId}: {ex.Message}");
                return (false, null, "An error occurred while retrieving expenses");
            }
        }


        // ==================== CATEGORIES AND MOODS ====================

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            try
            {
                var categories = await _context.ExpenseCategories
                    .OrderBy(c => c.Name) // Alphabetical order
                    .Select(c => new CategoryDto
                    {
                        CategoryID = c.CategoryID,
                        Name = c.Name
                    })
                    .ToListAsync();

                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting categories: {ex.Message}");
                return new List<CategoryDto>();
            }
        }

        public async Task<List<MoodDto>> GetMoodsAsync()
        {
            try
            {
                var moods = await _context.Moods
                    .OrderBy(m => m.Description) // Alphabetical order
                    .Select(m => new MoodDto
                    {
                        MoodID = m.MoodID,
                        Description = m.Description
                    })
                    .ToListAsync();

                return moods;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting moods: {ex.Message}");
                return new List<MoodDto>();
            }
        }
        public async Task<(bool success, ExpenseResponseDto? expense, decimal newBalance, string errorMessage)>
    UpdateExpenseAsync(int childId, int expenseId, UpdateExpenseDto dto)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var child = await _context.Children.FindAsync(childId);
                    if (child == null)
                        return (false, null, 0, "Child not found");

                    var expense = await _context.Expenses
                        .FirstOrDefaultAsync(e => e.ExpenseID == expenseId && e.ChildID == childId);

                    if (expense == null)
                        return (false, null, child.CurrentBalance, "Expense not found");

                    var category = await _context.ExpenseCategories.FindAsync(dto.CategoryID);
                    if (category == null)
                        return (false, null, child.CurrentBalance, "Invalid category selected");

                    bool isOtherCategory = category.Name.Equals("Other", StringComparison.OrdinalIgnoreCase);

                    if (isOtherCategory && string.IsNullOrWhiteSpace(dto.ItemName))
                        return (false, null, child.CurrentBalance, "Please provide an item name when category is 'Other'");

                    string? finalItemName = isOtherCategory ? dto.ItemName?.Trim() : null;

                    var mood = await _context.Moods.FindAsync(dto.MoodID);
                    if (mood == null)
                        return (false, null, child.CurrentBalance, "Invalid mood selected");

                    decimal oldAmount = expense.MoneyAmount;
                    decimal newAmount = dto.MoneyAmount;
                    decimal difference = oldAmount - newAmount; // Positive if refund, negative if extra spend

                    // If they spent more, check if they have enough balance
                    if (difference < 0 && child.CurrentBalance + difference < 0)
                    {
                        return (false, null, child.CurrentBalance,
                            $"Insufficient balance to increase expense amount. You need {-difference:F2} EGP but only have {child.CurrentBalance:F2} EGP.");
                    }

                    // Update child's balance
                    child.CurrentBalance += difference;
                    _context.Children.Update(child);

                    // Update expense details
                    expense.CategoryID = dto.CategoryID;
                    expense.MoodID = dto.MoodID;
                    expense.Note = string.IsNullOrWhiteSpace(dto.Note) ? null : dto.Note.Trim();
                    expense.ItemName = finalItemName;
                    expense.MoneyAmount = dto.MoneyAmount;
                    expense.LogDate = ResolveLogDate(dto.LogDate);
                    _context.Expenses.Update(expense);

                    // If amount changed, record a corrective transaction
                    if (difference != 0)
                    {
                        string directionDesc = difference > 0 ? "refunded" : "debited";
                        string correctionDesc = $"Corrective transaction for expense adjustment. Expense ID: {expenseId}. Amount adjusted from {oldAmount:F2} to {newAmount:F2}. {Math.Abs(difference):F2} EGP {directionDesc} to balance.";

                        _context.Transactions.Add(new Transaction
                        {
                            Type = "ExpenseAdjustment",
                            Amount = difference,
                            BalanceAfter = child.CurrentBalance,
                            Description = correctionDesc,
                            TransactionDate = DateTime.UtcNow,
                            ChildID = childId,
                            ParentID = null,
                            AllowanceID = null
                        });
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    await _notificationService.CheckAndNotifyLowBalanceAsync(childId);
                    await _notificationService.NotifyAllParentsOfChildAsync(
                        childId,
                        "Expense Updated 📝",
                        $"{child.FName} updated their expense. Category is now {category.Name}, amount adjusted from {oldAmount:F2} to {newAmount:F2} EGP.",
                        $"/children/{childId}/expenses"
                    );

                    var expenseResponse = new ExpenseResponseDto
                    {
                        ExpenseID = expense.ExpenseID,
                        ItemName = isOtherCategory ? finalItemName : null,
                        CategoryName = category.Name,
                        MoodDescription = mood.Description,
                        Amount = expense.MoneyAmount,
                        LogDate = expense.LogDate,
                        Note = expense.Note
                    };

                    return (true, expenseResponse, child.CurrentBalance, string.Empty);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error updating expense {ExpenseId} for child {ChildId}", expenseId, childId);
                    return (false, null, 0, $"Error: {ex.Message}");
                }
            });
        }

    }
}