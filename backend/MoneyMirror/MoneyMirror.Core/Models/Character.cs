using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Represents a character type available for children to select.
    /// Each character has a unique personality and visual style.
    /// Characters are seeded in the database as static reference data.
    /// </summary>
    public class Character
    {
        /// <summary>
        /// Primary key for the Character entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CharacterID { get; set; }

        /// <summary>
        /// Internal character type identifier.
        /// Values: "Nova", "Luna", "Cosmo", "Aura"
        /// Used for enum mapping and image path generation.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string CharacterType { get; set; }

        /// <summary>
        /// Display name shown to children.
        /// Example: "Nova the Explorer"
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Description of character personality.
        /// Example: "Energetic and loves adventures! Nova is always excited to help you reach your goals! 🚀"
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Base path for character images.
        /// Example: "/characters/nova"
        /// Actual image URLs are constructed as: {BasePath}/{state}.png
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string BasePath { get; set; }

        /// <summary>
        /// Indicates if this character is currently available for selection.
        /// Can be used to temporarily disable characters.
        /// </summary>
        [Required]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Display order for character selection screen.
        /// Lower numbers appear first.
        /// </summary>
        [Required]
        public int DisplayOrder { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Collection of children who selected this character.
        /// One character can be selected by many children.
        /// </summary>
        public virtual ICollection<Child> Children { get; set; } = new List<Child>();
    }
}