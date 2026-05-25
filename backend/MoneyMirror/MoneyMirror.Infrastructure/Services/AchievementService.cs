using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Achievement;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Core.Models;
using MoneyMirror.Infrastructure.Data;

namespace MoneyMirror.Infrastructure.Services
{
    public class AchievementService : IAchievementService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AchievementService> _logger;

        public AchievementService(ApplicationDbContext context, ILogger<AchievementService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CheckAndUnlockAsync(int childId, string category)
        {
            var child = await _context.Children.FindAsync(childId);
            if (child == null) return;

            int currentCount = category switch
            {
                "Quiz" => child.QuizCount,
                "Goal" => child.GoalCount,
                "Expense" => child.ExpenseCount,
                _ => 0
            };

            var earnedIds = await _context.ChildAchievements
                .Where(ca => ca.ChildID == childId)
                .Select(ca => ca.AchievementTypeID)
                .ToListAsync();

            var toUnlock = await _context.AchievementTypes
                .Where(at => at.Category == category
                          && at.Threshold <= currentCount
                          && !earnedIds.Contains(at.AchievementTypeID))
                .ToListAsync();

            foreach (var achievement in toUnlock)
            {
                _context.ChildAchievements.Add(new ChildAchievement
                {
                    ChildID = childId,
                    AchievementTypeID = achievement.AchievementTypeID,
                    EarnedDate = DateTime.UtcNow
                });

                _logger.LogInformation("Child {ChildId} unlocked achievement: {Name}", childId, achievement.Name);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<(bool success, List<AchievementCategoryDto> categories, string errorMessage)>
            GetMyAchievementsAsync(int childId)
        {
            try
            {
                var child = await _context.Children.FindAsync(childId);
                if (child == null)
                    return (false, null, "Child not found");

                // Load all 12 badge types
                var allBadges = await _context.AchievementTypes
                    .OrderBy(at => at.Category)
                    .ThenBy(at => at.Threshold)
                    .ToListAsync();

                // Load this child's earned achievements
                var earned = await _context.ChildAchievements
                    .Where(ca => ca.ChildID == childId)
                    .ToListAsync();

                var earnedDict = earned.ToDictionary(ca => ca.AchievementTypeID, ca => ca.EarnedDate);

                var categoryCountMap = new Dictionary<string, int>
                {
                    { "Quiz",    child.QuizCount    },
                    { "Goal",    child.GoalCount     },
                    { "Expense", child.ExpenseCount  }
                };

                var categories = allBadges
                    .GroupBy(at => at.Category)
                    .Select(g => new AchievementCategoryDto
                    {
                        Category = g.Key,
                        CurrentCount = categoryCountMap.GetValueOrDefault(g.Key, 0),
                        Badges = g.Select(at => new AchievementBadgeDto
                        {
                            AchievementTypeID = at.AchievementTypeID,
                            Name = at.Name,
                            IconURL = at.IconURL,
                            Threshold = at.Threshold,
                            IsUnlocked = earnedDict.ContainsKey(at.AchievementTypeID),
                            EarnedDate = earnedDict.TryGetValue(at.AchievementTypeID, out var d) ? d : null,
                            CurrentCount = categoryCountMap.GetValueOrDefault(g.Key, 0)
                        }).ToList()
                    })
                    .ToList();

                return (true, categories, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting achievements for child {ChildId}", childId);
                return (false, null, "An error occurred while loading your achievements");
            }
        }
    }
}