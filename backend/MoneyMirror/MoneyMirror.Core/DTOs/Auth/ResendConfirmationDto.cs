namespace MoneyMirror.Core.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for resending email confirmation link.
    /// Used when a parent didn't receive or lost their original confirmation email.
    /// Used as input for POST /api/auth/resend-confirmation endpoint.
    /// Validation is handled by ResendConfirmationDtoValidator using FluentValidation.
    /// </summary>
    public class ResendConfirmationDto
    {
        /// <summary>
        /// Parent's registered email address.
        /// A new confirmation link will be sent to this email.
        /// Example: "john.smith@email.com"
        /// </summary>
        public string Email { get; set; }
    }
}