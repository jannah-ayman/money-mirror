using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MoneyMirror.Infrastructure.Services
{
    /// Service for sending emails using SendGrid API.
    /// Updated to send 6-digit codes instead of clickable links.
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _sendGridApiKey;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _sendGridApiKey = _configuration["SendGrid:ApiKey"]
                ?? throw new InvalidOperationException("SendGrid API Key not configured");
            _senderEmail = _configuration["SendGrid:SenderEmail"]
                ?? throw new InvalidOperationException("SendGrid SenderEmail not configured");
            _senderName = _configuration["SendGrid:SenderName"]
                ?? "Money Mirror";
        }

        /// <summary>
        /// Sends email confirmation code to newly registered parents.
        /// They copy the code and enter it in the app.
        /// </summary>
        public async Task<bool> SendEmailConfirmationCodeAsync(string toEmail, string toName, string confirmationCode)
        {
            var subject = "Welcome to Money Mirror - Confirm Your Email";

            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f9f9f9; }}
                        .code-box {{ 
                            background-color: #fff; 
                            border: 2px dashed #4CAF50; 
                            padding: 20px; 
                            text-align: center; 
                            margin: 20px 0;
                            border-radius: 8px;
                        }}
                        .code {{ 
                            font-size: 32px; 
                            font-weight: bold; 
                            color: #4CAF50; 
                            letter-spacing: 8px;
                            font-family: 'Courier New', monospace;
                        }}
                        .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #666; }}
                        .warning {{ color: #e74c3c; font-weight: bold; }}
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
                            
                            <p><strong>To complete your registration, enter this code in the app:</strong></p>
                            
                            <div class='code-box'>
                                <p style='margin: 0; color: #666; font-size: 14px;'>Your Confirmation Code:</p>
                                <p class='code'>{confirmationCode}</p>
                            </div>

                            <p class='warning'>⏰ This code will expire in 15 minutes.</p>
                            
                            <p style='font-size: 14px; color: #666;'>
                                Simply copy the code above and paste it into the confirmation screen in the Money Mirror app.
                            </p>
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
        /// Sends password reset code to parent's email.
        /// </summary>
        public async Task<bool> SendPasswordResetCodeAsync(string toEmail, string toName, string resetCode)
        {
            var subject = "Money Mirror - Password Reset Code";

            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #FF9800; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f9f9f9; }}
                        .code-box {{ 
                            background-color: #fff; 
                            border: 2px dashed #FF9800; 
                            padding: 20px; 
                            text-align: center; 
                            margin: 20px 0;
                            border-radius: 8px;
                        }}
                        .code {{ 
                            font-size: 32px; 
                            font-weight: bold; 
                            color: #FF9800; 
                            letter-spacing: 8px;
                            font-family: 'Courier New', monospace;
                        }}
                        .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #666; }}
                        .warning {{ color: #e74c3c; font-weight: bold; }}
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
                            
                            <p><strong>Use this code to reset your password:</strong></p>
                            
                            <div class='code-box'>
                                <p style='margin: 0; color: #666; font-size: 14px;'>Your Reset Code:</p>
                                <p class='code'>{resetCode}</p>
                            </div>

                            <p class='warning'>⏰ This code will expire in 15 minutes.</p>
                            
                            <p style='font-size: 14px; color: #666;'>
                                Enter this code in the Money Mirror app, then create your new password.
                            </p>
                            
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
        /// Sends email change confirmation code to NEW email address.
        /// </summary>
        public async Task<bool> SendEmailChangeCodeAsync(string toEmail, string toName, string confirmationCode)
        {
            var subject = "Money Mirror - Confirm Your New Email";

            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #2196F3; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f9f9f9; }}
                        .code-box {{ 
                            background-color: #fff; 
                            border: 2px dashed #2196F3; 
                            padding: 20px; 
                            text-align: center; 
                            margin: 20px 0;
                            border-radius: 8px;
                        }}
                        .code {{ 
                            font-size: 32px; 
                            font-weight: bold; 
                            color: #2196F3; 
                            letter-spacing: 8px;
                            font-family: 'Courier New', monospace;
                        }}
                        .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #666; }}
                        .warning {{ color: #e74c3c; font-weight: bold; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Confirm Your New Email</h1>
                        </div>
                        <div class='content'>
                            <h2>Hi {toName},</h2>
                            <p>You requested to change your Money Mirror email address to this email.</p>
                            
                            <p><strong>Use this code to confirm your new email:</strong></p>
                            
                            <div class='code-box'>
                                <p style='margin: 0; color: #666; font-size: 14px;'>Your Confirmation Code:</p>
                                <p class='code'>{confirmationCode}</p>
                            </div>

                            <p class='warning'>⏰ This code will expire in 15 minutes.</p>
                            
                            <p style='font-size: 14px; color: #666;'>
                                Enter this code in the Money Mirror app to complete your email change.
                            </p>
                            
                            <p>If you didn't request this email change, please ignore this message.</p>
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
        /// (This one doesn't need codes - keeping original)
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
        /// Private helper method that actually sends the email via Brevo.
        /// </summary>
        private async Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlContent)
        {
            try
            {
                var config = new sib_api_v3_sdk.Client.Configuration();
                config.ApiKey["api-key"] = _sendGridApiKey;

                var apiInstance = new sib_api_v3_sdk.Api.TransactionalEmailsApi(config);
                var email = new sib_api_v3_sdk.Model.SendSmtpEmail(
                    sender: new sib_api_v3_sdk.Model.SendSmtpEmailSender(_senderName, _senderEmail),
                    to: new List<sib_api_v3_sdk.Model.SendSmtpEmailTo> {
                new sib_api_v3_sdk.Model.SendSmtpEmailTo(toEmail, toName)
                    },
                    subject: subject,
                    htmlContent: htmlContent
                );

                await apiInstance.SendTransacEmailAsync(email);
                _logger.LogInformation($"Email sent to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Email failed: {ex.Message}");
                return false;
            }
        }
    }
}