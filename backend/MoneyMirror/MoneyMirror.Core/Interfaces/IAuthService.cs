using MoneyMirror.Core.DTOs.Auth;
using MoneyMirror.Core.DTOs.Parent;
using MoneyMirror.Core.Enums;

namespace MoneyMirror.Core.Interfaces
{
    /// Interface for authentication and authorization operations.
    /// Now uses 6-digit codes instead of email links.
    public interface IAuthService
    {
        // ==================== REGISTRATION & EMAIL CONFIRMATION ====================

        /// <summary>
        /// Registers a new parent account.
        /// Sends a 6-digit confirmation code to email.
        /// Parent cannot login until they enter the code.
        /// </summary>
        /// <param name="registerDto">Registration data from parent</param>
        /// <returns>Tuple: (success flag, message, parentId if successful)</returns>
        Task<(bool success, string message, int? parentId)> RegisterParentAsync(RegisterParentDto registerDto);

        /// <summary>
        /// Confirms email using 6-digit code (instead of clicking link).
        /// Activates the account so parent can login.
        /// </summary>
        /// <param name="dto">Email and 6-digit code</param>
        /// <returns>Tuple: (success flag, message, auth response if auto-login enabled)</returns>
        Task<(bool success, string message, AuthResponseDto? authResponse)> ConfirmEmailWithCodeAsync(ConfirmEmailWithCodeDto dto);

        /// <summary>
        /// Resends email confirmation code if expired or not received.
        /// Generates new 6-digit code and sends email.
        /// </summary>
        /// <param name="dto">Email address</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> ResendConfirmationCodeAsync(ResendConfirmationCodeDto dto);

        // ==================== PASSWORD RESET ====================

        /// <summary>
        /// Initiates password reset by sending 6-digit code to email.
        /// Code is valid for 15 minutes.
        /// </summary>
        /// <param name="dto">Email address</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> ForgotPasswordAsync(ForgotPasswordDto dto);

        /// <summary>
        /// Verifies the password reset code before allowing password change.
        /// This is an optional step - can be used to show "code is valid" message.
        /// </summary>
        /// <param name="dto">Email and code</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> VerifyResetCodeAsync(VerifyResetCodeDto dto);

        /// <summary>
        /// Resets password using verified code.
        /// Automatically logs in parent after successful reset.
        /// </summary>
        /// <param name="dto">Email, code, and new password</param>
        /// <returns>Tuple: (success flag, message, auth response if auto-login enabled)</returns>
        Task<(bool success, string message, AuthResponseDto? authResponse)> ResetPasswordWithCodeAsync(ResetPasswordWithCodeDto dto);

        // ==================== EMAIL CHANGE ====================

        /// <summary>
        /// Initiates email change by sending 6-digit code to NEW email address.
        /// Verifies current password before proceeding.
        /// Old email remains active until new email is confirmed with code.
        /// </summary>
        /// <param name="parentId">ID of parent changing email</param>
        /// <param name="dto">New email and current password</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> RequestEmailChangeAsync(int parentId, RequestEmailChangeDto dto);

        /// <summary>
        /// Confirms email change using 6-digit code sent to new email.
        /// Applies the email change after successful verification.
        /// Revokes all refresh tokens to force re-login with new email.
        /// </summary>
        /// <param name="dto">Old email, new email, and code</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> ConfirmEmailChangeWithCodeAsync(ConfirmEmailChangeWithCodeDto dto);

        // ==================== LOGIN & TOKEN MANAGEMENT ====================

        /// <summary>
        /// Authenticates a parent and generates JWT tokens.
        /// Verifies email/password, checks email confirmation status.
        /// </summary>
        /// <param name="loginDto">Login credentials from parent</param>
        /// <returns>Tuple: (auth response if successful, failure reason if failed)</returns>
        Task<(AuthResponseDto? auth, LoginFailureReason? failure)> LoginAsync(LoginDto loginDto);

        /// <summary>
        /// Generates new access token using a valid refresh token.
        /// Allows user to stay logged in without re-entering credentials.
        /// </summary>
        /// <param name="refreshTokenDto">Current tokens from client</param>
        /// <returns>New AuthResponseDto with fresh tokens if successful, null if failed</returns>
        Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);

        /// <summary>
        /// Revokes a refresh token (logout functionality).
        /// Marks the refresh token as invalid in the database.
        /// </summary>
        /// <param name="parentId">ID of the parent logging out</param>
        /// <returns>True if successfully revoked, False otherwise</returns>
        Task<bool> RevokeRefreshTokenAsync(int parentId);

        // ==================== PROFILE MANAGEMENT ====================

        /// <summary>
        /// Updates parent profile information (name, phone).
        /// Email and password updates handled separately for security.
        /// </summary>
        /// <param name="parentId">ID of parent to update</param>
        /// <param name="updateDto">Updated profile data</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> UpdateParentProfileAsync(int parentId, UpdateParentProfileDto updateDto);

        /// <summary>
        /// Gets the parent's own profile information.
        /// Shows current details before editing.
        /// </summary>
        /// <param name="parentId">ID of the parent (from JWT token)</param>
        /// <returns>Tuple: (success flag, profile data, error message)</returns>
        Task<(bool success, ParentProfileResponseDto? profile, string errorMessage)> GetMyProfileAsync(int parentId);

        /// <summary>
        /// Gets the parent's main dashboard data.
        /// Shows welcome message and quick cards for all children.
        /// </summary>
        /// <param name="parentId">ID of the parent (from JWT token)</param>
        /// <returns>Tuple: (success flag, dashboard data, error message)</returns>
        Task<(bool success, ParentDashboardDto? dashboard, string errorMessage)> GetMyDashboardAsync(int parentId);

        /// <summary>
        /// Gets detailed information for a specific child.
        /// Includes balance, quick stats, and allowance info.
        /// </summary>
        /// <param name="parentId">ID of the parent (from JWT token)</param>
        /// <param name="childId">ID of the child to view</param>
        /// <returns>Tuple: (success flag, child detailed card, error message)</returns>
        Task<(bool success, ChildDetailedCardDto? childCard, string errorMessage)> GetChildDetailedCardAsync(int parentId, int childId);

        // ==================== ACCOUNT DELETION & RECOVERY ====================

        /// <summary>
        /// Soft deletes a parent account (marks as deleted, doesn't remove data).
        /// Requires current password for security verification.
        /// </summary>
        /// <param name="parentId">ID of parent to delete</param>
        /// <param name="currentPassword">Parent's password for verification</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> DeleteParentAccountAsync(int parentId, string currentPassword);

        /// <summary>
        /// Recovers a soft-deleted parent account within the 30-day grace period.
        /// Restores access and reinstates ParentChild relationships.
        /// </summary>
        /// <param name="email">Parent's email address</param>
        /// <param name="password">Parent's password for verification</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> RecoverDeletedAccountAsync(string email, string password);

        /// <summary>
        /// Background job: Permanently deletes accounts where PermanentDeletionDate has passed.
        /// Should be called by a scheduled background service.
        /// </summary>
        /// <returns>Count of accounts permanently deleted</returns>
        Task<int> PermanentlyDeleteExpiredAccountsAsync();

        // ==================== UTILITY ====================

        /// <summary>
        /// Checks if an email address is already registered in the system.
        /// Used to prevent duplicate registrations.
        /// </summary>
        /// <param name="email">Email address to check</param>
        /// <returns>True if email exists, False if available</returns>
        Task<bool> EmailExistsAsync(string email);
    }
}