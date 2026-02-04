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
        public async Task<List<CharacterInfoDto>> GetAvailableCharactersAsync()
        {
            return await _context.Characters
                .Select(c => new CharacterInfoDto
                {
                    CharacterID = c.CharacterID,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.DefaultImageUrl
                })
                .ToListAsync();
        }
        public async Task<(bool success, SelectCharacterResponseDto response, string errorMessage)>
    SelectCharacterAsync(int childId, SelectCharacterDto dto)
        {
            var child = await _context.Children.FindAsync(childId);
            if (child == null)
                return (false, null, "Child not found");

            var character = await _context.Characters.FindAsync(dto.CharacterID);
            if (character == null)
                return (false, null, "Character not found");

            child.CharacterID = character.CharacterID;
            await _context.SaveChangesAsync();

            return (true, new SelectCharacterResponseDto
            {
                CharacterID = character.CharacterID,
                CharacterName = character.Name,
                Message = $"You chose {character.Name}! 🚀"
            }, null);
        }
        public async Task<(bool success, CharacterStateResponseDto response, string errorMessage)>
            GetCharacterStateAsync(int childId, GetCharacterStateDto dto)
        {
            var child = await _context.Children
                .Include(c => c.SelectedCharacter)
                    .ThenInclude(ch => ch.CharacterStates)
                .FirstOrDefaultAsync(c => c.ChildID == childId);

            if (child == null)
                return (false, null, "Child not found");

            if (child.SelectedCharacter == null)
                return (false, null, "No character selected");

            var state = child.SelectedCharacter.CharacterStates
                .FirstOrDefault(s => s.ScreenContext == dto.ScreenContext);

            return (true, new CharacterStateResponseDto
            {
                CharacterName = child.SelectedCharacter.Name,
                ImageUrl = state?.ImageUrl ?? child.SelectedCharacter.DefaultImageUrl,
                Message = state?.Message ?? $"{child.SelectedCharacter.Name} is here to help! 🚀"
            }, null);
        }
        public async Task<(bool success, string imageUrl, string errorMessage)>
    GetProfilePictureUrlAsync(int childId)
        {
            var child = await _context.Children
                .Include(c => c.SelectedCharacter)
                .FirstOrDefaultAsync(c => c.ChildID == childId);

            if (child == null)
                return (false, null, "Child not found");

            if (child.SelectedCharacter == null)
                return (false, null, "No character selected");

            return (true, child.SelectedCharacter.DefaultImageUrl, null);
        }

    }
}