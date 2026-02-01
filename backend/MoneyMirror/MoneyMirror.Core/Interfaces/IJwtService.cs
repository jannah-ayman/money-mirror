using System.Security.Claims;
using MoneyMirror.Core.Models;

namespace MoneyMirror.Core.Interfaces
{
    /// Interface for JWT token generation and validation operations.
    /// Handles creation of access tokens and refresh tokens for authentication.
    public interface IJwtService
    {
        /// Generates a JWT access token for an authenticated parent.
        /// Token contains parent's ID, email, and role as claims.
        /// Token is short-lived (15 minutes) for security.
        /// <param name="parent">The authenticated parent user</param>
        /// <returns>JWT token string (e.g., "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...")</returns>
        string GenerateAccessToken(Parent parent);

        /// Generates a cryptographically secure refresh token.
        /// Refresh token is a random GUID stored in database.
        /// Used to obtain new access tokens without re-login.
        /// <returns>Refresh token string (e.g., "550e8400-e29b-41d4-a716-446655440000")</returns>
        string GenerateRefreshToken();

        /// Extracts claims (user information) from a JWT token.
        /// Can extract from expired tokens (useful for refresh token flow).
        /// <param name="token">The JWT token to parse</param>
        /// <returns>ClaimsPrincipal containing user claims, or null if invalid</returns>
        ClaimsPrincipal? GetPrincipalFromToken(string token);

        /// Validates a JWT token's signature and expiration.
        /// <param name="token">The JWT token to validate</param>
        /// <returns>True if token is valid and not expired, False otherwise</returns>
        bool ValidateToken(string token);
    }
}
