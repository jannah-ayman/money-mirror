using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMirror.Core.Enums.CharacterEnums
{
    /// <summary>
    /// Visual states that characters can display.
    /// State changes based on the screen/context the child is viewing.
    /// </summary>
    public enum CharacterState
    {
        Idle,           // Default state, used for profile picture
        Happy,          // Positive feedback (goal reached, good spending)
        Excited,        // Major achievement or milestone
        Thinking,       // Reviewing expenses, making decisions
        Encouraging,    // Motivational screens, goal progress
        Celebrating,    // Goal completion, badges earned
        Neutral,        // Transaction history, balance viewing
        Curious,        // Learning modules, quizzes
        Proud,          // Savings growth, consistent good behavior
        Worried         // Low balance, overspending warning
    }
}
