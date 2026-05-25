namespace MoneyMirror.Core.Interfaces
{
    public interface IAchievementService
    {
        Task CheckAndUnlockAsync(int childId, string category);
    }
}