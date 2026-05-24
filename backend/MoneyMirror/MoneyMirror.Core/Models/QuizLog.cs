using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    public class QuizLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogID { get; set; }

        [Required]
        public DateTime CompletedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int ChildID { get; set; }

        [Required]
        public int StoryID { get; set; }

        // Which of the 4 answers the child picked
        [Required]
        public int AnswerID { get; set; }

        // Navigation
        [ForeignKey("ChildID")]
        public virtual Child Child { get; set; }

        [ForeignKey("StoryID")]
        public virtual StoryQuizTemplate StoryQuizTemplate { get; set; }

        [ForeignKey("AnswerID")]
        public virtual QuizAnswer QuizAnswer { get; set; }
    }
}