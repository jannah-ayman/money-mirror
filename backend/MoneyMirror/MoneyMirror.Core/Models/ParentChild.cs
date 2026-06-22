using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MoneyMirror.Core.Enums;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Junction table for many-to-many relationship between Parent and Child.
    /// Allows one parent to manage multiple children, and one child to be
    /// managed by multiple parents (e.g., divorced/separated parents sharing custody).
    /// Composite primary key: (ChildID, ParentID)
    /// </summary>
    public class ParentChild
    {

        [Required]
        public int ChildID { get; set; }

        [Required]
        public int ParentID { get; set; }

        /// Relationship of this parent to this specific child.
        [Required]
        public ParentRole Role { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        [ForeignKey("ChildID")]
        public virtual Child Child { get; set; }

        [ForeignKey("ParentID")]
        public virtual Parent Parent { get; set; }
    }
}