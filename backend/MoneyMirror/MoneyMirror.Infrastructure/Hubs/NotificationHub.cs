using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MoneyMirror.Infrastructure.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var parentId = Context.User?.FindFirst("ParentId")?.Value;
            var childId = Context.User?.FindFirst("ChildId")?.Value;

            if (parentId != null)
                await Groups.AddToGroupAsync(Context.ConnectionId, $"parent-{parentId}");
            else if (childId != null)
                await Groups.AddToGroupAsync(Context.ConnectionId, $"child-{childId}");

            await base.OnConnectedAsync();
        }
    }
}