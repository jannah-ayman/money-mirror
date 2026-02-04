namespace MoneyMirror.Core.DTOs.Character
{
    /// <summary>
    /// DTO for character selection request.
    /// Uses CharacterID from database.
    /// </summary>
    public class SelectCharacterDto
    {
        public int CharacterID { get; set; }
    }
}