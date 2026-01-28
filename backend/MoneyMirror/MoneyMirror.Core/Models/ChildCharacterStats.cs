using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Records instances when the animated character guide reacts to a child's actions.
    /// Tracks which character states were triggered for each child and why.
    /// Used for engagement analytics and to create a personalized character experience.
    /// Many-to-many relationship between Child and CharacterStats with additional context.
    /// </summary>
    public class ChildCharacterStats
    {
        /// <summary>
        /// Primary key for the ChildCharacterStats entity
        /// Could also use composite key (ChildID, StatsID, TriggerEvent) if preferred
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Optional data/context about this specific character interaction.
        /// Could store JSON with details like:
        /// - Related expense ID
        /// - Related goal ID
        /// - Duration character was shown
        /// - User interaction (did they click/dismiss?)
        /// Example: {"relatedExpenseID": 123, "duration": 5, "dismissed": false}
        /// </summary>
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string? StatsData { get; set; } // JSON for flexible data storage

        /// <summary>
        /// The event/action that triggered this character reaction.
        /// Examples:
        /// - "ExpenseLogged" - character reacts to an expense being logged
        /// - "GoalReached" - celebration when goal is completed
        /// - "Overspending" - worried/sad reaction when spending too much
        /// - "ConsistentSaving" - happy reaction for good behavior
        /// - "QuizCompleted" - encouraging reaction after quiz
        /// Used to analyze which triggers generate most engagement.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string TriggerEvent { get; set; }

        /// <summary>
        /// Foreign key to CharacterStats table.
        /// The specific character state/animation that was shown.
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
        /// Reference to the character state that was triggered
        /// </summary>
        [ForeignKey("StatsID")]
        public virtual CharacterStats CharacterStats { get; set; }

        /// <summary>
        /// Reference to the child who saw this character reaction
        /// </summary>
        [ForeignKey("ChildID")]
        public virtual Child Child { get; set; }
    }
}
