namespace MoneyMirror.Core.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for initiating password reset flow.
    /// Parent provides their email to receive a password reset link.
    /// Used as input for POST /api/auth/forgot-password endpoint.
    /// Validation is handled by ForgotPasswordDtoValidator using FluentValidation.
    /// </summary>
    public class ForgotPasswordDto
    {
        /// <summary>
        /// Email address to send reset code to.
        /// Example: "john@email.com"
        /// </summary>
        public string Email { get; set; }
    }
}