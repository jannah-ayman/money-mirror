using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Represents different states/reactions of the animated character guide.
    /// The character provides visual feedback to children based on their financial actions.
    /// Examples: Happy reaction when saving, sad reaction when overspending, 
    ///           celebration animation when reaching goals.
    /// Predefined character states are consistent across all children.
    /// </summary>
    public class CharacterStats
    {
        /// <summary>
        /// Primary key for the CharacterStats entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StatsID { get; set; }

        /// <summary>
        /// URL/path to the animation file or image for this character state.
        /// Could be stored on Cloudinary or local asset server.
        /// Examples: 
        /// - "https://res.cloudinary.com/moneymirror/character/happy.gif"
        /// - "https://res.cloudinary.com/moneymirror/character/sad.png"
        /// - "https://res.cloudinary.com/moneymirror/character/celebration.json" (Lottie)
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string AnimationURL { get; set; }

        /// <summary>
        /// Text description of this character state/reaction.
        /// Examples: "Happy", "Sad", "Excited", "Worried", "Celebrating"
        /// Used for accessibility and system logging.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Collection of child-character interactions using this state.
        /// Tracks when children see specific character reactions.
        /// One character state can be shown to many children across multiple events.
        /// </summary>
        public virtual ICollection<ChildCharacterStats> ChildCharacterStats { get; set; } = new List<ChildCharacterStats>();
    }
}
