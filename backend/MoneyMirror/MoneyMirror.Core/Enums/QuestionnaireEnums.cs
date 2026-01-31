namespace MoneyMirror.Core.Enums
{
    // ==================== Section 1 – Child Profile ====================

    public enum ChildAgeGroup
    {
        Age_6_8,
        Age_9_11,
        Age_12_14
    }

    public enum ChildGender
    {
        Male,
        Female
    }

    // ==================== Section 2 – Allowance Details ====================

    public enum AllowanceFrequency
    {
        Daily,
        Weekly,
        Monthly
    }

    // ==================== Section 3 – Expenses ====================

    public enum SpendingCategory
    {
        Food_Drinks,
        Entertainment,
        Clothes_Accessories,
        School_Supplies,
        Other
    }

    public enum SpendingPlanning
    {
        Always,
        Sometimes,
        Never
    }

    public enum OutOfMoneyBehavior
    {
        Ask_More,
        Stop_Spending,
        Postpone_Purchases,
        Other
    }

    public enum SpendingAffectsSaving
    {
        Yes_Often,
        Sometimes,
        Rarely,
        No
    }

    public enum SpendingPace
    {
        Spends_Right_Away,
        Spends_Gradually,
        Spreads_Over_Time,
        Saves_Most,
        Rarely_Spends
    }

    // ==================== Section 4 – Savings ====================

    public enum SavingGoal
    {
        Toy_Or_Game,
        Something_Big,
        Trip_Or_Outing,
        Emergencies,
        Other
    }

    public enum SavingPercentage
    {
        Less_Than_10,
        Between_10_30,
        More_Than_30,
        Does_Not_Save
    }

    public enum SavingSuccessRate
    {
        Always,
        Sometimes,
        Rarely,
        Never
    }

    // ==================== Section 5 – Moods & Habits ====================

    public enum FeelingAfterSpending
    {
        Happy,
        Regretful,
        Neutral
    }

    public enum SavingFailureReason
    {
        Unexpected_Expenses,
        Lack_Self_Control,
        Low_Allowance,
        Other
    }

    public enum SatisfactionPreference
    {
        Spend_Immediately,
        Save_For_Later,
        Balance_Both
    }

    public enum TalksAboutMoney
    {
        Often,
        Sometimes,
        Never
    }

    public enum FeelingWhenSavingGrows
    {
        Motivated,
        Proud,
        Doesnt_Matter
    }

    // ==================== Section 6 – Financial Personality ====================

    public enum ReactionTo100
    {
        Spend_All,
        Spend_And_Save,
        Save_All
    }

    public enum MoneyPriority
    {
        Enjoy_Now,
        Balance_Enjoy_Save,
        Save_For_Future
    }

    public enum ReactionToExpensiveItem
    {
        Buy_Immediately,
        Save_Gradually,
        Skip_And_Save
    }

    public enum ReactionToMoreAllowance
    {
        Spend_More,
        Save_More,
        Mix_Both
    }

    public enum MoneyMindset
    {
        Enjoy_Moment,
        Enjoy_And_Prepare,
        Secure_Future
    }
}