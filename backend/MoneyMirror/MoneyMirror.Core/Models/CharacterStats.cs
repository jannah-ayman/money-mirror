using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Represents a specific visual/emotional state for a character.
    /// Examples: Nova-Happy, Luna-Thinking, Cosmo-Celebrating
    /// Each character has 10 predefined states that display in different contexts.
    /// State changes dynamically based on child's actions and screen context.
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
        /// Foreign key to Character table.
        /// Links this state to its parent character.
        /// Required - every state belongs to exactly one character.
        /// </summary>
        [Required]
        public int CharacterID { get; set; }

        /// <summary>
        /// The emotional/visual state name (matches CharacterState enum).
        /// Values: "Idle", "Happy", "Excited", "Thinking", "Encouraging", 
        ///         "Celebrating", "Neutral", "Curious", "Proud", "Worried"
        /// Used to programmatically select appropriate state for context.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string State { get; set; }

        /// <summary>
        /// Full URL/path to the animation or image file for this state.
        /// Can be: .png (static image), .gif (animated), .json (Lottie animation)
        /// Example: "/characters/nova/happy.png"
        /// Served from wwwroot/characters folder in API.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string AnimationURL { get; set; }

        /// <summary>
        /// Description of when/why this state is displayed.
        /// Used for documentation and admin purposes.
        /// Example: "Shown when child completes a savings goal"
        /// Helps designers/developers understand state usage.
        /// </summary>
        [MaxLength(500)]
        public string? UsageContext { get; set; }

        /// <summary>
        /// Whether this state is currently active and can be displayed.
        /// Allows temporarily disabling states without deletion.
        /// Use case: Testing new animations, seasonal variations.
        /// </summary>
        [Required]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Timestamp when this state was created/added.
        /// Useful for version tracking and analytics.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Reference to the parent character this state belongs to.
        /// </summary>
        [ForeignKey("CharacterID")]
        public virtual Character Character { get; set; }

        /// <summary>
        /// Collection of instances when children viewed this specific state.
        /// Used for engagement analytics and A/B testing.
        /// Tracks which states resonate most with children.
        /// </summary>
        public virtual ICollection<ChildCharacterStats> ChildCharacterStats { get; set; } = new List<ChildCharacterStats>();
    }
}