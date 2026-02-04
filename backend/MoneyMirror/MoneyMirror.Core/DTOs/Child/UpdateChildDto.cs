namespace MoneyMirror.Core.DTOs.Child
{
    /// <summary>
    /// Data Transfer Object for updating a child's basic information.
    /// Parent can edit first name, last name, and date of birth.
    /// Used as input for PUT /api/children/{childId} endpoint.
    /// Validation is handled by UpdateChildDtoValidator using FluentValidation.
    /// </summary>
    public class UpdateChildDto
    {
        /// <summary>
        /// Child's first name.
        /// Example: "Emma"
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Child's last name.
        /// Example: "Smith"
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Child's date of birth.
        /// Age and age group will be recalculated automatically.
        /// Example: "2015-03-15"
        /// </summary>
        public DateTime DateOfBirth { get; set; }
    }

    /// <summary>
    /// Response after updating child details.
    /// Returns the updated child information.
    /// </summary>
    public class UpdateChildResponseDto
    {
        public int ChildID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string AgeGroup { get; set; }
    }

    /// <summary>
    /// Response after regenerating a child's login code.
    /// Returns the new code.
    /// </summary>
    public class RegenerateCodeResponseDto
    {
        /// <summary>
        /// The new 6-character login code.
        /// Example: "ABC123"
        /// </summary>
        public string NewLoginCode { get; set; }

        /// <summary>
        /// Child's name for confirmation.
        /// Example: "Emma Smith"
        /// </summary>
        public string ChildName { get; set; }
    }
}