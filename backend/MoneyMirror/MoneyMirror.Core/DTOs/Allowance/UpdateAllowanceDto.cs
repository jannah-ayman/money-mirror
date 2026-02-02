namespace MoneyMirror.Core.DTOs.Allowance
{
    /// <summary>
    /// Data Transfer Object for setting or updating a child's recurring allowance.
    /// Parent specifies frequency, amount, and schedule details.
    /// Used as input for PUT /api/allowance/{childId} endpoint.
    /// Validation is handled by UpdateAllowanceDtoValidator using FluentValidation.
    /// </summary>
    public class UpdateAllowanceDto
    {
        /// <summary>
        /// Frequency of allowance.
        /// Values: "Daily", "Weekly", "Monthly"
        /// Example: "Weekly"
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Amount to credit each cycle.
        /// Example: 50.00 = 50 Egyptian Pounds per week
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// For Daily allowances: Hour of day (0-23) to credit.
        /// Example: 9 = 9:00 AM UTC
        /// Required if Type = "Daily", ignored otherwise.
        /// </summary>
        public int? DailyHour { get; set; }

        /// <summary>
        /// For Weekly allowances: Day of week to credit.
        /// Values: "Saturday", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday"
        /// Example: "Saturday"
        /// Required if Type = "Weekly", ignored otherwise.
        /// </summary>
        public string? WeeklyDay { get; set; }

        /// <summary>
        /// For Monthly allowances: Day of month (1-31) to credit.
        /// Example: 15 = 15th of each month
        /// Required if Type = "Monthly", ignored otherwise.
        /// </summary>
        public int? MonthlyDay { get; set; }

        /// <summary>
        /// Whether this allowance is active.
        /// True = credits will be made automatically
        /// False = allowance is paused
        /// Default: true
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}