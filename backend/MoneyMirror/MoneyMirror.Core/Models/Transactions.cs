using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Records all financial transactions for a child's balance.
    /// Includes allowances, bonuses, expenses, and goal transfers.
    /// Provides complete audit trail for balance changes.
    /// </summary>
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionID { get; set; }

        /// <summary>
        /// Type of transaction.
        /// Values: "AllowanceCredit", "BonusCredit", "Expense", "GoalTransfer"
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        /// <summary>
        /// Amount of money involved.
        /// Positive = credit (allowance, bonus)
        /// Negative = debit (expense, goal transfer) - for future
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Balance after this transaction was applied.
        /// Useful for showing "Your balance was 50.00, now it's 60.00"
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal BalanceAfter { get; set; }

        /// <summary>
        /// Description/reason for the transaction.
        /// Examples:
        /// - "Weekly allowance credited"
        /// - "Bonus for good grades"
        /// - "Bought ice cream" (future expense)
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Timestamp when transaction occurred.
        /// </summary>
        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Foreign key to Child table.
        /// The child whose balance was affected.
        /// </summary>
        [Required]
        public int ChildID { get; set; }

        /// <summary>
        /// Foreign key to Parent table (nullable).
        /// Set if a parent initiated this transaction (bonus, allowance setup).
        /// Null if child initiated (logged expense - future).
        /// </summary>
        public int? ParentID { get; set; }

        /// <summary>
        /// Optional reference to the Allowance record that triggered this credit.
        /// Helps link back to the recurring allowance schedule.
        /// Null for bonuses or expenses.
        /// </summary>
        public int? AllowanceID { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        [ForeignKey("ChildID")]
        public virtual Child Child { get; set; }

        [ForeignKey("ParentID")]
        public virtual Parent? Parent { get; set; }

        [ForeignKey("AllowanceID")]
        public virtual Allowance? Allowance { get; set; }
    }
}