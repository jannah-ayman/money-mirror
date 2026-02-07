namespace MoneyMirror.Core.DTOs.Expense
{
    /// <summary>
    /// Data Transfer Object for returning expense details.
    /// Used in responses after logging an expense or getting expense history.
    /// </summary>
    public class ExpenseResponseDto
    {
        /// <summary>
        /// Unique identifier for this expense.
        /// </summary>
        public int ExpenseID { get; set; }

        /// <summary>
        /// Name of the item purchased.
        /// Example: "Ice Cream"
        /// </summary>
        public string? ItemName { get; set; }

        /// <summary>
        /// Name of the category (not ID - easier for frontend to display).
        /// Example: "Snacks / Food"
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Description of the mood (not ID).
        /// Example: "Happy"
        /// </summary>
        public string MoodDescription { get; set; }

        /// <summary>
        /// Amount spent on this purchase.
        /// Example: 15.50
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// When this expense was logged.
        /// </summary>
        public DateTime LogDate { get; set; }

        /// <summary>
        /// Optional note about the purchase.
        /// Can be null.
        /// </summary>
        public string? Note { get; set; }
    }
}