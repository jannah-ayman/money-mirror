// MoneyMirror.Infrastructure/Services/ChatbotService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Chatbot;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Infrastructure.Data;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace MoneyMirror.Infrastructure.Services
{
    public class ChatbotService : IChatbotService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChatbotService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _aiServiceUrl;
        private const int WindowDays = 30;

        public ChatbotService(
            ApplicationDbContext context,
            ILogger<ChatbotService> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _aiServiceUrl = configuration["AIService:Url"] ?? "http://localhost:5000";
        }

        public async Task<(bool success, ChatbotResponseDto? response, string errorMessage)>
        GetChildReplyAsync(int childId, ChildChatRequestDto dto)
        {
            try
            {
                var child = await _context.Children
                    .Include(c => c.PersonalityType)
                    .FirstOrDefaultAsync(c => c.ChildID == childId);

                if (child == null)
                    return (false, null, "Child not found");

                var windowStart = DateTime.UtcNow.AddDays(-WindowDays);

                var expenses = await _context.Expenses
                    .Include(e => e.ExpenseCategory)
                    .Include(e => e.Mood)
                    .Where(e => e.ChildID == childId && e.LogDate >= windowStart)
                    .ToListAsync();

                var allowance = await _context.Allowances
                    .FirstOrDefaultAsync(a => a.ChildID == childId && a.IsRecurring && a.IsActive);

                var activeGoal = await _context.SavingsGoals
                    .Where(g => g.ChildID == childId && g.Status == "Active")
                    .OrderByDescending(g => g.TargetAmount > 0 ? g.CurrentAmount / g.TargetAmount : 0)
                    .FirstOrDefaultAsync();

                var topCategory = expenses
                    .GroupBy(e => e.ExpenseCategory.Name)
                    .OrderByDescending(g => g.Sum(e => e.MoneyAmount))
                    .FirstOrDefault()?.Key ?? "None yet";

                var topMood = expenses
                    .GroupBy(e => e.Mood.Description)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault()?.Key ?? "None yet";

                double goalPercent = activeGoal != null && activeGoal.TargetAmount > 0
                    ? Math.Round((double)(activeGoal.CurrentAmount / activeGoal.TargetAmount) * 100, 1)
                    : 0;

                var payload = new
                {
                    role = "child",
                    message = dto.Message,
                    history = dto.History ?? new List<ChatMessage>(),
                    age = child.Age,
                    personality_type = child.PersonalityType?.ParentName ?? "Pending Analysis",
                    personality_child_name = child.PersonalityType?.ChildName ?? "Money Explorer",
                    current_balance = child.CurrentBalance,
                    allowance_amount = allowance?.Amount ?? 0,
                    allowance_type = allowance?.Type ?? "Not set",
                    top_spending_category = topCategory,
                    top_mood_when_spending = topMood,
                    total_spent_last_30_days = expenses.Sum(e => e.MoneyAmount),
                    active_goal_title = activeGoal?.Title ?? "No active goal",
                    active_goal_progress_percent = goalPercent,
                    quiz_count = child.QuizCount
                };

                return await CallAiChatAsync(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetChildReplyAsync for child {ChildId}", childId);
                return (false, null, "An error occurred");
            }
        }

        public async Task<(bool success, ChatbotResponseDto? response, string errorMessage)>
    GetParentReplyAsync(int parentId, ParentChatRequestDto dto)
        {
            try
            {
                bool isLinked = await _context.ParentChildren
                    .AnyAsync(pc => pc.ParentID == parentId && pc.ChildID == dto.ChildId);

                if (!isLinked)
                    return (false, null, "You are not authorized to ask about this child");

                var parent = await _context.Parents.FindAsync(parentId);
                var child = await _context.Children
                    .Include(c => c.PersonalityType)
                    .FirstOrDefaultAsync(c => c.ChildID == dto.ChildId);

                if (parent == null || child == null)
                    return (false, null, "Parent or child not found");

                var windowStart = DateTime.UtcNow.AddDays(-14);

                var expenses = await _context.Expenses
                    .Include(e => e.ExpenseCategory)
                    .Include(e => e.Mood)
                    .Where(e => e.ChildID == dto.ChildId && e.LogDate >= windowStart)
                    .ToListAsync();

                var activeGoals = await _context.SavingsGoals
                    .Where(g => g.ChildID == dto.ChildId && g.Status == "Active")
                    .ToListAsync();

                var completedGoals = await _context.SavingsGoals
                    .Where(g => g.ChildID == dto.ChildId && g.Status == "Success" && g.StartDate >= windowStart)
                    .ToListAsync();

                var allowance = await _context.Allowances
                    .FirstOrDefaultAsync(a => a.ChildID == dto.ChildId && a.IsRecurring && a.IsActive);

                var allTemplates = await _context.AnalysisAdviceTemplates.ToListAsync();

                decimal totalSpend = expenses.Sum(e => e.MoneyAmount);

                var goalSavings = await _context.Transactions
                    .Where(t => t.ChildID == dto.ChildId && t.Type == "GoalTransfer" && t.TransactionDate >= windowStart)
                    .SumAsync(t => Math.Abs(t.Amount));

                // Build flattened strings
                var traits = child.PersonalityType?.Traits != null
                    ? string.Join(", ", System.Text.Json.JsonSerializer.Deserialize<List<string>>(child.PersonalityType.Traits) ?? new())
                    : "Not available";

                var recommendations = child.PersonalityType?.StaticRecommendation != null
                    ? string.Join(" | ", System.Text.Json.JsonSerializer.Deserialize<List<string>>(child.PersonalityType.StaticRecommendation) ?? new())
                    : "Not available";

                var dimensions = child.ImpulsiveSpenderScore.HasValue
                    ? $"Impulsive Spender: {child.ImpulsiveSpenderScore:F1}% | Prudent Saver: {child.PrudentSaverScore:F1}% | Goal-Oriented Planner: {child.GoalOrientedPlannerScore:F1}% | Bargain Hunter: {child.BargainHunterScore:F1}% (last updated: {child.LastPersonalityUpdateDate?.ToString("yyyy-MM-dd") ?? "never"})"
                    : "Not yet calculated";

                var topCategory = expenses
                    .GroupBy(e => e.ExpenseCategory.Name)
                    .OrderByDescending(g => g.Sum(e => e.MoneyAmount))
                    .FirstOrDefault();

                var topMood = expenses
                    .GroupBy(e => e.Mood.Description)
                    .OrderByDescending(g => g.Sum(e => e.MoneyAmount))
                    .FirstOrDefault();

                var bestGoal = activeGoals
                    .Where(g => g.TargetAmount > 0)
                    .OrderByDescending(g => g.CurrentAmount / g.TargetAmount)
                    .FirstOrDefault();

                var recentActivity = $"Total spent: {totalSpend:F2} EGP across {expenses.Count} purchases. " +
                    $"Top category: {topCategory?.Key ?? "none"} ({topCategory?.Sum(e => e.MoneyAmount):F2} EGP). " +
                    $"Top spending mood: {topMood?.Key ?? "none"}. " +
                    $"Saved toward goals: {goalSavings:F2} EGP. " +
                    $"Current balance: {child.CurrentBalance:F2} EGP. " +
                    $"Allowance: {(allowance != null ? $"{allowance.Amount:F2} EGP {allowance.Type}" : "not set")}. " +
                    $"Active goals: {activeGoals.Count}. Completed goals (this period): {completedGoals.Count}. " +
                    (bestGoal != null ? $"Most progressed goal: \"{bestGoal.Title}\" at {Math.Round((double)(bestGoal.CurrentAmount / bestGoal.TargetAmount) * 100, 1)}%." : "No active goals.");

                // Evaluate triggers using the same logic as AnalysisService
                var triggeredAlerts = new List<string>();
                var triggeredStrengths = new List<string>();

                if (expenses.Any() && totalSpend > 0 && topMood != null)
                {
                    double moodPct = (double)(topMood.Sum(e => e.MoneyAmount) / totalSpend) * 100;
                    if (moodPct > 60)
                    {
                        var t = allTemplates.FirstOrDefault(x => x.TriggerKey == "HIGH_MOOD_SPENDING");
                        if (t != null) triggeredAlerts.Add($"{t.Title}: {child.FName} spends {Math.Round(moodPct)}% of their money when {topMood.Key}.");
                    }
                }

                if (!activeGoals.Any())
                {
                    var t = allTemplates.FirstOrDefault(x => x.TriggerKey == "NO_ACTIVE_GOALS");
                    if (t != null) triggeredAlerts.Add($"{t.Title}: {child.FName} has no active savings goals.");
                }

                if (completedGoals.Count >= 2)
                {
                    var t = allTemplates.FirstOrDefault(x => x.TriggerKey == "GOAL_STREAK");
                    if (t != null) triggeredStrengths.Add($"{t.Title}: {child.FName} completed {completedGoals.Count} goals recently.");
                }

                if (bestGoal != null)
                {
                    double progress = (double)(bestGoal.CurrentAmount / bestGoal.TargetAmount) * 100;
                    if (progress >= 50)
                    {
                        var t = allTemplates.FirstOrDefault(x => x.TriggerKey == "SAVING_PROGRESS");
                        if (t != null) triggeredStrengths.Add($"{t.Title}: {child.FName} is {Math.Round(progress)}% toward \"{bestGoal.Title}\".");
                    }
                }

                var alertsText = triggeredAlerts.Any()
                    ? string.Join(" | ", triggeredAlerts)
                    : "No active alerts";

                var strengthsText = triggeredStrengths.Any()
                    ? string.Join(" | ", triggeredStrengths)
                    : "No current strengths triggered";

                var payload = new
                {
                    role = "parent",
                    message = dto.Message,
                    history = dto.History ?? new List<ChatMessage>(),
                    parent_first_name = parent.FName,
                    child_first_name = child.FName,
                    child_age = child.Age,
                    personality_parent_name = child.PersonalityType?.ParentName ?? "Pending Analysis",
                    personality_type = MapPersonalityToKey(child.PersonalityType?.ParentName),
                    traits,
                    static_recommendations = recommendations,
                    behavioral_dimensions = dimensions,
                    recent_activity = recentActivity,
                    alerts = alertsText,
                    strengths = strengthsText
                };

                return await CallAiChatAsync(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetParentReplyAsync for child {ChildId}", dto.ChildId);
                return (false, null, "An error occurred");
            }
        }

        private async Task<(bool success, ChatbotResponseDto? response, string errorMessage)>
            CallAiChatAsync(object payload)
        {
            try
            {
                var httpResponse = await _httpClient.PostAsJsonAsync($"{_aiServiceUrl}/api/chat", payload);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    var err = await httpResponse.Content.ReadAsStringAsync();
                    _logger.LogWarning("AI chatbot returned {Status}: {Error}", httpResponse.StatusCode, err);
                    return (false, null, "AI service error");
                }

                var result = await httpResponse.Content.ReadFromJsonAsync<AiChatResponse>();

                if (result == null || !result.Success)
                    return (false, null, result?.Error ?? "Unknown error from AI service");

                return (true, new ChatbotResponseDto { Response = result.Response }, string.Empty);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Could not connect to AI chatbot service");
                return (false, null, "Could not connect to AI service");
            }
        }

        private static string MapPersonalityToKey(string? parentName) => parentName switch
        {
            "Impulsive Spender" => "IMPULSIVE_SPENDER",
            "Prudent Saver" => "PRUDENT_SAVER",
            "Goal-Oriented Planner" => "GOAL_ORIENTED_PLANNER",
            "Bargain Hunter" => "BARGAIN_HUNTER",
            _ => "PENDING_ANALYSIS"
        };

        private class AiChatResponse
        {
            [JsonPropertyName("success")]
            public bool Success { get; set; }

            [JsonPropertyName("response")]
            public string? Response { get; set; }

            [JsonPropertyName("error")]
            public string? Error { get; set; }
        }
    }
}