using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Represents a category for classifying expenses.
    /// Examples: "Toys", "Food", "Clothes", "Entertainment", "School Supplies"
    /// Used for tracking spending patterns and generating analytics.
    /// Categories are predefined and consistent across all children.
    /// </summary>
    public class ExpenseCategory
    {
        /// <summary>
        /// Primary key for the ExpenseCategory entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryID { get; set; }

        /// <summary>
        /// Name of the expense category.
        /// Should be simple and kid-friendly.
        /// Example: "Toys", "Snacks", "Books"
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Collection of expenses categorized under this category.
        /// One category can apply to many expenses across all children.
        /// </summary>
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
