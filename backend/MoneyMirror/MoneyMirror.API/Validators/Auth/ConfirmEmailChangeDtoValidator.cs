using FluentValidation;
using MoneyMirror.Core.DTOs.Auth;

namespace MoneyMirror.API.Validators.Auth
{
    /// <summary>
    /// FluentValidation validator for ConfirmEmailChangeDto.
    /// Defines validation rules for confirming email change.
    /// </summary>
    public class ConfirmEmailChangeDtoValidator : AbstractValidator<ConfirmEmailChangeDto>
    {
        public ConfirmEmailChangeDtoValidator()
        {
            // Old email validation
            RuleFor(x => x.OldEmail)
                .NotEmpty()
                .WithMessage("Old email is required")
                .EmailAddress()
                .WithMessage("Invalid old email format");

            // New email validation
            RuleFor(x => x.NewEmail)
                .NotEmpty()
                .WithMessage("New email is required")
                .EmailAddress()
                .WithMessage("Invalid new email format");

            // Token validation
            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("Confirmation token is required");
        }
    }
}
