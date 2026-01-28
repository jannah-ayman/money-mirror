using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Records a child's response to a story quiz.
    /// Each log entry captures which answer the child selected, its score value,
    /// and which personality type it leans toward. Used to refine personality profiling over time.
    /// </summary>
    public class QuizLog
    {
        /// <summary>
        /// Primary key for the QuizLog entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogID { get; set; }

        /// <summary>
        /// Name/identifier for this quiz session (optional).
        /// Example: "Weekly Quiz", "New Story Challenge"
        /// Can be null for automatically generated quiz sessions.
        /// </summary>
        [MaxLength(200)]
        public string? Name { get; set; }

        /// <summary>
        /// The zero-based index of the answer the child selected.
        /// Used to retrieve the specific answer details from StoryQuizTemplate.AnswerOptions JSON.
        /// Example: 0 = first answer, 1 = second answer, 2 = third answer
        /// </summary>
        [Required]
        public int SelectedAnswerIndex { get; set; }

        /// <summary>
        /// Score value of the selected answer (copied from AnswerOptions JSON).
        /// Used for aggregating total personality scores across multiple quizzes.
        /// Range typically 1-10, with higher values indicating stronger alignment.
        /// </summary>
        [Required]
        public int ScoreValue { get; set; }

        /// <summary>
        /// Timestamp when the child completed this quiz
        /// </summary>
        [Required]
        public DateTime CompletedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Foreign key to Child table.
        /// The child who took this quiz.
        /// </summary>
        [Required]
        public int ChildID { get; set; }

        /// <summary>
        /// Foreign key to StoryQuizTemplate table.
        /// The story scenario the child responded to.
        /// </summary>
        [Required]
        public int StoryID { get; set; }

        /// <summary>
        /// Foreign key to PersonalityType table.
        /// The personality type this answer choice leans toward (copied from AnswerOptions JSON).
        /// Used to calculate which personality type the child most aligns with.
        /// </summary>
        [Required]
        public int TypeID { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Reference to the child who took this quiz
        /// </summary>
        [ForeignKey("ChildID")]
        public virtual Child Child { get; set; }

        /// <summary>
        /// Reference to the story template used in this quiz
        /// </summary>
        [ForeignKey("StoryID")]
        public virtual StoryQuizTemplate StoryQuizTemplate { get; set; }

        /// <summary>
        /// Reference to the personality type this answer leans toward
        /// </summary>
        [ForeignKey("TypeID")]
        public virtual PersonalityType PersonalityType { get; set; }
    }
}
