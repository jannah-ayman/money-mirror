using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Represents a savings goal for a child.
    /// Can be created by the child themselves or set as a challenge by a parent.
    /// Tracks progress toward the target amount and includes optional rewards for parent challenges.
    /// </summary>
    public class SavingsGoal
    {
        /// <summary>
        /// Primary key for the SavingsGoal entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GoalID { get; set; }

        /// <summary>
        /// Title/name of the savings goal.
        /// Example: "New Bicycle", "Video Game", "Summer Camp"
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// Detailed description of the goal.
        /// Example: "I want to save for a blue mountain bike!"
        /// </summary>
        [MaxLength(1000)]
        public string? Desc { get; set; }

        /// <summary>
        /// Target amount the child needs to save to complete the goal.
        /// Stored as DECIMAL(10,2) for precise currency calculations.
        /// Example: 150.00 = $150.00 target
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TargetAmount { get; set; }

        /// <summary>
        /// Current amount saved toward this goal.
        /// Updates as child allocates money from expenses or allowances.
        /// Stored as DECIMAL(10,2) for precise currency calculations.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal CurrentAmount { get; set; } = 0.00m;

        /// <summary>
        /// Timestamp when the goal was created/started
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Optional deadline/target date to complete the goal.
        /// Used to calculate urgency and encourage consistent saving.
        /// Null if no specific deadline is set.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Indicates if this is a parent-created challenge goal.
        /// True = parent challenge (usually includes reward)
        /// False = child-created personal goal
        /// </summary>
        [Required]
        public bool IsChallenge { get; set; } = false;

        /// <summary>
        /// Current status of the goal.
        /// Values: "Active", "Completed", "Abandoned"
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        /// <summary>
        /// Optional reward value for parent challenge goals.
        /// Null for child-created goals.
        /// Example: 10.00 = $10.00 bonus allowance upon goal completion
        /// Stored as DECIMAL(10,2) for precise currency calculations.
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal? RewardValue { get; set; }

        /// <summary>
        /// Foreign key to Child table.
        /// The child working toward this goal (required - all goals belong to a child).
        /// </summary>
        [Required]
        public int ChildID { get; set; }

        /// <summary>
        /// Foreign key to Parent table (nullable).
        /// Set if this is a parent-created challenge goal.
        /// Null if this is a child-created personal goal.
        /// </summary>
        public int? ParentID { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Reference to the child working toward this goal
        /// </summary>
        [ForeignKey("ChildID")]
        public virtual Child Child { get; set; }

        /// <summary>
        /// Reference to the parent who created this challenge goal (if applicable).
        /// Null for child-created goals.
        /// </summary>
        [ForeignKey("ParentID")]
        public virtual Parent? Parent { get; set; }
    }
}
