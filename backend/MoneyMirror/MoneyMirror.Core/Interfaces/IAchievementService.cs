using MoneyMirror.Core.DTOs.Achievement;

namespace MoneyMirror.Core.Interfaces
{
    public interface IAchievementService
    {
        Task CheckAndUnlockAsync(int childId, string category);
        Task<(bool success, List<AchievementCategoryDto> categories, string errorMessage)> GetMyAchievementsAsync(int childId);
    }
}