namespace MoneyMirror.Core.DTOs.Child
{
    /// <summary>
    /// Data Transfer Object for adding an existing child to a parent's account.
    /// Parent provides the child's login code to link the child to their account.
    /// Used as input for POST /api/children/add-existing endpoint.
    /// Supports shared custody - multiple parents can add the same child.
    /// Validation is handled by AddExistingChildDtoValidator using FluentValidation.
    /// </summary>
    public class AddExistingChildDto
    {
        /// <summary>
        /// The unique 6-character login code of the child to add.
        /// Example: "ABC123"
        /// </summary>
        public string Code { get; set; }
    }
}
