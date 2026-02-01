namespace MoneyMirror.Core.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for updating parent profile information.
    /// Allows parents to update their personal details (name, phone).
    /// Email updates handled separately via ChangeEmailDto (requires re-verification).
    /// Password updates handled via existing ForgotPassword/ResetPassword flow.
    /// Used as input for PUT /api/auth/profile endpoint.
    /// Validation is handled by UpdateParentProfileDtoValidator using FluentValidation.
    /// </summary>
    public class UpdateParentProfileDto
    {
        /// <summary>
        /// Parent's updated first name.
        /// Example: "John"
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Parent's updated last name.
        /// Example: "Smith"
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Parent's updated phone number (optional).
        /// Can be set to null to remove phone number.
        /// Example: "+1-234-567-8900"
        /// </summary>
        public string? PhoneNumber { get; set; }
    }
}