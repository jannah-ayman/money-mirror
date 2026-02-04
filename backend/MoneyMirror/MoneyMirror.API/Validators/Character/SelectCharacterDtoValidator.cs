using FluentValidation;
using MoneyMirror.Core.DTOs.Character;
using MoneyMirror.Core.Enums;

namespace MoneyMirror.API.Validators.Character
{
    /// <summary>
    /// FluentValidation validator for SelectCharacterDto.
    /// Ensures valid character type is selected.
    /// </summary>
    public class SelectCharacterDtoValidator : AbstractValidator<SelectCharacterDto>
    {
        public SelectCharacterDtoValidator()
        {
            // Character type validation
            RuleFor(x => x.CharacterType)
                .IsInEnum()
                .WithMessage("Invalid character type. Must be Nova, Luna, Cosmo, or Aura");
        }
    }

    /// <summary>
    /// FluentValidation validator for GetCharacterStateDto.
    /// Ensures valid screen context and optional context data.
    /// </summary>
    public class GetCharacterStateDtoValidator : AbstractValidator<GetCharacterStateDto>
    {
        public GetCharacterStateDtoValidator()
        {
            // Screen context validation
            RuleFor(x => x.ScreenContext)
                .IsInEnum()
                .WithMessage("Invalid screen context");

            // Context data validation (optional, must be valid JSON if provided)
            RuleFor(x => x.ContextData)
                .Must(BeValidJsonOrNull)
                .WithMessage("Context data must be valid JSON or null")
                .When(x => !string.IsNullOrEmpty(x.ContextData));
        }

        /// <summary>
        /// Validates that context data is valid JSON.
        /// </summary>
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