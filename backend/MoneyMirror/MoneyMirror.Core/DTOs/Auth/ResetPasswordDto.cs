namespace MoneyMirror.Core.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for resetting password.
    /// Parent provides email, reset token (from email link), and new password.
    /// Used as input for POST /api/auth/reset-password endpoint.
    /// Validation is handled by ResetPasswordDtoValidator using FluentValidation.
    /// </summary>
    public class ResetPasswordDto
    {
        /// <summary>
        /// Parent's email address.
        /// Example: "john.smith@email.com"
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password reset token from the email link.
        /// Generated and sent via email during forgot password flow.
        /// Example: "550e8400-e29b-41d4-a716-446655440000"
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// New password to set for the account.
        /// Must meet password complexity requirements.
        /// Example: "NewP@ssw0rd!"
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Confirmation of new password to prevent typos.
        /// Must match NewPassword exactly.
        /// </summary>
        public string ConfirmNewPassword { get; set; }
    }
}