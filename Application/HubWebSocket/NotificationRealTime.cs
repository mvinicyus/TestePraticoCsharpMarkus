using Domain.Interface.HubWebSocket;
using Microsoft.AspNetCore.SignalR;

namespace Application.HubWebSocket
{
    public class NotificationRealTime : INotificationRealTime
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationRealTime(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task NotifyAsync(string message, string postId)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message, postId);
        }
    }
}
