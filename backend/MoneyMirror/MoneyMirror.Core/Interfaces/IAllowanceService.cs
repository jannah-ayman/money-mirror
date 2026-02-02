using MoneyMirror.Core.DTOs.Allowance;

namespace MoneyMirror.Core.Interfaces
{
    /// <summary>
    /// Interface for allowance and balance management operations.
    /// Handles recurring allowances, one-time bonuses, balance tracking, and transaction history.
    /// </summary>
    public interface IAllowanceService
    {
        // ==================== ALLOWANCE CONFIGURATION ====================

        /// <summary>
        /// Creates or updates a recurring allowance schedule for a child.
        /// If child already has an active recurring allowance, it will be deactivated and replaced.
        /// Only the parent linked to this child can perform this operation.
        /// </summary>
        /// <param name="parentId">ID of the parent (from JWT token)</param>
        /// <param name="childId">ID of the child to configure allowance for</param>
        /// <param name="dto">Allowance configuration (frequency, amount, schedule)</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> UpdateAllowanceAsync(
            int parentId,
            int childId,
            UpdateAllowanceDto dto);

        /// <summary>
        /// Gets the current recurring allowance configuration for a child.
        /// Returns null if no active recurring allowance is set up.
        /// Only the parent linked to this child can view this.
        /// </summary>
        /// <param name="parentId">ID of the parent (from JWT token)</param>
        /// <param name="childId">ID of the child</param>
        /// <returns>Tuple: (success flag, allowance details or null, error message)</returns>
        Task<(bool success, AllowanceDetailsDto? allowance, string errorMessage)> GetAllowanceAsync(
            int parentId,
            int childId);

        // ==================== BONUS ====================

        /// <summary>
        /// Gives a one-time bonus to a child.
        /// Credits the child's balance immediately and creates a transaction record.
        /// Only the parent linked to this child can give bonuses.
        /// </summary>
        /// <param name="parentId">ID of the parent (from JWT token)</param>
        /// <param name="childId">ID of the child receiving the bonus</param>
        /// <param name="dto">Bonus amount and reason</param>
        /// <returns>Tuple: (success flag, new balance, error message)</returns>
        Task<(bool success, decimal newBalance, string errorMessage)> GiveBonusAsync(
            int parentId,
            int childId,
            GiveBonusDto dto);

        // ==================== BALANCE (PARENT VIEW) ====================

        /// <summary>
        /// Gets a child's current balance.
        /// Parent version - includes additional context.
        /// Only the parent linked to this child can view balance.
        /// </summary>
        /// <param name="parentId">ID of the parent (from JWT token)</param>
        /// <param name="childId">ID of the child</param>
        /// <returns>Tuple: (success flag, balance info, error message)</returns>
        Task<(bool success, BalanceResponseDto? balance, string errorMessage)> GetChildBalanceAsync(
            int parentId,
            int childId);

        // ==================== BALANCE (CHILD VIEW) ====================

        /// <summary>
        /// Gets the logged-in child's current balance.
        /// Child version - simpler response.
        /// </summary>
        /// <param name="childId">ID of the child (from JWT token)</param>
        /// <returns>Tuple: (success flag, balance info, error message)</returns>
        Task<(bool success, BalanceResponseDto? balance, string errorMessage)> GetMyBalanceAsync(
            int childId);

        // ==================== TRANSACTION HISTORY (PARENT VIEW) ====================

        /// <summary>
        /// Gets transaction history for a child.
        /// Parent version - can filter by date range and transaction type.
        /// Only the parent linked to this child can view transactions.
        /// </summary>
        /// <param name="parentId">ID of the parent (from JWT token)</param>
        /// <param name="childId">ID of the child</param>
        /// <param name="startDate">Optional: Filter transactions from this date</param>
        /// <param name="endDate">Optional: Filter transactions until this date</param>
        /// <param name="type">Optional: Filter by type ("AllowanceCredit", "BonusCredit", or "All")</param>
        /// <returns>Tuple: (success flag, transaction history, error message)</returns>
        Task<(bool success, TransactionHistoryDto? history, string errorMessage)> GetTransactionHistoryAsync(
            int parentId,
            int childId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string type = "All");

        // ==================== TRANSACTION HISTORY (CHILD VIEW) ====================

        /// <summary>
        /// Gets the logged-in child's transaction history.
        /// Child version - simpler filtering.
        /// </summary>
        /// <param name="childId">ID of the child (from JWT token)</param>
        /// <param name="startDate">Optional: Filter transactions from this date</param>
        /// <returns>Tuple: (success flag, transaction history, error message)</returns>
        Task<(bool success, TransactionHistoryDto? history, string errorMessage)> GetMyTransactionsAsync(
            int childId,
            DateTime? startDate = null);

        // ==================== CHILD ALLOWANCE INFO ====================

        /// <summary>
        /// Gets child-friendly information about their allowance.
        /// Shows a simple message and next payment date.
        /// </summary>
        /// <param name="childId">ID of the child (from JWT token)</param>
        /// <returns>Tuple: (success flag, allowance info, error message)</returns>
        Task<(bool success, ChildAllowanceInfoDto? info, string errorMessage)> GetMyAllowanceInfoAsync(
            int childId);

        // ==================== BACKGROUND JOB ====================

        /// <summary>
        /// Credits all due recurring allowances.
        /// Called by Hangfire background job every hour.
        /// Checks all active recurring allowances and credits those that are due.
        /// </summary>
        /// <returns>Number of allowances successfully credited</returns>
        Task<int> CreditScheduledAllowancesAsync();
    }
}
