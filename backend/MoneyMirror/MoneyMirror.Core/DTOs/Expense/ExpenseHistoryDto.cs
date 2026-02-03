namespace MoneyMirror.Core.DTOs.Expense
{
    /// <summary>
    /// Data Transfer Object for returning expense history with summary.
    /// Used as output for GET /api/expense/my-expenses and /api/expense/{childId}/expenses endpoints.
    /// </summary>
    public class ExpenseHistoryDto
    {
        /// <summary>
        /// List of expenses matching the filter criteria.
        /// Ordered by date (newest first).
        /// </summary>
        public List<ExpenseResponseDto> Expenses { get; set; }

        /// <summary>
        /// Total number of expenses in this result.
        /// Example: 15
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Sum of all expense amounts in this result.
        /// Example: 127.50 (total spent in filtered period)
        /// </summary>
        public decimal TotalSpent { get; set; }
    }
}