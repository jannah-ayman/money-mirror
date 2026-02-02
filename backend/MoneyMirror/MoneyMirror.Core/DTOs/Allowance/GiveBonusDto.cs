namespace MoneyMirror.Core.DTOs.Allowance
{
    /// <summary>
    /// Data Transfer Object for giving a one-time bonus to a child.
    /// Parent specifies amount and reason.
    /// Bonus is credited immediately to child's balance.
    /// Used as input for POST /api/allowance/{childId}/bonus endpoint.
    /// Validation is handled by GiveBonusDtoValidator using FluentValidation.
    /// </summary>
    public class GiveBonusDto
    {
        /// <summary>
        /// Amount of bonus to give.
        /// Must be greater than zero.
        /// Example: 100.00 = 100 Egyptian Pounds
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Reason for giving this bonus.
        /// Shown in transaction history for both parent and child.
        /// Examples:
        /// - "Excellent report card!"
        /// - "Helped with chores all week"
        /// - "Birthday gift"
        /// Required - helps track why bonuses were given.
        /// </summary>
        public string Reason { get; set; }
    }
}