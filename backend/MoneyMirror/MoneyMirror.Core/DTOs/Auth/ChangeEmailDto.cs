namespace MoneyMirror.Core.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for changing parent's email address.
    /// Requires current password for security verification.
    /// Sends confirmation email to new address before applying change.
    /// Old email remains active until new email is confirmed.
    /// Used as input for POST /api/auth/change-email endpoint.
    /// Validation is handled by ChangeEmailDtoValidator using FluentValidation.
    /// </summary>
    public class ChangeEmailDto
    {
        /// <summary>
        /// Parent's new email address.
        /// Must be unique (not already registered by another parent).
        /// Will receive a confirmation link before change is applied.
        /// Example: "newemail@example.com"
        /// </summary>
        public string NewEmail { get; set; }

        /// <summary>
        /// Parent's current password for security verification.
        /// Required to prevent unauthorized email changes.
        /// Example: "CurrentP@ssw0rd!"
        /// </summary>
        public string CurrentPassword { get; set; }
    }
}