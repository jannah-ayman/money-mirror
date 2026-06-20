using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
   
    /// Represents a savings goal for a child.
    /// Can be created by the child themselves or set as a challenge by a parent.
    /// Tracks progress toward the target amount and includes optional rewards for parent challenges.
    
    public class SavingsGoal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GoalID { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

      
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TargetAmount { get; set; }

      
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal CurrentAmount { get; set; } = 0.00m;
       
        [Required]
        [Column(TypeName = "decimal(10,2)")]
      public decimal SavedAmountBeforeRefund { get; set; } = 0.00m;


        [Required]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime? EndDate { get; set; }


        [Required]
        public bool IsChallenge { get; set; } = false;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        [Column(TypeName = "decimal(10,2)")]
        public decimal? RewardValue { get; set; }

        [Required]
        public int ChildID { get; set; }

    
        public int? ParentID { get; set; }


        // ==================== NAVIGATION PROPERTIES ====================

    
        [ForeignKey("ChildID")]
        public virtual Child Child { get; set; }

        [ForeignKey("ParentID")]
        public virtual Parent? Parent { get; set; }
    }
}
