using System.ComponentModel.DataAnnotations;

namespace MoneyMirror.Core.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for parent registration.
    /// Represents the data a parent must provide when signing up.
    /// Used as input for POST /api/auth/register endpoint.
    /// </summary>
    public class RegisterParentDto
    {
        /// <summary>
        /// Parent's email address - will be used for login.
        /// Must be unique and in valid email format.
        /// Example: "john.smith@email.com"
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string Email { get; set; }

        /// <summary>
        /// Parent's plain text password (will be hashed before storing).
        /// Minimum 8 characters for security.
        /// Should contain at least one uppercase, one lowercase, one digit, one special character.
        /// Example: "MyP@ssw0rd!"
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character"
        )]
        public string Password { get; set; }


        /// <summary>
        /// Password confirmation to ensure user didn't make a typo.
        /// Must match Password field exactly.
        /// </summary>
        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Parent's first name.
        /// Example: "John"
        /// </summary>
        [Required(ErrorMessage = "First name is required")]
        [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string FirstName { get; set; }

        /// <summary>
        /// Parent's last name.
        /// Example: "Smith"
        /// </summary>
        [Required(ErrorMessage = "Last name is required")]
        [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string LastName { get; set; }

        /// <summary>
        /// Parent's phone number (optional).
        /// Can be used for SMS notifications in the future.
        /// Example: "+1-234-567-8900"
        /// </summary>
        [Phone(ErrorMessage = "Invalid phone number format")]
        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? PhoneNumber { get; set; }
    }
}
