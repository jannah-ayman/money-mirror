using FluentValidation;
using MoneyMirror.Core.DTOs.Allowance;

namespace MoneyMirror.API.Validators.Allowance
{
    /// <summary>
    /// FluentValidation validator for UpdateAllowanceDto.
    /// Defines validation rules for setting/updating recurring allowance.
    /// Ensures schedule fields are provided based on frequency type.
    /// </summary>
    public class UpdateAllowanceDtoValidator : AbstractValidator<UpdateAllowanceDto>
    {
        public UpdateAllowanceDtoValidator()
        {
            // ==================== Type Validation ====================
            RuleFor(x => x.Type)
                .NotEmpty()
                .WithMessage("Allowance type is required")
                .Must(type => type == "Daily" || type == "Weekly" || type == "Monthly")
                .WithMessage("Allowance type must be 'Daily', 'Weekly', or 'Monthly'");

            // ==================== Amount Validation ====================
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Allowance amount must be greater than zero")
                .LessThanOrEqualTo(10000)
                .WithMessage("Allowance amount cannot exceed 10,000");

            // ==================== Daily Schedule Validation ====================
            // If Type is Daily, DailyHour is required and must be 0-23
            RuleFor(x => x.DailyHour)
                .NotNull()
                .WithMessage("Daily hour is required for daily allowances")
                .InclusiveBetween(0, 23)
                .WithMessage("Daily hour must be between 0 (midnight) and 23 (11 PM)")
                .When(x => x.Type == "Daily");

            // ==================== Weekly Schedule Validation ====================
            // If Type is Weekly, WeeklyDay is required and must be a valid day name
            RuleFor(x => x.WeeklyDay)
                .NotEmpty()
                .WithMessage("Weekly day is required for weekly allowances")
                .Must(day => new[] { "Saturday", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" }.Contains(day))
                .WithMessage("Weekly day must be a valid day of the week (Saturday-Friday)")
                .When(x => x.Type == "Weekly");

            // ==================== Monthly Schedule Validation ====================
            // If Type is Monthly, MonthlyDay is required and must be 1-31
            RuleFor(x => x.MonthlyDay)
                .NotNull()
                .WithMessage("Monthly day is required for monthly allowances")
                .InclusiveBetween(1, 31)
                .WithMessage("Monthly day must be between 1 and 31")
                .When(x => x.Type == "Monthly");
        }
    }

    /// <summary>
    /// FluentValidation validator for GiveBonusDto.
    /// Defines validation rules for giving one-time bonuses.
    /// </summary>
    public class GiveBonusDtoValidator : AbstractValidator<GiveBonusDto>
    {
        public GiveBonusDtoValidator()
        {
            // ==================== Amount Validation ====================
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Bonus amount must be greater than zero")
                .LessThanOrEqualTo(50000)
                .WithMessage("Bonus amount cannot exceed 50,000");

            // ==================== Reason Validation ====================
            RuleFor(x => x.Reason)
                .MaximumLength(500)
                .WithMessage("Reason cannot exceed 500 characters");
        }
    }
}