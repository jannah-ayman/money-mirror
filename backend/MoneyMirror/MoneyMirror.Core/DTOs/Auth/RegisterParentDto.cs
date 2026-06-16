namespace MoneyMirror.Core.DTOs.Auth
{
    /// Used as input for POST /api/auth/register endpoint.
    /// Validation is handled by RegisterParentDtoValidator using FluentValidation.
    public class RegisterParentDto
    {
        public string Email { get; set; }

        /// Should contain at least one uppercase, one lowercase, one digit, one special character.
        /// Example: "MyP@ssw0rd!"
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}