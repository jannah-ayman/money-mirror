using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

            // Find badges in this category that match current count and aren't already earned
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

                _logger.LogInformation(
                    "Child {ChildId} unlocked achievement: {Name}", childId, achievement.Name);

                // TODO: trigger notification to child when notifications are implemented
            }

            await _context.SaveChangesAsync();
        }
    }
}