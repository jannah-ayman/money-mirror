using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Represents an allowance given to a child by a parent.
    /// Can be either recurring (automatic, scheduled) or one-time bonus (manual).
    /// 
    /// RECURRING ALLOWANCE:
    /// - Frequency: Daily, Weekly, or Monthly
    /// - Amount: Fixed amount credited automatically by background job
    /// - Schedule: Configured by parent (e.g., "every Saturday at 9 AM")
    /// - Creates Transaction records each time it's credited
    /// 
    /// ONE-TIME BONUS:
    /// - IsRecurring = false
    /// - Given immediately by parent
    /// - Creates one Transaction record
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
        /// Type of allowance frequency.
        /// Values: "Daily", "Weekly", "Monthly"
        /// Determines how often the allowance is automatically credited.
        /// Ignored if IsRecurring = false (one-time bonus).
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Type { get; set; }

        /// <summary>
        /// Amount of money given as allowance.
        /// For recurring: amount credited each cycle (daily/weekly/monthly)
        /// For bonus: one-time amount
        /// Stored as DECIMAL(10,2) for precise currency calculations.
        /// Example: 50.00 = 50 Egyptian Pounds
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Timestamp when the parent created/configured this allowance.
        /// For recurring allowances, this is the initial setup date.
        /// For bonuses, this is when the bonus was given.
        /// </summary>
        [Required]
        public DateTime SetDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when the allowance was actually given to the child.
        /// For recurring allowances: updated each time the allowance is credited by background job.
        /// For bonuses: same as SetDate (given immediately).
        /// Nullable if recurring allowance hasn't been credited yet.
        /// </summary>
        public DateTime? GivenDate { get; set; }

        // ==================== NEW FIELDS FOR RECURRING ALLOWANCES ====================

        /// <summary>
        /// Indicates if this is a recurring allowance or a one-time bonus.
        /// True = recurring allowance (automatically credited on schedule by background job)
        /// False = one-time bonus (already given, no automatic crediting)
        /// Default: true
        /// </summary>
        [Required]
        public bool IsRecurring { get; set; } = true;
        public string? Reason { get; set; }

        /// <summary>
        /// For Daily allowances: Hour of day (0-23) when allowance should be credited.
        /// Example: 9 = credit at 9:00 AM UTC daily
        /// Null if Type is not "Daily".
        /// Background job checks this to determine when to credit.
        /// </summary>
        public int? DailyHour { get; set; }

        /// <summary>
        /// For Weekly allowances: Day of week when allowance should be credited.
        /// Values: "Saturday", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday"
        /// Example: "Saturday" = credit every Saturday
        /// Null if Type is not "Weekly".
        /// Background job checks this to determine when to credit.
        /// </summary>
        [MaxLength(20)]
        public string? WeeklyDay { get; set; }

        /// <summary>
        /// For Monthly allowances: Day of month (1-31) when allowance should be credited.
        /// Example: 15 = credit on the 15th of each month
        /// Special handling for months with fewer days:
        /// - If MonthlyDay = 31 but current month has 30 days, credit on the 30th
        /// - If MonthlyDay = 31 but current month (Feb) has 28/29 days, credit on last day
        /// Null if Type is not "Monthly".
        /// </summary>
        public int? MonthlyDay { get; set; }

        /// <summary>
        /// Timestamp of the last successful credit by the background job.
        /// Helps prevent double-crediting within the same period.
        /// Example: If last credited on Jan 15 at 9:00 AM, don't credit again until next week/month.
        /// Null if this recurring allowance has never been credited yet.
        /// Updated by background job each time it credits the allowance.
        /// </summary>
        public DateTime? LastCreditedDate { get; set; }

        /// <summary>
        /// Indicates if this recurring allowance is currently active.
        /// True = background job will credit this allowance on schedule
        /// False = background job will skip this allowance (paused by parent)
        /// Parents can pause/resume allowances without deleting them.
        /// Default: true
        /// </summary>
        [Required]
        public bool IsActive { get; set; } = true;

        // ==================== FOREIGN KEYS ====================

        /// <summary>
        /// Foreign key to Child table.
        /// The child receiving this allowance.
        /// Required - every allowance belongs to exactly one child.
        /// </summary>
        [Required]
        public int ChildID { get; set; }

        /// <summary>
        /// Foreign key to Parent table.
        /// The parent who set/gave this allowance.
        /// Required - every allowance is created by exactly one parent.
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

        /// <summary>
        /// Collection of transactions created by this allowance.
        /// For recurring allowances: multiple transactions (one per credit)
        /// For bonuses: typically one transaction
        /// </summary>
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}