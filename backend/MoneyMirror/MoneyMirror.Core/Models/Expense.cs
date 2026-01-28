using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Represents an expense/purchase logged by a child.
    /// Includes amount, category, mood, optional notes, and the item purchased.
    /// Used for tracking spending patterns, generating analytics, and AI personality profiling.
    /// </summary>
    public class Expense
    {
        /// <summary>
        /// Primary key for the Expense entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExpenseID { get; set; }

        /// <summary>
        /// Name/description of the item purchased.
        /// Example: "Action Figure", "Ice Cream", "Comic Book"
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string ItemName { get; set; }

        /// <summary>
        /// Amount of money spent on this expense.
        /// Deducted from child's CurrentBalance when logged.
        /// Stored as DECIMAL(10,2) for precise currency calculations.
        /// Example: 5.99 = $5.99
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal MoneyAmount { get; set; }

        /// <summary>
        /// Timestamp when the expense was logged by the child.
        /// May differ from actual purchase date if logged later.
        /// </summary>
        [Required]
        public DateTime LogDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Optional notes/comments about the purchase.
        /// Example: "Birthday present for friend", "Regret buying this"
        /// Used for AI analysis and parent insights.
        /// </summary>
        [MaxLength(500)]
        public string? Note { get; set; }

        /// <summary>
        /// Foreign key to Child table.
        /// The child who made this purchase and logged this expense.
        /// </summary>
        [Required]
        public int ChildID { get; set; }

        /// <summary>
        /// Foreign key to ExpenseCategory table.
        /// Categorizes the type of purchase (Toys, Food, etc.)
        /// </summary>
        [Required]
        public int CategoryID { get; set; }

        /// <summary>
        /// Foreign key to Mood table.
        /// The child's mood/emotion when making this purchase.
        /// Used to analyze emotional spending patterns.
        /// </summary>
        [Required]
        public int MoodID { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Reference to the child who logged this expense
        /// </summary>
        [ForeignKey("ChildID")]
        public virtual Child Child { get; set; }

        /// <summary>
        /// Reference to the category this expense belongs to
        /// </summary>
        [ForeignKey("CategoryID")]
        public virtual ExpenseCategory ExpenseCategory { get; set; }

        /// <summary>
        /// Reference to the mood associated with this expense
        /// </summary>
        [ForeignKey("MoodID")]
        public virtual Mood Mood { get; set; }
    }
}
