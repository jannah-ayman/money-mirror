namespace MoneyMirror.Core.DTOs.Auth
{
    /// <summary>
    /// DTO for requesting email change.
    /// System sends a 6-digit code to the NEW email address.
    /// Used as input for POST /api/auth/request-email-change
    /// </summary>
    public class RequestEmailChangeDto
    {
        /// <summary>
        /// New email address.
        /// Example: "newemail@example.com"
        /// </summary>
        public string NewEmail { get; set; }

        /// <summary>
        /// Current password for security verification.
        /// Example: "CurrentP@ssw0rd!"
        /// </summary>
        public string CurrentPassword { get; set; }
    }
}