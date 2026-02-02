namespace MoneyMirror.Core.DTOs.Child
{
    /// <summary>
    /// Data Transfer Object for displaying a child in the parent's children list.
    /// Contains basic information needed for the "Manage Children" tab.
    /// Used as output for GET /api/children/my-children endpoint.
    /// </summary>
    public class ChildSummaryDto
    {
        /// <summary>
        /// Child's unique identifier.
        /// </summary>
        public int ChildID { get; set; }

        /// <summary>
        /// Child's full name (FirstName + LastName).
        /// Example: "Emma Smith"
        /// </summary>
        public string ChildName { get; set; }

        /// <summary>
        /// Child's date of birth.
        /// </summary>
        public DateTime DOB { get; set; }

        /// <summary>
        /// Child's age calculated from date of birth.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// The unique 6-character login code for this child.
        /// Parent can share this with the child or other guardians.
        /// Example: "ABC123"
        /// </summary>
        public string LoginCode { get; set; }

        /// <summary>
        /// Timestamp when this child was added to the system.
        /// Used for sorting (newest first).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Indicates whether the personality profile has been finalized by AI analysis.
        /// False = using temporary "Pending Analysis" personality type
        /// True = real AI analysis has been completed
        /// </summary>
        public bool IsPersonalityFinalized { get; set; }

        /// <summary>
        /// Brief personality type name (if available).
        /// Example: "Pending Analysis", "Speedy Spender", "Treasure Keeper"
        /// </summary>
        public string? PersonalityTypeName { get; set; }
    }
}
