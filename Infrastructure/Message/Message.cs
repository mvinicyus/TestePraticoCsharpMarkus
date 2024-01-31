namespace Infrastructure.Message
{
    public abstract class Message
    {
        protected Message()
        {
            MessageType = GetType().Name;
            AggregateId = Guid.NewGuid();
        }

        public string MessageType { get; set; }
        public Guid AggregateId { get; set; }
    }
}
