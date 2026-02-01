using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// Represents a parent/guardian account in the Money Mirror application.
    /// Parents can manage multiple children, set allowances, create challenge goals,
    /// and monitor their children's spending habits.
    public class Parent
    {
        /// Primary key for the Parent entity
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ParentID { get; set; }

        /// Parent's email address - used for login and notifications.
        /// Must be unique across all parent accounts.
        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        /// Hashed password for parent authentication.
        /// Should NEVER store plain text passwords - use ASP.NET Core Identity hashing.
        [Required]
        [MaxLength(255)]
        public string HashedPassword { get; set; }

        /// Parent's phone number (optional) - for SMS notifications if implemented
        [MaxLength(20)]
        public string? PhoneNum { get; set; }

        [Required]
        [MaxLength(100)]
        public string FName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LName { get; set; }

        /// Timestamp when the parent account was created.
        /// Automatically set to current UTC time on creation.
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ==================== AUTHENTICATION FIELDS ====================

        /// Indicates whether the parent's email has been confirmed.
        /// Parents cannot log in until email is verified.
        /// Set to true after clicking email confirmation link.
        /// </summary>
        [Required]
        public bool IsEmailConfirmed { get; set; } = false;

        /// Token sent to parent's email for email confirmation.
        /// Generated as GUID when account is created.
        /// Null after email is confirmed.
        /// </summary>
        [MaxLength(500)]
        public string? EmailConfirmationToken { get; set; }

        /// Expiration timestamp for email confirmation token.
        /// Tokens are valid for 24 hours after generation.
        /// Null after email is confirmed.
        /// </summary>
        public DateTime? EmailConfirmationTokenExpiry { get; set; }

        /// Token sent to parent's email for password reset.
        /// Generated as GUID when password reset is requested.
        /// Null after password is successfully reset or token expires.
        [MaxLength(500)]
        public string? PasswordResetToken { get; set; }

        /// Expiration timestamp for password reset token.
        /// Tokens are valid for 1 hour after generation.
        /// Null when no active reset request exists.
        public DateTime? PasswordResetTokenExpiry { get; set; }

        /// Long-lived refresh token for obtaining new access tokens.
        /// Generated during login, valid for 7 days.
        /// Used to refresh expired access tokens without re-login.
        /// Stored as hashed value for security.
        [MaxLength(500)]
        public string? RefreshToken { get; set; }

        /// Expiration timestamp for refresh token.
        /// Refresh tokens expire after 7 days.
        /// User must log in again after expiration.
        public DateTime? RefreshTokenExpiry { get; set; }
        // Soft delete fields
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        /// Tracks whether the account deletion is permanent (after grace period)
        /// or temporary (within recovery window).
        public bool IsPermanentlyDeleted { get; set; } = false;

        /// Date when the account will be permanently deleted (30 days after DeletedAt).
        /// Null if account is not deleted or already permanently deleted.
        public DateTime? PermanentDeletionDate { get; set; }
        // Email change fields
        public string? NewEmail { get; set; }
        public string? EmailChangeToken { get; set; }
        public DateTime? EmailChangeTokenExpiry { get; set; }
        // ==================== NAVIGATION PROPERTIES ====================

        /// Collection of children linked to this parent account.
        /// Uses ParentChild junction table for many-to-many relationship.
        /// One parent can manage multiple children.
        public virtual ICollection<ParentChild> ParentChildren { get; set; } = new List<ParentChild>();

        /// Collection of notifications sent to this parent.
        /// Includes goal updates, spending alerts, and profile changes.
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        /// Collection of allowances set by this parent for their children.
        /// Includes both recurring allowances and one-time bonuses.
        public virtual ICollection<Allowance> Allowances { get; set; } = new List<Allowance>();

        /// Collection of challenge goals created by this parent for their children.
        /// Parent-created goals typically include rewards (bonus allowance, badges).
        public virtual ICollection<SavingsGoal> ChallengeGoals { get; set; } = new List<SavingsGoal>();
    }
}
