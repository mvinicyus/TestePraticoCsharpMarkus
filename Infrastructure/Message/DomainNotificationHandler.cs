using MediatR;

namespace Infrastructure.Message
{
    public class DomainNotificationHandler : INotificationHandler<DomainNotification>
    {
        private readonly ICollection<DomainNotification> _notifications;

        public DomainNotificationHandler()
        {
            _notifications = new List<DomainNotification>();
        }

        public Task Handle(DomainNotification message, CancellationToken cancellationToken)
        {
            _notifications.Add(message);
            return Task.CompletedTask;
        }

        public virtual IEnumerable<DomainNotification> GetErrorMessages()
        {
            return _notifications.Where(notification => notification.IsError);
        }

        public virtual IEnumerable<DomainNotification> GetNotifications()
        {
            return _notifications.Where(notification => notification.IsError);
        }

        public virtual bool HasErrorNotifications()
        {
            return _notifications.Any(notification => !notification.IsError);
        }

        public virtual bool HasAnyNotifications()
        {
            return _notifications.Any();
        }

        public virtual bool IsValidOperation()
        {
            return !_notifications.Any(notification => notification.IsError);
        }
    }
}
