namespace MoneyMirror.Core.Enums
{
    // ==================== Question 1: Child Age Group ====================
    public enum ChildAgeGroup
    {
        Age_6_8,           // Ages 6-8
        Age_9_11,          // Ages 9-11
        Age_12_14,         // Ages 12-14
        Older_Than_14      // Ages 15+
    }

    // ==================== Question 2: Has Allowance ====================
    public enum HasAllowance
    {
        Yes,
        No
    }

    // ==================== Question 3: Spending Pace ====================
    public enum SpendingPace
    {
        Spends_Right_Away,
        Spends_Gradually,
        Saves_Part_Of_It
    }

    // ==================== Question 4: Spending Categories (Multi-select) ====================
    // This will be stored as a JSON array in the database
    public enum SpendingCategory
    {
        Food_And_Drinks,
        Entertainment,
        Clothes_And_Accessories,
        School_Supplies
    }

    // ==================== Question 5: Out of Money Behavior ====================
    public enum OutOfMoneyBehavior
    {
        Ask_For_More,
        Stop_Spending,
        Postpone_Purchases
    }

    // ==================== Question 6: Tries to Save ====================
    public enum TriesToSave
    {
        Yes,
        No
    }

    // ==================== Question 7: Money Mindset ====================
    public enum MoneyMindset
    {
        Enjoys_Spending_Immediately,
        Balances_Spending_And_Saving,
        Saves_For_The_Future
    }

    // ==================== Question 8: Feeling After Spending ====================
    public enum FeelingAfterSpending
    {
        Happy,
        Regretful,
        Neutral
    }

    // ==================== Question 9: Feeling When Saving Grows ====================
    public enum FeelingWhenSavingGrows
    {
        Motivated,
        Proud,
        Doesnt_Matter_Much
    }

    // ==================== Question 10: Reaction to 100 EGP ====================
    public enum ReactionTo100
    {
        Spend_All_Now,
        Spend_Part_Save_Part,
        Save_All_For_Future
    }
}