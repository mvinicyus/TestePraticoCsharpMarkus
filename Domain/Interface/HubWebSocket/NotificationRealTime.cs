namespace Domain.Interface.HubWebSocket
{
    public interface INotificationRealTime
    {
        Task NotifyAsync(string message, string postId);
    }
}
