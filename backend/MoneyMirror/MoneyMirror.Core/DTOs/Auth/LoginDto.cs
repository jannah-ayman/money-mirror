using System.ComponentModel.DataAnnotations;

namespace MoneyMirror.Core.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for parent login.
    /// Represents the credentials a parent must provide to authenticate.
    /// Used as input for POST /api/auth/login endpoint.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Parent's registered email address.
        /// Must match an existing parent account.
        /// Example: "john.smith@email.com"
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        /// <summary>
        /// Parent's password in plain text (will be verified against hashed password).
        /// Example: "MyP@ssw0rd!"
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
