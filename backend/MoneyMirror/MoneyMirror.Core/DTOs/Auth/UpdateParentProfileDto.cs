namespace MoneyMirror.Core.DTOs.Auth
{

    /// Email updates handled separately via ChangeEmailDto (requires re-verification).
    /// Password updates handled via existing ForgotPassword/ResetPassword flow.
    /// Used as input for PUT /api/auth/profile endpoint.
    /// Validation is handled by UpdateParentProfileDtoValidator using FluentValidation.
    public class UpdateParentProfileDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}