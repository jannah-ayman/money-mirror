using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Auth;
using MoneyMirror.Core.DTOs.Parent;
using MoneyMirror.Core.Enums;
using MoneyMirror.Core.Helpers;
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
        public async Task<(bool success, string message, int? parentId)> RegisterParentAsync(RegisterParentDto registerDto)
        {
            try
            {
                // STEP 1: Check if email already exists
                var existingParent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.Email.ToLower() == registerDto.Email.ToLower().Trim());

                if (existingParent != null)
                {
                    // Same deletion checks as before...
                    if (!existingParent.IsDeleted)
                    {
                        _logger.LogWarning($"Registration attempt with existing email: {registerDto.Email}");
                        return (false, "Email is already registered", null);
                    }
                    // ... other deletion status checks
                }

                // STEP 2: Hash password
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                // STEP 3: Generate 6-digit confirmation CODE (instead of GUID token)
                string confirmationCode = CodeGenerator.Generate6DigitCode();
                DateTime codeExpiry = CodeGenerator.GetCodeExpiration(15); // 15 minutes

                // STEP 4: Create parent entity
                var newParent = new Parent
                {
                    Email = registerDto.Email.ToLower().Trim(),
                    HashedPassword = hashedPassword,
                    FName = registerDto.FirstName.Trim(),
                    LName = registerDto.LastName.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    IsEmailConfirmed = false,

                    // ✅ NEW: Store code and expiry (instead of token)
                    EmailConfirmationCode = confirmationCode,
                    EmailConfirmationCodeExpiry = codeExpiry
                };

                // STEP 5: Save to database
                await _context.Parents.AddAsync(newParent);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    $"Parent registered: {newParent.Email} (ID: {newParent.ParentID}). Code: {confirmationCode}");

                // STEP 6: Send email with CODE (not link)
                bool emailSent = await _emailService.SendEmailConfirmationCodeAsync(
                    newParent.Email,
                    $"{newParent.FName} {newParent.LName}",
                    confirmationCode // ✅ Send the 6-digit code
                );

                if (!emailSent)
                {
                    _logger.LogWarning($"Failed to send confirmation code to {newParent.Email}");
                    return (true,
                        "Account created, but confirmation email failed. " +
                        "Please use 'Resend Code' option.",
                        newParent.ParentID);
                }

                return (true,
                    "Registration successful! Please check your email for a 6-digit code.",
                    newParent.ParentID);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during registration: {ex.Message}");
                return (false, "An error occurred during registration.", null);
            }
        }

        /// <summary>
        /// STEP 2: CONFIRM EMAIL WITH CODE (New method - replaces link-based confirmation)
        /// User enters the 6-digit code they received in email.
        /// After confirmation, automatically log them in.
        /// </summary>
        public async Task<(bool success, string message, AuthResponseDto? authResponse)>
            ConfirmEmailWithCodeAsync(ConfirmEmailWithCodeDto dto)
        {
            try
            {
                // STEP 1: Find parent by email
                var parent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.Email.ToLower() == dto.Email.ToLower().Trim());

                if (parent == null)
                {
                    _logger.LogWarning($"Email confirmation attempt with non-existent email: {dto.Email}");
                    return (false, "Invalid email or code", null);
                }

                // STEP 2: Check if already confirmed
                if (parent.IsEmailConfirmed)
                {
                    _logger.LogInformation($"Email already confirmed: {dto.Email}");
                    return (false, "Email is already confirmed. Please log in.", null);
                }

                // STEP 3: Verify the code
                if (parent.EmailConfirmationCode != dto.Code.Trim())
                {
                    _logger.LogWarning($"Invalid confirmation code for: {dto.Email}");
                    return (false, "Invalid or incorrect code", null);
                }

                // STEP 4: Check if code has expired
                if (CodeGenerator.IsCodeExpired(parent.EmailConfirmationCodeExpiry))
                {
                    _logger.LogWarning($"Expired confirmation code for: {dto.Email}");
                    return (false,
                        "This code has expired. Please request a new one.",
                        null);
                }

                // STEP 5: Mark email as confirmed
                parent.IsEmailConfirmed = true;
                parent.EmailConfirmationCode = null; // Clear the code
                parent.EmailConfirmationCodeExpiry = null;

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Email confirmed successfully: {dto.Email}");

                // STEP 6: Auto-login - generate JWT tokens
                string accessToken = _jwtService.GenerateAccessToken(parent);
                string refreshToken = _jwtService.GenerateRefreshToken();

                int accessTokenExpirationMinutes = int.Parse(
                    _configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15");
                int refreshTokenExpirationDays = int.Parse(
                    _configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");

                DateTime accessTokenExpiry = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes);
                DateTime refreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);

                // Store refresh token
                parent.RefreshToken = BCrypt.Net.BCrypt.HashPassword(refreshToken);
                parent.RefreshTokenExpiry = refreshTokenExpiry;

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                // Build auth response
                var authResponse = new AuthResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessTokenExpiration = accessTokenExpiry,
                    RefreshTokenExpiration = refreshTokenExpiry,
                    ParentId = parent.ParentID,
                    Email = parent.Email,
                    FullName = $"{parent.FName} {parent.LName}"
                };

                return (true,
                    "Email confirmed successfully! Welcome to Money Mirror!",
                    authResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during email confirmation: {ex.Message}");
                return (false, "An error occurred during confirmation.", null);
            }
        }

        /// <summary>
        /// STEP 3: RESEND CONFIRMATION CODE
        /// Generates new code if original expired or wasn't received.
        /// </summary>
        public async Task<(bool success, string message)>
            ResendConfirmationCodeAsync(ResendConfirmationCodeDto dto)
        {
            try
            {
                // STEP 1: Find parent by email
                var parent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.Email.ToLower() == dto.Email.ToLower().Trim());

                // For security, don't reveal if email exists
                if (parent == null)
                {
                    _logger.LogWarning($"Resend code requested for non-existent email: {dto.Email}");
                    return (true,
                        "If that email is registered and not confirmed, " +
                        "you will receive a new code shortly."
        
                        );
                }

                // STEP 2: Check if already confirmed
                if (parent.IsEmailConfirmed)
                {
                    _logger.LogInformation($"Resend code requested for confirmed email: {dto.Email}");
                    return (false,
                        "This email is already confirmed. Please log in.");
                }

                // STEP 3: Generate NEW code
                string newCode = CodeGenerator.Generate6DigitCode();
                DateTime newExpiry = CodeGenerator.GetCodeExpiration(15);

                parent.EmailConfirmationCode = newCode;
                parent.EmailConfirmationCodeExpiry = newExpiry;

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"New confirmation code generated for: {dto.Email}. Code: {newCode}");

                // STEP 4: Send email
                bool emailSent = await _emailService.SendEmailConfirmationCodeAsync(
                    parent.Email,
                    $"{parent.FName} {parent.LName}",
                    newCode
                );

                if (!emailSent)
                {
                    _logger.LogWarning($"Failed to send new code to {parent.Email}");
                    return (false, "Failed to send email. Please try again later.");
                }

                return (true, "A new confirmation code has been sent to your email.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error resending confirmation code: {ex.Message}");
                return (false, "An error occurred. Please try again later.");
            }
        }

        /// Authenticates a parent and generates JWT tokens.
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

        // ==================== PART 2: PASSWORD RESET ====================
        // Add these methods to your AuthService.cs class

        /// STEP 1: REQUEST PASSWORD RESET (Updated to send code instead of link)
        /// Sends 6-digit code to email.
        public async Task<(bool success, string message)> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            try
            {
                // STEP 1: Find parent by email
                var parent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.Email.ToLower() == dto.Email.ToLower().Trim());

                // For security, always return success (prevent email enumeration)
                if (parent == null)
                {
                    _logger.LogWarning($"Password reset requested for non-existent email: {dto.Email}");
                    return (true,
                        "If that email is registered, you will receive a reset code shortly.");
                }

                // STEP 2: Generate 6-digit reset CODE
                string resetCode = CodeGenerator.Generate6DigitCode();
                DateTime codeExpiry = CodeGenerator.GetCodeExpiration(15); // 15 minutes

                // STEP 3: Store code in database
                parent.PasswordResetCode = resetCode;
                parent.PasswordResetCodeExpiry = codeExpiry;

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Password reset code generated for: {parent.Email}. Code: {resetCode}");

                // STEP 4: Send email with CODE
                bool emailSent = await _emailService.SendPasswordResetCodeAsync(
                    parent.Email,
                    $"{parent.FName} {parent.LName}",
                    resetCode
                );

                if (!emailSent)
                {
                    _logger.LogWarning($"Failed to send reset code to {parent.Email}");
                }

                return (true,
                    "If that email is registered, you will receive a reset code shortly.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during forgot password: {ex.Message}");
                return (false, "An error occurred. Please try again later.");
            }
        }

        

        /// <summary>
        /// RESET PASSWORD WITH CODE
        /// Sets new password after verifying the code.
        /// Automatically logs in the user after successful reset.
        /// </summary>
        public async Task<(bool success, string message, AuthResponseDto? authResponse)>
            ResetPasswordWithCodeAsync(ResetPasswordWithCodeDto dto)
        {
            try
            {
                // STEP 1: Find parent
                var parent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.Email.ToLower() == dto.Email.ToLower().Trim());

                if (parent == null)
                {
                    _logger.LogWarning($"Password reset attempt for non-existent email: {dto.Email}");
                    return (false, "Invalid email or code", null);
                }

                // STEP 2: Verify code
                if (parent.PasswordResetCode != dto.Code.Trim())
                {
                    _logger.LogWarning($"Invalid reset code during password reset for: {dto.Email}");
                    return (false, "Invalid or incorrect code", null);
                }

                // STEP 3: Check expiration
                if (CodeGenerator.IsCodeExpired(parent.PasswordResetCodeExpiry))
                {
                    _logger.LogWarning($"Expired reset code during password reset for: {dto.Email}");
                    return (false, "This code has expired. Please request a new one.", null);
                }

                // STEP 4: Hash new password
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

                // STEP 5: Update password and clear reset code
                parent.HashedPassword = hashedPassword;
                parent.PasswordResetCode = null;
                parent.PasswordResetCodeExpiry = null;

                // STEP 6: Revoke all refresh tokens (force re-login on other devices)
                parent.RefreshToken = null;
                parent.RefreshTokenExpiry = null;

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Password reset successfully for: {dto.Email}");

                // STEP 7: Auto-login - generate new tokens
                string accessToken = _jwtService.GenerateAccessToken(parent);
                string refreshToken = _jwtService.GenerateRefreshToken();

                int accessTokenExpirationMinutes = int.Parse(
                    _configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15");
                int refreshTokenExpirationDays = int.Parse(
                    _configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");

                DateTime accessTokenExpiry = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes);
                DateTime refreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);

                // Store new refresh token
                parent.RefreshToken = BCrypt.Net.BCrypt.HashPassword(refreshToken);
                parent.RefreshTokenExpiry = refreshTokenExpiry;

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                // Build auth response
                var authResponse = new AuthResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessTokenExpiration = accessTokenExpiry,
                    RefreshTokenExpiration = refreshTokenExpiry,
                    ParentId = parent.ParentID,
                    Email = parent.Email,
                    FullName = $"{parent.FName} {parent.LName}"
                };

                return (true,
                    "Password has been reset successfully! You are now logged in.",
                    authResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during password reset: {ex.Message}");
                return (false, "An error occurred during password reset.", null);
            }
        }
        // ==================== PART 3: EMAIL CHANGE ====================
        // Add these methods to your AuthService.cs class

        /// <summary>
        /// STEP 1: REQUEST EMAIL CHANGE (Updated to send code to NEW email)
        /// Sends 6-digit code to the new email address.
        /// </summary>
        public async Task<(bool success, string message)>
            RequestEmailChangeAsync(int parentId, RequestEmailChangeDto dto)
        {
            try
            {
                // STEP 1: Find parent
                var parent = await _context.Parents.FindAsync(parentId);

                if (parent == null)
                {
                    _logger.LogWarning($"Email change request for non-existent parent: {parentId}");
                    return (false, "Parent account not found");
                }

                // Check if deleted
                if (parent.IsDeleted)
                {
                    _logger.LogWarning($"Email change request for deleted parent: {parent.Email}");
                    return (false, "Account has been deleted");
                }

                // STEP 2: Verify current password
                bool passwordValid = BCrypt.Net.BCrypt.Verify(
                    dto.CurrentPassword,
                    parent.HashedPassword);

                if (!passwordValid)
                {
                    _logger.LogWarning($"Email change with wrong password for: {parent.Email}");
                    return (false, "Current password is incorrect");
                }

                // STEP 3: Check if new email is already in use
                string normalizedNewEmail = dto.NewEmail.ToLower().Trim();
                if (normalizedNewEmail == parent.Email.ToLower())
                    return (false, "This is already your current email address");

                bool emailExists = await _context.Parents
                    .AnyAsync(p => p.Email.ToLower() == normalizedNewEmail
                                && p.ParentID != parentId);

                if (emailExists)
                {
                    _logger.LogWarning($"Email change to already registered email: {normalizedNewEmail}");
                    return (false, "This email address is already registered");
                }

                // STEP 4: Generate 6-digit code for email change
                string changeCode = CodeGenerator.Generate6DigitCode();
                DateTime codeExpiry = CodeGenerator.GetCodeExpiration(15);

                // STEP 5: Store new email and code (don't update actual email yet!)
                parent.NewEmail = normalizedNewEmail;
                parent.EmailChangeCode = changeCode;
                parent.EmailChangeCodeExpiry = codeExpiry;

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    $"Email change initiated: {parent.Email} -> {normalizedNewEmail}. Code: {changeCode}");

                // STEP 6: Send code to NEW email address
                bool emailSent = await _emailService.SendEmailChangeCodeAsync(
                    normalizedNewEmail, // ✅ Send to NEW email
                    $"{parent.FName} {parent.LName}",
                    changeCode
                );

                if (!emailSent)
                {
                    _logger.LogWarning($"Failed to send email change code to {normalizedNewEmail}");
                    return (false, "Failed to send confirmation email. Please try again.");
                }

                return (true,
                    $"A confirmation code has been sent to {normalizedNewEmail}. " +
                    "Please check that email and enter the code to complete the change.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during email change request: {ex.Message}");
                return (false, "An error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// STEP 2: CONFIRM EMAIL CHANGE WITH CODE
        /// Verifies code and applies the email change.
        /// Revokes all tokens (user must log in with new email).
        /// </summary>
        public async Task<(bool success, string message)>
            ConfirmEmailChangeWithCodeAsync(ConfirmEmailChangeWithCodeDto dto)
        {
            try
            {
                // STEP 1: Find parent by OLD email
                var parent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.Email.ToLower() == dto.OldEmail.ToLower().Trim());

                if (parent == null)
                {
                    _logger.LogWarning($"Email change confirmation for non-existent email: {dto.OldEmail}");
                    return (false, "Invalid email change request");
                }

                // STEP 2: Verify the NEW email matches what's stored
                if (parent.NewEmail?.ToLower() != dto.NewEmail.ToLower().Trim())
                {
                    _logger.LogWarning($"Email change confirmation with mismatched new email for: {parent.Email}");
                    return (false, "Invalid email change request");
                }

                // STEP 3: Verify the code
                if (parent.EmailChangeCode != dto.Code.Trim())
                {
                    _logger.LogWarning($"Invalid email change code for: {parent.Email}");
                    return (false, "Invalid or incorrect code");
                }

                // STEP 4: Check expiration
                if (CodeGenerator.IsCodeExpired(parent.EmailChangeCodeExpiry))
                {
                    _logger.LogWarning($"Expired email change code for: {parent.Email}");
                    return (false, "This code has expired. Please request a new one.");
                }

                // STEP 5: Check if new email was taken while waiting for confirmation
                string normalizedNewEmail = dto.NewEmail.ToLower().Trim();
                bool emailNowTaken = await _context.Parents
                    .AnyAsync(p => p.Email.ToLower() == normalizedNewEmail
                                && p.ParentID != parent.ParentID);

                if (emailNowTaken)
                {
                    _logger.LogWarning($"Email change but new email now taken: {normalizedNewEmail}");
                    return (false,
                        "This email address has been registered by another user. " +
                        "Please choose a different email.");
                }

                // STEP 6: Apply the email change
                string oldEmail = parent.Email;
                parent.Email = normalizedNewEmail;
                parent.IsEmailConfirmed = true; // New email is confirmed

                // STEP 7: Clear email change fields
                parent.NewEmail = null;
                parent.EmailChangeCode = null;
                parent.EmailChangeCodeExpiry = null;

                // STEP 8: Revoke all refresh tokens (force re-login with new email)
                parent.RefreshToken = null;
                parent.RefreshTokenExpiry = null;

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Email changed successfully: {oldEmail} -> {normalizedNewEmail}");

                return (true,
                    "Email changed successfully! Please log in again with your new email address.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error confirming email change: {ex.Message}");
                return (false, "An error occurred during email change confirmation.");
            }
        }

        /// Refreshes JWT tokens using a valid refresh token.
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

        
        /// Resends email confirmation link to a parent.
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

                parent.EmailConfirmationCode = confirmationToken;
                parent.EmailConfirmationCodeExpiry = tokenExpiry;

                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"New confirmation token generated for: {email}");

                // STEP 4: Send confirmation email
                string frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:19006";
                string confirmationLink = $"{frontendUrl}/auth/confirm-email?email={parent.Email}&token={confirmationToken}";

                bool emailSent = await _emailService.SendEmailConfirmationCodeAsync(
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
        /// Updates parent profile information (name, phone).
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

        /// Confirms email change using token.
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
                if (parent.EmailChangeCode != confirmDto.Token)
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
                if (parent.EmailChangeCodeExpiry == null ||
                    parent.EmailChangeCodeExpiry < DateTime.UtcNow)
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
                parent.EmailChangeCode = null;
                parent.EmailChangeCodeExpiry = null;

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

        /// Soft deletes a parent account with 30-day recovery grace period.
        public async Task<(bool success, string message)> DeleteParentAccountAsync(int parentId, string currentPassword)
        {
            try
            {
                // STEP 1: Find parent
                var parent = await _context.Parents
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
        /// Recovers a soft-deleted parent account within grace period.
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

                return (true, "Account recovered successfully! Please log in to view your data.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error recovering account: {ex.Message}");
                return (false, "An error occurred while recovering your account. Please try again later.");
            }
        }
        /// Permanently deletes accounts past grace period by anonymizing PII.
        /// This should be called by a scheduled background service.
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
                    parent.HashedPassword = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()); // Random unusable password

                    // Clear all tokens
                    parent.RefreshToken = null;
                    parent.RefreshTokenExpiry = null;
                    parent.EmailConfirmationCode = null;
                    parent.EmailConfirmationCodeExpiry = null;
                    parent.PasswordResetCode = null;
                    parent.PasswordResetCodeExpiry = null;
                    parent.EmailChangeCode = null;
                    parent.EmailChangeCodeExpiry = null;
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
        /// <summary>
        /// Gets the logged-in parent's profile information.
        /// Shows current details that can be edited.
        /// </summary>
        public async Task<(bool success, ParentProfileResponseDto? profile, string errorMessage)>
            GetMyProfileAsync(int parentId)
        {
            try
            {
                // STEP 1: Find parent with their children count
                var parent = await _context.Parents
                    .Include(p => p.ParentChildren)
                    .FirstOrDefaultAsync(p => p.ParentID == parentId);

                if (parent == null)
                {
                    _logger.LogWarning($"Profile request for non-existent parent {parentId}");
                    return (false, null, "Parent not found");
                }

                // STEP 2: Check if account is deleted
                if (parent.IsDeleted)
                {
                    _logger.LogWarning($"Profile request for deleted parent {parentId}");
                    return (false, null, "Account has been deleted");
                }

                // STEP 3: Build response
                var profile = new ParentProfileResponseDto
                {
                    ParentID = parent.ParentID,
                    Email = parent.Email,
                    FirstName = parent.FName,
                    LastName = parent.LName,
                    CreatedAt = parent.CreatedAt,
                    IsEmailConfirmed = parent.IsEmailConfirmed,
                    TotalChildren = parent.ParentChildren.Count
                };

                _logger.LogInformation($"Profile loaded for parent {parentId}");

                return (true, profile, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting profile for parent {parentId}: {ex.Message}");
                return (false, null, "An error occurred while loading your profile");
            }
        }

        /// <summary>
        /// Gets the parent's main dashboard.
        /// Shows welcome message and quick cards for all children (for the top buttons).
        /// </summary>
        public async Task<(bool success, ParentDashboardDto? dashboard, string errorMessage)>
            GetMyDashboardAsync(int parentId)
        {
            try
            {
                // STEP 1: Find parent
                var parent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.ParentID == parentId);

                if (parent == null)
                {
                    _logger.LogWarning($"Dashboard request for non-existent parent {parentId}");
                    return (false, null, "Parent not found");
                }

                // STEP 2: Get all children with their basic info
                var children = await _context.ParentChildren
                    .Where(pc => pc.ParentID == parentId)
                    .Include(pc => pc.Child)
                    .OrderBy(pc => pc.Child.CreatedAt) // Oldest first (or change to OrderByDescending for newest first)
                    .Select(pc => new ChildQuickCardDto
                    {
                        ChildID = pc.Child.ChildID,
                        FirstName = pc.Child.FName,
                        CurrentBalance = pc.Child.CurrentBalance,
                        Age = pc.Child.Age,
                        AvatarUrl = null 
                    })
                    .ToListAsync();

                // STEP 3: Build dashboard
                var dashboard = new ParentDashboardDto
                {
                    FirstName = parent.FName,
                    TotalChildren = children.Count,
                    Children = children
                };

                _logger.LogInformation($"Dashboard loaded for parent {parentId}");

                return (true, dashboard, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting dashboard for parent {parentId}: {ex.Message}");
                return (false, null, "An error occurred while loading your dashboard");
            }
        }

        /// <summary>
        /// Gets detailed information for a specific child.
        /// This is called when parent clicks on a child's button.
        /// Shows balance, quick stats, and buttons to manage allowance/expenses/goals/reports.
        /// </summary>
        public async Task<(bool success, ChildDetailedCardDto? childCard, string errorMessage)>
            GetChildDetailedCardAsync(int parentId, int childId)
        {
            try
            {
                // STEP 1: Verify parent-child relationship
                bool isLinked = await _context.ParentChildren
                    .AnyAsync(pc => pc.ParentID == parentId && pc.ChildID == childId);

                if (!isLinked)
                {
                    _logger.LogWarning($"Parent {parentId} attempted to view non-linked child {childId}");
                    return (false, null, "You are not authorized to view this child");
                }

                // STEP 2: Get child with personality type
                var child = await _context.Children
                    .Include(c => c.PersonalityType)
                    .FirstOrDefaultAsync(c => c.ChildID == childId);

                if (child == null)
                {
                    return (false, null, "Child not found");
                }

                // STEP 3: Calculate this month's stats
                var firstDayOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

                // Total spent this month
                var totalSpentThisMonth = await _context.Expenses
                    .Where(e => e.ChildID == childId && e.LogDate >= firstDayOfMonth)
                    .SumAsync(e => e.MoneyAmount);

                // Count expenses this month
                var expensesCountThisMonth = await _context.Expenses
                    .CountAsync(e => e.ChildID == childId && e.LogDate >= firstDayOfMonth);

                // Count active goals
                var activeGoalsCount = await _context.SavingsGoals
                    .CountAsync(g => g.ChildID == childId && g.Status == "Active");

                // STEP 4: Get allowance info (if exists)
                var allowance = await _context.Allowances
                    .FirstOrDefaultAsync(a => a.ChildID == childId && a.IsRecurring && a.IsActive);

                ChildAllowanceInfoSummaryDto? allowanceInfo = null;
                if (allowance != null)
                {
                    // Calculate next payment date (simple estimate)
                    DateTime? nextPaymentDate = CalculateNextPaymentDate(allowance);

                    allowanceInfo = new ChildAllowanceInfoSummaryDto
                    {
                        Type = allowance.Type,
                        Amount = allowance.Amount,
                        NextPaymentDate = nextPaymentDate,
                        IsActive = allowance.IsActive
                    };
                }

                // STEP 5: Build quick stats
                var quickStats = new ChildQuickStatsDto
                {
                    TotalSpentThisMonth = totalSpentThisMonth,
                    ExpensesCountThisMonth = expensesCountThisMonth,
                    ActiveGoalsCount = activeGoalsCount,
                    PersonalityTypeName = child.PersonalityType?.ParentName
                };

                // STEP 6: Build detailed card
                var childCard = new ChildDetailedCardDto
                {
                    ChildID = child.ChildID,
                    FirstName = child.FName,
                    LastName = child.LName,
                    Age = child.Age,
                    Gender = child.Gender,
                    CurrentBalance = child.CurrentBalance,
                    QuickStats = quickStats,
                    AllowanceInfo = allowanceInfo
                };

                _logger.LogInformation($"Parent {parentId} loaded detailed card for child {childId}");

                return (true, childCard, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting child card for parent {parentId}, child {childId}: {ex.Message}");
                return (false, null, "An error occurred while loading child information");
            }
        }

        // ==================== HELPER METHOD ====================

        /// <summary>
        /// Calculates approximate next payment date for an allowance.
        /// This is a simplified version - you already have this logic in AllowanceService.
        /// </summary>
        private DateTime? CalculateNextPaymentDate(Allowance allowance)
        {
            DateTime now = DateTime.UtcNow;

            switch (allowance.Type)
            {
                case "Daily":
                    var nextDaily = new DateTime(now.Year, now.Month, now.Day, allowance.DailyHour!.Value, 0, 0);
                    if (nextDaily <= now)
                        nextDaily = nextDaily.AddDays(1);
                    return nextDaily;

                case "Weekly":
                    DayOfWeek targetDay = Enum.Parse<DayOfWeek>(allowance.WeeklyDay!);
                    int daysUntilTarget = ((int)targetDay - (int)now.DayOfWeek + 7) % 7;
                    if (daysUntilTarget == 0 && allowance.LastCreditedDate?.Date == now.Date)
                        daysUntilTarget = 7;
                    return now.Date.AddDays(daysUntilTarget);

                case "Monthly":
                    int targetDayOfMonth = allowance.MonthlyDay!.Value;
                    int daysInCurrentMonth = DateTime.DaysInMonth(now.Year, now.Month);
                    int effectiveDay = Math.Min(targetDayOfMonth, daysInCurrentMonth);

                    if (now.Day < effectiveDay)
                    {
                        return new DateTime(now.Year, now.Month, effectiveDay);
                    }
                    else
                    {
                        var nextMonth = now.AddMonths(1);
                        int daysInNextMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
                        int effectiveDayNextMonth = Math.Min(targetDayOfMonth, daysInNextMonth);
                        return new DateTime(nextMonth.Year, nextMonth.Month, effectiveDayNextMonth);
                    }

                default:
                    return null;
            }
        }
    }
}