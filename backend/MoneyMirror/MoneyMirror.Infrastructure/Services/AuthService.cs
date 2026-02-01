using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Auth;
using MoneyMirror.Core.Enums;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Core.Models;
using MoneyMirror.Infrastructure.Data;

namespace MoneyMirror.Infrastructure.Services
{
    /// Service implementing authentication and authorization logic.
    /// Handles parent registration, login, email confirmation, and token refresh.
    /// Uses BCrypt for password hashing and JWT for token-based authentication.
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        /// Constructor - dependency injection provides all required services.
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

        /// Registers a new parent account.
        /// Steps:
        /// 1. Check if email already exists
        /// 2. Hash the password using BCrypt
        /// 3. Create parent record in database
        /// 4. Generate email confirmation token
        /// 5. Send confirmation email
        public async Task<(bool success, string message, int? parentId)> RegisterParentAsync(RegisterParentDto registerDto)
        {
            try
            {
                // Replace the email exists check (around line 40) with this enhanced version:

                // STEP 1: Check if email already exists
                var existingParent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.Email.ToLower() == registerDto.Email.ToLower().Trim());

                if (existingParent != null)
                {
                    // Account exists - check deletion status
                    if (!existingParent.IsDeleted)
                    {
                        _logger.LogWarning($"Registration attempt with existing email: {registerDto.Email}");
                        return (false, "Email is already registered", null);
                    }
                    else if (existingParent.IsPermanentlyDeleted)
                    {
                        _logger.LogWarning($"Registration attempt with permanently deleted email: {registerDto.Email}");
                        return (false, "This email address cannot be used. Please use a different email or contact support.", null);
                    }
                    else if (existingParent.PermanentDeletionDate != null && existingParent.PermanentDeletionDate > DateTime.UtcNow)
                    {
                        // Account is soft-deleted and within grace period
                        _logger.LogWarning($"Registration attempt with soft-deleted email: {registerDto.Email}");
                        return (false,
                                $"This email is associated with a deleted account scheduled for permanent deletion on " +
                                $"{existingParent.PermanentDeletionDate:yyyy-MM-dd}. " +
                                "If this is your account, you can recover it from the login page.",
                                null);
                    }
                    else
                    {
                        // Soft-deleted but grace period expired (should be cleaned up by background job)
                        _logger.LogWarning($"Registration attempt with expired soft-deleted email: {registerDto.Email}");
                        return (false, "This email address cannot be used. Please contact support.", null);
                    }
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

        /// Authenticates a parent and generates JWT tokens.
        /// Steps:
        /// 1. Find parent by email
        /// 2. Verify password using BCrypt
        /// 3. Check if email is confirmed
        /// 4. Generate access token and refresh token
        /// 5. Store refresh token in database
        /// 6. Return tokens and parent info
        public async Task<(AuthResponseDto? auth, LoginFailureReason? failure)> LoginAsync(LoginDto loginDto)
        {
            try
            {
                // STEP 1: Find parent by email (case-insensitive)
                var parent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.Email.ToLower() == loginDto.Email.ToLower().Trim());

                if (parent == null)
                {
                    _logger.LogWarning($"Login attempt with non-existent email: {loginDto.Email}");
                    return (null, LoginFailureReason.InvalidCredentials);
                }

                // STEP 2: Check deletion status
                if (parent.IsDeleted)
                {
                    // Soft-deleted and within grace period → recoverable
                    if (!parent.IsPermanentlyDeleted &&
                        parent.PermanentDeletionDate != null &&
                        parent.PermanentDeletionDate > DateTime.UtcNow)
                    {
                        _logger.LogWarning($"Login attempt on soft-deleted account within grace period: {loginDto.Email}");
                        return (null, LoginFailureReason.SoftDeletedRecoverable);
                    }

                    // Permanently deleted or grace period expired
                    _logger.LogWarning($"Login attempt on permanently deleted or expired account: {loginDto.Email}");
                    return (null, LoginFailureReason.PermanentlyDeleted);
                }

                // STEP 3: Verify password using BCrypt
                bool passwordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, parent.HashedPassword);

                if (!passwordValid)
                {
                    _logger.LogWarning($"Login attempt with invalid password for: {loginDto.Email}");
                    return (null, LoginFailureReason.InvalidCredentials);
                }

                // STEP 4: Check if email is confirmed
                if (!parent.IsEmailConfirmed)
                {
                    _logger.LogWarning($"Login attempt with unconfirmed email: {loginDto.Email}");
                    return (null, LoginFailureReason.EmailNotConfirmed);
                }

                // STEP 5: Generate tokens
                string accessToken = _jwtService.GenerateAccessToken(parent);
                string refreshToken = _jwtService.GenerateRefreshToken();

                int accessTokenExpirationMinutes = int.Parse(
                    _configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15");
                int refreshTokenExpirationDays = int.Parse(
                    _configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");

                DateTime accessTokenExpiry = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes);
                DateTime refreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);

                // STEP 6: Store refresh token in database (hashed)
                parent.RefreshToken = BCrypt.Net.BCrypt.HashPassword(refreshToken);
                parent.RefreshTokenExpiry = refreshTokenExpiry;

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Parent logged in successfully: {parent.Email}");

                // STEP 7: Return authentication response
                return (
                    new AuthResponseDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        AccessTokenExpiration = accessTokenExpiry,
                        RefreshTokenExpiration = refreshTokenExpiry,
                        ParentId = parent.ParentID,
                        Email = parent.Email,
                        FullName = $"{parent.FName} {parent.LName}"
                    },
                    null
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during parent login: {ex.Message}");
                return (null, LoginFailureReason.InvalidCredentials);
            }
        }


        /// Confirms a parent's email address using the token from the confirmation link.
        /// Steps:
        /// 1. Find parent by email
        /// 2. Verify token matches and hasn't expired
        /// 3. Mark email as confirmed
        /// 4. Clear confirmation token
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

        /// Refreshes JWT tokens using a valid refresh token.
        /// Steps:
        /// 1. Extract parent ID from expired access token
        /// 2. Find parent in database
        /// 3. Verify refresh token matches database and hasn't expired
        /// 4. Generate new access token and refresh token
        /// 5. Store new refresh token in database
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

        /// Revokes a parent's refresh token (logout).
        /// Clears the stored refresh token so it can't be used again.
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

        /// Checks if an email address is already registered.
        /// Used to prevent duplicate registrations.
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Parents
                .AnyAsync(p => p.Email.ToLower() == email.ToLower().Trim());
        }

        /// Initiates password reset flow.
        /// Steps:
        /// 1. Find parent by email (if doesn't exist, still return success for security)
        /// 2. Generate password reset token
        /// 3. Store token with 1-hour expiration
        /// 4. Send password reset email
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

        /// Resets a parent's password using reset token.
        /// Steps:
        /// 1. Find parent by email
        /// 2. Verify token matches and hasn't expired
        /// 3. Hash new password
        /// 4. Update password and clear reset token
        /// 5. Optionally revoke refresh tokens (force re-login on all devices)
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

        /// Resends email confirmation link to a parent.
        /// Steps:
        /// 1. Find parent by email
        /// 2. Check if already confirmed
        /// 3. Generate new confirmation token
        /// 4. Send new confirmation email
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
        // ==================== ADD THESE METHODS TO AuthService.cs ====================
        // Place them at the end of the class, before the closing brace

        /// Updates parent profile information (name, phone).
        /// Steps:
        /// 1. Find parent by ID
        /// 2. Verify account is not deleted
        /// 3. Update fields
        /// 4. Save changes
        public async Task<(bool success, string message)> UpdateParentProfileAsync(int parentId, UpdateParentProfileDto updateDto)
        {
            try
            {
                // STEP 1: Find parent
                var parent = await _context.Parents.FindAsync(parentId);

                if (parent == null)
                {
                    _logger.LogWarning($"Profile update attempt for non-existent parent ID: {parentId}");
                    return (false, "Parent account not found");
                }

                // STEP 2: Check if account is deleted
                if (parent.IsDeleted)
                {
                    _logger.LogWarning($"Profile update attempt for deleted parent: {parent.Email}");
                    return (false, "Account has been deleted");
                }

                // STEP 3: Update fields
                parent.FName = updateDto.FirstName.Trim();
                parent.LName = updateDto.LastName.Trim();
                parent.PhoneNum = string.IsNullOrWhiteSpace(updateDto.PhoneNumber)
                    ? null
                    : updateDto.PhoneNumber.Trim();

                // STEP 4: Save changes
                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Profile updated successfully for parent: {parent.Email}");

                return (true, "Profile updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating parent profile: {ex.Message}");
                return (false, "An error occurred while updating your profile. Please try again later.");
            }
        }

        /// Initiates email change process.
        /// Steps:
        /// 1. Find parent by ID
        /// 2. Verify current password
        /// 3. Check if new email is already in use
        /// 4. Generate email change token
        /// 5. Send verification email to NEW email address
        public async Task<(bool success, string message)> ChangeEmailAsync(int parentId, ChangeEmailDto changeEmailDto)
        {
            try
            {
                // STEP 1: Find parent
                var parent = await _context.Parents.FindAsync(parentId);

                if (parent == null)
                {
                    _logger.LogWarning($"Email change attempt for non-existent parent ID: {parentId}");
                    return (false, "Parent account not found");
                }

                // Check if account is deleted
                if (parent.IsDeleted)
                {
                    _logger.LogWarning($"Email change attempt for deleted parent: {parent.Email}");
                    return (false, "Account has been deleted");
                }

                // STEP 2: Verify current password
                bool passwordValid = BCrypt.Net.BCrypt.Verify(changeEmailDto.CurrentPassword, parent.HashedPassword);

                if (!passwordValid)
                {
                    _logger.LogWarning($"Email change attempt with invalid password for: {parent.Email}");
                    return (false, "Current password is incorrect");
                }

                // STEP 3: Check if new email is already in use
                string normalizedNewEmail = changeEmailDto.NewEmail.ToLower().Trim();

                bool emailExists = await _context.Parents
                    .AnyAsync(p => p.Email.ToLower() == normalizedNewEmail && p.ParentID != parentId);

                if (emailExists)
                {
                    _logger.LogWarning($"Email change attempt to already registered email: {normalizedNewEmail}");
                    return (false, "This email address is already registered");
                }

                // STEP 4: Generate email change token
                string emailChangeToken = Guid.NewGuid().ToString();
                int tokenExpirationHours = int.Parse(
                    _configuration["EmailConfirmation:TokenExpirationHours"] ?? "24");
                DateTime tokenExpiry = DateTime.UtcNow.AddHours(tokenExpirationHours);

                // Store new email and token (don't update actual email yet!)
                parent.NewEmail = normalizedNewEmail;
                parent.EmailChangeToken = emailChangeToken;
                parent.EmailChangeTokenExpiry = tokenExpiry;

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Email change initiated for parent: {parent.Email} to {normalizedNewEmail}");

                // STEP 5: Send verification email to NEW email address
                string frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:19006";
                string confirmationLink = $"{frontendUrl}/auth/confirm-email-change?oldEmail={parent.Email}&newEmail={normalizedNewEmail}&token={emailChangeToken}";

                // Use existing email confirmation template but customize message
                bool emailSent = await _emailService.SendEmailConfirmationAsync(
                    normalizedNewEmail,
                    $"{parent.FName} {parent.LName}",
                    confirmationLink
                );

                if (!emailSent)
                {
                    _logger.LogWarning($"Failed to send email change confirmation to {normalizedNewEmail}");
                    return (false, "Failed to send confirmation email. Please try again later.");
                }

                return (true, "A confirmation link has been sent to your new email address. Please verify it to complete the change.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during email change: {ex.Message}");
                return (false, "An error occurred while changing your email. Please try again later.");
            }
        }

        /// Confirms email change using token.
        /// Steps:
        /// 1. Find parent by old email
        /// 2. Verify token and new email match
        /// 3. Update email to new address
        /// 4. Clear email change tokens
        /// 5. Revoke all refresh tokens (force re-login with new email)
        public async Task<(bool success, string message)> ConfirmEmailChangeAsync(ConfirmEmailChangeDto confirmDto)
        {
            try
            {
                // STEP 1: Find parent by old email
                var parent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.Email.ToLower() == confirmDto.OldEmail.ToLower().Trim());

                if (parent == null)
                {
                    _logger.LogWarning($"Email change confirmation attempt with non-existent old email: {confirmDto.OldEmail}");
                    return (false, "Invalid email change link");
                }

                // STEP 2: Verify token and new email
                if (parent.EmailChangeToken != confirmDto.Token)
                {
                    _logger.LogWarning($"Email change confirmation with invalid token for: {parent.Email}");
                    return (false, "Invalid email change link");
                }

                if (parent.NewEmail?.ToLower() != confirmDto.NewEmail.ToLower().Trim())
                {
                    _logger.LogWarning($"Email change confirmation with mismatched new email for: {parent.Email}");
                    return (false, "Invalid email change link");
                }

                // Check if token has expired
                if (parent.EmailChangeTokenExpiry == null ||
                    parent.EmailChangeTokenExpiry < DateTime.UtcNow)
                {
                    _logger.LogWarning($"Email change confirmation with expired token for: {parent.Email}");
                    return (false, "Email change link has expired. Please request a new one.");
                }

                // Check if new email was taken while waiting for confirmation
                string normalizedNewEmail = confirmDto.NewEmail.ToLower().Trim();
                bool emailNowTaken = await _context.Parents
                    .AnyAsync(p => p.Email.ToLower() == normalizedNewEmail && p.ParentID != parent.ParentID);

                if (emailNowTaken)
                {
                    _logger.LogWarning($"Email change confirmation but email now taken: {normalizedNewEmail}");
                    return (false, "This email address has been registered by another user. Please choose a different email.");
                }

                // STEP 3: Update email
                string oldEmail = parent.Email;
                parent.Email = normalizedNewEmail;
                parent.IsEmailConfirmed = true; // New email is now confirmed

                // STEP 4: Clear email change tokens
                parent.NewEmail = null;
                parent.EmailChangeToken = null;
                parent.EmailChangeTokenExpiry = null;

                // STEP 5: Revoke all refresh tokens (force re-login with new email)
                parent.RefreshToken = null;
                parent.RefreshTokenExpiry = null;

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Email changed successfully from {oldEmail} to {normalizedNewEmail}");

                return (true, "Email changed successfully! Please log in again with your new email address.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error confirming email change: {ex.Message}");
                return (false, "An error occurred while confirming email change. Please try again later.");
            }
        }

        /// Soft deletes a parent account.
        /// Steps:
        /// 1. Find parent by ID
        /// 2. Verify password
        /// 3. Mark account as deleted
        /// 4. Remove all ParentChild relationships
        /// 5. Revoke refresh tokens
        /// Children are preserved - only relationships are removed
        /// <summary>
        /// Soft deletes a parent account with 30-day recovery grace period.
        /// Steps:
        /// 1. Find parent by ID
        /// 2. Verify password
        /// 3. Mark account as soft-deleted
        /// 4. Set 30-day grace period
        /// 5. Remove all ParentChild relationships (children preserved)
        /// 6. Revoke refresh tokens
        /// </summary>
        public async Task<(bool success, string message)> DeleteParentAccountAsync(int parentId, string currentPassword)
        {
            try
            {
                // STEP 1: Find parent
                var parent = await _context.Parents
                    .Include(p => p.ParentChildren) // Load relationships
                    .FirstOrDefaultAsync(p => p.ParentID == parentId);

                if (parent == null)
                {
                    _logger.LogWarning($"Delete attempt for non-existent parent ID: {parentId}");
                    return (false, "Parent account not found");
                }

                // Check if already deleted
                if (parent.IsDeleted && !parent.IsPermanentlyDeleted)
                {
                    _logger.LogInformation($"Delete attempt for already deleted parent: {parent.Email}");
                    return (false, $"Account already scheduled for deletion on {parent.PermanentDeletionDate:yyyy-MM-dd}. Contact support to recover.");
                }

                if (parent.IsPermanentlyDeleted)
                {
                    _logger.LogInformation($"Delete attempt for permanently deleted parent ID: {parentId}");
                    return (false, "Account has been permanently deleted and cannot be modified.");
                }

                // STEP 2: Verify password
                bool passwordValid = BCrypt.Net.BCrypt.Verify(currentPassword, parent.HashedPassword);

                if (!passwordValid)
                {
                    _logger.LogWarning($"Delete attempt with invalid password for: {parent.Email}");
                    return (false, "Password is incorrect");
                }

                // STEP 3: Mark as soft-deleted with grace period
                parent.IsDeleted = true;
                parent.DeletedAt = DateTime.UtcNow;
                parent.IsPermanentlyDeleted = false;
                parent.PermanentDeletionDate = DateTime.UtcNow.AddDays(30); // 30-day grace period

                // STEP 4: Remove all ParentChild relationships
                // Children are NOT deleted - only the relationships are removed
                _context.ParentChildren.RemoveRange(parent.ParentChildren);

                // STEP 5: Revoke refresh tokens (prevent login)
                parent.RefreshToken = null;
                parent.RefreshTokenExpiry = null;

                // Save changes
                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Parent account soft deleted: {parent.Email} (ID: {parentId}). " +
                                      $"Permanent deletion scheduled for: {parent.PermanentDeletionDate:yyyy-MM-dd HH:mm}");

                return (true, $"Account scheduled for deletion. You have until {parent.PermanentDeletionDate:yyyy-MM-dd} to recover your account. " +
                             "Your children's learning data has been preserved.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting parent account: {ex.Message}");
                return (false, "An error occurred while deleting your account. Please try again later.");
            }
        }
        /// <summary>
        /// Recovers a soft-deleted parent account within grace period.
        /// Steps:
        /// 1. Find parent by email
        /// 2. Verify password
        /// 3. Check if within grace period
        /// 4. Restore account status
        /// 5. DO NOT restore ParentChild relationships (parents must re-link children)
        /// </summary>
        public async Task<(bool success, string message)> RecoverDeletedAccountAsync(string email, string password)
        {
            try
            {
                // STEP 1: Find parent by email
                var parent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower().Trim());

                if (parent == null)
                {
                    _logger.LogWarning($"Recovery attempt for non-existent email: {email}");
                    return (false, "Account not found");
                }

                // STEP 2: Check deletion status
                if (!parent.IsDeleted)
                {
                    _logger.LogInformation($"Recovery attempt for active account: {email}");
                    return (false, "Account is not deleted");
                }

                if (parent.IsPermanentlyDeleted)
                {
                    _logger.LogWarning($"Recovery attempt for permanently deleted account: {email}");
                    return (false, "Account has been permanently deleted and cannot be recovered");
                }

                // STEP 3: Check if within grace period
                if (parent.PermanentDeletionDate == null || parent.PermanentDeletionDate <= DateTime.UtcNow)
                {
                    _logger.LogWarning($"Recovery attempt after grace period for: {email}");
                    return (false, "Recovery period has expired. Account cannot be recovered");
                }

                // STEP 4: Verify password
                bool passwordValid = BCrypt.Net.BCrypt.Verify(password, parent.HashedPassword);

                if (!passwordValid)
                {
                    _logger.LogWarning($"Recovery attempt with invalid password for: {email}");
                    return (false, "Password is incorrect");
                }

                // STEP 5: Restore account
                parent.IsDeleted = false;
                parent.DeletedAt = null;
                parent.IsPermanentlyDeleted = false;
                parent.PermanentDeletionDate = null;

                // NOTE: ParentChild relationships are NOT automatically restored
                // Parent must re-link their children through the app

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Parent account recovered successfully: {email}");

                return (true, "Account recovered successfully! Please log in and re-link your children.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error recovering account: {ex.Message}");
                return (false, "An error occurred while recovering your account. Please try again later.");
            }
        }
        /// <summary>
        /// Permanently deletes accounts past grace period by anonymizing PII.
        /// This should be called by a scheduled background service.
        /// Steps:
        /// 1. Find all accounts where PermanentDeletionDate has passed
        /// 2. Anonymize personal information
        /// 3. Mark as permanently deleted
        /// 4. Keep database record for audit trail
        /// </summary>
        public async Task<int> PermanentlyDeleteExpiredAccountsAsync()
        {
            try
            {
                // STEP 1: Find expired accounts
                var expiredAccounts = await _context.Parents
                    .Where(p => p.IsDeleted
                             && !p.IsPermanentlyDeleted
                             && p.PermanentDeletionDate != null
                             && p.PermanentDeletionDate <= DateTime.UtcNow)
                    .ToListAsync();

                if (!expiredAccounts.Any())
                {
                    _logger.LogInformation("No expired accounts to permanently delete");
                    return 0;
                }

                // STEP 2: Anonymize each account
                foreach (var parent in expiredAccounts)
                {
                    // Anonymize PII
                    parent.Email = $"deleted_{parent.ParentID}@deleted.local";
                    parent.FName = "Deleted";
                    parent.LName = "User";
                    parent.PhoneNum = null;
                    parent.HashedPassword = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()); // Random unusable password

                    // Clear all tokens
                    parent.RefreshToken = null;
                    parent.RefreshTokenExpiry = null;
                    parent.EmailConfirmationToken = null;
                    parent.EmailConfirmationTokenExpiry = null;
                    parent.PasswordResetToken = null;
                    parent.PasswordResetTokenExpiry = null;
                    parent.EmailChangeToken = null;
                    parent.EmailChangeTokenExpiry = null;
                    parent.NewEmail = null;

                    // Mark as permanently deleted
                    parent.IsPermanentlyDeleted = true;
                    parent.PermanentDeletionDate = null; // Clear since deletion is now complete

                    _logger.LogInformation($"Permanently deleted parent account ID: {parent.ParentID}");
                }

                // STEP 3: Save changes
                _context.Parents.UpdateRange(expiredAccounts);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Permanently deleted {expiredAccounts.Count} expired accounts");

                return expiredAccounts.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during permanent deletion of expired accounts: {ex.Message}");
                return 0;
            }
        }
    }
}