using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Character;
using MoneyMirror.Core.Enums;
using MoneyMirror.Core.Enums.CharacterEnums;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Infrastructure.Data;
using System.Text.Json;

namespace MoneyMirror.Infrastructure.Services
{
    /// <summary>
    /// Service implementing character selection and state management logic.
    /// Handles character selection, dynamic state determination, and image URL generation.
    /// All character images are stored locally in wwwroot/characters/{character}/{state}.png
    /// </summary>
    public class CharacterService : ICharacterService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CharacterService> _logger;

        // Base path for character images (relative to wwwroot)
        private const string CHARACTER_BASE_PATH = "/characters";

        public CharacterService(
            ApplicationDbContext context,
            ILogger<CharacterService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets all available characters with their information.
        /// Character data is hardcoded as it's fixed content.
        /// </summary>
        public async Task<List<CharacterInfoDto>> GetAvailableCharactersAsync()
        {
            // Character information is static and predefined
            var characters = new List<CharacterInfoDto>
            {
                new CharacterInfoDto
                {
                    CharacterType = "Nova",
                    DisplayName = "Nova the Explorer",
                    Description = "Energetic and loves adventures! Nova is always excited to help you reach your goals! 🚀",
                    PreviewImageUrl = $"{CHARACTER_BASE_PATH}/nova/idle.png"
                },
                new CharacterInfoDto
                {
                    CharacterType = "Luna",
                    DisplayName = "Luna the Thinker",
                    Description = "Calm and thoughtful! Luna helps you make smart decisions about your money. 🌙",
                    PreviewImageUrl = $"{CHARACTER_BASE_PATH}/luna/idle.png"
                },
                new CharacterInfoDto
                {
                    CharacterType = "Cosmo",
                    DisplayName = "Cosmo the Curious",
                    Description = "Curious and playful! Cosmo loves learning new things about saving and spending! ⭐",
                    PreviewImageUrl = $"{CHARACTER_BASE_PATH}/cosmo/idle.png"
                },
                new CharacterInfoDto
                {
                    CharacterType = "Aura",
                    DisplayName = "Aura the Wise",
                    Description = "Wise and encouraging! Aura believes in you and celebrates every achievement! ✨",
                    PreviewImageUrl = $"{CHARACTER_BASE_PATH}/aura/idle.png"
                }
            };

            await Task.CompletedTask; // For async consistency
            return characters;
        }

        /// <summary>
        /// Updates a child's selected character.
        /// </summary>
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

                // STEP 2: Validate character type
                if (!Enum.IsDefined(typeof(CharacterType), dto.CharacterType))
                {
                    _logger.LogWarning($"Invalid character type selected: {dto.CharacterType}");
                    return (false, null, "Invalid character type");
                }

                // STEP 3: Update child's character (stored as string)
                string characterName = dto.CharacterType.ToString();
                child.SelectedCharacter = characterName;

                // STEP 4: Generate profile picture URL (idle state)
                string profileImageUrl = GenerateImageUrl(dto.CharacterType, CharacterState.Idle);
                child.AvatarUrl = profileImageUrl;

                // STEP 5: Save changes
                _context.Children.Update(child);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Child {childId} selected character: {characterName}");

                // STEP 6: Build response
                var response = new SelectCharacterResponseDto
                {
                    CharacterType = characterName,
                    ProfileImageUrl = profileImageUrl,
                    Message = $"You chose {GetCharacterDisplayName(dto.CharacterType)}! Great choice! 🎉"
                };

                return (true, response, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error selecting character for child {childId}: {ex.Message}");
                return (false, null, "An error occurred while selecting character");
            }
        }

        /// <summary>
        /// Gets character state and image for current screen context.
        /// </summary>
        public async Task<(bool success, CharacterStateResponseDto? response, string errorMessage)>
            GetCharacterStateAsync(int childId, GetCharacterStateDto dto)
        {
            try
            {
                // STEP 1: Find child and get their selected character
                var child = await _context.Children.FindAsync(childId);

                if (child == null)
                {
                    _logger.LogWarning($"Character state request for non-existent child {childId}");
                    return (false, null, "Child not found");
                }

                // STEP 2: Get character type (default to Nova if not set)
                CharacterType characterType = CharacterType.Nova;
                if (!string.IsNullOrEmpty(child.SelectedCharacter))
                {
                    if (!Enum.TryParse<CharacterType>(child.SelectedCharacter, out characterType))
                    {
                        _logger.LogWarning($"Invalid character type stored for child {childId}: {child.SelectedCharacter}");
                        characterType = CharacterType.Nova; // Fallback
                    }
                }

                // STEP 3: Determine appropriate character state based on context
                CharacterState state = DetermineCharacterState(dto.ScreenContext, dto.ContextData);

                // STEP 4: Generate image URL
                string imageUrl = GenerateImageUrl(characterType, state);

                // STEP 5: Generate character message (optional)
                string? message = GenerateCharacterMessage(characterType, state, dto.ScreenContext);

                // STEP 6: Build response
                var response = new CharacterStateResponseDto
                {
                    CharacterType = characterType.ToString(),
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

        /// <summary>
        /// Gets child's profile picture URL (idle state of selected character).
        /// </summary>
        public async Task<(bool success, string? imageUrl, string errorMessage)>
            GetProfilePictureUrlAsync(int childId)
        {
            try
            {
                var child = await _context.Children.FindAsync(childId);

                if (child == null)
                {
                    return (false, null, "Child not found");
                }

                // Return stored avatar URL or generate default
                if (!string.IsNullOrEmpty(child.AvatarUrl))
                {
                    return (true, child.AvatarUrl, string.Empty);
                }

                // No character selected - return default (Nova idle)
                string defaultUrl = GenerateImageUrl(CharacterType.Nova, CharacterState.Idle);
                return (true, defaultUrl, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting profile picture for child {childId}: {ex.Message}");
                return (false, null, "An error occurred");
            }
        }

        // ==================== HELPER METHODS ====================

        /// <summary>
        /// Determines appropriate character state based on screen context and additional data.
        /// This is the core logic for dynamic character states.
        /// </summary>
        public CharacterState DetermineCharacterState(ScreenContext screenContext, string? contextData)
        {
            // Parse context data if provided
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

            // Determine state based on screen and data
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

        /// <summary>
        /// Determines state for goal progress screen based on progress percentage.
        /// </summary>
        private CharacterState DetermineGoalProgressState(ContextDataModel? data)
        {
            if (data?.GoalProgress == null)
                return CharacterState.Encouraging;

            // Near completion (80%+) → Excited
            if (data.GoalProgress >= 80)
                return CharacterState.Excited;

            // Good progress (50%+) → Proud
            if (data.GoalProgress >= 50)
                return CharacterState.Proud;

            // Some progress (25%+) → Encouraging
            if (data.GoalProgress >= 25)
                return CharacterState.Encouraging;

            // Just started → Encouraging
            return CharacterState.Encouraging;
        }

        /// <summary>
        /// Generates image URL for character and state.
        /// Format: /characters/{character_name}/{state}.png
        /// Example: /characters/nova/happy.png
        /// </summary>
        private string GenerateImageUrl(CharacterType characterType, CharacterState state)
        {
            string characterName = characterType.ToString().ToLower();
            string stateName = state.ToString().ToLower();

            return $"{CHARACTER_BASE_PATH}/{characterName}/{stateName}.png";
        }

        /// <summary>
        /// Generates contextual message from character based on state and screen.
        /// Each character has slightly different personality in their messages.
        /// </summary>
        public string? GenerateCharacterMessage(CharacterType characterType, CharacterState state, ScreenContext screenContext)
        {
            // Generate message based on character personality and context
            return (characterType, state, screenContext) switch
            {
                // Dashboard messages
                (CharacterType.Nova, CharacterState.Idle, ScreenContext.Dashboard) => "Ready for today's adventure? Let's check your balance! 🚀",
                (CharacterType.Luna, CharacterState.Idle, ScreenContext.Dashboard) => "Welcome back! Let's see how you're doing today. 🌙",
                (CharacterType.Cosmo, CharacterState.Idle, ScreenContext.Dashboard) => "Hi there! What fun things shall we explore today? ⭐",
                (CharacterType.Aura, CharacterState.Idle, ScreenContext.Dashboard) => "Hello! I'm proud of your progress so far! ✨",

                // Low balance warnings
                (CharacterType.Nova, CharacterState.Worried, _) => "Uh oh! Your balance is getting low. Time to save up! 💰",
                (CharacterType.Luna, CharacterState.Worried, _) => "Let's think carefully about our next purchase. Balance is low. 💭",
                (CharacterType.Cosmo, CharacterState.Worried, _) => "Hmm, we should be careful with spending now! 🤔",
                (CharacterType.Aura, CharacterState.Worried, _) => "Don't worry! Let's make a plan to build your balance back up. 🌟",

                // Goal completion celebrations
                (CharacterType.Nova, CharacterState.Celebrating, ScreenContext.GoalCompleted) => "YES! You did it! Goal reached! I knew you could! 🎉",
                (CharacterType.Luna, CharacterState.Celebrating, ScreenContext.GoalCompleted) => "Wonderful! Your patience and planning paid off! 🌙✨",
                (CharacterType.Cosmo, CharacterState.Celebrating, ScreenContext.GoalCompleted) => "WOOHOO! That was amazing! You're a star! ⭐🎊",
                (CharacterType.Aura, CharacterState.Celebrating, ScreenContext.GoalCompleted) => "I'm so proud of you! You achieved your goal! ✨🎉",

                // Expense logging
                (CharacterType.Nova, CharacterState.Thinking, ScreenContext.LogExpense) => "Let me help you track this expense! 🚀",
                (CharacterType.Luna, CharacterState.Thinking, ScreenContext.LogExpense) => "Good idea to log this. It helps you understand your spending! 💭",
                (CharacterType.Cosmo, CharacterState.Thinking, ScreenContext.LogExpense) => "What did you buy? Let's record it together! 📝",
                (CharacterType.Aura, CharacterState.Thinking, ScreenContext.LogExpense) => "Tracking expenses is a wise habit! ✨",

                // Goal progress
                (CharacterType.Nova, CharacterState.Proud, ScreenContext.GoalProgress) => "Look how much you've saved! Keep going! 💪",
                (CharacterType.Luna, CharacterState.Proud, ScreenContext.GoalProgress) => "Your steady progress is impressive! 🌙",
                (CharacterType.Cosmo, CharacterState.Proud, ScreenContext.GoalProgress) => "Wow! You're doing great! Almost there! ⭐",
                (CharacterType.Aura, CharacterState.Proud, ScreenContext.GoalProgress) => "I knew you had it in you! Keep saving! ✨",

                // Achievements
                (CharacterType.Nova, CharacterState.Excited, ScreenContext.Achievement) => "A new badge! You're on fire! 🔥",
                (CharacterType.Luna, CharacterState.Excited, ScreenContext.Achievement) => "Well deserved! Your hard work is paying off! 🌟",
                (CharacterType.Cosmo, CharacterState.Excited, ScreenContext.Achievement) => "AWESOME! A new achievement unlocked! 🏆",
                (CharacterType.Aura, CharacterState.Excited, ScreenContext.Achievement) => "Magnificent! Another achievement earned! ✨",

                // Quizzes
                (CharacterType.Nova, CharacterState.Curious, ScreenContext.Quiz) => "Ooh, a quiz! I love learning new things! 🚀",
                (CharacterType.Luna, CharacterState.Curious, ScreenContext.Quiz) => "Let's think through this carefully together. 🤔",
                (CharacterType.Cosmo, CharacterState.Curious, ScreenContext.Quiz) => "Quiz time! This is going to be fun! 📚",
                (CharacterType.Aura, CharacterState.Curious, ScreenContext.Quiz) => "Every question helps you learn and grow! 💡",

                // Default: no message
                _ => null
            };
        }

        /// <summary>
        /// Gets display name for character type.
        /// </summary>
        private string GetCharacterDisplayName(CharacterType characterType)
        {
            return characterType switch
            {
                CharacterType.Nova => "Nova the Explorer",
                CharacterType.Luna => "Luna the Thinker",
                CharacterType.Cosmo => "Cosmo the Curious",
                CharacterType.Aura => "Aura the Wise",
                _ => characterType.ToString()
            };
        }

        // ==================== HELPER CLASSES ====================

        /// <summary>
        /// Model for parsing context data JSON.
        /// Contains additional info for state determination.
        /// </summary>
        private class ContextDataModel
        {
            public bool? HasLowBalance { get; set; }
            public decimal? CurrentBalance { get; set; }
            public decimal? GoalProgress { get; set; } // Percentage (0-100)
            public int? ExpenseCount { get; set; }
            public bool? IsFirstTime { get; set; }
        }
    }
}