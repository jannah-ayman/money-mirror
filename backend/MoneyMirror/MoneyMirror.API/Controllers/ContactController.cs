using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMirror.Core.DTOs.Common;
using MoneyMirror.Core.Interfaces;
using System.Net;

namespace MoneyMirror.API.Controllers
{
   
    /// Controller handling contact and feedback operations.
    /// Route: POST /api/contact-us
 
    [ApiController]
    [Route("api/contact-us")]
    public class ContactController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<ContactController> _logger;
        private const string AdminEmail = "jannahayman169@gmail.com";

        public ContactController(IEmailService emailService, ILogger<ContactController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Receives contact messages and emails them to the administrator.
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object>>> SendContactMessage([FromBody] ContactUsRequestDto dto)
        {
            // Inputs are automatically validated by FluentValidationFilter using ContactUsRequestDtoValidator.

            // HTML encode values to prevent HTML/Script injection inside the email client.
            var safeName = WebUtility.HtmlEncode(dto.Name);
            var safeEmail = WebUtility.HtmlEncode(dto.Email);
            var safeSubject = WebUtility.HtmlEncode(dto.Subject);
            var safeMessage = WebUtility.HtmlEncode(dto.Message).Replace("\n", "<br />");

            // Build custom HTML email message
            var emailBody = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ padding: 20px; background-color: #f9f9f9; border: 1px solid #ddd; border-radius: 0 0 8px 8px; }}
                        .field {{ margin-bottom: 15px; }}
                        .label {{ font-weight: bold; color: #4CAF50; }}
                        .message-box {{ background-color: white; border-left: 4px solid #4CAF50; padding: 15px; margin-top: 10px; border-radius: 4px; white-space: pre-wrap; }}
                        .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #666; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>📬 New Support Request</h2>
                        </div>
                        <div class='content'>
                            <div class='field'>
                                <span class='label'>Name:</span> {safeName}
                            </div>
                            <div class='field'>
                                <span class='label'>Email:</span> {safeEmail}
                            </div>
                            <div class='field'>
                                <span class='label'>Subject:</span> {safeSubject}
                            </div>
                            <div class='field'>
                                <span class='label'>Message:</span>
                                <div class='message-box'>{safeMessage}</div>
                            </div>
                        </div>
                        <div class='footer'>
                            <p>This is an automated notification from the Money Mirror system.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            _logger.LogInformation("Sending contact message from {SenderEmail} to {AdminEmail}", dto.Email, AdminEmail);

            var success = await _emailService.SendNotificationEmailAsync(
                AdminEmail,
                "Money Mirror Support",
                $"[Contact Us] {dto.Subject}",
                emailBody
            );

            if (!success)
            {
                _logger.LogError("Failed to send contact message from {SenderEmail}", dto.Email);
                return BadRequest(ApiResponse<object>.ErrorResponse("An error occurred while sending your message. Please try again later."));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Your message has been sent successfully. We will get back to you shortly!"));
        }
    }
}
