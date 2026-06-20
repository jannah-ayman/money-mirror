namespace MoneyMirror.Core.DTOs.Expense
{
    
    /// Data Transfer Object for logging a new expense.
    /// Child provides these details when they spend money.
    /// Used as input for POST /api/expense/log endpoint.
    /// Validation is handled by LogExpenseDtoValidator using FluentValidation.
    
    public class LogExpenseDto
    {
 
        public string? ItemName { get; set; }

        public int CategoryID { get; set; }

     
        public int MoodID { get; set; }

        public decimal MoneyAmount { get; set; }

        public string? Note { get; set; }
        public DateTime? LogDate { get; set; }

    }
}