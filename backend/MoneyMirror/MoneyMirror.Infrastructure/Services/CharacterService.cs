using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Character;
using MoneyMirror.Core.Enums.CharacterEnums;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Infrastructure.Data;
using System.Text.Json;

namespace MoneyMirror.Infrastructure.Services
{
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
            try
            {
                var characters = await _context.Characters
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.DisplayOrder)
                    .Select(c => new CharacterInfoDto
                    {
                        CharacterID = c.CharacterID,
                        CharacterType = c.CharacterType,
                        DisplayName = c.DisplayName,
                        Description = c.Description,
                        PreviewImageUrl = $"{c.BasePath}/idle.png"
                    })
                    .ToListAsync();

                return characters;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting available characters: {ex.Message}");
                return new List<CharacterInfoDto>();
            }
        }

        public async Task<(bool success, SelectCharacterResponseDto? response, string errorMessage)>
            SelectCharacterAsync(int childId, SelectCharacterDto dto)
        {
            try
            {
                // STEP 1: Find child
                var child = await _context.Children.FindAsync(childId);

                if (child == null)
                {
                    _logger.LogWarning($"Character selection attempt for non-existent child {childId}");
                    return (false, null, "Child not found");
                }

                // STEP 2: Validate character exists
                var character = await _context.Characters.FindAsync(dto.CharacterID);

                if (character == null)
                {
                    _logger.LogWarning($"Invalid character ID selected: {dto.CharacterID}");
                    return (false, null, "Invalid character selected");
                }

                if (!character.IsActive)
                {
                    _logger.LogWarning($"Inactive character selected: {dto.CharacterID}");
                    return (false, null, "This character is not currently available");
                }

                // STEP 3: Update child's character
                child.CharacterID = character.CharacterID;

                _context.Children.Update(child);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Child {childId} selected character: {character.CharacterType}");

                // STEP 4: Build response
                var response = new SelectCharacterResponseDto
                {
                    CharacterID = character.CharacterID,
                    CharacterType = character.CharacterType,
                    DisplayName = character.DisplayName,
                    ProfileImageUrl = $"{character.BasePath}/idle.png",
                    Message = $"You chose {character.DisplayName}! Great choice! 🎉"
                };

                return (true, response, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error selecting character for child {childId}: {ex.Message}");
                return (false, null, "An error occurred while selecting character");
            }
        }

        public async Task<(bool success, CharacterStateResponseDto? response, string errorMessage)>
            GetCharacterStateAsync(int childId, GetCharacterStateDto dto)
        {
            try
            {
                // STEP 1: Find child with their selected character
                var child = await _context.Children
                    .Include(c => c.Character)
                    .FirstOrDefaultAsync(c => c.ChildID == childId);

                if (child == null)
                {
                    _logger.LogWarning($"Character state request for non-existent child {childId}");
                    return (false, null, "Child not found");
                }

                // STEP 2: Get character (use default if not selected)
                var character = child.Character;
                if (character == null)
                {
                    // Default to Nova if no character selected
                    character = await _context.Characters
                        .FirstOrDefaultAsync(c => c.CharacterType == "Nova");

                    if (character == null)
                    {
                        _logger.LogError("Default character 'Nova' not found in database");
                        return (false, null, "Character data unavailable");
                    }
                }

                // STEP 3: Determine appropriate state
                CharacterState state = DetermineCharacterState(dto.ScreenContext, dto.ContextData);

                // STEP 4: Generate image URL
                string imageUrl = $"{character.BasePath}/{state.ToString().ToLower()}.png";

                // STEP 5: Generate message
                string? message = GenerateCharacterMessage(character.CharacterID, state, dto.ScreenContext);

                // STEP 6: Build response
                var response = new CharacterStateResponseDto
                {
                    CharacterID = character.CharacterID,
                    CharacterType = character.CharacterType,
                    State = state.ToString(),
                    ImageUrl = imageUrl,
                    Message = message
                };

                return (true, response, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting character state for child {childId}: {ex.Message}");
                return (false, null, "An error occurred while getting character state");
            }
        }

        public async Task<(bool success, string? imageUrl, string errorMessage)>
            GetProfilePictureUrlAsync(int childId)
        {
            try
            {
                var child = await _context.Children
                    .Include(c => c.Character)
                    .FirstOrDefaultAsync(c => c.ChildID == childId);

                if (child == null)
                {
                    return (false, null, "Child not found");
                }

                var character = child.Character;
                if (character == null)
                {
                    // Default to Nova
                    character = await _context.Characters
                        .FirstOrDefaultAsync(c => c.CharacterType == "Nova");

                    if (character == null)
                    {
                        return (false, null, "Character data unavailable");
                    }
                }

                string imageUrl = $"{character.BasePath}/idle.png";
                return (true, imageUrl, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting profile picture for child {childId}: {ex.Message}");
                return (false, null, "An error occurred");
            }
        }

        public CharacterState DetermineCharacterState(ScreenContext screenContext, string? contextData)
        {
            ContextDataModel? data = null;
            if (!string.IsNullOrEmpty(contextData))
            {
                try
                {
                    data = JsonSerializer.Deserialize<ContextDataModel>(contextData);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Failed to parse context data: {ex.Message}");
                }
            }

            return screenContext switch
            {
                ScreenContext.Dashboard => data?.HasLowBalance == true ? CharacterState.Worried : CharacterState.Idle,
                ScreenContext.Profile => CharacterState.Idle,
                ScreenContext.Balance => data?.HasLowBalance == true ? CharacterState.Worried : CharacterState.Neutral,
                ScreenContext.LogExpense => CharacterState.Thinking,
                ScreenContext.ExpenseHistory => CharacterState.Neutral,
                ScreenContext.GoalProgress => DetermineGoalProgressState(data),
                ScreenContext.GoalCompleted => CharacterState.Celebrating,
                ScreenContext.LowBalance => CharacterState.Worried,
                ScreenContext.Achievement => CharacterState.Excited,
                ScreenContext.Quiz => CharacterState.Curious,
                ScreenContext.TransactionHistory => CharacterState.Neutral,
                ScreenContext.SavingsGrowth => CharacterState.Proud,
                _ => CharacterState.Idle
            };
        }

        private CharacterState DetermineGoalProgressState(ContextDataModel? data)
        {
            if (data?.GoalProgress == null)
                return CharacterState.Encouraging;

            if (data.GoalProgress >= 80)
                return CharacterState.Excited;

            if (data.GoalProgress >= 50)
                return CharacterState.Proud;

            if (data.GoalProgress >= 25)
                return CharacterState.Encouraging;

            return CharacterState.Encouraging;
        }

        public string? GenerateCharacterMessage(int characterId, CharacterState state, ScreenContext screenContext)
        {
            return (characterId, state, screenContext) switch
            {
                // Nova (ID 1)
                (1, CharacterState.Idle, ScreenContext.Dashboard) => "Ready for today's adventure? Let's check your balance! 🚀",
                (1, CharacterState.Worried, _) => "Uh oh! Your balance is getting low. Time to save up! 💰",
                (1, CharacterState.Celebrating, ScreenContext.GoalCompleted) => "YES! You did it! Goal reached! I knew you could! 🎉",
                (1, CharacterState.Thinking, ScreenContext.LogExpense) => "Let me help you track this expense! 🚀",
                (1, CharacterState.Proud, ScreenContext.GoalProgress) => "Look how much you've saved! Keep going! 💪",
                (1, CharacterState.Excited, ScreenContext.Achievement) => "A new badge! You're on fire! 🔥",
                (1, CharacterState.Curious, ScreenContext.Quiz) => "Ooh, a quiz! I love learning new things! 🚀",

                // Luna (ID 2)
                (2, CharacterState.Idle, ScreenContext.Dashboard) => "Welcome back! Let's see how you're doing today. 🌙",
                (2, CharacterState.Worried, _) => "Let's think carefully about our next purchase. Balance is low. 💭",
                (2, CharacterState.Celebrating, ScreenContext.GoalCompleted) => "Wonderful! Your patience and planning paid off! 🌙✨",
                (2, CharacterState.Thinking, ScreenContext.LogExpense) => "Good idea to log this. It helps you understand your spending! 💭",
                (2, CharacterState.Proud, ScreenContext.GoalProgress) => "Your steady progress is impressive! 🌙",
                (2, CharacterState.Excited, ScreenContext.Achievement) => "Well deserved! Your hard work is paying off! 🌟",
                (2, CharacterState.Curious, ScreenContext.Quiz) => "Let's think through this carefully together. 🤔",

                // Cosmo (ID 3)
                (3, CharacterState.Idle, ScreenContext.Dashboard) => "Hi there! What fun things shall we explore today? ⭐",
                (3, CharacterState.Worried, _) => "Hmm, we should be careful with spending now! 🤔",
                (3, CharacterState.Celebrating, ScreenContext.GoalCompleted) => "WOOHOO! That was amazing! You're a star! ⭐🎊",
                (3, CharacterState.Thinking, ScreenContext.LogExpense) => "What did you buy? Let's record it together! 📝",
                (3, CharacterState.Proud, ScreenContext.GoalProgress) => "Wow! You're doing great! Almost there! ⭐",
                (3, CharacterState.Excited, ScreenContext.Achievement) => "AWESOME! A new achievement unlocked! 🏆",
                (3, CharacterState.Curious, ScreenContext.Quiz) => "Quiz time! This is going to be fun! 📚",

                // Aura (ID 4)
                (4, CharacterState.Idle, ScreenContext.Dashboard) => "Hello! I'm proud of your progress so far! ✨",
                (4, CharacterState.Worried, _) => "Don't worry! Let's make a plan to build your balance back up. 🌟",
                (4, CharacterState.Celebrating, ScreenContext.GoalCompleted) => "I'm so proud of you! You achieved your goal! ✨🎉",
                (4, CharacterState.Thinking, ScreenContext.LogExpense) => "Tracking expenses is a wise habit! ✨",
                (4, CharacterState.Proud, ScreenContext.GoalProgress) => "I knew you had it in you! Keep saving! ✨",
                (4, CharacterState.Excited, ScreenContext.Achievement) => "Magnificent! Another achievement earned! ✨",
                (4, CharacterState.Curious, ScreenContext.Quiz) => "Every question helps you learn and grow! 💡",

                _ => null
            };
        }

        private class ContextDataModel
        {
            public bool? HasLowBalance { get; set; }
            public decimal? CurrentBalance { get; set; }
            public decimal? GoalProgress { get; set; }
            public int? ExpenseCount { get; set; }
            public bool? IsFirstTime { get; set; }
        }
    }
}