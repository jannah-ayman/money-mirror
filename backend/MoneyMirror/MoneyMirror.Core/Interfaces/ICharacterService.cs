using MoneyMirror.Core.DTOs.Character;
using MoneyMirror.Core.Enums;
using MoneyMirror.Core.Enums.CharacterEnums;

namespace MoneyMirror.Core.Interfaces
{
    /// <summary>
    /// Interface for character selection and state management operations.
    /// Handles character selection, state determination, and image URL generation.
    /// </summary>
    public interface ICharacterService
    {
        /// <summary>
        /// Gets list of all available characters with their information.
        /// Used for character selection screen.
        /// </summary>
        /// <returns>List of character information DTOs</returns>
        Task<List<CharacterInfoDto>> GetAvailableCharactersAsync();

        /// <summary>
        /// Updates a child's selected character.
        /// Sets character type and updates profile picture URL.
        /// </summary>
        /// <param name="childId">ID of the child (from JWT token)</param>
        /// <param name="dto">Character selection data</param>
        /// <returns>Tuple: (success flag, response data, error message)</returns>
        Task<(bool success, SelectCharacterResponseDto? response, string errorMessage)>
            SelectCharacterAsync(int childId, SelectCharacterDto dto);

        /// <summary>
        /// Gets the appropriate character state and image for current context.
        /// Determines character emotion/state based on screen and context data.
        /// </summary>
        /// <param name="childId">ID of the child (from JWT token)</param>
        /// <param name="dto">Screen context and additional data</param>
        /// <returns>Tuple: (success flag, character state response, error message)</returns>
        Task<(bool success, CharacterStateResponseDto? response, string errorMessage)>
            GetCharacterStateAsync(int childId, GetCharacterStateDto dto);

        /// <summary>
        /// Gets child's current character profile picture URL.
        /// Returns idle state image for selected character.
        /// </summary>
        /// <param name="childId">ID of the child</param>
        /// <returns>Tuple: (success flag, image URL, error message)</returns>
        Task<(bool success, string? imageUrl, string errorMessage)>
            GetProfilePictureUrlAsync(int childId);

        /// <summary>
        /// Determines appropriate character state based on screen context and data.
        /// Internal helper method used by GetCharacterStateAsync.
        /// </summary>
        /// <param name="screenContext">Current screen/page</param>
        /// <param name="contextData">Additional context (balance, progress, etc.)</param>
        /// <returns>Character state to display</returns>
        CharacterState DetermineCharacterState(ScreenContext screenContext, string? contextData);

        /// <summary>
        /// Generates character message/dialogue based on state and context.
        /// Provides encouraging, helpful, or celebratory messages.
        /// </summary>
        /// <param name="characterType">Child's selected character</param>
        /// <param name="state">Current character state</param>
        /// <param name="screenContext">Current screen</param>
        /// <returns>Character message or null</returns>
        string? GenerateCharacterMessage(CharacterType characterType, CharacterState state, ScreenContext screenContext);
    }
}