using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Represents a type of achievement/badge that children can earn.
    /// Examples: "First Goal Completed", "7-Day Streak", "Savings Champion", "Quiz Master"
    /// Defines the criteria for earning the achievement and the visual icon/badge.
    /// Predefined achievements are consistent across all children.
    /// </summary>
    public class AchievementType
    {
        /// <summary>
        /// Primary key for the AchievementType entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AchievementTypeID { get; set; }

        /// <summary>
        /// Display name of the achievement.
        /// Should be encouraging and kid-friendly.
        /// Example: "Super Saver", "Quiz Champion", "Goal Crusher"
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// Description of what the child must do to earn this achievement.
        /// Defines the specific criteria/rules for unlocking.
        /// Example: "Complete your first savings goal", 
        ///          "Log expenses for 7 days in a row",
        ///          "Answer 10 story quizzes correctly"
        /// Used for both display to users and system logic to check completion.
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string Criteria { get; set; }

        /// <summary>
        /// URL/path to the icon/badge image for this achievement.
        /// Could be stored on Cloudinary or local asset server.
        /// Example: "https://res.cloudinary.com/moneymirror/badges/super-saver.png"
        /// Displayed in child's "Trophy Case" when earned.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string IconURL { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Collection of children who have earned this achievement.
        /// Uses ChildAchievement junction table for many-to-many relationship.
        /// One achievement type can be earned by many children.
        /// </summary>
        public virtual ICollection<ChildAchievement> ChildAchievements { get; set; } = new List<ChildAchievement>();
    }
}
