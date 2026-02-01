using FluentValidation;
using MoneyMirror.Core.DTOs.Auth;

namespace MoneyMirror.API.Validators.Auth
{
    /// <summary>
    /// FluentValidation validator for ResendConfirmationDto.
    /// Defines validation rules for resending email confirmation.
    /// </summary>
    public class ResendConfirmationDtoValidator : AbstractValidator<ResendConfirmationDto>
    {
        public ResendConfirmationDtoValidator()
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