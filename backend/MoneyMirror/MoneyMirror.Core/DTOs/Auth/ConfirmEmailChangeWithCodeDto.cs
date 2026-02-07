using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMirror.Core.DTOs.Auth
{
    /// <summary>
    /// DTO for confirming email change with code.
    /// User enters the code sent to their NEW email address.
    /// Used as input for POST /api/auth/confirm-email-change-with-code
    /// </summary>
    public class ConfirmEmailChangeWithCodeDto
    {
        /// <summary>
        /// Current (old) email address.
        /// Example: "oldemail@example.com"
        /// </summary>
        public string OldEmail { get; set; }

        /// <summary>
        /// New email address (being confirmed).
        /// Example: "newemail@example.com"
        /// </summary>
        public string NewEmail { get; set; }

        /// <summary>
        /// 6-digit confirmation code from email sent to new address.
        /// Example: "614728"
        /// </summary>
        public string Code { get; set; }
    }
}
