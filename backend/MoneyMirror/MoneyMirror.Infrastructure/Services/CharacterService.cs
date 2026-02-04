using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Character;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Infrastructure.Data;

namespace MoneyMirror.Infrastructure.Services
{
    /// <summary>
    /// Simple service for character operations.
    /// Handles character selection and image retrieval.
    /// </summary>
    public class CharacterService : ICharacterService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CharacterService> _logger;

        public CharacterService(
            ApplicationDbContext context,
            ILogger<CharacterService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets all available characters.
        /// </summary>
        public async Task<List<CharacterDto>> GetAllCharactersAsync()
        {
            try
            {
                var characters = await _context.Characters
                    .Select(c => new CharacterDto
                    {
                        CharacterID = c.CharacterID,
                        Name = c.Name,
                        Description = c.Description,
                        DefaultImageUrl = c.DefaultImageUrl
                    })
                    .ToListAsync();

                return characters;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting characters: {ex.Message}");
                return new List<CharacterDto>();
            }
        }

        /// <summary>
        /// Selects a character for a child.
        /// </summary>
        public async Task<(bool success, string message)> SelectCharacterAsync(
            int childId,
            int characterId)
        {
            try
            {
                // STEP 1: Find child
                var child = await _context.Children.FindAsync(childId);

                if (child == null)
                {
                    return (false, "Child not found");
                }

                // STEP 2: Verify character exists
                var character = await _context.Characters.FindAsync(characterId);

                if (character == null)
                {
                    return (false, "Character not found");
                }

                // STEP 3: Update child's selected character
                child.CharacterID = characterId;

                _context.Children.Update(child);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Child {childId} selected character {character.Name}");

                return (true, $"You chose {character.Name}! 🚀");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error selecting character: {ex.Message}");
                return (false, "An error occurred");
            }
        }

        /// <summary>
        /// Gets character image for current screen.
        /// Returns specific state image if exists, otherwise default image.
        /// </summary>
        public async Task<(bool success, CharacterImageResponseDto image, string errorMessage)>
            GetCharacterImageAsync(int childId, string screenContext)
        {
            try
            {
                // STEP 1: Find child and their selected character
                var child = await _context.Children
                    .Include(c => c.SelectedCharacter)
                        .ThenInclude(ch => ch.CharacterStates)
                    .FirstOrDefaultAsync(c => c.ChildID == childId);

                if (child == null)
                {
                    return (false, null, "Child not found");
                }

                if (child.SelectedCharacter == null)
                {
                    return (false, null, "No character selected. Please choose a character first!");
                }

                // STEP 2: Try to find specific state for this screen
                var characterState = child.SelectedCharacter.CharacterStates
                    .FirstOrDefault(s => s.ScreenContext == screenContext);

                // STEP 3: Build response (use state if found, otherwise default)
                var response = new CharacterImageResponseDto
                {
                    CharacterName = child.SelectedCharacter.Name,
                    ImageUrl = characterState?.ImageUrl ?? child.SelectedCharacter.DefaultImageUrl,
                    Message = characterState?.Message ?? $"{child.SelectedCharacter.Name} is here to help! 🚀"
                };

                return (true, response, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting character image: {ex.Message}");
                return (false, null, "An error occurred");
            }
        }
    }
}