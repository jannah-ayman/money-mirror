using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMirror.Core.DTOs.Auth
{
    /// <summary>
    /// DTO for setting new password after code verification.
    /// Used as input for POST /api/auth/reset-password-with-code
    /// </summary>
    public class ResetPasswordWithCodeDto
    {
        /// <summary>
        /// Email address.
        /// Example: "john@email.com"
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 6-digit reset code (already verified).
        /// Example: "927351"
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// New password.
        /// Example: "NewP@ssw0rd!"
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Confirmation of new password.
        /// Must match NewPassword exactly.
        /// </summary>
        public string ConfirmNewPassword { get; set; }
    }
}
