using FluentValidation;
using MoneyMirror.Core.DTOs.Auth;

namespace MoneyMirror.API.Validators.Auth
{
    /// <summary>
    /// FluentValidation validator for LoginDto.
    /// Defines validation rules for parent login.
    /// </summary>
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            // Email validation
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format");

            // Password validation
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required");
        }
    }
}