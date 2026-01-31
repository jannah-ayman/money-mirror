using System.ComponentModel.DataAnnotations;

namespace MoneyMirror.Core.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for refreshing JWT tokens.
    /// Used when the access token expires and client needs a new one.
    /// Used as input for POST /api/auth/refresh-token endpoint.
    /// </summary>
    public class RefreshTokenDto
    {
        /// <summary>
        /// The expired or soon-to-expire access token.
        /// Server will extract user information from this token (even if expired).
        /// Example: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
        /// </summary>
        [Required(ErrorMessage = "Access token is required")]
        public string AccessToken { get; set; }

        /// <summary>
        /// The refresh token received during login.
        /// Must be valid (not expired, matches database record).
        /// Example: "550e8400-e29b-41d4-a716-446655440000"
        /// </summary>
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; }
    }
}
