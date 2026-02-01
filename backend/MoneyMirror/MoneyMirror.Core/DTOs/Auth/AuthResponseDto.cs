namespace MoneyMirror.Core.DTOs.Auth
{
    /// Data Transfer Object returned after successful authentication.
    /// Contains JWT tokens and basic parent information.
    /// Used as output for POST /api/auth/login and POST /api/auth/refresh-token endpoints.
    public class AuthResponseDto
    {
        /// Short-lived JWT access token (valid for 15 minutes).
        /// Include this in Authorization header for protected API requests.
        /// Format: "Bearer {AccessToken}"
        public string AccessToken { get; set; }

        /// Long-lived refresh token (valid for 7 days).
        /// Use this to obtain a new access token when the old one expires.
        /// Store securely on client (not in localStorage, use secure HTTP-only cookie if web).
        public string RefreshToken { get; set; }

        /// Timestamp when the access token expires (UTC).
        /// Client should request a new token before this time.
        /// Example: "2025-02-01T15:30:00Z"
        public DateTime AccessTokenExpiration { get; set; }

        /// Timestamp when the refresh token expires (UTC).
        /// User must log in again after this time.
        /// Example: "2025-02-08T00:00:00Z"
        public DateTime RefreshTokenExpiration { get; set; }

        /// Parent's unique identifier from the database.
        /// Use this to make parent-specific API calls.
        public int ParentId { get; set; }

        /// Parent's email address.
        /// Useful for displaying in UI.
        public string Email { get; set; }

        /// Parent's full name.
        /// Concatenated from FirstName + LastName.

        public string FullName { get; set; }
    }
}
