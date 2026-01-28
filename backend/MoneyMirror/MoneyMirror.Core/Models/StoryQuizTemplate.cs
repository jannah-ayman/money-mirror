using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Represents a story-based financial quiz scenario.
    /// Each story presents a financial situation with one question and multiple answer choices.
    /// Answers are scored and mapped to personality types to help refine child's financial profile.
    /// Answer options stored as JSON for flexibility.
    /// </summary>
    public class StoryQuizTemplate
    {
        /// <summary>
        /// Primary key for the StoryQuizTemplate entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StoryID { get; set; }

        /// <summary>
        /// Short, engaging title for the story scenario.
        /// Example: "The Toy Store Dilemma", "Birthday Money Challenge"
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// The story narrative/setup that presents the financial situation.
        /// Should be age-appropriate (6-14) and engaging.
        /// Example: "You walk into a toy store with your weekly allowance of $10. 
        /// You see a toy you really want that costs $8..."
        /// </summary>
        [Required]
        [MaxLength(2000)]
        public string Scenario { get; set; }

        /// <summary>
        /// The question asked at the end of the scenario.
        /// Example: "What do you do with your money?"
        /// Each story has exactly ONE question with multiple answer choices.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string QuestionText { get; set; }

        /// <summary>
        /// JSON array containing all answer options for this story.
        /// Each answer object includes:
        /// - answerText: The answer choice text
        /// - scoreValue: Numeric weight for personality scoring (1-10)
        /// - personalityTypeID: Which personality type this answer leans toward
        /// 
        /// Example JSON structure:
        /// [
        ///   {
        ///     "answerText": "Buy the toy right away!",
        ///     "scoreValue": 5,
        ///     "personalityTypeID": 1
        ///   },
        ///   {
        ///     "answerText": "Save all my money for something bigger",
        ///     "scoreValue": 10,
        ///     "personalityTypeID": 2
        ///   },
        ///   {
        ///     "answerText": "Buy the toy and save the rest",
        ///     "scoreValue": 7,
        ///     "personalityTypeID": 3
        ///   }
        /// ]
        /// 
        /// SQL Server will validate this as proper JSON via CHECK constraint in DbContext.
        /// </summary>
        [Required]
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string AnswerOptions { get; set; } // JSON array

        /// <summary>
        /// Minimum age this story is appropriate for.
        /// Used to filter stories based on child's age.
        /// Example: 6 = suitable for ages 6+
        /// </summary>
        [Required]
        public int TargetAgeMin { get; set; }

        /// <summary>
        /// Maximum age this story is appropriate for.
        /// Used to filter stories based on child's age.
        /// Example: 10 = suitable for ages up to 10
        /// </summary>
        [Required]
        public int TargetAgeMax { get; set; }

        /// <summary>
        /// Timestamp when this story template was created.
        /// Useful for managing story library over time.
        /// </summary>
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // No navigation properties needed for this entity in the current design
        // QuizLogs will reference this via StoryID foreign key
    }
}
