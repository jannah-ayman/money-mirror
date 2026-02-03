using FluentValidation;
using MoneyMirror.Core.DTOs.Expense;

namespace MoneyMirror.API.Validators.Expense
{
    /// <summary>
    /// FluentValidation validator for LogExpenseDto.
    /// Defines validation rules for logging expenses.
    /// </summary>
    public class LogExpenseDtoValidator : AbstractValidator<LogExpenseDto>
    {
        public LogExpenseDtoValidator()
        {
            // Item name validation
            RuleFor(x => x.ItemName)
                .NotEmpty()
                .WithMessage("Item name is required")
                .MaximumLength(200)
                .WithMessage("Item name cannot exceed 200 characters")
                .MinimumLength(2)
                .WithMessage("Item name must be at least 2 characters");

            // Category ID validation
            RuleFor(x => x.CategoryID)
                .GreaterThan(0)
                .WithMessage("Please select a valid category");

            // Mood ID validation
            RuleFor(x => x.MoodID)
                .GreaterThan(0)
                .WithMessage("Please select how you felt about this purchase");

            // Amount validation
            RuleFor(x => x.MoneyAmount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero")
                .LessThanOrEqualTo(10000)
                .WithMessage("Amount cannot exceed 10,000");

            // Note validation (optional)
            RuleFor(x => x.Note)
                .MaximumLength(500)
                .WithMessage("Note cannot exceed 500 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Note));
        }
    }
}