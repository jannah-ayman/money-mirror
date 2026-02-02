using FluentValidation;

namespace MoneyMirror.API.Validators.Child
{
    /// <summary>
    /// FluentValidation validator for CompleteInitialProfilingDto.
    /// Validates all questionnaire fields to ensure data integrity.
    /// </summary>
    public class CompleteInitialProfilingDtoValidator : AbstractValidator<CompleteInitialProfilingDto>
    {
        public CompleteInitialProfilingDtoValidator()
        {
            RuleFor(x => x.ChildFirstName)
    .NotEmpty()
    .MaximumLength(100);

            RuleFor(x => x.ChildLastName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.DOB)
                .LessThan(DateTime.UtcNow)
                .WithMessage("DOB must be in the past");

            // Allowance amount validation
            RuleFor(x => x.AllowanceAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Allowance amount must be zero or positive")
                .LessThanOrEqualTo(10000)
                .WithMessage("Allowance amount seems unreasonably high");

            // Enum validations (ensure they're defined values)
            RuleFor(x => x.ChildAgeGroup)
                .IsInEnum()
                .WithMessage("Invalid age group");

            RuleFor(x => x.ChildGender)
                .IsInEnum()
                .WithMessage("Invalid gender");

            RuleFor(x => x.AllowanceFrequency)
                .IsInEnum()
                .WithMessage("Invalid allowance frequency");

            RuleFor(x => x.PrimarySpendingCategory)
                .IsInEnum()
                .WithMessage("Invalid spending category");

            RuleFor(x => x.SpendingPlanning)
                .IsInEnum()
                .WithMessage("Invalid spending planning value");

            RuleFor(x => x.OutOfMoneyBehavior)
                .IsInEnum()
                .WithMessage("Invalid out of money behavior");

            RuleFor(x => x.SpendingAffectsSaving)
                .IsInEnum()
                .WithMessage("Invalid spending affects saving value");

            RuleFor(x => x.SpendingPace)
                .IsInEnum()
                .WithMessage("Invalid spending pace");

            RuleFor(x => x.SavingGoal)
                .IsInEnum()
                .WithMessage("Invalid saving goal");

            RuleFor(x => x.SavingPercentage)
                .IsInEnum()
                .WithMessage("Invalid saving percentage");

            RuleFor(x => x.SavingSuccessRate)
                .IsInEnum()
                .WithMessage("Invalid saving success rate");

            RuleFor(x => x.FeelingAfterSpending)
                .IsInEnum()
                .WithMessage("Invalid feeling after spending");

            RuleFor(x => x.SavingFailureReason)
                .IsInEnum()
                .WithMessage("Invalid saving failure reason");

            RuleFor(x => x.SatisfactionPreference)
                .IsInEnum()
                .WithMessage("Invalid satisfaction preference");

            RuleFor(x => x.TalksAboutMoney)
                .IsInEnum()
                .WithMessage("Invalid talks about money value");

            RuleFor(x => x.FeelingWhenSavingGrows)
                .IsInEnum()
                .WithMessage("Invalid feeling when saving grows");

            RuleFor(x => x.ReactionTo100)
                .IsInEnum()
                .WithMessage("Invalid reaction to $100");

            RuleFor(x => x.MoneyPriority)
                .IsInEnum()
                .WithMessage("Invalid money priority");

            RuleFor(x => x.ReactionToExpensiveItem)
                .IsInEnum()
                .WithMessage("Invalid reaction to expensive item");

            RuleFor(x => x.ReactionToMoreAllowance)
                .IsInEnum()
                .WithMessage("Invalid reaction to more allowance");

            RuleFor(x => x.MoneyMindset)
                .IsInEnum()
                .WithMessage("Invalid money mindset");
        }
    }
}
