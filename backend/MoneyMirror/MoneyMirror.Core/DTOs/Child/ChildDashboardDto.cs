namespace MoneyMirror.Core.DTOs.Child
{
    /// <summary>
    /// Data Transfer Object for child's main dashboard.
    /// Contains everything a child sees on their home screen.
    /// </summary>
    public class ChildDashboardDto
    {
        /// <summary>
        /// Child's first name for greeting
        /// Example: "Emma"
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Current account balance
        /// Example: 125.50
        /// </summary>
        public decimal CurrentBalance { get; set; }

        /// <summary>
        /// Avatar URL (for future implementation)
        /// Example: "https://cloudinary.com/avatars/cat.png"
        /// Null if not set yet
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Personality type child-friendly name
        /// Example: "Smart Spender"
        /// </summary>
        public string PersonalityName { get; set; }

        /// <summary>
        /// Fun facts to display on dashboard
        /// Example: "You're great at planning your purchases! 🎯"
        /// </summary>
        public string? FunFacts { get; set; }

        /// <summary>
        /// Count of unlogged expenses (if we track this in future)
        /// For now, can be 0
        /// </summary>
        public int UnloggedExpensesCount { get; set; }

        /// <summary>
        /// Count of active savings goals
        /// For future implementation
        /// </summary>
        public int ActiveGoalsCount { get; set; }
    }
}