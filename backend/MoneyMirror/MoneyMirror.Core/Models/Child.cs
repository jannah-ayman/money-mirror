using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Represents a child user account in the Money Mirror application.
    /// Children use a simple login code system instead of email/password.
    /// Track their expenses, goals, allowances, and financial personality.
    /// </summary>
    public class Child
    {
        /// <summary>
        /// Primary key for the Child entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChildID { get; set; }

        /// <summary>
        /// Unique 6-character login code (e.g., "ABC123").
        /// Generated automatically when child account is created.
        /// Used for authentication instead of email/password.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string LoginCode { get; set; }

        /// <summary>
        /// Child's first name.
        /// Example: "Emma"
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FName { get; set; }

        /// <summary>
        /// Child's last name.
        /// Example: "Smith"
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string LName { get; set; }

        /// <summary>
        /// Date of birth.
        /// Used to calculate age and determine age-appropriate content.
        /// </summary>
        [Required]
        public DateTime DOB { get; set; }

        /// <summary>
        /// Current age calculated from DOB.
        /// Updated daily by background job.
        /// </summary>
        [Required]
        public int Age { get; set; }

        /// <summary>
        /// Gender (optional).
        /// Values: "Boy", "Girl", or null
        /// </summary>
        [MaxLength(10)]
        public string? Gender { get; set; }

        /// <summary>
        /// Current balance in the child's account.
        /// Increases with allowances/bonuses, decreases with expenses.
        /// Stored as DECIMAL(10,2) for precise currency calculations.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal CurrentBalance { get; set; } = 0.00m;

        /// <summary>
        /// Indicates whether the personality profile has been finalized by AI analysis.
        /// False = using temporary "Pending Analysis" personality type
        /// True = real AI analysis has been completed and personality type is final
        /// </summary>
        [Required]
        public bool IsPersonalityFinalized { get; set; } = false;

        /// <summary>
        /// Timestamp when the child account was created.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ==================== FOREIGN KEYS ====================

        /// <summary>
        /// Foreign key to PersonalityType table.
        /// The child's financial personality classification.
        /// Null if not yet determined.
        /// </summary>
        public int? TypeID { get; set; }

        /// <summary>
        /// Foreign key to Character table.
        /// The character selected by this child.
        /// Null if child hasn't selected a character yet.
        /// </summary>
        public int? CharacterID { get; set; }

        // ==================== AUTHENTICATION FIELDS ====================

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
        /// Child must log in again after expiration.
        /// </summary>
        public DateTime? RefreshTokenExpiry { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Reference to the child's personality type.
        /// Contains parent-facing and child-facing names, traits, and recommendations.
        /// </summary>
        [ForeignKey("TypeID")]
        public virtual PersonalityType? PersonalityType { get; set; }

        /// <summary>
        /// Reference to the selected character.
        /// Contains character display name, description, and image paths.
        /// Null if child hasn't selected a character yet.
        /// </summary>
        [ForeignKey("CharacterID")]
        public virtual Character? Character { get; set; }

        /// <summary>
        /// Collection of parent-child relationships.
        /// Uses ParentChild junction table for many-to-many relationship.
        /// One child can be managed by multiple parents (e.g., divorced parents).
        /// </summary>
        public virtual ICollection<ParentChild> ParentChildren { get; set; } = new List<ParentChild>();

        /// <summary>
        /// Collection of notifications sent to this child.
        /// Includes reminders to log expenses, goal progress updates, and encouragement.
        /// </summary>
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        /// <summary>
        /// Collection of allowances given to this child.
        /// Includes both recurring allowances and one-time bonuses.
        /// </summary>
        public virtual ICollection<Allowance> Allowances { get; set; } = new List<Allowance>();

        /// <summary>
        /// Collection of savings goals created by or assigned to this child.
        /// Includes both child-created goals and parent challenge goals.
        /// </summary>
        public virtual ICollection<SavingsGoal> SavingsGoals { get; set; } = new List<SavingsGoal>();

        /// <summary>
        /// Collection of expenses logged by this child.
        /// Each expense includes amount, category, mood, and optional notes.
        /// </summary>
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

        /// <summary>
        /// Collection of quiz responses from interactive story quizzes.
        /// Used to refine personality profiling over time.
        /// </summary>
        public virtual ICollection<QuizLog> QuizLogs { get; set; } = new List<QuizLog>();

        /// <summary>
        /// Collection of achievements/badges earned by this child.
        /// Uses ChildAchievement junction table for many-to-many relationship.
        /// </summary>
        public virtual ICollection<ChildAchievement> ChildAchievements { get; set; } = new List<ChildAchievement>();

        /// <summary>
        /// Collection of character state interactions (happy, sad reactions).
        /// Tracks how the animated character guide responds to child's actions.
        /// </summary>
        public virtual ICollection<ChildCharacterStats> ChildCharacterStats { get; set; } = new List<ChildCharacterStats>();

        /// <summary>
        /// One-to-one relationship with initial profiling questionnaire.
        /// Stores parent's responses during child account setup.
        /// </summary>
        public virtual InitialProfilingQuestionnaire? InitialProfilingQuestionnaire { get; set; }

        /// <summary>
        /// Collection of personalized advice entries for this child.
        /// Generated by AI based on spending patterns and personality type.
        /// </summary>
        public virtual ICollection<Advice> Advices { get; set; } = new List<Advice>();

        /// <summary>
        /// Collection of all financial transactions for this child.
        /// Includes allowances, bonuses, expenses, goal transfers.
        /// </summary>
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}