namespace MoneyMirror.Core.DTOs.Character
{
    public class CharacterStateResponseDto
    {
        public int CharacterID { get; set; }
        public string CharacterType { get; set; }
        public string State { get; set; }
        public string ImageUrl { get; set; }
        public string? Message { get; set; }
    }
}