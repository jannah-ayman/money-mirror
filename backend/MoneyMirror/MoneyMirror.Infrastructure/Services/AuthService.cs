using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Auth;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Core.Models;
using MoneyMirror.Infrastructure.Data;

namespace MoneyMirror.Infrastructure.Services
{
    /// <summary>
    /// Service implementing authentication and authorization logic.
    /// Handles parent registration, login, email confirmation, and token refresh.
    /// Uses BCrypt for password hashing and JWT for token-based authentication.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        /// <summary>
        /// Constructor - dependency injection provides all required services.
        /// </summary>
        public AuthService(
            ApplicationDbContext context,
            IJwtService jwtService,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new parent account.
        /// Steps:
        /// 1. Check if email already exists
        /// 2. Hash the password using BCrypt
        /// 3. Create parent record in database
        /// 4. Generate email confirmation token
        /// 5. Send confirmation email
        /// </summary>
        public async Task<(bool success, string message, int? parentId)> RegisterParentAsync(RegisterParentDto registerDto)
        {
            try
            {
                // STEP 1: Check if email already exists
                var emailExists = await EmailExistsAsync(registerDto.Email);
                if (emailExists)
                {
                    _logger.LogWarning($"Registration attempt with existing email: {registerDto.Email}");
                    return (false, "Email is already registered", null);
                }

                // STEP 2: Hash the password using BCrypt
                // BCrypt automatically generates a salt and handles secure hashing
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                // STEP 3: Generate email confirmation token (a unique GUID)
                string confirmationToken = Guid.NewGuid().ToString();

                // Token expires in 24 hours (configurable)
                int tokenExpirationHours = int.Parse(
                    _configuration["EmailConfirmation:TokenExpirationHours"] ?? "24");
                DateTime tokenExpiry = DateTime.UtcNow.AddHours(tokenExpirationHours);

                // STEP 4: Create new parent entity
                var newParent = new Parent
                {
                    Email = registerDto.Email.ToLower().Trim(), // Normalize email
                    HashedPassword = hashedPassword,
                    FName = registerDto.FirstName.Trim(),
                    LName = registerDto.LastName.Trim(),
                    PhoneNum = registerDto.PhoneNumber?.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    IsEmailConfirmed = false,
                    EmailConfirmationToken = confirmationToken,
                    EmailConfirmationTokenExpiry = tokenExpiry
                };

                // STEP 5: Save to database
                await _context.Parents.AddAsync(newParent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Parent registered successfully: {newParent.Email} (ID: {newParent.ParentID})");

                // STEP 6: Send confirmation email
                string frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:19006";
                string confirmationLink = $"{frontendUrl}/auth/confirm-email?email={newParent.Email}&token={confirmationToken}";

                bool emailSent = await _emailService.SendEmailConfirmationAsync(
                    newParent.Email,
                    $"{newParent.FName} {newParent.LName}",
                    confirmationLink
                );

                if (!emailSent)
                {
                    _logger.LogWarning($"Failed to send confirmation email to {newParent.Email}");
                    // Still return success since account was created
                    return (true, "Account created, but confirmation email failed to send. Please contact support.", newParent.ParentID);
                }

                return (true, "Registration successful! Please check your email to confirm your account.", newParent.ParentID);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during parent registration: {ex.Message}");
                return (false, "An error occurred during registration. Please try again later.", null);
            }
        }

        /// <summary>
        /// Authenticates a parent and generates JWT tokens.
        /// Steps:
        /// 1. Find parent by email
        /// 2. Verify password using BCrypt
        /// 3. Check if email is confirmed
        /// 4. Generate access token and refresh token
        /// 5. Store refresh token in database
        /// 6. Return tokens and parent info
        /// </summary>
        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            try
            {
                // STEP 1: Find parent by email (case-insensitive)
                var parent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.Email.ToLower() == loginDto.Email.ToLower().Trim());

                if (parent == null)
                {
                    _logger.LogWarning($"Login attempt with non-existent email: {loginDto.Email}");
                    return null; // Don't reveal if email exists or not (security)
                }

                // STEP 2: Verify password using BCrypt
                // BCrypt.Verify compares plain text password with hashed password
                bool passwordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, parent.HashedPassword);

                if (!passwordValid)
                {
                    _logger.LogWarning($"Login attempt with invalid password for: {loginDto.Email}");
                    return null;
                }

                // STEP 3: Check if email is confirmed
                if (!parent.IsEmailConfirmed)
                {
                    _logger.LogWarning($"Login attempt with unconfirmed email: {loginDto.Email}");
                    return null; // Return null - controller will send specific message
                }

                // STEP 4: Generate tokens
                string accessToken = _jwtService.GenerateAccessToken(parent);
                string refreshToken = _jwtService.GenerateRefreshToken();

                // Calculate expiration times
                int accessTokenExpirationMinutes = int.Parse(
                    _configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15");
                int refreshTokenExpirationDays = int.Parse(
                    _configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");

                DateTime accessTokenExpiry = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes);
                DateTime refreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);

                // STEP 5: Store refresh token in database (hashed for security)
                parent.RefreshToken = BCrypt.Net.BCrypt.HashPassword(refreshToken);
                parent.RefreshTokenExpiry = refreshTokenExpiry;

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Parent logged in successfully: {parent.Email}");

                // STEP 6: Return authentication response
                return new AuthResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken, // Send plain text refresh token to client
                    AccessTokenExpiration = accessTokenExpiry,
                    RefreshTokenExpiration = refreshTokenExpiry,
                    ParentId = parent.ParentID,
                    Email = parent.Email,
                    FullName = $"{parent.FName} {parent.LName}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during parent login: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Confirms a parent's email address using the token from the confirmation link.
        /// Steps:
        /// 1. Find parent by email
        /// 2. Verify token matches and hasn't expired
        /// 3. Mark email as confirmed
        /// 4. Clear confirmation token
        /// </summary>
        public async Task<(bool success, string message)> ConfirmEmailAsync(string email, string token)
        {
            try
            {
                // STEP 1: Find parent by email
                var parent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower().Trim());

                if (parent == null)
                {
                    _logger.LogWarning($"Email confirmation attempt with non-existent email: {email}");
                    return (false, "Invalid confirmation link");
                }

                // Check if already confirmed
                if (parent.IsEmailConfirmed)
                {
                    _logger.LogInformation($"Email already confirmed: {email}");
                    return (true, "Email is already confirmed. You can now log in.");
                }

                // STEP 2: Verify token
                if (parent.EmailConfirmationToken != token)
                {
                    _logger.LogWarning($"Email confirmation attempt with invalid token for: {email}");
                    return (false, "Invalid confirmation link");
                }

                // Check if token has expired
                if (parent.EmailConfirmationTokenExpiry == null ||
                    parent.EmailConfirmationTokenExpiry < DateTime.UtcNow)
                {
                    _logger.LogWarning($"Email confirmation attempt with expired token for: {email}");
                    return (false, "Confirmation link has expired. Please request a new one.");
                }

                // STEP 3: Mark email as confirmed
                parent.IsEmailConfirmed = true;
                parent.EmailConfirmationToken = null; // Clear token (can't be reused)
                parent.EmailConfirmationTokenExpiry = null;

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Email confirmed successfully: {email}");

                return (true, "Email confirmed successfully! You can now log in.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during email confirmation: {ex.Message}");
                return (false, "An error occurred during email confirmation. Please try again later.");
            }
        }

        /// <summary>
        /// Refreshes JWT tokens using a valid refresh token.
        /// Steps:
        /// 1. Extract parent ID from expired access token
        /// 2. Find parent in database
        /// 3. Verify refresh token matches database and hasn't expired
        /// 4. Generate new access token and refresh token
        /// 5. Store new refresh token in database
        /// </summary>
        public async Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            try
            {
                // STEP 1: Extract claims from expired access token
                var principal = _jwtService.GetPrincipalFromToken(refreshTokenDto.AccessToken);

                if (principal == null)
                {
                    _logger.LogWarning("Refresh token attempt with invalid access token");
                    return null;
                }

                // Get parent ID from token claims
                var parentIdClaim = principal.FindFirst("ParentId")?.Value;
                if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
                {
                    _logger.LogWarning("Refresh token attempt with missing ParentId claim");
                    return null;
                }

                // STEP 2: Find parent in database
                var parent = await _context.Parents.FindAsync(parentId);

                if (parent == null)
                {
                    _logger.LogWarning($"Refresh token attempt for non-existent parent ID: {parentId}");
                    return null;
                }

                // STEP 3: Verify refresh token
                if (string.IsNullOrEmpty(parent.RefreshToken))
                {
                    _logger.LogWarning($"Refresh token attempt but parent has no stored token: {parent.Email}");
                    return null;
                }

                // Verify refresh token matches database (compare hashed)
                bool refreshTokenValid = BCrypt.Net.BCrypt.Verify(
                    refreshTokenDto.RefreshToken,
                    parent.RefreshToken);

                if (!refreshTokenValid)
                {
                    _logger.LogWarning($"Refresh token attempt with invalid token for: {parent.Email}");
                    return null;
                }

                // Check if refresh token has expired
                if (parent.RefreshTokenExpiry == null || parent.RefreshTokenExpiry < DateTime.UtcNow)
                {
                    _logger.LogWarning($"Refresh token attempt with expired token for: {parent.Email}");
                    return null;
                }

                // STEP 4: Generate new tokens
                string newAccessToken = _jwtService.GenerateAccessToken(parent);
                string newRefreshToken = _jwtService.GenerateRefreshToken();

                // Calculate new expiration times
                int accessTokenExpirationMinutes = int.Parse(
                    _configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15");
                int refreshTokenExpirationDays = int.Parse(
                    _configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");

                DateTime newAccessTokenExpiry = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes);
                DateTime newRefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);

                // STEP 5: Update refresh token in database
                parent.RefreshToken = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);
                parent.RefreshTokenExpiry = newRefreshTokenExpiry;

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Tokens refreshed successfully for: {parent.Email}");

                return new AuthResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    AccessTokenExpiration = newAccessTokenExpiry,
                    RefreshTokenExpiration = newRefreshTokenExpiry,
                    ParentId = parent.ParentID,
                    Email = parent.Email,
                    FullName = $"{parent.FName} {parent.LName}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during token refresh: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Revokes a parent's refresh token (logout).
        /// Clears the stored refresh token so it can't be used again.
        /// </summary>
        public async Task<bool> RevokeRefreshTokenAsync(int parentId)
        {
            try
            {
                var parent = await _context.Parents.FindAsync(parentId);

                if (parent == null)
                {
                    _logger.LogWarning($"Revoke token attempt for non-existent parent ID: {parentId}");
                    return false;
                }

                // Clear refresh token
                parent.RefreshToken = null;
                parent.RefreshTokenExpiry = null;

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Refresh token revoked for parent: {parent.Email}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error revoking refresh token: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if an email address is already registered.
        /// Used to prevent duplicate registrations.
        /// </summary>
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Parents
                .AnyAsync(p => p.Email.ToLower() == email.ToLower().Trim());
        }

        /// <summary>
        /// Initiates password reset flow.
        /// Steps:
        /// 1. Find parent by email (if doesn't exist, still return success for security)
        /// 2. Generate password reset token
        /// 3. Store token with 1-hour expiration
        /// 4. Send password reset email
        /// </summary>
        public async Task<(bool success, string message)> ForgotPasswordAsync(string email)
        {
            try
            {
                // STEP 1: Find parent by email
                var parent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower().Trim());

                // For security, always return success even if email doesn't exist
                // (prevents email enumeration attacks)
                if (parent == null)
                {
                    _logger.LogWarning($"Password reset requested for non-existent email: {email}");
                    return (true, "If that email address is registered, you will receive a password reset link shortly.");
                }

                // STEP 2: Generate reset token
                string resetToken = Guid.NewGuid().ToString();
                DateTime tokenExpiry = DateTime.UtcNow.AddHours(1); // 1 hour expiration

                // STEP 3: Store token in database
                parent.PasswordResetToken = resetToken;
                parent.PasswordResetTokenExpiry = tokenExpiry;

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Password reset token generated for: {parent.Email}");

                // STEP 4: Send reset email
                string frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:19006";
                string resetLink = $"{frontendUrl}/auth/reset-password?email={parent.Email}&token={resetToken}";

                bool emailSent = await _emailService.SendPasswordResetEmailAsync(
                    parent.Email,
                    $"{parent.FName} {parent.LName}",
                    resetLink
                );

                if (!emailSent)
                {
                    _logger.LogWarning($"Failed to send password reset email to {parent.Email}");
                }

                return (true, "If that email address is registered, you will receive a password reset link shortly.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during forgot password: {ex.Message}");
                return (false, "An error occurred while processing your request. Please try again later.");
            }
        }

        /// <summary>
        /// Resets a parent's password using reset token.
        /// Steps:
        /// 1. Find parent by email
        /// 2. Verify token matches and hasn't expired
        /// 3. Hash new password
        /// 4. Update password and clear reset token
        /// 5. Optionally revoke refresh tokens (force re-login on all devices)
        /// </summary>
        public async Task<(bool success, string message)> ResetPasswordAsync(string email, string token, string newPassword)
        {
            try
            {
                // STEP 1: Find parent by email
                var parent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower().Trim());

                if (parent == null)
                {
                    _logger.LogWarning($"Password reset attempt with non-existent email: {email}");
                    return (false, "Invalid password reset link");
                }

                // STEP 2: Verify token
                if (parent.PasswordResetToken != token)
                {
                    _logger.LogWarning($"Password reset attempt with invalid token for: {email}");
                    return (false, "Invalid password reset link");
                }

                // Check if token has expired
                if (parent.PasswordResetTokenExpiry == null ||
                    parent.PasswordResetTokenExpiry < DateTime.UtcNow)
                {
                    _logger.LogWarning($"Password reset attempt with expired token for: {email}");
                    return (false, "Password reset link has expired. Please request a new one.");
                }

                // STEP 3: Hash new password
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                // STEP 4: Update password and clear reset token
                parent.HashedPassword = hashedPassword;
                parent.PasswordResetToken = null;
                parent.PasswordResetTokenExpiry = null;

                // STEP 5: Revoke refresh tokens (force re-login on all devices for security)
                parent.RefreshToken = null;
                parent.RefreshTokenExpiry = null;

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Password reset successfully for: {email}");

                return (true, "Password has been reset successfully. Please log in with your new password.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during password reset: {ex.Message}");
                return (false, "An error occurred while resetting your password. Please try again later.");
            }
        }

        /// <summary>
        /// Resends email confirmation link to a parent.
        /// Steps:
        /// 1. Find parent by email
        /// 2. Check if already confirmed
        /// 3. Generate new confirmation token
        /// 4. Send new confirmation email
        /// </summary>
        public async Task<(bool success, string message)> ResendConfirmationEmailAsync(string email)
        {
            try
            {
                // STEP 1: Find parent by email
                var parent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower().Trim());

                if (parent == null)
                {
                    _logger.LogWarning($"Resend confirmation requested for non-existent email: {email}");
                    // For security, don't reveal if email exists or not
                    return (true, "If that email address is registered and not yet confirmed, you will receive a confirmation link shortly.");
                }

                // STEP 2: Check if already confirmed
                if (parent.IsEmailConfirmed)
                {
                    _logger.LogInformation($"Resend confirmation requested for already confirmed email: {email}");
                    return (false, "This email address is already confirmed. You can log in now.");
                }

                // STEP 3: Generate new confirmation token
                string confirmationToken = Guid.NewGuid().ToString();
                int tokenExpirationHours = int.Parse(
                    _configuration["EmailConfirmation:TokenExpirationHours"] ?? "24");
                DateTime tokenExpiry = DateTime.UtcNow.AddHours(tokenExpirationHours);

                parent.EmailConfirmationToken = confirmationToken;
                parent.EmailConfirmationTokenExpiry = tokenExpiry;

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"New confirmation token generated for: {email}");

                // STEP 4: Send confirmation email
                string frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:19006";
                string confirmationLink = $"{frontendUrl}/auth/confirm-email?email={parent.Email}&token={confirmationToken}";

                bool emailSent = await _emailService.SendEmailConfirmationAsync(
                    parent.Email,
                    $"{parent.FName} {parent.LName}",
                    confirmationLink
                );

                if (!emailSent)
                {
                    _logger.LogWarning($"Failed to resend confirmation email to {parent.Email}");
                    return (false, "Failed to send confirmation email. Please try again later.");
                }

                return (true, "A new confirmation link has been sent to your email address.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during resend confirmation: {ex.Message}");
                return (false, "An error occurred while sending confirmation email. Please try again later.");
            }
        }
    }
}