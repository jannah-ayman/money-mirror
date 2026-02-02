namespace MoneyMirror.Core.DTOs.Child
{
    /// <summary>
    /// Data Transfer Object for refreshing child JWT tokens.
    /// Used when the access token expires and child needs a new one.
    /// Used as input for POST /api/children/refresh-token endpoint.
    /// Validation is handled by ChildRefreshTokenDtoValidator using FluentValidation.
    /// </summary>
    public class ChildRefreshTokenDto
    {
        /// <summary>
        /// The expired or soon-to-expire access token.
        /// Server will extract child information from this token (even if expired).
        /// Example: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// The refresh token received during login.
        /// Must be valid (not expired, matches database record).
        /// Example: "GK8z5M3QpL2vR9XnB7Yw1F6Dc4Hj0Ps..."
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
