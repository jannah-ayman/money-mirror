using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MoneyMirror.Core.Enums;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Represents the initial profiling questionnaire filled out by parents when creating a child account.
    /// Contains all 25 questions about the child's financial behavior, habits, and personality.
    /// Used to establish baseline personality insights before AI analysis.
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
        /// Indicates whether the questionnaire has been completed.
        /// True = all questions answered and submitted
        /// </summary>
        [Required]
        public bool IsCompleted { get; set; } = false;

        /// <summary>
        /// Timestamp when the questionnaire was completed
        /// </summary>
        public DateTime? CompletedDate { get; set; }

        // ==================== Section 1: Child Profile ====================

        /// <summary>
        /// Child's age group category (Age_6_8, Age_9_11, Age_12_14)
        /// </summary>
        [Required]
        public ChildAgeGroup ChildAgeGroup { get; set; }

        /// <summary>
        /// Child's gender (Male, Female)
        /// </summary>
        [Required]
        public ChildGender ChildGender { get; set; }

        // ==================== Section 2: Allowance Details ====================

        /// <summary>
        /// How often the child receives allowance (Daily, Weekly, Monthly)
        /// </summary>
        [Required]
        public AllowanceFrequency AllowanceFrequency { get; set; }

        /// <summary>
        /// Amount of allowance the child receives per period
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal AllowanceAmount { get; set; }

        // ==================== Section 3: Expenses ====================

        /// <summary>
        /// Primary category where child spends money
        /// </summary>
        [Required]
        public SpendingCategory PrimarySpendingCategory { get; set; }

        /// <summary>
        /// How often child plans purchases before spending
        /// </summary>
        [Required]
        public SpendingPlanning SpendingPlanning { get; set; }

        /// <summary>
        /// Child's behavior when money runs out
        /// </summary>
        [Required]
        public OutOfMoneyBehavior OutOfMoneyBehavior { get; set; }

        /// <summary>
        /// Whether spending prevents saving
        /// </summary>
        [Required]
        public SpendingAffectsSaving SpendingAffectsSaving { get; set; }

        /// <summary>
        /// Pace at which child spends allowance
        /// </summary>
        [Required]
        public SpendingPace SpendingPace { get; set; }

        // ==================== Section 4: Savings ====================

        /// <summary>
        /// What child typically saves for
        /// </summary>
        [Required]
        public SavingGoal SavingGoal { get; set; }

        /// <summary>
        /// Percentage of allowance typically saved
        /// </summary>
        [Required]
        public SavingPercentage SavingPercentage { get; set; }

        /// <summary>
        /// How often child reaches savings goals
        /// </summary>
        [Required]
        public SavingSuccessRate SavingSuccessRate { get; set; }

        // ==================== Section 5: Moods & Habits ====================

        /// <summary>
        /// Child's typical feeling after spending
        /// </summary>
        [Required]
        public FeelingAfterSpending FeelingAfterSpending { get; set; }

        /// <summary>
        /// Main reason child fails to save
        /// </summary>
        [Required]
        public SavingFailureReason SavingFailureReason { get; set; }

        /// <summary>
        /// Child's preference between immediate vs delayed gratification
        /// </summary>
        [Required]
        public SatisfactionPreference SatisfactionPreference { get; set; }

        /// <summary>
        /// How often child talks about money matters
        /// </summary>
        [Required]
        public TalksAboutMoney TalksAboutMoney { get; set; }

        /// <summary>
        /// Child's emotional response to growing savings
        /// </summary>
        [Required]
        public FeelingWhenSavingGrows FeelingWhenSavingGrows { get; set; }

        // ==================== Section 6: Financial Personality ====================

        /// <summary>
        /// What child would do with unexpected $100
        /// </summary>
        [Required]
        public ReactionTo100 ReactionTo100 { get; set; }

        /// <summary>
        /// Child's overall money priority
        /// </summary>
        [Required]
        public MoneyPriority MoneyPriority { get; set; }

        /// <summary>
        /// Child's approach to expensive desired items
        /// </summary>
        [Required]
        public ReactionToExpensiveItem ReactionToExpensiveItem { get; set; }

        /// <summary>
        /// What child would do if allowance increased
        /// </summary>
        [Required]
        public ReactionToMoreAllowance ReactionToMoreAllowance { get; set; }

        /// <summary>
        /// Child's overall money mindset
        /// </summary>
        [Required]
        public MoneyMindset MoneyMindset { get; set; }

        // ==================== Foreign Keys ====================

        /// <summary>
        /// Foreign key to Child table.
        /// The child this questionnaire is about.
        /// </summary>
        [Required]
        public int ChildID { get; set; }

        /// <summary>
        /// Foreign key to PersonalityType table (nullable).
        /// The personality type calculated from this questionnaire.
        /// Null until AI analysis is performed.
        /// </summary>
        public int? CalculatedTypeID { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Reference to the child this questionnaire is about
        /// </summary>
        [ForeignKey("ChildID")]
        public virtual Child Child { get; set; }

        /// <summary>
        /// Reference to the calculated personality type (if analysis has been performed)
        /// </summary>
        [ForeignKey("CalculatedTypeID")]
        public virtual PersonalityType? CalculatedPersonalityType { get; set; }
    }
}