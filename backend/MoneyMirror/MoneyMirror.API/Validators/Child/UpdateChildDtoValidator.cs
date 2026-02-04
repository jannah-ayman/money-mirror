using FluentValidation;
using MoneyMirror.Core.DTOs.Child;

namespace MoneyMirror.API.Validators.Child
{
    /// <summary>
    /// FluentValidation validator for UpdateChildDto.
    /// Validates child's updated basic information.
    /// </summary>
    public class UpdateChildDtoValidator : AbstractValidator<UpdateChildDto>
    {
        public UpdateChildDtoValidator()
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

            // Date of birth validation
            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .WithMessage("Date of birth is required")
                .LessThan(DateTime.UtcNow)
                .WithMessage("Date of birth must be in the past")
                .Must(BeValidChildAge)
                .WithMessage("Child must be between 0 and 18 years old");
        }

        /// <summary>
        /// Validates that the date of birth results in an age between 0 and 18
        /// </summary>
        private bool BeValidChildAge(DateTime dob)
        {
            var today = DateTime.UtcNow;
            var age = today.Year - dob.Year;

            if (dob.Date > today.AddYears(-age))
                age--;

            return age >= 0 && age <= 18;
        }
    }
}