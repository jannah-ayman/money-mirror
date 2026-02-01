namespace MoneyMirror.Core.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for parent login.
    /// Represents the credentials a parent must provide to authenticate.
    /// Used as input for POST /api/auth/login endpoint.
    /// Validation is handled by LoginDtoValidator using FluentValidation.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Parent's registered email address.
        /// Must match an existing parent account.
        /// Example: "john.smith@email.com"
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Parent's password in plain text (will be verified against hashed password).
        /// Example: "MyP@ssw0rd!"
        /// </summary>
        public string Password { get; set; }
    }
}