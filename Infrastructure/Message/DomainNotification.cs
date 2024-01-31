using MediatR;

namespace Infrastructure.Message
{
    public class DomainNotification : Message, INotification
    {
        public DomainNotification(string key,
                                  string value,
                                  bool isError = false)
        {
            Timestamp = DateTime.UtcNow;
            DomainNotificationId = Guid.NewGuid();
            Version = 1;
            Key = key;
            Value = value;
            IsError = isError;
        }

        public DateTime Timestamp { get; }
        public Guid DomainNotificationId { get; }
        public string Key { get; }
        public string Value { get; }
        public int Version { get; }
        public bool IsError { get; }
    }
}
