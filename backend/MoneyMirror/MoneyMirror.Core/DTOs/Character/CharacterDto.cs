namespace MoneyMirror.Core.DTOs.Character
{
    /// <summary>
    /// Simple DTO for character information.
    /// Used when listing available characters for selection.
    /// </summary>
    public class CharacterDto
    {
        public int CharacterID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DefaultImageUrl { get; set; }
    }

    /// <summary>
    /// DTO for selecting a character.
    /// </summary>
    public class SelectCharacterDto
    {
        public int CharacterID { get; set; }
    }

    /// <summary>
    /// DTO for getting character image for a specific screen.
    /// </summary>
    public class GetCharacterImageDto
    {
        /// <summary>
        /// Which screen the child is currently on.
        /// Examples: "Dashboard", "Expenses", "Savings", "Goals"
        /// </summary>
        public string ScreenContext { get; set; }
    }

    /// <summary>
    /// Response DTO with character image and message.
    /// </summary>
    public class CharacterImageResponseDto
    {
        public string CharacterName { get; set; }
        public string ImageUrl { get; set; }
        public string Message { get; set; }
    }
}