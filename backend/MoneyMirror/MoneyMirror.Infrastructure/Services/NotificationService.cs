using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Notifications;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Core.Models;
using MoneyMirror.Infrastructure.Data;
using MoneyMirror.Infrastructure.Hubs;

namespace MoneyMirror.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            ApplicationDbContext context,
            IHubContext<NotificationHub> hubContext,
            ILogger<NotificationService> logger)
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifyParentAsync(int parentId, string title, string message, string? link = null)
        {
            var notification = new Notification
            {
                TargetType = "Parent",
                Title = title,
                Message = message,
                Link = link,
                DeliveryMethod = "InApp",
                ParentID = parentId,
                IsRead = false,
                SentDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            await _hubContext.Clients
                .Group($"parent-{parentId}")
                .SendAsync("ReceiveNotification", new
                {
                    notification.NotificationID,
                    notification.Title,
                    notification.Message,
                    notification.Link,
                    notification.SentDate
                });

            _logger.LogInformation("Notified parent {ParentId}: {Title}", parentId, title);
        }

        public async Task NotifyChildAsync(int childId, string title, string message, string? link = null)
        {
            var notification = new Notification
            {
                TargetType = "Child",
                Title = title,
                Message = message,
                Link = link,
                DeliveryMethod = "InApp",
                ChildID = childId,
                IsRead = false,
                SentDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            await _hubContext.Clients
                .Group($"child-{childId}")
                .SendAsync("ReceiveNotification", new
                {
                    notification.NotificationID,
                    notification.Title,
                    notification.Message,
                    notification.Link,
                    notification.SentDate
                });

            _logger.LogInformation("Notified child {ChildId}: {Title}", childId, title);
        }

        public async Task NotifyAllParentsOfChildAsync(int childId, string title, string message, string? link = null)
        {
            var parentIds = await _context.ParentChildren
                .Where(pc => pc.ChildID == childId)
                .Select(pc => pc.ParentID)
                .ToListAsync();

            foreach (var parentId in parentIds)
                await NotifyParentAsync(parentId, title, message, link);
        }

        public async Task<(bool success, List<NotificationDto> notifications, string errorMessage)>
            GetMyNotificationsAsync(string targetType, int userId, bool includeRead = false)
        {
            try
            {
                var query = _context.Notifications
                    .Where(n => n.TargetType == targetType);

                if (targetType == "Parent")
                    query = query.Where(n => n.ParentID == userId);
                else
                    query = query.Where(n => n.ChildID == userId);

                if (!includeRead)
                    query = query.Where(n => !n.IsRead);

                var notifications = await query
                    .OrderByDescending(n => n.SentDate)
                    .Select(n => new NotificationDto
                    {
                        NotificationID = n.NotificationID,
                        Title = n.Title,
                        Message = n.Message,
                        IsRead = n.IsRead,
                        SentDate = n.SentDate,
                        Link = n.Link,
                        DeliveryMethod = n.DeliveryMethod
                    })
                    .ToListAsync();

                return (true, notifications, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications for {TargetType} {UserId}", targetType, userId);
                return (false, new List<NotificationDto>(), "An error occurred while loading notifications.");
            }
        }

        public async Task<(bool success, int count, string errorMessage)>
            GetUnreadCountAsync(string targetType, int userId)
        {
            try
            {
                var query = _context.Notifications
                    .Where(n => n.TargetType == targetType && !n.IsRead);

                if (targetType == "Parent")
                    query = query.Where(n => n.ParentID == userId);
                else
                    query = query.Where(n => n.ChildID == userId);

                int count = await query.CountAsync();
                return (true, count, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count for {TargetType} {UserId}", targetType, userId);
                return (false, 0, "An error occurred.");
            }
        }

        public async Task<(bool success, string errorMessage)>
            MarkAsReadAsync(int notificationId, string targetType, int userId)
        {
            try
            {
                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.NotificationID == notificationId
                                           && n.TargetType == targetType
                                           && (targetType == "Parent"
                                               ? n.ParentID == userId
                                               : n.ChildID == userId));

                if (notification == null)
                    return (false, "Notification not found.");

                notification.IsRead = true;
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification {Id} as read", notificationId);
                return (false, "An error occurred.");
            }
        }

        public async Task<(bool success, string errorMessage)>
            MarkAllAsReadAsync(string targetType, int userId)
        {
            try
            {
                var query = _context.Notifications
                    .Where(n => n.TargetType == targetType && !n.IsRead);

                if (targetType == "Parent")
                    query = query.Where(n => n.ParentID == userId);
                else
                    query = query.Where(n => n.ChildID == userId);

                var notifications = await query.ToListAsync();

                foreach (var n in notifications)
                    n.IsRead = true;

                _context.Notifications.UpdateRange(notifications);
                await _context.SaveChangesAsync();

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all as read for {TargetType} {UserId}", targetType, userId);
                return (false, "An error occurred.");
            }
        }

        public async Task SendDailyExpenseRemindersAsync()
        {
            try
            {
                var todayUtc = DateTime.UtcNow.Date;

                var childrenWithExpensesToday = await _context.Expenses
                    .Where(e => e.LogDate.Date == todayUtc)
                    .Select(e => e.ChildID)
                    .Distinct()
                    .ToListAsync();

                var allChildren = await _context.Children
                    .Select(c => c.ChildID)
                    .ToListAsync();

                var childrenToRemind = allChildren
                    .Where(id => !childrenWithExpensesToday.Contains(id))
                    .ToList();

                foreach (var childId in childrenToRemind)
                {
                    await NotifyChildAsync(
                        childId,
                        "Don't forget! 📝",
                        "Did you spend any money today? Log your expenses and keep your streak going! 🚀",
                        "/expenses/log"
                    );
                }

                _logger.LogInformation("Sent daily reminders to {Count} children", childrenToRemind.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending daily expense reminders");
            }
        }
    }
}