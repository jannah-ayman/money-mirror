using FluentValidation;
using MoneyMirror.Core.DTOs.Expense;

namespace MoneyMirror.API.Validators.Expense
{
    public class UpdateExpenseDtoValidator : AbstractValidator<UpdateExpenseDto>
    {
        public UpdateExpenseDtoValidator()
        {
            RuleFor(x => x.CategoryID)
                .GreaterThan(0)
                .WithMessage("Please select a valid category");

            RuleFor(x => x.MoodID)
                .GreaterThan(0)
                .WithMessage("Please select how you felt about this purchase");

            RuleFor(x => x.MoneyAmount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero")
                .LessThanOrEqualTo(10000)
                .WithMessage("Amount cannot exceed 10,000");

            RuleFor(x => x.Note)
                .MaximumLength(500)
                .WithMessage("Note cannot exceed 500 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Note));
        }
    }
}
