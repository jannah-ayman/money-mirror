using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Represents a mood/emotion that a child can associate with an expense.
    /// Uses emojis for visual representation to make it engaging for children.
    /// Examples: 😊 Happy, 😢 Sad, 😐 Neutral, 😍 Excited, 😔 Regretful
    /// Used to analyze correlation between moods and spending behavior.
    /// </summary>
    public class Mood
    {
        /// <summary>
        /// Primary key for the Mood entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MoodID { get; set; }

        /// <summary>
        /// Emoji representation of this mood.
        /// Stored as Unicode character.
        /// Example: "😊", "😢", "😐"
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string Emoji { get; set; }

        /// <summary>
        /// Text description of the mood for accessibility and database clarity.
        /// Example: "Happy", "Sad", "Excited", "Regretful"
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Description { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Collection of expenses associated with this mood.
        /// One mood can be selected for many expenses across all children.
        /// </summary>
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
