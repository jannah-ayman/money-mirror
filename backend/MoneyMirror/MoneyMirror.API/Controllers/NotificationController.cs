using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMirror.Core.DTOs.Common;
using MoneyMirror.Core.DTOs.Notifications;
using MoneyMirror.Core.Interfaces;

namespace MoneyMirror.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private (string targetType, int userId, bool valid) GetCallerInfo()
        {
            var parentId = User.FindFirst("ParentId")?.Value;
            var childId = User.FindFirst("ChildId")?.Value;

            if (parentId != null && int.TryParse(parentId, out int pId))
                return ("Parent", pId, true);
            if (childId != null && int.TryParse(childId, out int cId))
                return ("Child", cId, true);

            return (string.Empty, 0, false);
        }

        [HttpGet("my")]
        public async Task<ActionResult<ApiResponse<List<NotificationDto>>>> GetMyNotifications(
            [FromQuery] bool includeRead = false)
        {
            var (targetType, userId, valid) = GetCallerInfo();
            if (!valid)
                return BadRequest(ApiResponse<List<NotificationDto>>.ErrorResponse("Invalid token claims."));

            var (success, notifications, error) =
                await _notificationService.GetMyNotificationsAsync(targetType, userId, includeRead);

            if (!success)
                return BadRequest(ApiResponse<List<NotificationDto>>.ErrorResponse(error));

            string message = notifications.Count == 0
                ? "No notifications"
                : $"{notifications.Count} notification(s)";

            return Ok(ApiResponse<List<NotificationDto>>.SuccessResponse(notifications, message));
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount()
        {
            var (targetType, userId, valid) = GetCallerInfo();
            if (!valid)
                return BadRequest(ApiResponse<int>.ErrorResponse("Invalid token claims."));

            var (success, count, error) =
                await _notificationService.GetUnreadCountAsync(targetType, userId);

            if (!success)
                return BadRequest(ApiResponse<int>.ErrorResponse(error));

            return Ok(ApiResponse<int>.SuccessResponse(count, $"{count} unread notification(s)"));
        }

        [HttpPatch("{id}/read")]
        public async Task<ActionResult<ApiResponse<object>>> MarkAsRead(int id)
        {
            var (targetType, userId, valid) = GetCallerInfo();
            if (!valid)
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid token claims."));

            var (success, error) = await _notificationService.MarkAsReadAsync(id, targetType, userId);

            if (!success)
                return BadRequest(ApiResponse<object>.ErrorResponse(error));

            return Ok(ApiResponse<object>.SuccessResponse(null, "Marked as read."));
        }

        [HttpPatch("read-all")]
        public async Task<ActionResult<ApiResponse<object>>> MarkAllAsRead()
        {
            var (targetType, userId, valid) = GetCallerInfo();
            if (!valid)
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid token claims."));

            var (success, error) = await _notificationService.MarkAllAsReadAsync(targetType, userId);

            if (!success)
                return BadRequest(ApiResponse<object>.ErrorResponse(error));

            return Ok(ApiResponse<object>.SuccessResponse(null, "All notifications marked as read."));
        }
    }
}