using FluentValidation;
using MoneyMirror.Core.DTOs.Auth;

namespace MoneyMirror.API.Validators.Auth
{
    /// <summary>
    /// FluentValidation validator for ForgotPasswordDto.
    /// Defines validation rules for forgot password request.
    /// </summary>
    public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
    {
        public ForgotPasswordDtoValidator()
        {
            // Email validation
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format");
        }
    }
}