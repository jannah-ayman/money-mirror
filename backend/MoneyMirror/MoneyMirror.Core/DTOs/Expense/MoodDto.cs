namespace MoneyMirror.Core.DTOs.Expense
{
    /// <summary>
    /// Data Transfer Object for returning a mood.
    /// Used for mood picker in the frontend.
    /// </summary>
    public class MoodDto
    {
        /// <summary>
        /// Mood ID.
        /// </summary>
        public int MoodID { get; set; }

        /// <summary>
        /// Mood description.
        /// Example: "Happy"
        /// </summary>
        public string Description { get; set; }
    }
}