using MoneyMirror.Core.DTOs.Auth;
using MoneyMirror.Core.DTOs.Parent;
using MoneyMirror.Core.Enums;

namespace MoneyMirror.Core.Interfaces
{
    /// Interface for authentication and authorization operations.
    /// Handles parent registration, login, email confirmation, and token refresh.
    public interface IAuthService
    {
        /// Registers a new parent account.
        /// Creates parent record, generates email confirmation token, sends confirmation email.
        /// Parent cannot login until email is confirmed.
        /// <param name="registerDto">Registration data from parent</param>
        /// <returns>Tuple: (success flag, message, parentId if successful)</returns>
        Task<(bool success, string message, int? parentId)> RegisterParentAsync(RegisterParentDto registerDto);

        /// Authenticates a parent and generates JWT tokens.
        /// Verifies email/password, checks email confirmation status, generates access + refresh tokens.
        /// <param name="loginDto">Login credentials from parent</param>
        /// <returns>
        /// Tuple:
        /// - auth: AuthResponseDto if successful
        /// - failure: LoginFailureReason if login failed
        /// </returns>

        Task<(AuthResponseDto? auth, LoginFailureReason? failure)> LoginAsync(LoginDto loginDto);

        /// Confirms a parent's email address using the token sent via email.
        /// Activates the account so parent can login.
        /// <param name="email">Parent's email address</param>
        /// <param name="token">Email confirmation token from URL</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> ConfirmEmailAsync(string email, string token);

        /// Generates new access token using a valid refresh token.
        /// Allows user to stay logged in without re-entering credentials.
        /// <param name="refreshTokenDto">Current tokens from client</param>
        /// <returns>New AuthResponseDto with fresh tokens if successful, null if failed</returns>
        Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);

        /// Revokes a refresh token (logout functionality).
        /// Marks the refresh token as invalid in the database.
        /// <param name="parentId">ID of the parent logging out</param>
        /// <returns>True if successfully revoked, False otherwise</returns>
        Task<bool> RevokeRefreshTokenAsync(int parentId);

        /// Checks if an email is already registered in the system.
        /// Used to prevent duplicate registrations.
        /// <param name="email">Email address to check</param>
        /// <returns>True if email exists, False if available</returns>
        Task<bool> EmailExistsAsync(string email);

        /// Initiates password reset flow by generating a reset token and sending email.
        /// Token is valid for 1 hour.
        /// <param name="email">Parent's email address</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> ForgotPasswordAsync(string email);

        /// Resets a parent's password using the token from the reset email.
        /// Verifies token validity before updating password.
        /// <param name="email">Parent's email address</param>
        /// <param name="token">Password reset token from email</param>
        /// <param name="newPassword">New password to set</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> ResetPasswordAsync(string email, string token, string newPassword);

        /// Resends email confirmation link to a parent.
        /// Generates new confirmation token and sends new email.
        /// <param name="email">Parent's email address</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> ResendConfirmationEmailAsync(string email);

        /// Updates parent profile information (name, phone).
        /// Email and password updates handled separately for security.
        /// <param name="parentId">ID of parent to update</param>
        /// <param name="updateDto">Updated profile data</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> UpdateParentProfileAsync(int parentId, UpdateParentProfileDto updateDto);

        /// Initiates email change process by sending verification to new email.
        /// Verifies current password before proceeding.
        /// Old email remains active until new email is confirmed.
        /// <param name="parentId">ID of parent changing email</param>
        /// <param name="changeEmailDto">New email and current password</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> ChangeEmailAsync(int parentId, ChangeEmailDto changeEmailDto);

        /// Confirms email change using token from verification email.
        /// Applies the email change after successful verification.
        /// Revokes all refresh tokens to force re-login with new email.
        /// <param name="confirmDto">Old email, new email, and token</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> ConfirmEmailChangeAsync(ConfirmEmailChangeDto confirmDto);

        /// Soft deletes a parent account (marks as deleted, doesn't remove data).
        /// Removes all ParentChild relationships but preserves child accounts.
        /// Requires current password for security verification.
        /// <param name="parentId">ID of parent to delete</param>
        /// <param name="currentPassword">Parent's password for verification</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> DeleteParentAccountAsync(int parentId, string currentPassword);
        /// Recovers a soft-deleted parent account within the 30-day grace period.
        /// Restores access and reinstates ParentChild relationships.
        /// <param name="email">Parent's email address</param>
        /// <param name="password">Parent's password for verification</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> RecoverDeletedAccountAsync(string email, string password);

        /// Background job: Permanently deletes accounts where PermanentDeletionDate has passed.
        /// Anonymizes PII while preserving children's data for audit/educational purposes.
        /// Should be called by a scheduled background service (e.g., Hangfire, Azure Functions).
        /// <returns>Count of accounts permanently deleted</returns>
        Task<int> PermanentlyDeleteExpiredAccountsAsync();
        // ==================== NEW METHODS FOR PARENT DASHBOARD & PROFILE ====================

        /// <summary>
        /// Gets the parent's own profile information.
        /// Shows current details before editing.
        /// </summary>
        /// <param name="parentId">ID of the parent (from JWT token)</param>
        /// <returns>Tuple: (success flag, profile data, error message)</returns>
        Task<(bool success, ParentProfileResponseDto? profile, string errorMessage)>
        GetMyProfileAsync(int parentId);

        /// <summary>
        /// Gets the parent's main dashboard data.
        /// Shows welcome message and quick cards for all children.
        /// </summary>
        /// <param name="parentId">ID of the parent (from JWT token)</param>
        /// <returns>Tuple: (success flag, dashboard data, error message)</returns>
        Task<(bool success, ParentDashboardDto? dashboard, string errorMessage)>
        GetMyDashboardAsync(int parentId);

        /// <summary>
        /// Gets detailed information for a specific child.
        /// This is what shows when parent clicks a child's button on the dashboard.
        /// Includes balance, quick stats, and allowance info.
        /// </summary>
        /// <param name="parentId">ID of the parent (from JWT token)</param>
        /// <param name="childId">ID of the child to view</param>
        /// <returns>Tuple: (success flag, child detailed card, error message)</returns>
        Task<(bool success, ChildDetailedCardDto? childCard, string errorMessage)>
        GetChildDetailedCardAsync(int parentId, int childId);
    }

}