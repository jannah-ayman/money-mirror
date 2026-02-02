using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Core.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MoneyMirror.Infrastructure.Services
{
    /// Service for JWT token generation and validation.
    /// Implements secure token creation using HS256 algorithm.
    /// Supports both parent and child authentication.
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _accessTokenExpirationMinutes;

        /// Constructor - dependency injection provides configuration.
        /// Reads JWT settings from appsettings.json.
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;

            // Load JWT settings from configuration
            _secretKey = _configuration["Jwt:SecretKey"]
                ?? throw new InvalidOperationException("JWT SecretKey not configured");
            _issuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("JWT Issuer not configured");
            _audience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("JWT Audience not configured");
            _accessTokenExpirationMinutes = int.Parse(
                _configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15");
        }

        /// Generates a JWT access token for an authenticated parent.
        /// Token contains claims (user info) and is signed with secret key.
        public string GenerateAccessToken(Parent parent)
        {
            // Claims are pieces of information about the user stored in the token
            var claims = new[]
            {
                // "sub" (subject) = unique user identifier
                new Claim(JwtRegisteredClaimNames.Sub, parent.ParentID.ToString()),
                
                // "email" = user's email address
                new Claim(JwtRegisteredClaimNames.Email, parent.Email),
                
                // "name" = user's full name
                new Claim(JwtRegisteredClaimNames.Name, $"{parent.FName} {parent.LName}"),
                
                // "jti" (JWT ID) = unique identifier for this token
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                
                // Custom claim for user role
                new Claim(ClaimTypes.Role, "Parent"),
                
                // Custom claim for parent ID (easier to access than "sub")
                new Claim("ParentId", parent.ParentID.ToString())
            };

            // Create signing key from secret
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the token
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes),
                signingCredentials: credentials
            );

            // Serialize token to string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// Generates a JWT access token for an authenticated child.
        /// Token contains child-specific claims and is signed with secret key.
        public string GenerateAccessToken(Child child)
        {
            // Claims are pieces of information about the child stored in the token
            var claims = new[]
            {
                // "sub" (subject) = unique child identifier
                new Claim(JwtRegisteredClaimNames.Sub, child.ChildID.ToString()),
                
                // "name" = child's full name
                new Claim(JwtRegisteredClaimNames.Name, $"{child.FName} {child.LName}"),
                
                // "jti" (JWT ID) = unique identifier for this token
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                
                // Custom claim for user role
                new Claim(ClaimTypes.Role, "Child"),
                
                // Custom claim for child ID (easier to access than "sub")
                new Claim("ChildId", child.ChildID.ToString()),
                
                // Custom claim for login code (useful for debugging/logging)
                new Claim("LoginCode", child.LoginCode)
            };

            // Create signing key from secret
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the token
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes),
                signingCredentials: credentials
            );

            // Serialize token to string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// Generates a cryptographically secure refresh token.
        /// Returns a random GUID that will be stored in the database.
        public string GenerateRefreshToken()
        {
            // Generate a random 32-byte array
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            // Convert to Base64 string (URL-safe)
            return Convert.ToBase64String(randomBytes);
        }

        /// Extracts claims from a JWT token without validating expiration.
        /// Useful for refresh token flow where we need to get user info from expired token.
        public ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);

                // Validation parameters - note we're NOT validating lifetime
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = false, // Allow expired tokens
                    ClockSkew = TimeSpan.Zero
                };

                // Parse and validate token
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Verify it's a JWT token with correct algorithm
                if (validatedToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                // Token is invalid (bad signature, malformed, etc.)
                return null;
            }
        }

        // Validates a JWT token including expiration check.
        // Returns true only if token is valid and not expired.
        public bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);

                // Validation parameters - this time we DO validate lifetime
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true, // Check if token is expired
                    ClockSkew = TimeSpan.Zero // No grace period for expiration
                };

                tokenHandler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}