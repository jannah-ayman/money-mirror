using FluentValidation;
using MoneyMirror.Core.DTOs.Auth;

namespace MoneyMirror.API.Validators.Auth
{
    // ==================== EMAIL CONFIRMATION ====================

    public class ConfirmEmailWithCodeDtoValidator : AbstractValidator<ConfirmEmailWithCodeDto>
    {
        public ConfirmEmailWithCodeDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format");

            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("Confirmation code is required")
                .Length(6)
                .WithMessage("Confirmation code must be exactly 6 digits")
                .Matches(@"^\d{6}$")
                .WithMessage("Confirmation code must contain only numbers");
        }
    }

    // ==================== PASSWORD RESET ====================

    public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
    {
        public ForgotPasswordDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format");
        }
    }

    public class VerifyResetCodeDtoValidator : AbstractValidator<VerifyResetCodeDto>
    {
        public VerifyResetCodeDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format");

            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("Reset code is required")
                .Length(6)
                .WithMessage("Reset code must be exactly 6 digits")
                .Matches(@"^\d{6}$")
                .WithMessage("Reset code must contain only numbers");
        }
    }

    public class ResetPasswordWithCodeDtoValidator : AbstractValidator<ResetPasswordWithCodeDto>
    {
        public ResetPasswordWithCodeDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format");

            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("Reset code is required")
                .Length(6)
                .WithMessage("Reset code must be exactly 6 digits")
                .Matches(@"^\d{6}$")
                .WithMessage("Reset code must contain only numbers");

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

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty()
                .WithMessage("Password confirmation is required")
                .Equal(x => x.NewPassword)
                .WithMessage("Passwords do not match");
        }
    }

    // ==================== EMAIL CHANGE ====================

    public class RequestEmailChangeDtoValidator : AbstractValidator<RequestEmailChangeDto>
    {
        public RequestEmailChangeDtoValidator()
        {
            RuleFor(x => x.NewEmail)
                .NotEmpty()
                .WithMessage("New email is required")
                .EmailAddress()
                .WithMessage("Invalid email format")
                .MaximumLength(255)
                .WithMessage("Email cannot exceed 255 characters");

            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage("Current password is required for security verification");
        }
    }

    public class ConfirmEmailChangeWithCodeDtoValidator : AbstractValidator<ConfirmEmailChangeWithCodeDto>
    {
        public ConfirmEmailChangeWithCodeDtoValidator()
        {
            RuleFor(x => x.OldEmail)
                .NotEmpty()
                .WithMessage("Old email is required")
                .EmailAddress()
                .WithMessage("Invalid old email format");

            RuleFor(x => x.NewEmail)
                .NotEmpty()
                .WithMessage("New email is required")
                .EmailAddress()
                .WithMessage("Invalid new email format");

            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("Confirmation code is required")
                .Length(6)
                .WithMessage("Confirmation code must be exactly 6 digits")
                .Matches(@"^\d{6}$")
                .WithMessage("Confirmation code must contain only numbers");
        }
    }

    // ==================== RESEND CODE ====================

    public class ResendConfirmationCodeDtoValidator : AbstractValidator<ResendConfirmationCodeDto>
    {
        public ResendConfirmationCodeDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format");
        }
    }
}