using FluentValidation;
using MoneyMirror.Core.DTOs.Auth;

namespace MoneyMirror.API.Validators.Auth
{
    /// <summary>
    /// FluentValidation validator for RefreshTokenDto.
    /// Defines validation rules for token refresh.
    /// </summary>
    public class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
    {
        public RefreshTokenDtoValidator()
        {
            // Access token validation
            RuleFor(x => x.AccessToken)
                .NotEmpty()
                .WithMessage("Access token is required");

            // Refresh token validation
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token is required");
        }
    }
}