using MoneyMirror.Core.DTOs.Character;
using MoneyMirror.Core.Enums.CharacterEnums;

namespace MoneyMirror.Core.Interfaces
{
    public interface ICharacterService
    {
        Task<List<CharacterInfoDto>> GetAvailableCharactersAsync();

        Task<(bool success, SelectCharacterResponseDto? response, string errorMessage)>
            SelectCharacterAsync(int childId, SelectCharacterDto dto);

        Task<(bool success, CharacterStateResponseDto? response, string errorMessage)>
            GetCharacterStateAsync(int childId, GetCharacterStateDto dto);

        Task<(bool success, string? imageUrl, string errorMessage)>
            GetProfilePictureUrlAsync(int childId);

        CharacterState DetermineCharacterState(ScreenContext screenContext, string? contextData);

        string? GenerateCharacterMessage(int characterId, CharacterState state, ScreenContext screenContext);
    }
}