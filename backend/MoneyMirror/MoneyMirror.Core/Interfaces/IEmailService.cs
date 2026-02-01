namespace MoneyMirror.Core.Interfaces
{
    /// Interface for email sending operations using SendGrid.
    /// Handles all email communications including confirmation and notifications.
    public interface IEmailService
    {
        /// Sends an email confirmation message to a newly registered parent.
        /// Email contains a link with a unique token to verify the email address.
        /// <param name="toEmail">Recipient's email address</param>
        /// <param name="toName">Recipient's full name</param>
        /// <param name="confirmationLink">Full URL to the email confirmation endpoint</param>
        /// <returns>Task representing the async operation. True if sent successfully, False otherwise.</returns>
        Task<bool> SendEmailConfirmationAsync(string toEmail, string toName, string confirmationLink);

        /// Sends a password reset email to a parent who requested it.
        /// Email contains a link with a unique token to reset the password.
        /// <param name="toEmail">Recipient's email address</param>
        /// <param name="toName">Recipient's full name</param>
        /// <param name="resetLink">Full URL to the password reset endpoint</param>
        /// <returns>Task representing the async operation. True if sent successfully, False otherwise.</returns>
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string toName, string resetLink);

        /// Sends a weekly summary report email to a parent.
        /// Email contains spending analytics and insights about their children.
        /// <param name="toEmail">Recipient's email address</param>
        /// <param name="toName">Recipient's full name</param>
        /// <param name="reportContent">HTML content of the weekly report</param>
        /// <returns>Task representing the async operation. True if sent successfully, False otherwise.</returns>
        Task<bool> SendWeeklyReportAsync(string toEmail, string toName, string reportContent);

        /// Sends a generic notification email to a parent.
        /// Used for important alerts like goal completion, spending thresholds, etc.
        /// <param name="toEmail">Recipient's email address</param>
        /// <param name="toName">Recipient's full name</param>
        /// <param name="subject">Email subject line</param>
        /// <param name="htmlContent">HTML body content of the email</param>
        /// <returns>Task representing the async operation. True if sent successfully, False otherwise.</returns>
        Task<bool> SendNotificationEmailAsync(string toEmail, string toName, string subject, string htmlContent);
    }
}
