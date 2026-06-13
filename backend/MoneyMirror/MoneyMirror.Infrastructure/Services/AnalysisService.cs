using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Analysis;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Infrastructure.Data;
using System.Text.Json;

namespace MoneyMirror.Infrastructure.Services
{
    public class AnalysisService : IAnalysisService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AnalysisService> _logger;

        private const int AnalysisWindowDays = 14;

        private static readonly Dictionary<string, int> FrequencyThresholds = new()
        {
            { "Daily",   18 },
            { "Weekly",  10 },
            { "Monthly",  6 }
        };

        public AnalysisService(ApplicationDbContext context, ILogger<AnalysisService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(bool success, ChildAnalysisDto? analysis, string errorMessage)>
            GetChildAnalysisAsync(int parentId, int childId)
        {
            try
            {
                bool isLinked = await _context.ParentChildren
                    .AnyAsync(pc => pc.ParentID == parentId && pc.ChildID == childId);

                if (!isLinked)
                    return (false, null, "You are not authorized to view this child's analysis.");

                var child = await _context.Children
                    .Include(c => c.PersonalityType)
                    .FirstOrDefaultAsync(c => c.ChildID == childId);

                if (child == null)
                    return (false, null, "Child not found.");

                var windowStart = DateTime.UtcNow.AddDays(-AnalysisWindowDays);

                var expenses = await _context.Expenses
                    .Include(e => e.ExpenseCategory)
                    .Include(e => e.Mood)
                    .Where(e => e.ChildID == childId && e.LogDate >= windowStart)
                    .ToListAsync();

                var activeGoals = await _context.SavingsGoals
                    .Where(g => g.ChildID == childId && g.Status == "Active")
                    .ToListAsync();

                var completedGoals = await _context.SavingsGoals
                    .Where(g => g.ChildID == childId && g.Status == "Success" && g.StartDate >= windowStart)
                    .ToListAsync();

                var allowance = await _context.Allowances
                    .Where(a => a.ChildID == childId && a.IsRecurring && a.IsActive)
                    .FirstOrDefaultAsync();

                // Section 1: Personality profile
                var personalitySection = BuildPersonalitySection(child.PersonalityType);

                // Section 2: Behavioral dimensions from stored scores
                BehavioralDimensionsDto? dimensions = null;
                if (child.ImpulsiveSpenderScore.HasValue)
                {
                    dimensions = new BehavioralDimensionsDto
                    {
                        ImpulsiveSpender = child.ImpulsiveSpenderScore.Value,
                        PrudentSaver = child.PrudentSaverScore!.Value,
                        GoalOrientedPlanner = child.GoalOrientedPlannerScore!.Value,
                        BargainHunter = child.BargainHunterScore!.Value,
                        LastUpdatedDate = child.LastPersonalityUpdateDate
                    };
                }

                // Section 3: Threshold-based advice cards
                var allTemplates = await _context.AnalysisAdviceTemplates.ToListAsync();
                var triggeredAlerts = new List<TriggeredAdviceCardDto>();
                var triggeredStrengths = new List<TriggeredAdviceCardDto>();

                var goalSavings = await _context.Transactions
                    .Where(t => t.ChildID == childId
                             && t.Type == "GoalTransfer"
                             && t.TransactionDate >= windowStart)
                    .SumAsync(t => Math.Abs(t.Amount));

                var triggered = EvaluateTriggers(expenses, activeGoals, completedGoals, allowance, child.FName, goalSavings);

                foreach (var (triggerKey, dynamicDetail) in triggered)
                {
                    var template = allTemplates.FirstOrDefault(t => t.TriggerKey == triggerKey);
                    if (template == null) continue;

                    var card = new TriggeredAdviceCardDto
                    {
                        Title = template.Title,
                        Description = template.Description,
                        DynamicDetail = dynamicDetail,
                        ActionSteps = JsonSerializer.Deserialize<List<string>>(template.ActionSteps) ?? new(),
                        Type = template.Type,
                        Priority = template.Priority
                    };

                    if (template.Type == "Alert")
                        triggeredAlerts.Add(card);
                    else
                        triggeredStrengths.Add(card);
                }

                triggeredAlerts = triggeredAlerts.OrderBy(c => c.Priority).ToList();
                triggeredStrengths = triggeredStrengths.OrderBy(c => c.Priority).ToList();

                return (true, new ChildAnalysisDto
                {
                    PersonalityProfile = personalitySection,
                    BehavioralDimensions = dimensions,
                    Alerts = triggeredAlerts,
                    Strengths = triggeredStrengths,
                    DataWindowNote = $"Based on the last {AnalysisWindowDays} days"
                }, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating analysis for child {ChildId}", childId);
                return (false, null, "An error occurred while generating the analysis.");
            }
        }

        private static PersonalityProfileSectionDto BuildPersonalitySection(Core.Models.PersonalityType? pt)
        {
            if (pt == null)
                return new PersonalityProfileSectionDto
                {
                    ParentName = "Pending Analysis",
                    Description = "Personality analysis is still in progress.",
                    Traits = new(),
                    StaticRecommendations = new()
                };

            return new PersonalityProfileSectionDto
            {
                ParentName = pt.ParentName,
                Description = pt.Desc,
                Traits = JsonSerializer.Deserialize<List<string>>(pt.Traits) ?? new(),
                StaticRecommendations = JsonSerializer.Deserialize<List<string>>(pt.StaticRecommendation) ?? new()
            };
        }

        private static Dictionary<string, string> EvaluateTriggers(
    List<Core.Models.Expense> expenses,
    List<Core.Models.SavingsGoal> activeGoals,
    List<Core.Models.SavingsGoal> completedGoals,
    Core.Models.Allowance? allowance,
    string childFirstName,
    decimal goalSavings)
        {
            var triggered = new Dictionary<string, string>();
            decimal totalSpend = expenses.Sum(e => e.MoneyAmount);

            // HIGH_MOOD_SPENDING
            if (expenses.Any() && totalSpend > 0)
            {
                var topMood = expenses
                    .GroupBy(e => e.Mood.Description)
                    .Select(g => new { Mood = g.Key, Total = g.Sum(e => e.MoneyAmount) })
                    .OrderByDescending(g => g.Total)
                    .FirstOrDefault();

                if (topMood != null)
                {
                    double pct = (double)(topMood.Total / totalSpend) * 100;
                    if (pct > 60)
                        triggered["HIGH_MOOD_SPENDING"] =
                            $"{childFirstName} spends {Math.Round(pct)}% of their money when {topMood.Mood}.";
                }
            }

            // LOW_SAVINGS_RATIO
            if (allowance != null && allowance.Amount > 0)
            {
                decimal expectedIncome = allowance.Type switch
                {
                    "Daily" => allowance.Amount * AnalysisWindowDays,
                    "Weekly" => allowance.Amount * (AnalysisWindowDays / 7m),
                    "Monthly" => allowance.Amount * (AnalysisWindowDays / 30m),
                    _ => allowance.Amount
                };

                decimal savingsRatio = goalSavings / expectedIncome;
                decimal spendRatio = totalSpend / expectedIncome;
                if (savingsRatio < 0.15m && spendRatio > 0.5m)
                    triggered["LOW_SAVINGS_RATIO"] =
                        $"{childFirstName} spent {Math.Round((double)spendRatio * 100)}% of their expected allowance income but only saved {Math.Round((double)savingsRatio * 100)}% toward goals in the last {AnalysisWindowDays} days.";
            }

            // HIGH_SPENDING_FREQUENCY
            string allowanceType = allowance?.Type ?? "Weekly";
            int frequencyThreshold = FrequencyThresholds.GetValueOrDefault(allowanceType, 10);
            if (allowance != null && allowance.Amount > 0)
            {
                decimal avgSpendPerPurchase = expenses.Count > 0 ? totalSpend / expenses.Count : 0;
                decimal avgAllowancePerDay = allowance.Type switch
                {
                    "Daily" => allowance.Amount,
                    "Weekly" => allowance.Amount / 7m,
                    "Monthly" => allowance.Amount / 30m,
                    _ => allowance.Amount / 7m
                };
                decimal smallPurchaseThreshold = avgAllowancePerDay * 0.25m;

                if (expenses.Count > frequencyThreshold && avgSpendPerPurchase > smallPurchaseThreshold)
                    triggered["HIGH_SPENDING_FREQUENCY"] =
                        $"{childFirstName} made {expenses.Count} purchases averaging {Math.Round(avgSpendPerPurchase, 1)} EGP each in the last {AnalysisWindowDays} days.";
            }
            // IMPULSIVE_CATEGORY_FOCUS
            if (expenses.Any() && totalSpend > 0)
            {
                var topCategory = expenses
                    .GroupBy(e => e.ExpenseCategory.Name)
                    .Select(g => new { Category = g.Key, Total = g.Sum(e => e.MoneyAmount) })
                    .OrderByDescending(g => g.Total)
                    .FirstOrDefault();

                if (topCategory != null)
                {
                    double pct = (double)(topCategory.Total / totalSpend) * 100;
                    if (pct > 55)
                        triggered["IMPULSIVE_CATEGORY_FOCUS"] =
                            $"{Math.Round(pct)}% of {childFirstName}'s spending went to {topCategory.Category}.";
                }
            }

            // NO_ACTIVE_GOALS
            if (!activeGoals.Any())
                triggered["NO_ACTIVE_GOALS"] =
                    $"{childFirstName} has no active savings goals right now.";

            // LOW_BALANCE_DRAIN
            if (allowance != null && allowance.Amount > 0 && totalSpend > 0)
            {
                // How many cycles fit in the analysis window
                decimal expectedIncome = allowance.Type switch
                {
                    "Daily" => allowance.Amount * AnalysisWindowDays,        // 14 payments
                    "Weekly" => allowance.Amount * (AnalysisWindowDays / 7m), // 2 payments
                    "Monthly" => allowance.Amount * (AnalysisWindowDays / 30m),// 0.46 of a payment
                    _ => allowance.Amount
                };

                double drainRatio = (double)(totalSpend / expectedIncome);
                if (drainRatio >= 0.70)
                    triggered["LOW_BALANCE_DRAIN"] =
                        $"{childFirstName} spent {Math.Round(drainRatio * 100)}% of their expected {allowance.Type.ToLower()} allowance income in the last {AnalysisWindowDays} days.";
            }

            // GOAL_STREAK
            if (completedGoals.Count >= 2)
                triggered["GOAL_STREAK"] =
                    $"{childFirstName} completed {completedGoals.Count} savings goals recently.";

            // CONSISTENT_LOGGING
            if (expenses.Any())
            {
                var week1 = expenses.Any(e => e.LogDate >= DateTime.UtcNow.AddDays(-7));
                var week2 = expenses.Any(e => e.LogDate < DateTime.UtcNow.AddDays(-7));
                if (week1 && week2)
                    triggered["CONSISTENT_LOGGING"] =
                        $"{childFirstName} logged expenses in both weeks of the analysis period.";
            }

            // BALANCED_MOOD_SPENDING
            if (expenses.Any() && totalSpend > 0)
            {
                var maxMoodPct = expenses
                    .GroupBy(e => e.Mood.Description)
                    .Max(g => (double)(g.Sum(e => e.MoneyAmount) / totalSpend) * 100);

                if (maxMoodPct <= 40)
                    triggered["BALANCED_MOOD_SPENDING"] =
                        $"{childFirstName}'s spending is well-distributed across different moods.";
            }

            // SAVING_PROGRESS
            var bestGoal = activeGoals
                .Where(g => g.TargetAmount > 0)
                .OrderByDescending(g => g.CurrentAmount / g.TargetAmount)
                .FirstOrDefault();

            if (bestGoal != null)
            {
                double progress = (double)(bestGoal.CurrentAmount / bestGoal.TargetAmount) * 100;
                if (progress >= 50)
                    triggered["SAVING_PROGRESS"] =
                        $"{childFirstName} is {Math.Round(progress)}% of the way to their \"{bestGoal.Title}\" goal.";
            }

            return triggered;
        }
    }
}