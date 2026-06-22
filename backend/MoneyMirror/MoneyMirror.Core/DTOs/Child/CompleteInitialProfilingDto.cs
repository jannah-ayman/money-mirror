using MoneyMirror.Core.Enums;
using System;
using System.Collections.Generic;

namespace MoneyMirror.Core.DTOs.Child
{
    /// <summary>
    /// Data Transfer Object for completing the initial profiling questionnaire.
    /// Contains child basic info and answers to the 9 profiling questions.
    /// Age and age group are calculated automatically from Date of Birth.
    /// Used as input for POST /api/children/complete-initial-profiling endpoint.
    /// Validation is handled by CompleteInitialProfilingDtoValidator using FluentValidation.
    /// </summary>
    public class CompleteInitialProfilingDto
    {
        // ==================== Child Identity ====================

        public string ChildFirstName { get; set; }

        public string ChildLastName { get; set; }


        public DateTime DOB { get; set; }

        public string? Gender { get; set; }
        /// Relationship of the parent submitting this questionnaire to the child.
        public ParentRole Role { get; set; }

        // ==================== Question 2: Does your child receive a regular allowance or income? ====================

        public HasAllowance HasAllowance { get; set; }

        // ==================== Question 3: How does your child usually spend their allowance? ====================

        /// <summary>
        /// Pace at which child spends allowance
        /// (Spends_Right_Away, Spends_Gradually, Saves_Part_Of_It)
        /// </summary>
        public SpendingPace SpendingPace { get; set; }

        // ==================== Question 4: What does your child usually spend their allowance on? (Multi-select) ====================

        /// <summary>
        /// List of spending categories the child typically spends on.
        /// Multiple selections allowed: Food_And_Drinks, Entertainment, Clothes_And_Accessories, School_Supplies
        /// This will be serialized to JSON before storing in database.
        /// Example: [SpendingCategory.Food_And_Drinks, SpendingCategory.Entertainment]
        /// </summary>
        public List<SpendingCategory> SpendingCategories { get; set; } = new List<SpendingCategory>();

        // ==================== Question 5: When your child runs out of allowance before the next payment, what do they usually do? ====================

        /// <summary>
        /// Child's behavior when money runs out
        /// (Ask_For_More, Stop_Spending, Postpone_Purchases)
        /// </summary>
        public OutOfMoneyBehavior OutOfMoneyBehavior { get; set; }

        // ==================== Question 6: Does your child try to save part of their allowance? ====================

        /// <summary>
        /// Whether child attempts to save (Yes, No)
        /// </summary>
        public TriesToSave TriesToSave { get; set; }

        // ==================== Question 7: Which statement best describes your child's attitude toward money? ====================

        /// <summary>
        /// Child's overall money mindset
        /// (Enjoys_Spending_Immediately, Balances_Spending_And_Saving, Saves_For_The_Future)
        /// </summary>
        public MoneyMindset MoneyMindset { get; set; }

        // ==================== Question 8: How does your child usually feel after spending money on non-essential items? ====================

        /// <summary>
        /// Child's typical feeling after spending
        /// (Happy, Regretful, Neutral)
        /// </summary>
        public FeelingAfterSpending FeelingAfterSpending { get; set; }

        // ==================== Question 9: How does your child feel when they see their savings grow? ====================

        /// <summary>
        /// Child's emotional response to growing savings
        /// (Motivated, Proud, Doesnt_Matter_Much)
        /// </summary>
        public FeelingWhenSavingGrows FeelingWhenSavingGrows { get; set; }

        // ==================== Question 10: If your child receives 100 EGP today, what would they do? ====================

        /// <summary>
        /// What child would do with unexpected 100 EGP
        /// (Spend_All_Now, Spend_Part_Save_Part, Save_All_For_Future)
        /// </summary>
        public ReactionTo100 ReactionTo100 { get; set; }
    }
}