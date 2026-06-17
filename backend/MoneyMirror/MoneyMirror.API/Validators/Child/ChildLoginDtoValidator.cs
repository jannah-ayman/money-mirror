using FluentValidation;
using MoneyMirror.Core.DTOs.Child;

namespace MoneyMirror.API.Validators.Child
{
    /// <summary>
    /// FluentValidation validator for ChildLoginDto.
    /// Defines validation rules for child login with code.
    /// </summary>
    public class ChildLoginDtoValidator : AbstractValidator<ChildLoginDto>
    {
        public ChildLoginDtoValidator()
        {
            // Code validation
            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("Login code is required")
                .Length(6)
                .WithMessage("Login code must be exactly 6 characters")
                .Matches(@"^[A-Za-z0-9]+$")
                .WithMessage("Login code must contain only letters and numbers");
        }
    }
}
