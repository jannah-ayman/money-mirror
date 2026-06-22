using FluentValidation;
using MoneyMirror.Core.DTOs.Child;

namespace MoneyMirror.API.Validators.Child
{
    public class AddExistingChildDtoValidator : AbstractValidator<AddExistingChildDto>
    {
        public AddExistingChildDtoValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("Login code is required")
                .Length(6)
                .WithMessage("Login code must be exactly 6 characters")
                .Matches(@"^[A-Za-z0-9]+$")
                .WithMessage("Login code must contain only letters and numbers");

            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("Please select your relationship to the child");
        }
    }
}