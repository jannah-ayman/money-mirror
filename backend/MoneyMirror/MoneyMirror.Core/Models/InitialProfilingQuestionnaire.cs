using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Stores responses to the initial profiling questionnaire completed by parents
    /// when adding a new child. Establishes preliminary financial personality profile
    /// before the child starts using the app. One-to-one relationship with Child.
    /// </summary>
    public class InitialProfilingQuestionnaire
    {
        /// <summary>
        /// Primary key for the InitialProfilingQuestionnaire entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuestionnaireID { get; set; }

        /// <summary>
        /// Response to Question 1.
        /// Example question: "How does your child typically react when they receive money?"
        /// Stores parent's selected answer text or answer ID.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Question1Response { get; set; }

        /// <summary>
        /// Response to Question 2.
        /// Example question: "When shopping, does your child usually..."
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Question2Response { get; set; }

        /// <summary>
        /// Response to Question 3.
        /// Example question: "How often does your child save money they receive?"
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Question3Response { get; set; }

        /// <summary>
        /// Response to Question 4.
        /// Example question: "When your child wants something, do they..."
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Question4Response { get; set; }

        /// <summary>
        /// Response to Question 5.
        /// Example question: "How does your child feel after making a purchase?"
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Question5Response { get; set; }

        /// <summary>
        /// Response to Question 6.
        /// Example question: "Does your child compare prices before buying?"
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Question6Response { get; set; }

        /// <summary>
        /// Additional optional field for storing all responses as JSON if needed.
        /// Can be used for flexibility if questionnaire changes over time.
        /// Example: {"q1": "answer1", "q2": "answer2", ...}
        /// </summary>
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string? QuestionResponse { get; set; } // JSON for flexibility

        /// <summary>
        /// Foreign key to PersonalityType table.
        /// AI-calculated personality type based on questionnaire responses.
        /// Used as initial personality classification before spending data is available.
        /// </summary>
        [Required]
        public int CalculatedTypeID { get; set; }

        /// <summary>
        /// Indicates whether the questionnaire has been completed.
        /// False = in progress, True = submitted and processed
        /// </summary>
        [Required]
        public bool IsCompleted { get; set; } = false;

        /// <summary>
        /// Timestamp when the questionnaire was completed/submitted.
        /// Nullable until the questionnaire is finished.
        /// </summary>
        public DateTime? CompletedDate { get; set; }

        /// <summary>
        /// Foreign key to Child table (unique constraint).
        /// One questionnaire per child, one child per questionnaire.
        /// </summary>
        [Required]
        public int ChildID { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Reference to the child this questionnaire is about.
        /// One-to-one relationship.
        /// </summary>
        [ForeignKey("ChildID")]
        public virtual Child Child { get; set; }

        /// <summary>
        /// Reference to the calculated personality type from questionnaire responses
        /// </summary>
        [ForeignKey("CalculatedTypeID")]
        public virtual PersonalityType CalculatedPersonalityType { get; set; }
    }
}
