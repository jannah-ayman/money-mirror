namespace MoneyMirror.Core.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for confirming email change.
    /// Used when parent clicks confirmation link in email sent to new address.
    /// Completes the email change process after verification.
    /// Used as input for GET /api/auth/confirm-email-change endpoint.
    /// Validation is handled by ConfirmEmailChangeDtoValidator using FluentValidation.
    /// </summary>
    public class ConfirmEmailChangeDto
    {
        /// <summary>
        /// Parent's current (old) email address.
        /// Used to identify which parent account is changing email.
        /// Example: "oldemail@example.com"
        /// </summary>
        public string OldEmail { get; set; }

        /// <summary>
        /// Parent's new email address (being confirmed).
        /// Example: "newemail@example.com"
        /// </summary>
        public string NewEmail { get; set; }

        /// <summary>
        /// Email change confirmation token from the email link.
        /// Generated when email change was requested.
        /// Example: "550e8400-e29b-41d4-a716-446655440000"
        /// </summary>
        public string Token { get; set; }
    }
}
