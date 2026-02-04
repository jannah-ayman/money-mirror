namespace MoneyMirror.Core.DTOs.Character
{
    public class SelectCharacterResponseDto
    {
        public int CharacterID { get; set; }
        public string CharacterType { get; set; }
        public string DisplayName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Message { get; set; }
    }
}