using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{

    public class AchievementType
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AchievementTypeID { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        [Required]
        [MaxLength(20)]
        public string Category { get; set; } // "Quiz", "Goal", "Expense"

        [Required]
        public int Threshold { get; set; }

        /// URL/path to the icon/badge image for this achievement.
        [MaxLength(500)]
        public string? IconURL { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Collection of children who have earned this achievement.
        /// Uses ChildAchievement junction table for many-to-many relationship.
        /// One achievement type can be earned by many children.
        /// </summary>
        public virtual ICollection<ChildAchievement> ChildAchievements { get; set; } = new List<ChildAchievement>();
    }
}
