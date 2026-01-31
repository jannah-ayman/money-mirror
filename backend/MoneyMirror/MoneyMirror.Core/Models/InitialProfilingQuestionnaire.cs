using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MoneyMirror.Core.Enums;

namespace MoneyMirror.Core.Models
{
    /// <summary>
    /// Stores the 28-feature survey completed by parents when adding a new child.
    /// Establishes preliminary financial personality profile before the child starts using the app.
    /// One-to-one relationship with Child.
    /// </summary>
    public class InitialProfilingQuestionnaire
    {
        /// <summary>
        /// Primary key for the InitialProfilingQuestionnaire entity.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuestionnaireID { get; set; }

        // ==================== Section 1 – Child Profile ====================

        /// <summary>
        /// The child's age group.
        /// </summary>
        [Required]
        public ChildAgeGroup ChildAgeGroup { get; set; }

        /// <summary>
        /// The child's gender.
        /// </summary>
        [Required]
        public ChildGender ChildGender { get; set; }

        /// <summary>
        /// Whether the child receives a regular allowance.
        /// </summary>
        [Required]
        public bool HasAllowance { get; set; }

        // ==================== Section 2 – Allowance Details (Conditional) ====================
        // These fields are nullable — they only appear if HasAllowance = true.

        /// <summary>
        /// How often the child receives allowance.
        /// Nullable: only filled if HasAllowance = true.
        /// </summary>
        public AllowanceFrequency? AllowanceFrequency { get; set; }

        /// <summary>
        /// Average allowance amount.
        /// Nullable: only filled if HasAllowance = true.
        /// </summary>
        [Column(TypeName = "DECIMAL(10,2)")]
        public decimal? AllowanceAmount { get; set; }

        // ==================== Section 3 – Expenses ====================

        /// <summary>
        /// Categories the child spends on (multi-select).
        /// Stored as comma-separated values in the DB.
        /// </summary>
        [Required]
        [Column(TypeName = "NVARCHAR(500)")]
        public string SpendingCategoriesJson { get; set; }

        /// <summary>
        /// Average amount the child spends.
        /// </summary>
        [Required]
        [Column(TypeName = "DECIMAL(10,2)")]
        public decimal AverageSpendingAmount { get; set; }

        /// <summary>
        /// How often the child plans before spending.
        /// </summary>
        [Required]
        public SpendingPlanning SpendingPlanning { get; set; }

        /// <summary>
        /// What the child does when the allowance runs out.
        /// </summary>
        [Required]
        public OutOfMoneyBehavior OutOfMoneyBehavior { get; set; }

        /// <summary>
        /// How much spending affects the child's saving habits.
        /// </summary>
        [Required]
        public SpendingAffectsSaving SpendingAffectsSaving { get; set; }

        /// <summary>
        /// Whether the child tracks their expenses.
        /// </summary>
        [Required]
        public bool TracksExpenses { get; set; }

        /// <summary>
        /// How fast the child spends their money.
        /// </summary>
        [Required]
        public SpendingPace SpendingPace { get; set; }

        // ==================== Section 4 – Savings ====================

        /// <summary>
        /// Whether the child tries to save.
        /// </summary>
        [Required]
        public bool TriesToSave { get; set; }

        /// <summary>
        /// What the child saves for.
        /// Nullable: only filled if TriesToSave = true.
        /// </summary>
        public SavingGoal? SavingGoal { get; set; }

        /// <summary>
        /// Percentage of allowance the child saves.
        /// Nullable: only filled if TriesToSave = true.
        /// </summary>
        public SavingPercentage? SavingPercentage { get; set; }

        /// <summary>
        /// How often the child successfully saves.
        /// Nullable: only filled if TriesToSave = true.
        /// </summary>
        public SavingSuccessRate? SavingSuccessRate { get; set; }

        // ==================== Section 5 – Moods & Habits ====================

        /// <summary>
        /// How the child feels after spending.
        /// </summary>
        [Required]
        public FeelingAfterSpending FeelingAfterSpending { get; set; }

        /// <summary>
        /// Why the child fails to save.
        /// </summary>
        [Required]
        public SavingFailureReason SavingFailureReason { get; set; }

        /// <summary>
        /// The child's preference — spend now or save for later.
        /// </summary>
        [Required]
        public SatisfactionPreference SatisfactionPreference { get; set; }

        /// <summary>
        /// How often the child talks about money.
        /// </summary>
        [Required]
        public TalksAboutMoney TalksAboutMoney { get; set; }

        /// <summary>
        /// How the child feels when their savings grow.
        /// </summary>
        [Required]
        public FeelingWhenSavingGrows FeelingWhenSavingGrows { get; set; }

        /// <summary>
        /// Likert scale (1–5): how emotional the child gets when buying something.
        /// </summary>
        [Required]
        [Range(1, 5)]
        public int EmotionalSpendingScore { get; set; }

        // ==================== Section 6 – Financial Personality ====================

        /// <summary>
        /// What the child would do with 100 pounds.
        /// </summary>
        [Required]
        public ReactionTo100 ReactionTo100 { get; set; }

        /// <summary>
        /// What matters most to the child regarding money.
        /// </summary>
        [Required]
        public MoneyPriority MoneyPriority { get; set; }

        /// <summary>
        /// How the child reacts to an expensive item they want.
        /// </summary>
        [Required]
        public ReactionToExpensiveItem ReactionToExpensiveItem { get; set; }

        /// <summary>
        /// How the child reacts when they get more allowance.
        /// </summary>
        [Required]
        public ReactionToMoreAllowance ReactionToMoreAllowance { get; set; }

        /// <summary>
        /// The child's overall money mindset.
        /// </summary>
        [Required]
        public MoneyMindset MoneyMindset { get; set; }

        /// <summary>
        /// Likert scale (1–5): how often the child looks for deals/bargains.
        /// </summary>
        [Required]
        [Range(1, 5)]
        public int BargainHuntingScore { get; set; }

        // ==================== Metadata ====================

        /// <summary>
        /// Foreign key to PersonalityType table.
        /// AI-calculated personality type based on questionnaire responses.
        /// </summary>
        public int? CalculatedTypeID { get; set; }

        /// <summary>
        /// Indicates whether the questionnaire has been completed and processed.
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
        /// One questionnaire per child.
        /// </summary>
        [Required]
        public int ChildID { get; set; }

        // ==================== Navigation Properties ====================

        [ForeignKey("ChildID")]
        public virtual Child  Child { get; set; }

        [ForeignKey("CalculatedTypeID")]
        public virtual PersonalityType  CalculatedPersonalityType { get; set; }
    }
}