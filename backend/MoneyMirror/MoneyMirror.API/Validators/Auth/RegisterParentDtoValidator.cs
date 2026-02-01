using FluentValidation;
using MoneyMirror.Core.DTOs.Auth;

namespace MoneyMirror.API.Validators.Auth
{
    /// <summary>
    /// FluentValidation validator for RegisterParentDto.
    /// Defines all validation rules for parent registration.
    /// Automatically executed by FluentValidation middleware.
    /// </summary>
    public class RegisterParentDtoValidator : AbstractValidator<RegisterParentDto>
    {
        public RegisterParentDtoValidator()
        {
            // Email validation
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format")
                .MaximumLength(255)
                .WithMessage("Email cannot exceed 255 characters");

            // Password validation
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters")
                .Matches(@"[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]")
                .WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"[0-9]")
                .WithMessage("Password must contain at least one digit")
                .Matches(@"[@$!%*?&#]")
                .WithMessage("Password must contain at least one special character (@$!%*?&#)");

            // Confirm password validation
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Password confirmation is required")
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match");

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