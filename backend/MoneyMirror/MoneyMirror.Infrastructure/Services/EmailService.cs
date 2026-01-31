using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MoneyMirror.Infrastructure.Services
{
    /// <summary>
    /// Service for sending emails using SendGrid API.
    /// Handles all email communications for the Money Mirror application.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _sendGridApiKey;
        private readonly string _senderEmail;
        private readonly string _senderName;

        /// <summary>
        /// Constructor - dependency injection provides configuration and logging.
        /// </summary>
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Load SendGrid settings from configuration
            _sendGridApiKey = _configuration["SendGrid:ApiKey"]
                ?? throw new InvalidOperationException("SendGrid API Key not configured");
            _senderEmail = _configuration["SendGrid:SenderEmail"]
                ?? throw new InvalidOperationException("SendGrid SenderEmail not configured");
            _senderName = _configuration["SendGrid:SenderName"]
                ?? "Money Mirror";
        }

        /// <summary>
        /// Sends an email confirmation message to newly registered parents.
        /// </summary>
        public async Task<bool> SendEmailConfirmationAsync(string toEmail, string toName, string confirmationLink)
        {
            var subject = "Welcome to Money Mirror - Confirm Your Email";

            // HTML email body (you can make this prettier later!)
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f9f9f9; }}
                        .button {{ display: inline-block; padding: 12px 24px; margin: 20px 0; 
                                  background-color: #4CAF50; color: white; text-decoration: none; 
                                  border-radius: 5px; }}
                        .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #666; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Welcome to Money Mirror! 🎉</h1>
                        </div>
                        <div class='content'>
                            <h2>Hi {toName},</h2>
                            <p>Thank you for signing up for Money Mirror!</p>
                            <p>We're excited to help you teach your children about financial literacy in a fun and engaging way.</p>
                            <p>To get started, please confirm your email address by clicking the button below:</p>
                            <div style='text-align: center;'>
                                <a href='{confirmationLink}' class='button'>Confirm Email Address</a>
                            </div>
                            <p style='font-size: 12px; color: #666;'>
                                If the button doesn't work, copy and paste this link into your browser:<br>
                                <a href='{confirmationLink}'>{confirmationLink}</a>
                            </p>
                            <p><strong>This link will expire in 24 hours.</strong></p>
                        </div>
                        <div class='footer'>
                            <p>If you didn't create an account with Money Mirror, please ignore this email.</p>
                            <p>&copy; 2025 Money Mirror. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            return await SendEmailAsync(toEmail, toName, subject, htmlContent);
        }

        /// <summary>
        /// Sends a password reset email.
        /// </summary>
        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string toName, string resetLink)
        {
            var subject = "Money Mirror - Reset Your Password";

            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #FF9800; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f9f9f9; }}
                        .button {{ display: inline-block; padding: 12px 24px; margin: 20px 0; 
                                  background-color: #FF9800; color: white; text-decoration: none; 
                                  border-radius: 5px; }}
                        .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #666; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Password Reset Request</h1>
                        </div>
                        <div class='content'>
                            <h2>Hi {toName},</h2>
                            <p>We received a request to reset your Money Mirror password.</p>
                            <p>Click the button below to reset your password:</p>
                            <div style='text-align: center;'>
                                <a href='{resetLink}' class='button'>Reset Password</a>
                            </div>
                            <p style='font-size: 12px; color: #666;'>
                                If the button doesn't work, copy and paste this link into your browser:<br>
                                <a href='{resetLink}'>{resetLink}</a>
                            </p>
                            <p><strong>This link will expire in 1 hour.</strong></p>
                            <p>If you didn't request a password reset, please ignore this email and your password will remain unchanged.</p>
                        </div>
                        <div class='footer'>
                            <p>&copy; 2025 Money Mirror. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            return await SendEmailAsync(toEmail, toName, subject, htmlContent);
        }

        /// <summary>
        /// Sends a weekly summary report email.
        /// </summary>
        public async Task<bool> SendWeeklyReportAsync(string toEmail, string toName, string reportContent)
        {
            var subject = "Your Weekly Money Mirror Report";

            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #2196F3; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f9f9f9; }}
                        .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #666; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>📊 Your Weekly Report</h1>
                        </div>
                        <div class='content'>
                            <h2>Hi {toName},</h2>
                            <p>Here's your weekly summary of your children's financial activities:</p>
                            {reportContent}
                        </div>
                        <div class='footer'>
                            <p>&copy; 2025 Money Mirror. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            return await SendEmailAsync(toEmail, toName, subject, htmlContent);
        }

        /// <summary>
        /// Sends a generic notification email.
        /// </summary>
        public async Task<bool> SendNotificationEmailAsync(string toEmail, string toName, string subject, string htmlContent)
        {
            return await SendEmailAsync(toEmail, toName, subject, htmlContent);
        }

        /// <summary>
        /// Private helper method that actually sends the email via SendGrid.
        /// All other methods use this internally.
        /// </summary>
        private async Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlContent)
        {
            try
            {
                // Create SendGrid client
                var client = new SendGridClient(_sendGridApiKey);

                // Create sender and recipient
                var from = new EmailAddress(_senderEmail, _senderName);
                var to = new EmailAddress(toEmail, toName);

                // Create email message
                var msg = MailHelper.CreateSingleEmail(
                    from,
                    to,
                    subject,
                    plainTextContent: "Please view this email in an HTML-capable email client.",
                    htmlContent: htmlContent
                );

                // Send email
                var response = await client.SendEmailAsync(msg);

                // Check if successful (SendGrid returns 2xx status codes for success)
                if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                    response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    _logger.LogInformation($"Email sent successfully to {toEmail}");
                    return true;
                }
                else
                {
                    var body = await response.Body.ReadAsStringAsync();
                    _logger.LogError($"Failed to send email to {toEmail}. Status: {response.StatusCode}, Body: {body}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception while sending email to {toEmail}: {ex.Message}");
                return false;
            }
        }
    }
}
