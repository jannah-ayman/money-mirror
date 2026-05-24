using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    public class StoryQuizTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StoryID { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Scenario { get; set; }

        [Required]
        [MaxLength(500)]
        public string QuestionText { get; set; }

        [Required]
        public int TargetAgeMin { get; set; }

        [Required]
        public int TargetAgeMax { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ICollection<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();
        public virtual ICollection<QuizLog> QuizLogs { get; set; } = new List<QuizLog>();
    }
}