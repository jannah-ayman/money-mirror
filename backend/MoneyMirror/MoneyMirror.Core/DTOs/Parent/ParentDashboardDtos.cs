namespace MoneyMirror.Core.DTOs.Parent
{
    /// <summary>
    /// Data Transfer Object for parent's own profile view.
    /// Shows current information (before editing).
    /// Used for GET /api/auth/my-profile endpoint.
    /// </summary>
    public class ParentProfileResponseDto
    {
        /// <summary>
        /// Parent's unique ID
        /// </summary>
        public int ParentID { get; set; }

        public string Email { get; set; }

        /// <summary>
        /// Parent's first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Parent's last name
        /// </summary>
        public string LastName { get; set; }

        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Whether email is confirmed
        /// </summary>
        public bool IsEmailConfirmed { get; set; }

        /// <summary>
        /// Number of children under this parent
        /// </summary>
        public int TotalChildren { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for parent's main dashboard.
    /// Shows overview of parent account and children.
    /// </summary>
    public class ParentDashboardDto
    {
        /// <summary>
        /// Parent's first name for greeting
        /// Example: "John"
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Total number of children
        /// </summary>
        public int TotalChildren { get; set; }

        /// <summary>
        /// Quick summary cards for each child (for top buttons)
        /// Shows basic info without needing separate API calls
        /// </summary>
        public List<ChildQuickCardDto> Children { get; set; }
    }

    /// <summary>
    /// Quick summary card for a child shown on parent dashboard.
    /// This is what appears when parent has buttons for each child at the top.
    /// Shows just enough info for the parent to navigate.
    /// </summary>
    public class ChildQuickCardDto
    {
        /// <summary>
        /// Child's unique ID
        /// </summary>
        public int ChildID { get; set; }

        /// <summary>
        /// Child's first name (for the button)
        /// Example: "Emma"
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Child's current balance
        /// Shows on the button or card
        /// </summary>
        public decimal CurrentBalance { get; set; }

        /// <summary>
        /// Child's age
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Avatar URL (future feature)
        /// </summary>
        public string? AvatarUrl { get; set; }
    }

    /// <summary>
    /// Detailed view for when parent clicks on a specific child's button.
    /// Shows the child's balance and quick stats so parent knows what to manage.
    /// This is the "Emma's Section" that appears when parent clicks Emma's button.
    /// </summary>
    public class ChildDetailedCardDto
    {
        /// <summary>
        /// Child's basic info
        /// </summary>
        public int ChildID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }

        /// <summary>
        /// Current balance (main display)
        /// </summary>
        public decimal CurrentBalance { get; set; }

        /// <summary>
        /// Quick stats for parent to see
        /// </summary>
        public ChildQuickStatsDto QuickStats { get; set; }

        /// <summary>
        /// Allowance info (if set up)
        /// </summary>
        public ChildAllowanceInfoSummaryDto? AllowanceInfo { get; set; }
    }

    /// <summary>
    /// Quick statistics about a child.
    /// Shown in the child's detailed card.
    /// </summary>
    public class ChildQuickStatsDto
    {
        /// <summary>
        /// Total spent this month
        /// </summary>
        public decimal TotalSpentThisMonth { get; set; }

        /// <summary>
        /// Number of expenses logged this month
        /// </summary>
        public int ExpensesCountThisMonth { get; set; }

        /// <summary>
        /// Number of active savings goals
        /// </summary>
        public int ActiveGoalsCount { get; set; }

        /// <summary>
        /// Personality type name (parent-facing)
        /// Example: "Impulsive Spender"
        /// </summary>
        public string? PersonalityTypeName { get; set; }
    }

    /// <summary>
    /// Simple allowance info for child card.
    /// Shows if allowance is set up and when next payment is.
    /// </summary>
    public class ChildAllowanceInfoSummaryDto
    {
        /// <summary>
        /// Allowance frequency type
        /// Example: "Weekly", "Monthly"
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Amount per cycle
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// When next payment will be (approximate)
        /// </summary>
        public DateTime? NextPaymentDate { get; set; }

        /// <summary>
        /// Whether allowance is active
        /// </summary>
        public bool IsActive { get; set; }
    }
}