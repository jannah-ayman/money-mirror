using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Records instances when the animated character guide reacts to a child's actions.
    /// Tracks which character states were triggered for each child and in what context.
    /// Used for engagement analytics, personalization, and A/B testing character effectiveness.
    /// Junction table with additional context data.
    /// </summary>
    public class ChildCharacterStats
    {
        /// <summary>
        /// Primary key for the ChildCharacterStats entity.
        /// Auto-incrementing ID for each interaction record.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Timestamp when this character state was displayed to the child.
        /// Used for analytics: time-of-day patterns, frequency tracking.
        /// </summary>
        [Required]
        public DateTime DisplayedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The event/action that triggered this character reaction.
        /// Examples:
        /// - "ExpenseLogged" - character reacts to expense being logged
        /// - "GoalReached" - celebration when goal is completed
        /// - "Overspending" - worried/sad reaction when spending too much
        /// - "ConsistentSaving" - happy reaction for good behavior
        /// - "QuizCompleted" - encouraging reaction after quiz
        /// - "DashboardView" - idle state on dashboard
        /// Used to analyze which triggers generate most engagement.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string TriggerEvent { get; set; }

        /// <summary>
        /// Optional JSON data with context about this specific interaction.
        /// Flexible schema allows storing different data per trigger type.
        /// Examples:
        /// - {"relatedExpenseID": 123, "expenseAmount": 50.00}
        /// - {"relatedGoalID": 456, "goalProgress": 80}
        /// - {"screenDuration": 5, "userDismissed": false}
        /// Used for deep analytics and personalization.
        /// </summary>
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string? StatsData { get; set; }

        /// <summary>
        /// Whether the child interacted with the character (clicked, dismissed, etc.)
        /// Measures engagement: did child acknowledge the character's message?
        /// </summary>
        public bool? WasInteracted { get; set; }

        /// <summary>
        /// Duration in seconds the character state was visible.
        /// Helps measure attention and engagement.
        /// Null if duration not tracked for this interaction.
        /// </summary>
        public int? DurationSeconds { get; set; }

        /// <summary>
        /// Foreign key to CharacterStats table.
        /// The specific character state/animation that was shown.
        /// Links to exact state (e.g., Nova-Happy, Luna-Thinking).
        /// </summary>
        [Required]
        public int StatsID { get; set; }

        /// <summary>
        /// Foreign key to Child table.
        /// The child who experienced this character reaction.
        /// </summary>
        [Required]
        public int ChildID { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Reference to the character state that was triggered.
        /// Includes the animation URL and state details.
        /// </summary>
        [ForeignKey("StatsID")]
        public virtual CharacterStats CharacterStats { get; set; }

        /// <summary>
        /// Reference to the child who saw this character reaction.
        /// </summary>
        [ForeignKey("ChildID")]
        public virtual Child Child { get; set; }
    }
}