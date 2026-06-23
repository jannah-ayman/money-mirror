using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Junction table for many-to-many relationship between Child and AchievementType.
    /// Records when a child earns a specific achievement/badge.
    /// Composite primary key: (ChildID, AchievementTypeID)
    /// Includes earned date for tracking progress and displaying in trophy case.
    /// </summary>
    public class ChildAchievement
    {
        
        [Required]
        public DateTime EarnedDate { get; set; } = DateTime.UtcNow;

       
        [Required]
        public int ChildID { get; set; }

       
        [Required]
        public int AchievementTypeID { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Reference to the child who earned this achievement
        /// </summary>
        [ForeignKey("ChildID")]
        public virtual Child Child { get; set; }

        /// <summary>
        /// Reference to the achievement type that was earned
        /// </summary>
        [ForeignKey("AchievementTypeID")]
        public virtual AchievementType AchievementType { get; set; }
    }
}
