using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// Represents an expense/purchase logged by a child.
    /// Includes amount, category, mood, optional notes, and the item purchased.
    /// Used for tracking spending patterns, generating analytics, and AI personality profiling.
    public class Expense
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExpenseID { get; set; }

        [MaxLength(200)]
        public string? ItemName { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal MoneyAmount { get; set; }

        [Required]
        public DateTime LogDate { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Note { get; set; }


        [Required]
        public int ChildID { get; set; }

        /// Foreign key to ExpenseCategory table.
        /// Categorizes the type of purchase (Toys, Food, etc.)
        [Required]
        public int CategoryID { get; set; }

        /// Foreign key to Mood table.
        /// The child's mood/emotion when making this purchase.
        /// Used to analyze emotional spending patterns.
        [Required]
        public int MoodID { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// Reference to the child who logged this expense
        [ForeignKey("ChildID")]
        public virtual Child Child { get; set; }

        /// Reference to the category this expense belongs to
        [ForeignKey("CategoryID")]
        public virtual ExpenseCategory ExpenseCategory { get; set; }

        /// <summary>
        /// Reference to the mood associated with this expense
        /// </summary>
        [ForeignKey("MoodID")]
        public virtual Mood Mood { get; set; }
    }
}
