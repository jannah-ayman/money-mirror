using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        /// <summary>
        /// Foreign key to Child table.
        /// Part of composite primary key.
        /// </summary>
        [Required]
        public int ChildID { get; set; }

        /// <summary>
        /// Foreign key to Parent table.
        /// Part of composite primary key.
        /// </summary>
        [Required]
        public int ParentID { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Reference to the Child entity in this relationship
        /// </summary>
        [ForeignKey("ChildID")]
        public virtual Child Child { get; set; }

        /// <summary>
        /// Reference to the Parent entity in this relationship
        /// </summary>
        [ForeignKey("ParentID")]
        public virtual Parent Parent { get; set; }
    }
}