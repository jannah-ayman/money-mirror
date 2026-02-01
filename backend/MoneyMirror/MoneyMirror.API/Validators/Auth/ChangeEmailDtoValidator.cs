using FluentValidation;
using MoneyMirror.Core.DTOs.Auth;

namespace MoneyMirror.API.Validators.Auth
{
    /// <summary>
    /// FluentValidation validator for ChangeEmailDto.
    /// Defines validation rules for email change request.
    /// </summary>
    public class ChangeEmailDtoValidator : AbstractValidator<ChangeEmailDto>
    {
        public ChangeEmailDtoValidator()
        {
            // New email validation
            RuleFor(x => x.NewEmail)
                .NotEmpty()
                .WithMessage("New email is required")
                .EmailAddress()
                .WithMessage("Invalid email format")
                .MaximumLength(255)
                .WithMessage("Email cannot exceed 255 characters");

            // Current password validation
            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage("Current password is required for security verification");
        }
    }
}
