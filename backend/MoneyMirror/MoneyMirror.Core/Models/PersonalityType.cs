using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Represents a financial personality type classification.
    /// Examples: "Impulsive Spender", "Prudent Saver", "Goal-Oriented Planner", "Bargain Hunter"
    /// Each type has parent-facing and child-facing names, descriptions, traits, and recommendations.
    /// </summary>
    public class PersonalityType
    {
        /// <summary>
        /// Primary key for the PersonalityType entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TypeID { get; set; }

        /// <summary>
        /// Personality type name shown to parents (formal/analytical).
        /// Example: "Impulsive Spender"
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string ParentName { get; set; }

        /// <summary>
        /// Personality type name shown to children (fun/engaging).
        /// Example: "Speedy Spender" (for "Impulsive Spender")
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string ChildName { get; set; }

        /// <summary>
        /// Detailed description of this personality type.
        /// Explains typical behaviors and financial tendencies.
        /// Example: "Quick purchases driven by excitement, low savings ratios"
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string Desc { get; set; }

        /// <summary>
        /// JSON array of personality traits associated with this type.
        /// Example: ["Buys quickly", "Gets excited about new things", "Struggles to save"]
        /// Stored as JSON string in database, deserialized to List<string> in application.
        /// </summary>
        [Required]
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string Traits { get; set; } // JSON array

        /// <summary>
        /// Additional facts about this personality type for educational purposes.
        /// Example: "Did you know? 30% of kids are Speedy Spenders!"
        /// </summary>
        [MaxLength(500)]
        public string? FunFacts { get; set; }

        /// <summary>
        /// JSON array of 3-5 static recommendations for this personality type.
        /// These are pre-defined tips that don't change based on individual behavior.
        /// Example: ["Set a 24-hour waiting rule before purchases", "Use a visual savings jar"]
        /// Stored as JSON string in database, deserialized to List<string> in application.
        /// </summary>
        [Required]
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string StaticRecommendation { get; set; } // JSON array

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Collection of children currently classified as this personality type.
        /// One personality type can apply to many children.
        /// </summary>
        public virtual ICollection<Child> Children { get; set; } = new List<Child>();

    }
}