namespace MoneyMirror.Core.DTOs.Child
{
    /// <summary>
    /// Data Transfer Object for child login using their unique code.
    /// Children login with a simple 6-character code instead of email/password.
    /// Used as input for POST /api/children/login-with-code endpoint.
    /// Validation is handled by ChildLoginDtoValidator using FluentValidation.
    /// </summary>
    public class ChildLoginDto
    {
        /// <summary>
        /// The unique 6-character login code generated for the child.
        /// Example: "ABC123"
        /// </summary>
        public string Code { get; set; }
    }
}
