using MoneyMirror.Core.DTOs.Auth;

namespace MoneyMirror.Core.Interfaces
{
    /// <summary>
    /// Interface for authentication and authorization operations.
    /// Handles parent registration, login, email confirmation, and token refresh.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new parent account.
        /// Creates parent record, generates email confirmation token, sends confirmation email.
        /// Parent cannot login until email is confirmed.
        /// </summary>
        /// <param name="registerDto">Registration data from parent</param>
        /// <returns>Tuple: (success flag, message, parentId if successful)</returns>
        Task<(bool success, string message, int? parentId)> RegisterParentAsync(RegisterParentDto registerDto);

        /// <summary>
        /// Authenticates a parent and generates JWT tokens.
        /// Verifies email/password, checks email confirmation status, generates access + refresh tokens.
        /// </summary>
        /// <param name="loginDto">Login credentials from parent</param>
        /// <returns>AuthResponseDto if successful, null if failed</returns>
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);

        /// <summary>
        /// Confirms a parent's email address using the token sent via email.
        /// Activates the account so parent can login.
        /// </summary>
        /// <param name="email">Parent's email address</param>
        /// <param name="token">Email confirmation token from URL</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> ConfirmEmailAsync(string email, string token);

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

        /// <summary>
        /// Checks if an email is already registered in the system.
        /// Used to prevent duplicate registrations.
        /// </summary>
        /// <param name="email">Email address to check</param>
        /// <returns>True if email exists, False if available</returns>
        Task<bool> EmailExistsAsync(string email);

        /// <summary>
        /// Initiates password reset flow by generating a reset token and sending email.
        /// Token is valid for 1 hour.
        /// </summary>
        /// <param name="email">Parent's email address</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> ForgotPasswordAsync(string email);

        /// <summary>
        /// Resets a parent's password using the token from the reset email.
        /// Verifies token validity before updating password.
        /// </summary>
        /// <param name="email">Parent's email address</param>
        /// <param name="token">Password reset token from email</param>
        /// <param name="newPassword">New password to set</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> ResetPasswordAsync(string email, string token, string newPassword);

        /// <summary>
        /// Resends email confirmation link to a parent.
        /// Generates new confirmation token and sends new email.
        /// </summary>
        /// <param name="email">Parent's email address</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> ResendConfirmationEmailAsync(string email);

        /// <summary>
        /// Updates parent profile information (name, phone).
        /// Email and password updates handled separately for security.
        /// </summary>
        /// <param name="parentId">ID of parent to update</param>
        /// <param name="updateDto">Updated profile data</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> UpdateParentProfileAsync(int parentId, UpdateParentProfileDto updateDto);

        /// <summary>
        /// Initiates email change process by sending verification to new email.
        /// Verifies current password before proceeding.
        /// Old email remains active until new email is confirmed.
        /// </summary>
        /// <param name="parentId">ID of parent changing email</param>
        /// <param name="changeEmailDto">New email and current password</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> ChangeEmailAsync(int parentId, ChangeEmailDto changeEmailDto);

        /// <summary>
        /// Confirms email change using token from verification email.
        /// Applies the email change after successful verification.
        /// Revokes all refresh tokens to force re-login with new email.
        /// </summary>
        /// <param name="confirmDto">Old email, new email, and token</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> ConfirmEmailChangeAsync(ConfirmEmailChangeDto confirmDto);

        /// <summary>
        /// Soft deletes a parent account (marks as deleted, doesn't remove data).
        /// Removes all ParentChild relationships but preserves child accounts.
        /// Requires current password for security verification.
        /// </summary>
        /// <param name="parentId">ID of parent to delete</param>
        /// <param name="currentPassword">Parent's password for verification</param>
        /// <returns>Tuple: (success flag, message)</returns>
        Task<(bool success, string message)> DeleteParentAccountAsync(int parentId, string currentPassword);
    }
}