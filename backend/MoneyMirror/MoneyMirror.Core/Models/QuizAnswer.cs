using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    public class QuizAnswer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnswerID { get; set; }

        [Required]
        [MaxLength(500)]
        public string AnswerText { get; set; }

        // Message shown to child after choosing this answer
        [Required]
        [MaxLength(500)]
        public string FeedbackMessage { get; set; }

        [Required]
        public int StoryID { get; set; }

        // Each answer maps to exactly one personality type
        [Required]
        public int PersonalityTypeID { get; set; }

        // Navigation
        [ForeignKey("StoryID")]
        public virtual StoryQuizTemplate StoryQuizTemplate { get; set; }

        [ForeignKey("PersonalityTypeID")]
        public virtual PersonalityType PersonalityType { get; set; }

        public virtual ICollection<QuizLog> QuizLogs { get; set; } = new List<QuizLog>();
    }
}