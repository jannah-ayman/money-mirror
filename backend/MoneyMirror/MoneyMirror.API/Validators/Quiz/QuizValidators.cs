using FluentValidation;
using MoneyMirror.Core.DTOs.Quiz;

namespace MoneyMirror.API.Validators.Quiz
{
    public class SubmitQuizAnswerDtoValidator : AbstractValidator<SubmitQuizAnswerDto>
    {
        public SubmitQuizAnswerDtoValidator()
        {
            RuleFor(x => x.AnswerID)
                .GreaterThan(0)
                .WithMessage("Please select an answer");
        }
    }
}