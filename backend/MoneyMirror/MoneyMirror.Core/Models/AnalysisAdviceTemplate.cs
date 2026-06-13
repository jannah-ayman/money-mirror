using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMirror.Core.Models
{
    public class AnalysisAdviceTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdviceTemplateID { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string ActionSteps { get; set; } // JSON array

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } // "Alert" or "Strength"

        [Required]
        [MaxLength(100)]
        public string TriggerKey { get; set; }

        [Required]
        public int Priority { get; set; } // 1=high, 2=medium, 3=low
    }
}