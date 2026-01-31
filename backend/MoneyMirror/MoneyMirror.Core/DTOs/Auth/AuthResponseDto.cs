namespace MoneyMirror.Core.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object returned after successful authentication.
    /// Contains JWT tokens and basic parent information.
    /// Used as output for POST /api/auth/login and POST /api/auth/refresh-token endpoints.
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// Short-lived JWT access token (valid for 15 minutes).
        /// Include this in Authorization header for protected API requests.
        /// Format: "Bearer {AccessToken}"
        /// Example: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Long-lived refresh token (valid for 7 days).
        /// Use this to obtain a new access token when the old one expires.
        /// Store securely on client (not in localStorage, use secure HTTP-only cookie if web).
        /// Example: "550e8400-e29b-41d4-a716-446655440000"
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Timestamp when the access token expires (UTC).
        /// Client should request a new token before this time.
        /// Example: "2025-02-01T15:30:00Z"
        /// </summary>
        public DateTime AccessTokenExpiration { get; set; }

        /// <summary>
        /// Timestamp when the refresh token expires (UTC).
        /// User must log in again after this time.
        /// Example: "2025-02-08T00:00:00Z"
        /// </summary>
        public DateTime RefreshTokenExpiration { get; set; }

        /// <summary>
        /// Parent's unique identifier from the database.
        /// Use this to make parent-specific API calls.
        /// Example: 42
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// Parent's email address.
        /// Useful for displaying in UI.
        /// Example: "john.smith@email.com"
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Parent's full name.
        /// Concatenated from FirstName + LastName.
        /// Example: "John Smith"
        /// </summary>
        public string FullName { get; set; }
    }
}
