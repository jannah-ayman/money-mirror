using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Infrastructure.Data;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using MoneyMirror.Core.Helpers;

namespace MoneyMirror.Infrastructure.Services
{
    public class WeeklyPersonalityUpdateService : IWeeklyPersonalityUpdateService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WeeklyPersonalityUpdateService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _aiServiceUrl;
        private readonly INotificationService _notificationService;

        private const int WindowDays = 30;

        public WeeklyPersonalityUpdateService(
            ApplicationDbContext context,
            ILogger<WeeklyPersonalityUpdateService> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            INotificationService notificationService)
        {
            _context = context;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _aiServiceUrl = configuration["AIService:Url"] ?? "http://localhost:5000";
            _notificationService = notificationService;
        }

        public async Task RunWeeklyUpdateAsync()
        {
            _logger.LogInformation("Weekly personality update job started.");

            var children = await _context.Children
                .Include(c => c.PersonalityType)
                .ToListAsync();

            int updated = 0;
            int failed = 0;

            foreach (var child in children)
            {
                try
                {
                    var windowStart = DateTimeHelper.EgyptNow.AddDays(-WindowDays);

                    // Load behavioral data
                    var expenses = await _context.Expenses
                        .Include(e => e.ExpenseCategory)
                        .Include(e => e.Mood)
                        .Where(e => e.ChildID == child.ChildID && e.LogDate >= windowStart)
                        .ToListAsync();

                    var allGoals = await _context.SavingsGoals
                        .Where(g => g.ChildID == child.ChildID && g.StartDate >= windowStart)
                        .ToListAsync();

                    var completedGoals = allGoals.Where(g => g.Status == "Success").ToList();

                    var allowance = await _context.Allowances
                        .Where(a => a.ChildID == child.ChildID && a.IsRecurring && a.IsActive)
                        .FirstOrDefaultAsync();

                    var quizLogs = await _context.QuizLogs
                        .Include(q => q.QuizAnswer)
                            .ThenInclude(a => a.PersonalityType)
                        .Where(q => q.ChildID == child.ChildID && q.CompletedDate >= windowStart)
                        .ToListAsync();

                    // Build payload
                    var previousScores = new
                    {
                        impulsive_spender = (double)(child.ImpulsiveSpenderScore ?? 0),
                        prudent_saver = (double)(child.PrudentSaverScore ?? 0),
                        goal_oriented_planner = (double)(child.GoalOrientedPlannerScore ?? 0),
                        bargain_hunter = (double)(child.BargainHunterScore ?? 0)
                    };

                    var payload = BuildPayload(child.ChildID, expenses, allGoals, completedGoals, allowance, quizLogs, previousScores);

                    // Call Python
                    var response = await _httpClient.PostAsJsonAsync(
                        $"{_aiServiceUrl}/api/personality/weekly-update", payload);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("Python returned {Status} for child {ChildId}",
                            response.StatusCode, child.ChildID);
                        failed++;
                        continue;
                    }

                    var result = await response.Content.ReadFromJsonAsync<WeeklyUpdateResponse>();

                    if (result == null || !result.Success || result.Dimensions == null)
                    {
                        _logger.LogWarning("Invalid response from Python for child {ChildId}", child.ChildID);
                        failed++;
                        continue;
                    }

                    // Update personality type
                    var personalityType = await _context.PersonalityTypes
                        .FirstOrDefaultAsync(pt => pt.ParentName == result.ParentName);

                    if (personalityType != null)
                    {
                        child.TypeID = personalityType.TypeID;
                        child.IsPersonalityFinalized = true;
                    }

                    // Store dimension scores
                    child.ImpulsiveSpenderScore = (decimal)result.Dimensions.ImpulsiveSpender;
                    child.PrudentSaverScore = (decimal)result.Dimensions.PrudentSaver;
                    child.GoalOrientedPlannerScore = (decimal)result.Dimensions.GoalOrientedPlanner;
                    child.BargainHunterScore = (decimal)result.Dimensions.BargainHunter;
                    child.LastPersonalityUpdateDate = DateTimeHelper.EgyptNow;
                    await _notificationService.NotifyAllParentsOfChildAsync(
                        child.ChildID,
                        "Personality Updated 🧠",
                        $"{child.FName}'s financial personality has been updated to {result.ParentName}.",
                        $"/children/{child.ChildID}/analysis"
                    );
                    _context.Children.Update(child);
                    updated++;

                    _logger.LogInformation(
                        "Child {ChildId} updated: {PersonalityType} (I:{I} S:{S} P:{P} B:{B})",
                        child.ChildID, result.ParentName,
                        result.Dimensions.ImpulsiveSpender, result.Dimensions.PrudentSaver,
                        result.Dimensions.GoalOrientedPlanner, result.Dimensions.BargainHunter);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing child {ChildId}", child.ChildID);
                    failed++;
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Weekly personality update job completed. Updated: {Updated}, Failed: {Failed}",
                updated, failed);
        }

        private static object BuildPayload(
            int childId,
            List<Core.Models.Expense> expenses,
            List<Core.Models.SavingsGoal> allGoals,
            List<Core.Models.SavingsGoal> completedGoals,
            Core.Models.Allowance? allowance,
            List<Core.Models.QuizLog> quizLogs,
            object previousScores)
        {
            decimal totalSpend = expenses.Sum(e => e.MoneyAmount);
            decimal allowanceAmount = allowance?.Amount ?? 0;

            double savingsRatio = allowanceAmount > 0
                ? Math.Max(0, (double)((allowanceAmount - totalSpend) / allowanceAmount))
                : 0;

            double goalCompletionRate = allGoals.Count > 0
                ? Math.Round((double)completedGoals.Count / allGoals.Count, 4)
                : 0;

            var moodSpending = expenses
                .GroupBy(e => e.Mood.Description)
                .ToDictionary(
                    g => g.Key,
                    g => totalSpend > 0
                        ? Math.Round((double)(g.Sum(e => e.MoneyAmount) / totalSpend), 4)
                        : 0.0);

            var topCategories = expenses
                .GroupBy(e => e.ExpenseCategory.Name)
                .OrderByDescending(g => g.Sum(e => e.MoneyAmount))
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            var quizScores = new { impulsive = 0, saver = 0, planner = 0, bargain = 0 };
            int imp = 0, sav = 0, pla = 0, bar = 0;

            foreach (var log in quizLogs)
            {
                switch (log.QuizAnswer?.PersonalityType?.ParentName)
                {
                    case "Impulsive Spender": imp++; break;
                    case "Prudent Saver": sav++; break;
                    case "Goal-Oriented Planner": pla++; break;
                    case "Bargain Hunter": bar++; break;
                }
            }

            return new
            {
                child_id = childId,
                allowance_type = allowance?.Type ?? "Weekly",
                allowance_amount = allowanceAmount,
                spending_frequency = expenses.Count,
                savings_ratio = savingsRatio,
                goal_completion_rate = goalCompletionRate,
                mood_spending = moodSpending,
                top_categories = topCategories,
                quiz_scores = new { impulsive = imp, saver = sav, planner = pla, bargain = bar },
                previous_scores = previousScores  // ADD THIS
            };
        }

        // ==================== RESPONSE CLASSES ====================

        private class WeeklyUpdateResponse
        {
            [JsonPropertyName("success")]
            public bool Success { get; set; }

            [JsonPropertyName("personality_key")]
            public string? PersonalityKey { get; set; }

            [JsonPropertyName("parent_name")]
            public string? ParentName { get; set; }

            [JsonPropertyName("dimensions")]
            public WeeklyDimensions? Dimensions { get; set; }

            [JsonPropertyName("error")]
            public string? Error { get; set; }
        }

        private class WeeklyDimensions
        {
            [JsonPropertyName("IMPULSIVE_SPENDER")]
            public double ImpulsiveSpender { get; set; }

            [JsonPropertyName("PRUDENT_SAVER")]
            public double PrudentSaver { get; set; }

            [JsonPropertyName("GOAL_ORIENTED_PLANNER")]
            public double GoalOrientedPlanner { get; set; }

            [JsonPropertyName("BARGAIN_HUNTER")]
            public double BargainHunter { get; set; }
        }
    }
}