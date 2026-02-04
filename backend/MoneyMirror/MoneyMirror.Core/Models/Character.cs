using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Represents a space-themed character that children can choose.
    /// Each character has a name, description, and multiple visual states.
    /// </summary>
    public class Character
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CharacterID { get; set; }

        /// <summary>
        /// Character name (space-themed).
        /// Example: "Nova", "Cosmo", "Luna", "Stella"
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Short description of character personality.
        /// Example: "A friendly space explorer who loves adventures!"
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Default image URL when no specific state is needed.
        /// Example: "/images/characters/nova/default.png"
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string DefaultImageUrl { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Collection of children who have selected this character.
        /// One character can be chosen by many children.
        /// </summary>
        public virtual ICollection<Child> Children { get; set; } = new List<Child>();

        /// <summary>
        /// Collection of visual states for this character.
        /// Different images for different screens/contexts.
        /// </summary>
        public virtual ICollection<CharacterState> CharacterStates { get; set; } = new List<CharacterState>();
    }
}