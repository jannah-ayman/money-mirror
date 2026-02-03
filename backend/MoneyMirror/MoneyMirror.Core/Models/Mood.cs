using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    public class Mood
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MoodID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Description { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
