using FluentValidation;
using MoneyMirror.Core.DTOs.Child;

namespace MoneyMirror.API.Validators.Child
{
    /// <summary>
    /// FluentValidation validator for CompleteInitialProfilingDto.
    /// Validates all 10 questionnaire fields to ensure data integrity.
    /// </summary>
    public class CompleteInitialProfilingDtoValidator : AbstractValidator<CompleteInitialProfilingDto>
    {
        public CompleteInitialProfilingDtoValidator()
        {
            // ==================== Child Identity Validation ====================

            RuleFor(x => x.ChildFirstName)
                .NotEmpty()
                .WithMessage("Child's first name is required")
                .MaximumLength(100)
                .WithMessage("First name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z\s'-]+$")
                .WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes");

            RuleFor(x => x.ChildLastName)
                .NotEmpty()
                .WithMessage("Child's last name is required")
                .MaximumLength(100)
                .WithMessage("Last name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z\s'-]+$")
                .WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes");

            RuleFor(x => x.Gender)
                .Must(gender => string.IsNullOrWhiteSpace(gender) || gender == "Boy" || gender == "Girl")
                .WithMessage("Gender must be 'Boy', 'Girl', or left empty")
                .When(x => !string.IsNullOrWhiteSpace(x.Gender));
            
            // ==================== Question 1: Age Group ====================

            RuleFor(x => x.DOB)
                .NotEmpty()
                .WithMessage("Date of birth is required")
                .LessThan(DateTime.UtcNow)
                .WithMessage("Date of birth must be in the past")
                .Must(BeValidChildAge)
                .WithMessage("Child must be between 6 and 14 years old");

            // ==================== Question 2: Has Allowance ====================

            RuleFor(x => x.HasAllowance)
                .IsInEnum()
                .WithMessage("Please indicate if child receives allowance");

            // ==================== Question 3: Spending Pace ====================

            RuleFor(x => x.SpendingPace)
                .IsInEnum()
                .WithMessage("Invalid spending pace selection");

            // ==================== Question 4: Spending Categories (Multi-select) ====================

            RuleFor(x => x.SpendingCategories)
                .NotEmpty()
                .WithMessage("Please select at least one spending category")
                .Must(categories => categories != null && categories.Count > 0)
                .WithMessage("Please select at least one spending category")
                .Must(categories => categories != null && categories.Count <= 4)
                .WithMessage("Cannot select more than 4 categories")
                .Must(categories => categories != null && categories.All(c => Enum.IsDefined(typeof(MoneyMirror.Core.Enums.SpendingCategory), c)))
                .WithMessage("One or more invalid spending categories selected");

            // ==================== Question 5: Out of Money Behavior ====================

            RuleFor(x => x.OutOfMoneyBehavior)
                .IsInEnum()
                .WithMessage("Invalid out of money behavior selection");

            // ==================== Question 6: Tries to Save ====================

            RuleFor(x => x.TriesToSave)
                .IsInEnum()
                .WithMessage("Please indicate if child tries to save");

            // ==================== Question 7: Money Mindset ====================

            RuleFor(x => x.MoneyMindset)
                .IsInEnum()
                .WithMessage("Invalid money mindset selection");

            // ==================== Question 8: Feeling After Spending ====================

            RuleFor(x => x.FeelingAfterSpending)
                .IsInEnum()
                .WithMessage("Invalid feeling after spending selection");

            // ==================== Question 9: Feeling When Saving Grows ====================

            RuleFor(x => x.FeelingWhenSavingGrows)
                .IsInEnum()
                .WithMessage("Invalid feeling when saving grows selection");

            // ==================== Question 10: Reaction to 100 EGP ====================

            RuleFor(x => x.ReactionTo100)
                .IsInEnum()
                .WithMessage("Invalid reaction to 100 EGP selection");
        }

        /// <summary>
        /// Custom validator to ensure child is between 6 and 14 years old
        /// </summary>
        /// <summary>
        /// Custom validator to ensure child is between 0 and 18 years old
        /// </summary>
        private bool BeValidChildAge(DateTime dob)
        {
            var today = DateTime.UtcNow;
            var age = today.Year - dob.Year;

            if (dob.Date > today.AddYears(-age))
                age--;

            return age >= 0 && age <= 18;
        }
    }
}