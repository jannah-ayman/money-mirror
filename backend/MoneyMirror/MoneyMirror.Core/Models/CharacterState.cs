using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Represents a visual state/image for a character in a specific screen context.
    /// Example: Nova's happy image on the dashboard, Nova's thinking image on expenses page.
    /// </summary>
    public class CharacterState
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StateID { get; set; }

        /// <summary>
        /// Which screen/context this image is for.
        /// Examples: "Dashboard", "Expenses", "Savings", "Goals", "Profile"
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ScreenContext { get; set; }

        /// <summary>
        /// URL to the character image for this screen.
        /// Example: "/images/characters/nova/dashboard.png"
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Optional friendly message the character says on this screen.
        /// Example: "Let's check your savings progress! 🚀"
        /// </summary>
        [MaxLength(500)]
        public string Message { get; set; }

        /// <summary>
        /// Foreign key to Character table.
        /// Which character this state belongs to.
        /// </summary>
        [Required]
        public int CharacterID { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        [ForeignKey("CharacterID")]
        public virtual Character Character { get; set; }
    }
}