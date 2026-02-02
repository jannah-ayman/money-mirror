using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MoneyMirror.Core.Enums;

namespace MoneyMirror.Core.Models
{
    /// Represents the initial profiling questionnaire filled out by parents when creating a child account.
    /// Contains 10 questions about the child's financial behavior, habits, and personality.
    /// Used to establish baseline personality insights before AI analysis.
    public class InitialProfilingQuestionnaire
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuestionnaireID { get; set; }

        [Required]
        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedDate { get; set; }

        // ==================== Question 1: How old is your child? ====================

        // Child's age group category (Age_6_8, Age_9_11, Age_12_14)
        [Required]
        public ChildAgeGroup ChildAgeGroup { get; set; }

        // ==================== Question 2: Does your child receive a regular allowance or income? ====================

        // Whether the child receives regular allowance (Yes, No)
        [Required]
        public HasAllowance HasAllowance { get; set; }

        // ==================== Question 3: How does your child usually spend their allowance? ====================

        // (Spends_Right_Away, Spends_Gradually, Saves_Part_Of_It)
        [Required]
        public SpendingPace SpendingPace { get; set; }

        // ==================== Question 4: What does your child usually spend their allowance on? (Multi-select) ====================

        // JSON array of spending categories the child typically spends on.
        // Multiple selections allowed: Food_And_Drinks, Entertainment, Clothes_And_Accessories, School_Supplies
        // Stored as JSON string in database, deserialized to List<SpendingCategory> in application.
        // Example: ["Food_And_Drinks", "Entertainment"]
        [Required]
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string SpendingCategories { get; set; } // JSON array

        // ==================== Question 5: When your child runs out of allowance before the next payment, what do they usually do? ====================

        /// (Ask_For_More, Stop_Spending, Postpone_Purchases)
        [Required]
        public OutOfMoneyBehavior OutOfMoneyBehavior { get; set; }

        // ==================== Question 6: Does your child try to save part of their allowance? ====================

        /// Whether child attempts to save (Yes, No)
        [Required]
        public TriesToSave TriesToSave { get; set; }

        // ==================== Question 7: Which statement best describes your child's attitude toward money? ====================

        // Child's overall money mindset
        // (Enjoys_Spending_Immediately, Balances_Spending_And_Saving, Saves_For_The_Future)
        [Required]
        public MoneyMindset MoneyMindset { get; set; }

        // ==================== Question 8: How does your child usually feel after spending money on non-essential items? ====================

        /// Child's typical feeling after spending
        /// (Happy, Regretful, Neutral)
        [Required]
        public FeelingAfterSpending FeelingAfterSpending { get; set; }

        // ==================== Question 9: How does your child feel when they see their savings grow? ====================

        /// Child's emotional response to growing savings
        /// (Motivated, Proud, Doesnt_Matter_Much)
        [Required]
        public FeelingWhenSavingGrows FeelingWhenSavingGrows { get; set; }

        // ==================== Question 10: If your child receives 100 EGP today, what would they do? ====================

        // (Spend_All_Now, Spend_Part_Save_Part, Save_All_For_Future)
        [Required]
        public ReactionTo100 ReactionTo100 { get; set; }

        // ==================== Foreign Keys ====================

        [Required]
        public int ChildID { get; set; }

        public int? CalculatedTypeID { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        [ForeignKey("ChildID")]
        public virtual Child Child { get; set; }

        [ForeignKey("CalculatedTypeID")]
        public virtual PersonalityType? CalculatedPersonalityType { get; set; }
    }
}