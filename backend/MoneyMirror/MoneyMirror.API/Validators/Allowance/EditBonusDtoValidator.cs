using FluentValidation;

using MoneyMirror.Core.DTOs.Allowance;

/// <summary>
/// FluentValidation validator for EditBonusDto.
/// </summary>
public class EditBonusDtoValidator : AbstractValidator<EditBonusDto>
{
    public EditBonusDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Bonus amount must be greater than zero")
            .LessThanOrEqualTo(50000)
            .WithMessage("Bonus amount cannot exceed 50,000");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters");
    }
}
