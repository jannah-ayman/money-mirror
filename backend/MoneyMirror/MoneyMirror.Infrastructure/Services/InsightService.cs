using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Insight;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Infrastructure.Data;
using MoneyMirror.Core.Helpers;

namespace MoneyMirror.Infrastructure.Services
{
    public class InsightService : IInsightService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InsightService> _logger;

        private const int MinExpensesThreshold = 3;

        public InsightService(ApplicationDbContext context, ILogger<InsightService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private async Task<bool> IsParentLinkedToChildAsync(int parentId, int childId) =>
            await _context.ParentChildren.AnyAsync(pc => pc.ParentID == parentId && pc.ChildID == childId);

        // ==================== KEY INSIGHTS (PARENT) ====================

        public async Task<(bool success, KeyInsightsResponseDto? response, string errorMessage)>
            GetKeyInsightsAsync(int parentId, int childId)
        {
            try
            {
                if (!await IsParentLinkedToChildAsync(parentId, childId))
                    return (false, null, "You are not authorized to view this child's insights.");

                var child = await _context.Children.FindAsync(childId);
                if (child == null)
                    return (false, null, "Child not found.");

                var expenses = await _context.Expenses
                    .Include(e => e.ExpenseCategory)
                    .Include(e => e.Mood)
                    .Where(e => e.ChildID == childId)
                    .ToListAsync();

                var response = new KeyInsightsResponseDto();

                if (expenses.Count == 0)
                {
                    response.HasData = false;
                    response.EmptyStateMessage = $"{child.FName} hasn't logged any expenses yet. Insights will appear once spending data is available.";
                    return (true, response, string.Empty);
                }

                response.HasData = true;

                // ---- Most spent category ----
                if (expenses.Count >= MinExpensesThreshold)
                {
                    var topCategory = expenses
                        .GroupBy(e => e.ExpenseCategory.Name)
                        .Select(g => new { Category = g.Key, Total = g.Sum(e => e.MoneyAmount), Grand = expenses.Sum(e => e.MoneyAmount) })
                        .OrderByDescending(x => x.Total)
                        .First();

                    int percent = (int)Math.Round((topCategory.Total / topCategory.Grand) * 100);

                    response.Insights.Add(new KeyInsightDto
                    {
                        Insight = $"{child.FName} spends {percent}% of their money on {topCategory.Category}"
                    });
                }

                // ---- Mood-spending correlation ----
                var moodGroups = expenses
                    .GroupBy(e => e.Mood.Description)
                    .Select(g => new { Mood = g.Key, Count = g.Count(), AvgAmount = g.Average(e => e.MoneyAmount) })
                    .ToList();

                var topMoodGroup = moodGroups.OrderByDescending(g => g.AvgAmount).FirstOrDefault();

                if (topMoodGroup != null && topMoodGroup.Count >= MinExpensesThreshold)
                {
                    var otherExpenses = expenses.Where(e => e.Mood.Description != topMoodGroup.Mood).ToList();

                    if (otherExpenses.Any())
                    {
                        decimal otherAvg = otherExpenses.Average(e => e.MoneyAmount);

                        if (otherAvg > 0)
                        {
                            int diffPercent = (int)Math.Round(((topMoodGroup.AvgAmount - otherAvg) / otherAvg) * 100);

                            if (diffPercent > 0)
                            {
                                response.Insights.Add(new KeyInsightDto
                                {
                                    Insight = $"{child.FName} spends {diffPercent}% more when they're {topMoodGroup.Mood} compared to other moods"
                                });
                            }
                        }
                    }
                }

                // ---- Weekend vs weekday pattern (Friday + Saturday = weekend) ----
                var weekendExpenses = expenses
                    .Where(e => e.LogDate.DayOfWeek == DayOfWeek.Friday || e.LogDate.DayOfWeek == DayOfWeek.Saturday)
                    .ToList();

                var weekdayExpenses = expenses
                    .Where(e => e.LogDate.DayOfWeek != DayOfWeek.Friday && e.LogDate.DayOfWeek != DayOfWeek.Saturday)
                    .ToList();

                if (weekendExpenses.Count >= MinExpensesThreshold)
                {
                    decimal weekendAvgPerDay = weekendExpenses.Sum(e => e.MoneyAmount) / 2m; // Fri + Sat
                    decimal weekdayAvgPerDay = weekdayExpenses.Any()
                        ? weekdayExpenses.Sum(e => e.MoneyAmount) / 5m // Sun-Thu
                        : 0;

                    if (weekdayAvgPerDay > 0)
                    {
                        decimal ratio = weekendAvgPerDay / weekdayAvgPerDay;

                        if (ratio > 1)
                        {
                            string ratioText = ratio >= 2
                                ? $"{Math.Round(ratio, 1)}x more"
                                : $"{(int)Math.Round((ratio - 1) * 100)}% more";

                            response.Insights.Add(new KeyInsightDto
                            {
                                Insight = $"{child.FName} spends {ratioText} on weekends than weekdays"
                            });
                        }
                    }
                    else
                    {
                        response.Insights.Add(new KeyInsightDto
                        {
                            Insight = $"{child.FName} only spends money on weekends so far"
                        });
                    }
                }

                // ---- Average expense per category (top 2-3) ----
                var categoryAverages = expenses
                    .GroupBy(e => e.ExpenseCategory.Name)
                    .Select(g => new { Category = g.Key, Avg = g.Average(e => e.MoneyAmount), Count = g.Count() })
                    .Where(g => g.Count >= 1)
                    .OrderByDescending(g => g.Count)
                    .Take(3)
                    .ToList();

                if (categoryAverages.Any())
                {
                    var parts = categoryAverages.Select(c => $"{c.Category.ToLower()} average {c.Avg:F2} EGP");
                    response.Insights.Add(new KeyInsightDto
                    {
                        Insight = $"{child.FName}'s {string.Join(", ", parts)}"
                    });
                }

                // ---- Goal progress summary ----
                var goalInsight = await GetGoalProgressSummaryAsync(childId, child.FName);
                response.Insights.Add(goalInsight);

                // ---- Spending trend (this month vs last month) ----
                var now = DateTimeHelper.EgyptNow;
                var startOfThisMonth = new DateTime(now.Year, now.Month, 1);
                var startOfLastMonth = startOfThisMonth.AddMonths(-1);

                decimal thisMonthTotal = expenses
                    .Where(e => e.LogDate >= startOfThisMonth)
                    .Sum(e => e.MoneyAmount);

                decimal lastMonthTotal = expenses
                    .Where(e => e.LogDate >= startOfLastMonth && e.LogDate < startOfThisMonth)
                    .Sum(e => e.MoneyAmount);

                if (lastMonthTotal == 0)
                {
                    response.Insights.Add(new KeyInsightDto
                    {
                        Insight = $"Not enough data from last month to compare {child.FName}'s spending trend yet"
                    });
                }
                else
                {
                    int trendPercent = (int)Math.Round(((thisMonthTotal - lastMonthTotal) / lastMonthTotal) * 100);

                    if (trendPercent > 0)
                    {
                        response.Insights.Add(new KeyInsightDto
                        {
                            Insight = $"{child.FName} spent {trendPercent}% more this month compared to last month"
                        });
                    }
                    else if (trendPercent < 0)
                    {
                        response.Insights.Add(new KeyInsightDto
                        {
                            Insight = $"{child.FName} spent {Math.Abs(trendPercent)}% less this month compared to last month"
                        });
                    }
                    else
                    {
                        response.Insights.Add(new KeyInsightDto
                        {
                            Insight = $"{child.FName} spent about the same this month as last month"
                        });
                    }
                }

                return (true, response, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting key insights for child {ChildId}", childId);
                return (false, null, "An error occurred while generating insights.");
            }
        }

        // ==================== FUN FACTS (CHILD) ====================

        public async Task<(bool success, FunFactsResponseDto? response, string errorMessage)>
            GetFunFactsAsync(int childId)
        {
            try
            {
                var child = await _context.Children
                    .Include(c => c.PersonalityType)
                    .FirstOrDefaultAsync(c => c.ChildID == childId);

                if (child == null)
                    return (false, null, "Child not found.");

                var expenses = await _context.Expenses
                    .Include(e => e.ExpenseCategory)
                    .Include(e => e.Mood)
                    .Where(e => e.ChildID == childId)
                    .ToListAsync();

                var response = new FunFactsResponseDto();

                // ---- Slot 1: Personality type ----
                response.FunFacts.Add(BuildPersonalityFunFact(child));

                // ---- Slot 2: Top category ----
                response.FunFacts.Add(BuildTopCategoryFunFact(expenses));

                // ---- Slot 3: Top mood ----
                response.FunFacts.Add(BuildTopMoodFunFact(expenses));

                // ---- Slot 4: Biggest purchase this month ----
                response.FunFacts.Add(BuildBiggestPurchaseFunFact(expenses));

                return (true, response, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting fun facts for child {ChildId}", childId);
                return (false, null, "An error occurred while generating fun facts.");
            }
        }

        // ==================== FUN FACT BUILDERS ====================

        private FunFactDto BuildPersonalityFunFact(Core.Models.Child child)
        {
            string childName = child.PersonalityType?.ChildName ?? "Money Explorer";
            string funFacts = child.PersonalityType?.FunFacts ?? "Everyone has a unique way of handling money!";

            return new FunFactDto
            {
                Observation = $"🌟 You're a {childName}!",
                Tip = funFacts
            };
        }

        private FunFactDto BuildTopCategoryFunFact(List<Core.Models.Expense> expenses)
        {
            if (expenses.Count < MinExpensesThreshold)
            {
                return new FunFactDto
                {
                    Observation = "🧩 Keep logging your purchases!",
                    Tip = "The more you log, the more fun facts you'll unlock about your spending habits!"
                };
            }

            var topCategory = expenses
                .GroupBy(e => e.ExpenseCategory.Name)
                .OrderByDescending(g => g.Sum(e => e.MoneyAmount))
                .First()
                .Key;

            return new FunFactDto
            {
                Observation = $"🧸 Your favorite thing to buy is {topCategory}!",
                Tip = "Did you know saving just a little bit of your allowance each week can help you buy something even bigger?"
            };
        }

        private FunFactDto BuildTopMoodFunFact(List<Core.Models.Expense> expenses)
        {
            if (!expenses.Any())
            {
                return new FunFactDto
                {
                    Observation = "😊 Log expenses to discover your spending mood!",
                    Tip = "Tracking how you feel when you spend helps you make smarter money choices."
                };
            }

            var topMood = expenses
                .GroupBy(e => e.Mood.Description)
                .Select(g => new { Mood = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .First();

            return topMood.Mood.ToLower() switch
            {
                "happy" => new FunFactDto
                {
                    Observation = "😊 You spend the most when you're Happy!",
                    Tip = "It's great to enjoy spending — just make sure you're also saving a little each time!"
                },
                "excited" => new FunFactDto
                {
                    Observation = "🤩 You spend the most when you're Excited!",
                    Tip = "Excitement can lead to great buys — but try waiting a day before big purchases to be sure."
                },
                "sad" => new FunFactDto
                {
                    Observation = "😢 You tend to spend more when you're feeling Sad.",
                    Tip = "It's okay to treat yourself sometimes — just try not to let sad feelings drive big spending decisions."
                },
                "regretful" => new FunFactDto
                {
                    Observation = "😔 You often feel Regretful after spending.",
                    Tip = "Regretting a purchase is a sign you're learning! Try asking yourself if you really need it before buying."
                },
                "neutral" => new FunFactDto
                {
                    Observation = "😐 Most of your spending happens when you're feeling Neutral.",
                    Tip = "Calm spending is actually a great habit — you're not buying on impulse!"
                },
                _ => new FunFactDto
                {
                    Observation = $"Your most common spending mood is {topMood.Mood}!",
                    Tip = "Knowing how you feel when you spend is the first step to smarter money choices."
                }
            };
        }
        private FunFactDto BuildBiggestPurchaseFunFact(List<Core.Models.Expense> expenses)
        {
            var now = DateTimeHelper.EgyptNow;
            var startOfThisMonth = new DateTime(now.Year, now.Month, 1);

            var thisMonthExpenses = expenses.Where(e => e.LogDate >= startOfThisMonth).ToList();

            if (!thisMonthExpenses.Any())
            {
                return new FunFactDto
                {
                    Observation = "🛍️ No purchases logged this month yet!",
                    Tip = "Log your purchases to see your biggest one of the month here!"
                };
            }

            var biggest = thisMonthExpenses.OrderByDescending(e => e.MoneyAmount).First();

            return new FunFactDto
            {
                Observation = $"💰 Your biggest purchase this month was {biggest.MoneyAmount:F0} EGP!",
                Tip = "Big purchases are fine sometimes — just make sure you really need it before buying."
            };
        }

        // ==================== HELPERS ====================

        private static string GetMoodEmoji(string mood) => mood.ToLower() switch
        {
            "happy" => "😊",
            "sad" => "😢",
            "excited" => "🤩",
            "regretful" => "😔",
            "neutral" => "😐",
            _ => "🙂"
        };

        private async Task<KeyInsightDto> GetGoalProgressSummaryAsync(int childId, string childName)
        {
            var goals = await _context.SavingsGoals
                .Where(g => g.ChildID == childId)
                .ToListAsync();

            int completedThisMonth = goals.Count(g =>
                g.Status == "Success" &&
                g.StartDate.Year == DateTimeHelper.EgyptNow.Year &&
                g.StartDate.Month == DateTimeHelper.EgyptNow.Month);

            var activeGoal = goals
                .Where(g => g.Status == "Active")
                .OrderByDescending(g => g.TargetAmount > 0 ? g.CurrentAmount / g.TargetAmount : 0)
                .FirstOrDefault();

            if (activeGoal != null)
            {
                int percent = activeGoal.TargetAmount > 0
                    ? (int)Math.Round((activeGoal.CurrentAmount / activeGoal.TargetAmount) * 100)
                    : 0;

                return new KeyInsightDto
                {
                    Insight = $"{childName} is {percent}% of the way to their {activeGoal.Title} goal"
                };
            }

            if (completedThisMonth > 0)
            {
                return new KeyInsightDto
                {
                    Insight = $"{childName} completed {completedThisMonth} goal(s) this month"
                };
            }

            return new KeyInsightDto
            {
                Insight = $"{childName} doesn't have any active savings goals right now"
            };
        }
    }
}