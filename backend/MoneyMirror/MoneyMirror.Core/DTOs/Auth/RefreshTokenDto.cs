namespace MoneyMirror.Core.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for refreshing JWT tokens.
    /// Used when the access token expires and client needs a new one.
    /// Used as input for POST /api/auth/refresh-token endpoint.
    /// Validation is handled by RefreshTokenDtoValidator using FluentValidation.
    /// </summary>
    public class RefreshTokenDto
    {
        /// <summary>
        /// The expired or soon-to-expire access token.
        /// Server will extract user information from this token (even if expired).
        /// Example: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// The refresh token received during login.
        /// Must be valid (not expired, matches database record).
        /// Example: "550e8400-e29b-41d4-a716-446655440000"
        /// </summary>
        public string RefreshToken { get; set; }
    }
}