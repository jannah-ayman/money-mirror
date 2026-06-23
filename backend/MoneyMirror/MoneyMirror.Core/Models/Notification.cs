using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Represents a notification sent to either a parent or child.
    /// Supports both in-app notifications (via SignalR) and email notifications (via SendGrid).
    /// Examples: goal progress updates, spending alerts, profile changes, reminders.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Primary key for the Notification entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationID { get; set; }

        /// Values: "Parent" or "Child"
        /// Determines tone and complexity of message.
        [Required]
        [MaxLength(20)]
        public string TargetType { get; set; }

        /// Short title/subject of the notification.
        /// Example: "Goal Reached!", "Spending Alert", "New Quiz Available"
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        /// Full message content of the notification.
        /// Should be age-appropriate and engaging for children,
        /// informative and actionable for parents.
        [Required]
        [MaxLength(1000)]
        public string Message { get; set; }

        /// Timestamp when the notification was sent.
        /// Used for sorting notifications by recency.
        [Required]
        public DateTime SentDate { get; set; } = DateTime.UtcNow;

        /// Indicates whether the user has read/acknowledged this notification.
        /// False = unread, True = read
        [Required]
        public bool IsRead { get; set; } = false;

        /// Optional deep link to relevant section of the app.
        /// Example: "/child/goals/123" to navigate to a specific goal
        /// Null if notification doesn't require navigation.
        [MaxLength(500)]
        public string? Link { get; set; }

        /// <summary>
        /// Delivery method for this notification.
        /// Values: "InApp", "Email", "Both"
        /// Determines whether to send via SignalR, SendGrid, or both.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string DeliveryMethod { get; set; }

        /// <summary>
        /// Timestamp when the notification record was created in the database.
        /// May differ slightly from SentDate if there's a delay in sending.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Foreign key to Parent table (nullable).
        /// Set if this notification is for a parent, null if for a child.
        /// </summary>
        public int? ParentID { get; set; }

        /// <summary>
        /// Foreign key to Child table (nullable).
        /// Set if this notification is for a child, null if for a parent.
        /// </summary>
        public int? ChildID { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Reference to the parent recipient (if applicable).
        /// Null if notification is for a child.
        /// </summary>
        [ForeignKey("ParentID")]
        public virtual Parent? Parent { get; set; }

        /// <summary>
        /// Reference to the child recipient (if applicable).
        /// Null if notification is for a parent.
        /// </summary>
        [ForeignKey("ChildID")]
        public virtual Child? Child { get; set; }
    }
}