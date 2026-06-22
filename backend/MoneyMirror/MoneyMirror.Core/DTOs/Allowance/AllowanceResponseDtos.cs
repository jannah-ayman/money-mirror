namespace MoneyMirror.Core.DTOs.Allowance
{
    /// <summary>
    /// Data Transfer Object for returning a child's current balance.
    /// Used as output for GET /api/allowance/{childId}/balance endpoint.
    /// </summary>
    public class BalanceResponseDto
    {
        /// <summary>
        /// Child's current balance in Egyptian Pounds.
        /// Example: 125.50
        /// </summary>
        public decimal CurrentBalance { get; set; }

        /// <summary>
        /// Child's unique identifier.
        /// </summary>
        public int ChildId { get; set; }

        /// <summary>
        /// Child's full name.
        /// Example: "Emma Smith"
        /// </summary>
        public string ChildName { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for returning allowance configuration details.
    /// Used as output for GET /api/allowance/{childId} endpoint.
    /// Null if no recurring allowance is set up for this child.
    /// </summary>
    public class AllowanceDetailsDto
    {
        /// <summary>
        /// Allowance record ID.
        /// </summary>
        public int AllowanceId { get; set; }

        /// <summary>
        /// Frequency type: "Daily", "Weekly", or "Monthly".
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Amount credited per cycle.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Whether allowance is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// For Daily: Hour of day (0-23).
        /// Null if not Daily.
        /// </summary>
        public int? DailyHour { get; set; }

        /// <summary>
        /// For Weekly: Day of week.
        /// Null if not Weekly.
        /// </summary>
        public string? WeeklyDay { get; set; }

        /// <summary>
        /// For Monthly: Day of month (1-31).
        /// Null if not Monthly.
        /// </summary>
        public int? MonthlyDay { get; set; }

        /// <summary>
        /// Last time this allowance was credited by background job.
        /// Null if never credited yet.
        /// </summary>
        public DateTime? LastCreditedDate { get; set; }

        /// <summary>
        /// When this allowance was initially set up by parent.
        /// </summary>
        public DateTime SetDate { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for returning a single transaction record.
    /// Used in transaction history lists.
    /// </summary>
    public class TransactionDto
    {
        /// <summary>
        /// Transaction record ID.
        /// </summary>
        public int TransactionId { get; set; }

        /// <summary>
        /// Type of transaction: "AllowanceCredit", "BonusCredit", etc.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Amount involved (positive = credit, negative = debit).
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Child's balance after this transaction.
        /// </summary>
        public decimal BalanceAfter { get; set; }

        /// <summary>
        /// Description of why this transaction occurred.
        /// Example: "Weekly allowance credited", "Bonus for good grades!"
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// When this transaction occurred.
        /// </summary>
        public DateTime TransactionDate { get; set; }
        /// Name of the parent who gave this (null if not parent-initiated, e.g. expenses).
        public string? GivenByName { get; set; }

        /// Role of the parent who gave this (e.g. "Father", "Mother").
        public string? GivenByRole { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for returning transaction history.
    /// Used as output for GET /api/allowance/{childId}/transactions endpoint.
    /// </summary>
    public class TransactionHistoryDto
    {
        /// <summary>
        /// List of transactions matching the filter criteria.
        /// Ordered by date (newest first).
        /// </summary>
        public List<TransactionDto> Transactions { get; set; }

        /// <summary>
        /// Total amount of all credits (allowances + bonuses) in this result set.
        /// Useful for showing "You received 200.00 this month".
        /// </summary>
        public decimal TotalCredits { get; set; }

        /// <summary>
        /// Total number of transactions in this result set.
        /// </summary>
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for child-friendly allowance information.
    /// Used as output for GET /api/allowance/my-allowance endpoint (child endpoints).
    /// Shows when the next payment will be.
    /// </summary>
    public class ChildAllowanceInfoDto
    {
        /// <summary>
        /// Child-friendly message about their allowance.
        /// Example: "You get 50.00 every Saturday! 🎉"
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Estimated date/time of next allowance credit.
        /// Null if allowance is paused or not set up.
        /// </summary>
        public DateTime? NextPaymentDate { get; set; }

        /// <summary>
        /// Whether allowance is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Amount they receive per cycle.
        /// </summary>
        public decimal Amount { get; set; }
    }
}