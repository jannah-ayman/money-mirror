using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.Enums;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Infrastructure.Data;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MoneyMirror.Infrastructure.Services
{
    /// <summary>
    /// Service that calls the Python Flask AI API to calculate personality types.
    /// </summary>
    public class AIPersonalityService : IAIPersonalityService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AIPersonalityService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _aiServiceUrl;

        public AIPersonalityService(
            ApplicationDbContext context,
            ILogger<AIPersonalityService> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();

            // Get the AI service URL from configuration
            // Default to localhost:5000 if not configured
            _aiServiceUrl = configuration["AIService:Url"] ?? "http://localhost:5000";
        }

        public async Task<(bool success, string? parentPersonalityName, string errorMessage)>
            CalculatePersonalityFromQuestionnaireAsync(int questionnaireId)
        {
            try
            {
                // STEP 1: Get the questionnaire from database
                var questionnaire = await _context.InitialProfilingQuestionnaires
                    .FirstOrDefaultAsync(q => q.QuestionnaireID == questionnaireId);

                if (questionnaire == null)
                {
                    _logger.LogWarning($"Questionnaire {questionnaireId} not found");
                    return (false, null, "Questionnaire not found");
                }

                // STEP 2: Convert the questionnaire answers to the format the Python API expects
                var spendingCategories = JsonSerializer.Deserialize<List<SpendingCategory>>(
                    questionnaire.SpendingCategories) ?? new List<SpendingCategory>();

                // Convert enum values to strings that Python expects
                var requestData = new
                {
                    spending_pace = ConvertSpendingPace(questionnaire.SpendingPace),
                    tries_to_save = questionnaire.TriesToSave == TriesToSave.Yes ? "Yes" : "No",
                    out_of_money_behavior = ConvertOutOfMoneyBehavior(questionnaire.OutOfMoneyBehavior),
                    reaction_to_100 = ConvertReactionTo100(questionnaire.ReactionTo100),
                    money_mindset = ConvertMoneyMindset(questionnaire.MoneyMindset),
                    spending_categories = spendingCategories.Select(c => ConvertSpendingCategory(c)).ToList(),
                    feeling_after_spending = questionnaire.FeelingAfterSpending.ToString(),
                    feeling_when_saving_grows = ConvertFeelingWhenSavingGrows(questionnaire.FeelingWhenSavingGrows)
                };

                _logger.LogInformation($"Calling Python AI service at {_aiServiceUrl}/api/personality/calculate");
                _logger.LogInformation($"Request data: {JsonSerializer.Serialize(requestData)}");

                // STEP 3: Call the Python API
                var response = await _httpClient.PostAsJsonAsync(
                    $"{_aiServiceUrl}/api/personality/calculate",
                    requestData);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Python AI service returned error: {response.StatusCode} - {errorContent}");
                    return (false, null, $"AI service error: {response.StatusCode}");
                }

                // STEP 4: Parse the response
                var result = await response.Content.ReadFromJsonAsync<AIServiceResponse>();

                if (result == null || !result.Success)
                {
                    _logger.LogError($"Python AI service returned unsuccessful result");
                    return (false, null, result?.Error ?? "Unknown error from AI service");
                }

                _logger.LogInformation($"Python AI calculated personality: {result.ParentName} (Key: {result.PersonalityKey})");

                return (true, result.ParentName, string.Empty);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Failed to connect to Python AI service: {ex.Message}");
                _logger.LogError($"Make sure the Python Flask service is running on {_aiServiceUrl}");
                return (false, null, "Could not connect to AI service. Is it running?");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calling Python AI service: {ex.Message}");
                return (false, null, $"Error: {ex.Message}");
            }
        }

        // ==================== HELPER METHODS TO CONVERT ENUMS TO PYTHON FORMAT ====================

        private string ConvertSpendingPace(SpendingPace pace)
        {
            return pace switch
            {
                SpendingPace.Spends_Right_Away => "Spends it right away",
                SpendingPace.Spends_Gradually => "Spends it gradually",
                SpendingPace.Saves_Part_Of_It => "Saves part of it",
                _ => "Unknown"
            };
        }

        private string ConvertOutOfMoneyBehavior(OutOfMoneyBehavior behavior)
        {
            return behavior switch
            {
                OutOfMoneyBehavior.Ask_For_More => "Ask for more allowance",
                OutOfMoneyBehavior.Stop_Spending => "Stop spending",
                OutOfMoneyBehavior.Postpone_Purchases => "Postpone purchases",
                _ => "Unknown"
            };
        }

        private string ConvertReactionTo100(ReactionTo100 reaction)
        {
            return reaction switch
            {
                ReactionTo100.Spend_All_Now => "Spend it all now on something fun",
                ReactionTo100.Spend_Part_Save_Part => "Spend part, save part",
                ReactionTo100.Save_All_For_Future => "Save it all for a future goal",
                _ => "Unknown"
            };
        }

        private string ConvertMoneyMindset(MoneyMindset mindset)
        {
            return mindset switch
            {
                MoneyMindset.Enjoys_Spending_Immediately => "Enjoys spending immediately",
                MoneyMindset.Balances_Spending_And_Saving => "Balances spending and saving",
                MoneyMindset.Saves_For_The_Future => "Saves for the future",
                _ => "Unknown"
            };
        }

        private string ConvertSpendingCategory(SpendingCategory category)
        {
            return category switch
            {
                SpendingCategory.Food_And_Drinks => "Food & drinks",
                SpendingCategory.Entertainment => "Entertainment",
                SpendingCategory.Clothes_And_Accessories => "Clothes & accessories",
                SpendingCategory.School_Supplies => "School supplies",
                _ => "Unknown"
            };
        }

        private string ConvertFeelingWhenSavingGrows(FeelingWhenSavingGrows feeling)
        {
            return feeling switch
            {
                FeelingWhenSavingGrows.Motivated => "Motivated",
                FeelingWhenSavingGrows.Proud => "Proud",
                FeelingWhenSavingGrows.Doesnt_Matter_Much => "Doesn't matter much",
                _ => "Unknown"
            };
        }

        // ==================== RESPONSE CLASS ====================

        /// <summary>
        /// Represents the response from the Python AI service.
        /// Property names MUST match what Python sends.
        /// </summary>
        private class AIServiceResponse
        {
            // ✅ Use JsonPropertyName to match Python's response

            [JsonPropertyName("success")]
            public bool Success { get; set; }

            [JsonPropertyName("personality_key")]
            public string? PersonalityKey { get; set; }

            [JsonPropertyName("parent_name")]
            public string? ParentName { get; set; }

            [JsonPropertyName("child_name")]
            public string? ChildName { get; set; }

            [JsonPropertyName("message")]
            public string? Message { get; set; }

            [JsonPropertyName("error")]
            public string? Error { get; set; }
        }
    }
}