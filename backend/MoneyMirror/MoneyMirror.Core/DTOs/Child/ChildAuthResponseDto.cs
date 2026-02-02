namespace MoneyMirror.Core.DTOs.Child
{
    /// <summary>
    /// Data Transfer Object returned after successful child authentication.
    /// Contains JWT tokens and basic child information.
    /// Used as output for POST /api/children/login-with-code endpoint.
    /// </summary>
    public class ChildAuthResponseDto
    {
        /// <summary>
        /// Short-lived JWT access token (valid for 15 minutes).
        /// Include this in Authorization header for protected API requests.
        /// Format: "Bearer {AccessToken}"
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Long-lived refresh token (valid for 7 days).
        /// Use this to obtain a new access token when the old one expires.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Timestamp when the access token expires (UTC).
        /// Client should request a new token before this time.
        /// </summary>
        public DateTime AccessTokenExpiration { get; set; }

        /// <summary>
        /// Timestamp when the refresh token expires (UTC).
        /// Child must log in again after this time.
        /// </summary>
        public DateTime RefreshTokenExpiration { get; set; }

        /// <summary>
        /// Child's unique identifier from the database.
        /// Use this to make child-specific API calls.
        /// </summary>
        public int ChildId { get; set; }

        /// <summary>
        /// Child's first name.
        /// Useful for displaying in UI.
        /// </summary>
        public string ChildFirstName { get; set; }

        /// <summary>
        /// Child's age calculated from date of birth.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Child's personality profile information (if available).
        /// Null if personality analysis hasn't been completed yet.
        /// </summary>
        public PersonalityProfileDto? PersonalityProfile { get; set; }

        /// <summary>
        /// Indicates whether the personality profile has been finalized by AI analysis.
        /// False = using temporary "Pending Analysis" personality type
        /// True = real AI analysis has been completed
        /// </summary>
        public bool IsPersonalityFinalized { get; set; }
    }
}
