using MoneyMirror.Core.Enums.CharacterEnums;

namespace MoneyMirror.Core.DTOs.Character
{
    public class GetCharacterStateDto
    {
        public ScreenContext ScreenContext { get; set; }
        public string? ContextData { get; set; }
    }
}