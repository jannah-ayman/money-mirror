using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMirror.Core.DTOs.Auth
{
    /// <summary>
    /// DTO for resending confirmation code.
    /// Used when user didn't receive original code or it expired.
    /// Used as input for POST /api/auth/resend-confirmation-code
    /// </summary>
    public class ResendConfirmationCodeDto
    {
        /// <summary>
        /// Email address to resend code to.
        /// Example: "john@email.com"
        /// </summary>
        public string Email { get; set; }
    }
}
