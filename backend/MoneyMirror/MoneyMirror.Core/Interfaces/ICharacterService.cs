using MoneyMirror.Core.DTOs.Character;

namespace MoneyMirror.Core.Interfaces
{
    /// <summary>
    /// Simple interface for character operations.
    /// </summary>
    public interface ICharacterService
    {
        /// <summary>
        /// Gets all available space characters for selection.
        /// </summary>
        Task<List<CharacterDto>> GetAllCharactersAsync();

        /// <summary>
        /// Selects a character for a child.
        /// </summary>
        Task<(bool success, string message)> SelectCharacterAsync(int childId, int characterId);

        /// <summary>
        /// Gets the appropriate character image for the current screen.
        /// Falls back to default image if no specific state exists.
        /// </summary>
        Task<(bool success, CharacterImageResponseDto image, string errorMessage)>
            GetCharacterImageAsync(int childId, string screenContext);
    }
}