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
        /// Parent's registered email address.
        /// A password reset link will be sent to this email if it exists.
        /// Example: "john.smith@email.com"
        /// </summary>
        public string Email { get; set; }
    }
}