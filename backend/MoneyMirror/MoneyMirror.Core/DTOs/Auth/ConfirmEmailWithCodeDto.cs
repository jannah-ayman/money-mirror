

namespace MoneyMirror.Core.DTOs.Auth
{
    /// <summary>
    /// DTO for confirming email with 6-digit code (after registration).
    /// User enters the code they received in email.
    /// Used as input for POST /api/auth/confirm-email-with-code
    /// </summary>
    public class ConfirmEmailWithCodeDto
    {
        /// <summary>
        /// Parent's email address (the one they registered with).
        /// Example: "john@email.com"
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 6-digit confirmation code from email.
        /// Example: "483920"
        /// </summary>
        public string Code { get; set; }
    }
}
