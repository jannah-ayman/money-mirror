using FluentValidation;
using MoneyMirror.Core.DTOs.Child;

namespace MoneyMirror.API.Validators.Child
{
    /// <summary>
    /// FluentValidation validator for ChildRefreshTokenDto.
    /// Defines validation rules for child token refresh.
    /// </summary>
    public class ChildRefreshTokenDtoValidator : AbstractValidator<ChildRefreshTokenDto>
    {
        public ChildRefreshTokenDtoValidator()
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
