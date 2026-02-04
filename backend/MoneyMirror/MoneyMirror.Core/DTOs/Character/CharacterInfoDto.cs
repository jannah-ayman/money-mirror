namespace MoneyMirror.Core.DTOs.Character
{
    /// <summary>
    /// DTO for displaying available characters.
    /// Maps from Character model.
    /// </summary>
    public class CharacterInfoDto
    {
        public int CharacterID { get; set; }
        public string CharacterType { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string PreviewImageUrl { get; set; }
    }
}