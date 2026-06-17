using FluentValidation;
using MoneyMirror.Core.DTOs.Goals;

namespace MoneyMirror.API.Validators.Goals
{
    public class CreatePersonalGoalDtoValidator : AbstractValidator<CreatePersonalGoalDto>
    {
        public CreatePersonalGoalDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Goal title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.TargetAmount)
                .GreaterThan(0).WithMessage("Target amount must be greater than zero.")
                .LessThanOrEqualTo(100000).WithMessage("Target amount cannot exceed 100,000.");

            RuleFor(x => x.EndDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("End date must be in the future.")
                .When(x => x.EndDate.HasValue);
        }
    }

    public class CreateChallengeDtoValidator : AbstractValidator<CreateChallengeDto>
    {
        public CreateChallengeDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Challenge title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.TargetAmount)
                .GreaterThan(0).WithMessage("Target amount must be greater than zero.")
                .LessThanOrEqualTo(100000).WithMessage("Target amount cannot exceed 100,000.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required for challenges.")
                .GreaterThan(DateTime.UtcNow).WithMessage("End date must be in the future.");

            RuleFor(x => x.RewardValue)
                .GreaterThan(0).WithMessage("Reward must be greater than zero.")
                .LessThanOrEqualTo(50000).WithMessage("Reward cannot exceed 50,000.")
                .When(x => x.RewardValue.HasValue && x.RewardValue.Value > 0);
        }
    }
    public class AddMoneyToGoalDtoValidator : AbstractValidator<AddMoneyToGoalDto>
    {
        public AddMoneyToGoalDtoValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.")
                .LessThanOrEqualTo(100000).WithMessage("Amount cannot exceed 100,000.");
        }
    }
    public class EditPersonalGoalDtoValidator : AbstractValidator<EditPersonalGoalDto>
    {
        public EditPersonalGoalDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Goal title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");
        }
    }

    public class EditChallengeDtoValidator : AbstractValidator<EditChallengeDto>
    {
        public EditChallengeDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Challenge title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.")
                .GreaterThan(DateTime.UtcNow).WithMessage("End date must be in the future.");

            RuleFor(x => x.TargetAmount)
                .GreaterThan(0).WithMessage("Target amount must be greater than zero.")
                .LessThanOrEqualTo(100000).WithMessage("Target amount cannot exceed 100,000.")
                .When(x => x.TargetAmount.HasValue && x.TargetAmount.Value > 0);

            RuleFor(x => x.RewardValue)
                .GreaterThan(0).WithMessage("Reward must be greater than zero.")
                .LessThanOrEqualTo(50000).WithMessage("Reward cannot exceed 50,000.")
                .When(x => x.RewardValue.HasValue && x.RewardValue.Value > 0);
        }
    }
}
