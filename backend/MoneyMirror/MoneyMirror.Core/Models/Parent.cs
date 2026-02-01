using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Represents a parent/guardian account in the Money Mirror application.
    /// Parents can manage multiple children, set allowances, create challenge goals,
    /// and monitor their children's spending habits.
    /// </summary>
    public class Parent
    {
        /// <summary>
        /// Primary key for the Parent entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ParentID { get; set; }

        /// <summary>
        /// Parent's email address - used for login and notifications.
        /// Must be unique across all parent accounts.
        /// </summary>
        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Hashed password for parent authentication.
        /// Should NEVER store plain text passwords - use ASP.NET Core Identity hashing.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string HashedPassword { get; set; }

        /// <summary>
        /// Parent's phone number (optional) - for SMS notifications if implemented
        /// </summary>
        [MaxLength(20)]
        public string? PhoneNum { get; set; }

        /// <summary>
        /// Parent's first name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FName { get; set; }

        /// <summary>
        /// Parent's last name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string LName { get; set; }

        /// <summary>
        /// Timestamp when the parent account was created.
        /// Automatically set to current UTC time on creation.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ==================== AUTHENTICATION FIELDS ====================

        /// <summary>
        /// Indicates whether the parent's email has been confirmed.
        /// Parents cannot log in until email is verified.
        /// Set to true after clicking email confirmation link.
        /// </summary>
        [Required]
        public bool IsEmailConfirmed { get; set; } = false;

        /// <summary>
        /// Token sent to parent's email for email confirmation.
        /// Generated as GUID when account is created.
        /// Null after email is confirmed.
        /// </summary>
        [MaxLength(500)]
        public string? EmailConfirmationToken { get; set; }

        /// <summary>
        /// Expiration timestamp for email confirmation token.
        /// Tokens are valid for 24 hours after generation.
        /// Null after email is confirmed.
        /// </summary>
        public DateTime? EmailConfirmationTokenExpiry { get; set; }

        /// <summary>
        /// Token sent to parent's email for password reset.
        /// Generated as GUID when password reset is requested.
        /// Null after password is successfully reset or token expires.
        /// </summary>
        [MaxLength(500)]
        public string? PasswordResetToken { get; set; }

        /// <summary>
        /// Expiration timestamp for password reset token.
        /// Tokens are valid for 1 hour after generation.
        /// Null when no active reset request exists.
        /// </summary>
        public DateTime? PasswordResetTokenExpiry { get; set; }

        /// <summary>
        /// Long-lived refresh token for obtaining new access tokens.
        /// Generated during login, valid for 7 days.
        /// Used to refresh expired access tokens without re-login.
        /// Stored as hashed value for security.
        /// </summary>
        [MaxLength(500)]
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Expiration timestamp for refresh token.
        /// Refresh tokens expire after 7 days.
        /// User must log in again after expiration.
        /// </summary>
        public DateTime? RefreshTokenExpiry { get; set; }
        // Soft delete fields
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // Email change fields
        public string? NewEmail { get; set; }
        public string? EmailChangeToken { get; set; }
        public DateTime? EmailChangeTokenExpiry { get; set; }
        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Collection of children linked to this parent account.
        /// Uses ParentChild junction table for many-to-many relationship.
        /// One parent can manage multiple children.
        /// </summary>
        public virtual ICollection<ParentChild> ParentChildren { get; set; } = new List<ParentChild>();

        /// <summary>
        /// Collection of notifications sent to this parent.
        /// Includes goal updates, spending alerts, and profile changes.
        /// </summary>
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        /// <summary>
        /// Collection of allowances set by this parent for their children.
        /// Includes both recurring allowances and one-time bonuses.
        /// </summary>
        public virtual ICollection<Allowance> Allowances { get; set; } = new List<Allowance>();

        /// <summary>
        /// Collection of challenge goals created by this parent for their children.
        /// Parent-created goals typically include rewards (bonus allowance, badges).
        /// </summary>
        public virtual ICollection<SavingsGoal> ChallengeGoals { get; set; } = new List<SavingsGoal>();
    }
}
