namespace MoneyMirror.Core.DTOs.Expense
{
    /// <summary>
    /// Data Transfer Object for logging a new expense.
    /// Child provides these details when they spend money.
    /// Used as input for POST /api/expense/log endpoint.
    /// Validation is handled by LogExpenseDtoValidator using FluentValidation.
    /// </summary>
    public class LogExpenseDto
    {
        /// <summary>
        /// Name/description of the item purchased.
        /// Example: "Ice Cream", "Comic Book", "Birthday Gift"
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// ID of the expense category.
        /// Example: 1 = "Snacks / Food"
        /// Must be a valid category from ExpenseCategories table.
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// ID of the mood/feeling when making this purchase.
        /// Example: 1 = "Happy"
        /// Must be a valid mood from Moods table.
        /// </summary>
        public int MoodID { get; set; }

        /// <summary>
        /// Amount of money spent.
        /// Must be greater than 0 and less than or equal to child's current balance.
        /// Example: 15.50
        /// </summary>
        public decimal MoneyAmount { get; set; }

        /// <summary>
        /// Optional note/comment about the purchase.
        /// Example: "Shared with my friend", "Regret buying this"
        /// Can be null or empty.
        /// </summary>
        public string? Note { get; set; }
    }
}