using FluentValidation;
using MoneyMirror.Core.DTOs.Expense;

namespace MoneyMirror.API.Validators.Expense
{
    /// <summary>
    /// FluentValidation validator for LogExpenseDto.
    /// Defines validation rules for logging expenses.
    /// ItemName is only required when category is "Other".
    /// </summary>
    public class LogExpenseDtoValidator : AbstractValidator<LogExpenseDto>
    {
        public LogExpenseDtoValidator()
        {
            // Category ID validation - MUST be valid
            RuleFor(x => x.CategoryID)
                .GreaterThan(0)
                .WithMessage("Please select a valid category");

            // Mood ID validation - MUST be valid
            RuleFor(x => x.MoodID)
                .GreaterThan(0)
                .WithMessage("Please select how you felt about this purchase");

            // Amount validation - MUST be positive
            RuleFor(x => x.MoneyAmount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero")
                .LessThanOrEqualTo(10000)
                .WithMessage("Amount cannot exceed 10,000");

            // Note validation (optional) - if provided, has max length
            RuleFor(x => x.Note)
                .MaximumLength(500)
                .WithMessage("Note cannot exceed 500 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Note));

            // Validate LogDate is not in the future if provided
            RuleFor(x => x.LogDate)
             .Must(date => !date.HasValue || date.Value.Date <= DateTime.UtcNow.Date)
               .WithMessage("Log date cannot be in the future.");
        }
    }
}