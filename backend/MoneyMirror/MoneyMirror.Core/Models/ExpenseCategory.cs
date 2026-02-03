using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    public class ExpenseCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
