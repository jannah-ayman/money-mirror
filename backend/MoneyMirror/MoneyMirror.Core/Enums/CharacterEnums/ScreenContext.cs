using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMirror.Core.Enums.CharacterEnums
{
    /// <summary>
    /// Screen contexts that determine character state.
    /// Maps to specific pages/features in the child app.
    /// </summary>
    public enum ScreenContext
    {
        Dashboard,              // Main home screen → Idle or Encouraging
        Profile,                // Profile view → Idle
        Balance,                // Balance screen → Neutral
        LogExpense,             // Expense logging → Thinking
        ExpenseHistory,         // Past expenses → Neutral
        GoalProgress,           // Savings goals → Encouraging or Proud
        GoalCompleted,          // Just reached goal → Celebrating
        LowBalance,             // Balance warning → Worried
        Achievement,            // Badge earned → Excited
        Quiz,                   // Story quiz → Curious
        TransactionHistory,     // Allowance/bonus history → Neutral
        SavingsGrowth           // Savings increased → Happy or Proud
    }
}
