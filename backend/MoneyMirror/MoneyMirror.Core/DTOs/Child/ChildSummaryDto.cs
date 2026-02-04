namespace MoneyMirror.Core.DTOs.Child
{
    /// <summary>
    /// Data Transfer Object for displaying a child in the parent's children list.
    /// Contains basic information needed for the "Manage Children" tab.
    /// Used as output for GET /api/children/my-children endpoint.
    /// </summary>
    public class ChildSummaryDto
    {
        public int ChildID { get; set; }
        public string ChildName { get; set; }
        public DateTime DOB { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }

        public string LoginCode { get; set; }

        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Indicates whether the personality profile has been finalized by AI analysis.
        /// False = using temporary "Pending Analysis" personality type
        /// True = real AI analysis has been completed
        /// </summary>
        public bool IsPersonalityFinalized { get; set; }

        public string? PersonalityTypeName { get; set; }
    }
}
