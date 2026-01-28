using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Represents an allowance given to a child by a parent.
    /// Can be either recurring (weekly/monthly) or one-time bonus.
    /// Tracks when the allowance was set and when it was actually given to the child.
    /// </summary>
    public class Allowance
    {
        /// <summary>
        /// Primary key for the Allowance entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AllowanceID { get; set; }

        /// <summary>
        /// Type of allowance.
        /// Values: "Weekly", "Monthly", "Bonus"
        /// Determines frequency of recurring allowances or indicates one-time bonus.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Type { get; set; }

        /// <summary>
        /// Amount of money given as allowance.
        /// Stored as DECIMAL(10,2) for precise currency calculations.
        /// Example: 10.00 = $10.00 weekly allowance
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Timestamp when the parent set/scheduled this allowance.
        /// For recurring allowances, this is the initial setup date.
        /// </summary>
        [Required]
        public DateTime SetDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when the allowance was actually given to the child.
        /// For recurring allowances, this updates each time the allowance is paid out.
        /// Nullable if allowance is scheduled but not yet given.
        /// </summary>
        public DateTime? GivenDate { get; set; }

        /// <summary>
        /// Foreign key to Child table.
        /// The child receiving this allowance.
        /// </summary>
        [Required]
        public int ChildID { get; set; }

        /// <summary>
        /// Foreign key to Parent table.
        /// The parent who set/gave this allowance.
        /// </summary>
        [Required]
        public int ParentID { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Reference to the child receiving this allowance
        /// </summary>
        [ForeignKey("ChildID")]
        public virtual Child Child { get; set; }

        /// <summary>
        /// Reference to the parent who set this allowance
        /// </summary>
        [ForeignKey("ParentID")]
        public virtual Parent Parent { get; set; }
    }
}