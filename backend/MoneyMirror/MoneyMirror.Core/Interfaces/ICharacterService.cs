using MoneyMirror.Core.DTOs.Character;

namespace MoneyMirror.Core.Interfaces
{
    public interface ICharacterService
    {
        Task<List<CharacterInfoDto>> GetAvailableCharactersAsync();

        Task<(bool success, SelectCharacterResponseDto response, string errorMessage)>
            SelectCharacterAsync(int childId, SelectCharacterDto dto);

        Task<(bool success, CharacterStateResponseDto response, string errorMessage)>
            GetCharacterStateAsync(int childId, GetCharacterStateDto dto);

        Task<(bool success, string imageUrl, string errorMessage)>
            GetProfilePictureUrlAsync(int childId);
    }
}
