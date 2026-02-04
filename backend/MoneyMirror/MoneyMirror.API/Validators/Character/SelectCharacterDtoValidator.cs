using FluentValidation;
using MoneyMirror.Core.DTOs.Character;

namespace MoneyMirror.API.Validators.Character
{
    public class SelectCharacterDtoValidator : AbstractValidator<SelectCharacterDto>
    {
        public SelectCharacterDtoValidator()
        {
            RuleFor(x => x.CharacterID)
                .GreaterThan(0)
                .WithMessage("Please select a valid character");
        }
    }

    public class GetCharacterStateDtoValidator : AbstractValidator<GetCharacterStateDto>
    {
        public GetCharacterStateDtoValidator()
        {
            RuleFor(x => x.ScreenContext)
                .IsInEnum()
                .WithMessage("Invalid screen context");

            RuleFor(x => x.ContextData)
                .Must(BeValidJsonOrNull)
                .WithMessage("Context data must be valid JSON or null")
                .When(x => !string.IsNullOrEmpty(x.ContextData));
        }

        private bool BeValidJsonOrNull(string? contextData)
        {
            if (string.IsNullOrEmpty(contextData))
                return true;

            try
            {
                System.Text.Json.JsonDocument.Parse(contextData);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}