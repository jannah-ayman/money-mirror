using FluentValidation;
using MoneyMirror.Core.DTOs.Auth;

namespace MoneyMirror.API.Validators.Auth
{
    /// <summary>
    /// FluentValidation validator for UpdateParentProfileDto.
    /// Defines validation rules for updating parent profile.
    /// </summary>
    public class UpdateParentProfileDtoValidator : AbstractValidator<UpdateParentProfileDto>
    {
        public UpdateParentProfileDtoValidator()
        {
            // First name validation
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required")
                .MaximumLength(100)
                .WithMessage("First name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z\s'-]+$")
                .WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes");

            // Last name validation
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required")
                .MaximumLength(100)
                .WithMessage("Last name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z\s'-]+$")
                .WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes");

            // Phone number validation (optional)
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20)
                .WithMessage("Phone number cannot exceed 20 characters")
                .Matches(@"^[\d\s\-\+\(\)]+$")
                .WithMessage("Phone number can only contain digits, spaces, hyphens, plus signs, and parentheses")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber)); // Only validate if provided
        }
    }
}
