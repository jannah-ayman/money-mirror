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

        public ExpenseService(
            ApplicationDbContext context,
            ILogger<ExpenseService> logger)
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

        // ==================== EXPENSE LOGGING ====================

        public async Task<(bool success, ExpenseResponseDto? expense, decimal newBalance, string errorMessage)>
     LogExpenseAsync(int childId, LogExpenseDto dto)
        {
            // ✅ STEP 1: Get the execution strategy from EF Core
            var strategy = _context.Database.CreateExecutionStrategy();

            // ✅ STEP 2: Execute everything inside the strategy
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // STEP 3: Get child and verify they exist
                    var child = await _context.Children.FindAsync(childId);

                    if (child == null)
                    {
                        _logger.LogWarning($"Expense log attempt for non-existent child {childId}");
                        return (false, null, 0, "Child not found");
                    }

                    _logger.LogInformation($"Child {childId} current balance: {child.CurrentBalance}");

                    // STEP 4: Validate child has enough balance
                    if (child.CurrentBalance < dto.MoneyAmount)
                    {
                        _logger.LogWarning($"Child {childId} attempted to spend {dto.MoneyAmount} but only has {child.CurrentBalance}");
                        return (false, null, child.CurrentBalance,
                            $"Insufficient balance. You have {child.CurrentBalance:F2} but tried to spend {dto.MoneyAmount:F2}");
                    }

                    // STEP 5: Verify category exists
                    var category = await _context.ExpenseCategories.FindAsync(dto.CategoryID);

                    if (category == null)
                    {
                        _logger.LogWarning($"Invalid category ID {dto.CategoryID} for expense");
                        return (false, null, child.CurrentBalance, "Invalid category selected");
                    }

                    _logger.LogInformation($"Category found: {category.Name}");

                    // ⭐ STEP 5.5: NEW - Check if ItemName is required (category = "Other")
                    bool isOtherCategory = category.Name.Equals("Other", StringComparison.OrdinalIgnoreCase);

                    if (isOtherCategory && string.IsNullOrWhiteSpace(dto.ItemName))
                    {
                        _logger.LogWarning($"ItemName is required for 'Other' category but was not provided");
                        return (false, null, child.CurrentBalance,
                            "Please provide an item name when category is 'Other'");
                    }

                    // If category is NOT "Other", we don't need ItemName - set it to null
                    string? finalItemName = isOtherCategory
                        ? dto.ItemName?.Trim()
                        : null;

                    _logger.LogInformation($"Using ItemName: {finalItemName ?? "(none)"}");

                    // STEP 6: Verify mood exists
                    var mood = await _context.Moods.FindAsync(dto.MoodID);

                    if (mood == null)
                    {
                        _logger.LogWarning($"Invalid mood ID {dto.MoodID} for expense");
                        return (false, null, child.CurrentBalance, "Invalid mood selected");
                    }

                    _logger.LogInformation($"Mood found: {mood.Description}");

                    // STEP 7: Create the expense record
                    var expense = new Core.Models.Expense
                    {
                        ItemName = finalItemName, // ⭐ Use the finalized ItemName (null if not "Other")
                        MoneyAmount = dto.MoneyAmount,
                        LogDate = DateTime.UtcNow,
                        Note = string.IsNullOrWhiteSpace(dto.Note) ? null : dto.Note.Trim(),
                        ChildID = childId,
                        CategoryID = dto.CategoryID,
                        MoodID = dto.MoodID
                    };

                    _context.Expenses.Add(expense);
                    _logger.LogInformation($"Expense record created for child {childId}");

                    // STEP 8: Update child's balance (DECREASE it)
                    decimal oldBalance = child.CurrentBalance;
                    child.CurrentBalance -= dto.MoneyAmount;
                    _context.Children.Update(child);
                    _logger.LogInformation($"Balance updated: {oldBalance} -> {child.CurrentBalance}");

                    // STEP 9: Create transaction record
                    string transactionDesc = isOtherCategory && !string.IsNullOrWhiteSpace(finalItemName)
                        ? $"Spent on {finalItemName}"
                        : $"Spent on {category.Name}";

                    var transactionRecord = new Transaction
                    {
                        Type = "Expense",
                        Amount = dto.MoneyAmount,
                        BalanceAfter = child.CurrentBalance,
                        Description = transactionDesc,
                        TransactionDate = DateTime.UtcNow,
                        ChildID = childId,
                        ParentID = null,
                        AllowanceID = null
                    };

                    _context.Transactions.Add(transactionRecord);
                    _logger.LogInformation($"Transaction record created");

                    // STEP 10: Save everything to database
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Changes saved to database");

                    await transaction.CommitAsync();
                    _logger.LogInformation($"Transaction committed");

                    _logger.LogInformation(
                        $"Child {childId} logged expense: Category={category.Name}, ItemName={finalItemName ?? "(none)"}, Amount={dto.MoneyAmount}. New balance: {child.CurrentBalance}");

                    // STEP 11: Build response DTO
                    // ⭐ Only include ItemName in response if category was "Other"
                    var expenseResponse = new ExpenseResponseDto
                    {
                        ExpenseID = expense.ExpenseID,
                        ItemName = isOtherCategory ? finalItemName : null, // ⭐ Hide ItemName unless "Other"
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
                    _logger.LogError($"Error logging expense for child {childId}: {ex.Message}");
                    _logger.LogError($"Stack trace: {ex.StackTrace}");

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

                // Filter by end date
                if (endDate.HasValue)
                {
                    query = query.Where(e => e.LogDate <= endDate.Value);
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
                        // ⭐ Only include ItemName if category is "Other"
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

                if (endDate.HasValue)
                {
                    query = query.Where(e => e.LogDate <= endDate.Value);
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
                        // ⭐ Only include ItemName if category is "Other"
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
    }
}