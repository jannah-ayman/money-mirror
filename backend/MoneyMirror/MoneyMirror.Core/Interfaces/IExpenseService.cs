using MoneyMirror.Core.DTOs.Expense;

namespace MoneyMirror.Core.Interfaces
{
    /// <summary>
    /// Interface for expense logging and retrieval operations.
    /// Handles logging expenses, updating balances, and getting expense history.
    /// </summary>
    public interface IExpenseService
    {
        // ==================== EXPENSE LOGGING ====================

        /// <summary>
        /// Logs a new expense for a child.
        /// Validates balance, creates expense record, updates balance, creates transaction.
        /// </summary>
        /// <param name="childId">ID of the child (from JWT token)</param>
        /// <param name="dto">Expense details</param>
        /// <returns>Tuple: (success flag, expense response, new balance, error message)</returns>
        Task<(bool success, ExpenseResponseDto? expense, decimal newBalance, string errorMessage)>
            LogExpenseAsync(int childId, LogExpenseDto dto);

        // ==================== EXPENSE HISTORY (CHILD VIEW) ====================

        /// <summary>
        /// Gets the logged-in child's expense history.
        /// Can filter by date range, category, and mood.
        /// </summary>
        /// <param name="childId">ID of the child (from JWT token)</param>
        /// <param name="startDate">Optional: Filter from this date</param>
        /// <param name="endDate">Optional: Filter until this date</param>
        /// <param name="categoryId">Optional: Filter by category</param>
        /// <param name="moodId">Optional: Filter by mood</param>
        /// <returns>Tuple: (success flag, expense history, error message)</returns>
        Task<(bool success, ExpenseHistoryDto? history, string errorMessage)>
            GetMyExpensesAsync(
                int childId,
                DateTime? startDate = null,
                DateTime? endDate = null,
                int? categoryId = null,
                int? moodId = null);

        // ==================== EXPENSE HISTORY (PARENT VIEW) ====================

        /// <summary>
        /// Gets a child's expense history (parent view).
        /// Includes authorization check - parent must be linked to this child.
        /// Can filter by date range, category, and mood.
        /// </summary>
        /// <param name="parentId">ID of the parent (from JWT token)</param>
        /// <param name="childId">ID of the child to view expenses for</param>
        /// <param name="startDate">Optional: Filter from this date</param>
        /// <param name="endDate">Optional: Filter until this date</param>
        /// <param name="categoryId">Optional: Filter by category</param>
        /// <param name="moodId">Optional: Filter by mood</param>
        /// <returns>Tuple: (success flag, expense history, error message)</returns>
        Task<(bool success, ExpenseHistoryDto? history, string errorMessage)>
            GetChildExpensesAsync(
                int parentId,
                int childId,
                DateTime? startDate = null,
                DateTime? endDate = null,
                int? categoryId = null,
                int? moodId = null);

        // ==================== CATEGORIES AND MOODS ====================

        /// <summary>
        /// Gets all available expense categories.
        /// Used for dropdown in expense logging form.
        /// </summary>
        /// <returns>List of categories</returns>
        Task<List<CategoryDto>> GetCategoriesAsync();

        /// <summary>
        /// Gets all available moods.
        /// Used for mood picker in expense logging form.
        /// </summary>
        /// <returns>List of moods</returns>
        Task<List<MoodDto>> GetMoodsAsync();
        /// <summary>
        /// Updates an existing expense for a child.
        /// Recalculates balance, registers corrective transaction if amount changed.
        /// </summary>
        Task<(bool success, ExpenseResponseDto? expense, decimal newBalance, string errorMessage)>
            UpdateExpenseAsync(int childId, int expenseId, UpdateExpenseDto dto);

    }
}