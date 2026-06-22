using FluentValidation;
using MoneyMirror.Core.DTOs.Common;

namespace MoneyMirror.API.Validators.Common
{
   
    /// Validator for ContactUsRequestDto using FluentValidation.
    /// Automatically picked up and executed by FluentValidationFilter.
    
    public class ContactUsRequestDtoValidator : AbstractValidator<ContactUsRequestDto>
    {
        public ContactUsRequestDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required.")
                .MaximumLength(150).WithMessage("Subject cannot exceed 150 characters.");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required.")
                .MaximumLength(2000).WithMessage("Message cannot exceed 2000 characters.");
        }
    }
}
