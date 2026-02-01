using FluentValidation;
using MoneyMirror.Core.DTOs.Auth;

namespace MoneyMirror.API.Validators.Auth
{
    /// <summary>
    /// FluentValidation validator for ResetPasswordDto.
    /// Defines validation rules for password reset.
    /// </summary>
    public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordDtoValidator()
        {
            // Email validation
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format");

            // Token validation
            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("Reset token is required");

            // New password validation
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("New password is required")
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

            // Confirm new password validation
            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty()
                .WithMessage("Password confirmation is required")
                .Equal(x => x.NewPassword)
                .WithMessage("Passwords do not match");
        }
    }
}