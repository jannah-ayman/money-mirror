using MoneyMirror.Core.DTOs.Notifications;

namespace MoneyMirror.Core.Interfaces
{
    public interface INotificationService
    {
        Task NotifyParentAsync(int parentId, string title, string message, string? link = null);
        Task NotifyChildAsync(int childId, string title, string message, string? link = null);
        Task NotifyAllParentsOfChildAsync(int childId, string title, string message, string? link = null);

        Task<(bool success, List<NotificationDto> notifications, string errorMessage)>
            GetMyNotificationsAsync(string targetType, int userId, bool includeRead = false);

        Task<(bool success, int count, string errorMessage)>
            GetUnreadCountAsync(string targetType, int userId);

        Task<(bool success, string errorMessage)>
            MarkAsReadAsync(int notificationId, string targetType, int userId);

        Task<(bool success, string errorMessage)>
            MarkAllAsReadAsync(string targetType, int userId);

        Task SendDailyExpenseRemindersAsync();
    }
}