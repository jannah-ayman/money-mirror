using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Represents a child user account in the Money Mirror application.
    /// Children aged 6-14 log expenses, track savings goals, and learn financial literacy
    /// through gamified features like story quizzes and achievement badges.
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
        /// Unique login code generated for the child during parent signup.
        /// Used by children to log in without needing email/password.
        /// Example format: "ABC123XYZ"
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string LoginCode { get; set; }

        /// <summary>
        /// Child's first name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FName { get; set; }

        /// <summary>
        /// Child's date of birth - used to calculate age and determine
        /// age-appropriate content (story quizzes, UI complexity)
        /// </summary>
        [Required]
        public DateTime DOB { get; set; }

        /// <summary>
        /// Calculated age based on DOB.
        /// Target age range is 6-14 years old.
        /// Can be computed property or stored value updated periodically.
        /// </summary>
        [Required]
        public int Age { get; set; }

        /// <summary>
        /// Child's current balance/allowance available to spend.
        /// Increases when allowances are given, decreases when expenses are logged.
        /// Stored as DECIMAL(10,2) for precise currency calculations.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal CurrentBalance { get; set; } = 0.00m;

        /// <summary>
        /// Tracks whether the child has completed initial profiling questionnaire.
        /// False = needs to complete initial setup, True = profile established.
        /// </summary>
        [Required]
        public bool ProfileCompletionStatus { get; set; } = false;

        /// <summary>
        /// Timestamp when the child account was created
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Foreign key to PersonalityTypes table.
        /// Represents the child's current financial personality classification
        /// (e.g., "Impulsive Spender", "Prudent Saver").
        /// Nullable until initial profiling is complete.
        /// </summary>
        public int? TypeID { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Reference to the child's personality type.
        /// Contains parent-facing and child-facing names, traits, and recommendations.
        /// </summary>
        [ForeignKey("TypeID")]
        public virtual PersonalityType? PersonalityType { get; set; }

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
    }
}