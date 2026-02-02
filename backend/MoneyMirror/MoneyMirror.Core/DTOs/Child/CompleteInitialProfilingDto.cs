using MoneyMirror.Core.Enums;

public class CompleteInitialProfilingDto
{
    // ==================== Child Identity ====================
    public string ChildFirstName { get; set; }
    public string ChildLastName { get; set; }

    // Optional but recommended
    public DateTime DOB { get; set; }

    // ==================== Section 1: Child Profile ====================
    public ChildAgeGroup ChildAgeGroup { get; set; }
    public ChildGender ChildGender { get; set; }

    // ==================== Section 2: Allowance ====================
    public AllowanceFrequency AllowanceFrequency { get; set; }
    public decimal AllowanceAmount { get; set; }

    // ==================== Section 3: Expenses ====================
    public SpendingCategory PrimarySpendingCategory { get; set; }
    public SpendingPlanning SpendingPlanning { get; set; }
    public OutOfMoneyBehavior OutOfMoneyBehavior { get; set; }
    public SpendingAffectsSaving SpendingAffectsSaving { get; set; }
    public SpendingPace SpendingPace { get; set; }

    // ==================== Section 4: Savings ====================
    public SavingGoal SavingGoal { get; set; }
    public SavingPercentage SavingPercentage { get; set; }
    public SavingSuccessRate SavingSuccessRate { get; set; }

    // ==================== Section 5: Moods & Habits ====================
    public FeelingAfterSpending FeelingAfterSpending { get; set; }
    public SavingFailureReason SavingFailureReason { get; set; }
    public SatisfactionPreference SatisfactionPreference { get; set; }
    public TalksAboutMoney TalksAboutMoney { get; set; }
    public FeelingWhenSavingGrows FeelingWhenSavingGrows { get; set; }

    // ==================== Section 6: Financial Personality ====================
    public ReactionTo100 ReactionTo100 { get; set; }
    public MoneyPriority MoneyPriority { get; set; }
    public ReactionToExpensiveItem ReactionToExpensiveItem { get; set; }
    public ReactionToMoreAllowance ReactionToMoreAllowance { get; set; }
    public MoneyMindset MoneyMindset { get; set; }
}
