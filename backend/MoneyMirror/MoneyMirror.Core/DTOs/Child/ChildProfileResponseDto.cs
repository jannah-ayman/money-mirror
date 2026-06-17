namespace MoneyMirror.Core.DTOs.Child
{
    /// <summary>
    /// Data Transfer Object for returning a child's profile information.
    /// Used for the "My Profile" screen in the child app.
    /// Shows child's name, personality, and basic info.
    /// </summary>
    public class ChildProfileResponseDto
    {
        /// <summary>
        /// Child's unique ID
        /// </summary>
        public int ChildID { get; set; }

        /// <summary>
        /// Child's first name
        /// Example: "Emma"
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Child's last name
        /// Example: "Smith"
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Child's age calculated from DOB
        /// Example: 8
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Child's gender (optional)
        /// Example: "Girl", "Boy", or null
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// Current account balance
        /// Example: 125.50
        /// </summary>
        public decimal CurrentBalance { get; set; }

        /// <summary>
        /// Personality type information (child-friendly version)
        /// </summary>
        public PersonalityInfoDto PersonalityInfo { get; set; }

    }

    /// <summary>
    /// Simplified personality info for child view
    /// </summary>
    public class PersonalityInfoDto
    {
        /// <summary>
        /// Child-friendly personality name
        /// Example: "Smart Spender"
        /// </summary>
        public string ChildName { get; set; }

        /// <summary>
        /// Fun facts about this personality
        /// Example: "You're great at planning your purchases! 🎯"
        /// </summary>
        public string? FunFacts { get; set; }

        /// <summary>
        /// Whether this is a real AI-analyzed personality or temporary
        /// </summary>
        public bool IsFinalized { get; set; }
    }
}