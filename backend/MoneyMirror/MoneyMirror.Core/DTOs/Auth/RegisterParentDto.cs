namespace MoneyMirror.Core.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for parent registration.
    /// Represents the data a parent must provide when signing up.
    /// Used as input for POST /api/auth/register endpoint.
    /// Validation is handled by RegisterParentDtoValidator using FluentValidation.
    /// </summary>
    public class RegisterParentDto
    {
        /// <summary>
        /// Parent's email address - will be used for login.
        /// Must be unique and in valid email format.
        /// Example: "john.smith@email.com"
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Parent's plain text password (will be hashed before storing).
        /// Minimum 8 characters for security.
        /// Should contain at least one uppercase, one lowercase, one digit, one special character.
        /// Example: "MyP@ssw0rd!"
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Password confirmation to ensure user didn't make a typo.
        /// Must match Password field exactly.
        /// </summary>
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Parent's first name.
        /// Example: "John"
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Parent's last name.
        /// Example: "Smith"
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Parent's phone number (optional).
        /// Can be used for SMS notifications in the future.
        /// Example: "+1-234-567-8900"
        /// </summary>
        public string? PhoneNumber { get; set; }
    }
}