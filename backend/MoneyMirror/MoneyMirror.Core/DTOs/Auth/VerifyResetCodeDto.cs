using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMirror.Core.DTOs.Auth
{
    /// <summary>
    /// DTO for verifying the password reset code.
    /// User enters the code they received in email.
    /// Used as input for POST /api/auth/verify-reset-code
    /// </summary>
    public class VerifyResetCodeDto
    {
        /// <summary>
        /// Email address.
        /// Example: "john@email.com"
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 6-digit reset code from email.
        /// Example: "927351"
        /// </summary>
        public string Code { get; set; }
    }
}
