using FluentValidation;
using MoneyMirror.Core.DTOs.Child;

namespace MoneyMirror.API.Validators.Child
{
    /// <summary>
    /// FluentValidation validator for AddExistingChildDto.
    /// Defines validation rules for adding existing child by code.
    /// </summary>
    public class AddExistingChildDtoValidator : AbstractValidator<AddExistingChildDto>
    {
        public AddExistingChildDtoValidator()
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
