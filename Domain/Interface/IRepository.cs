namespace Domain.Interface
{
    public interface IRepository<TAggregate, in TKey> where TAggregate : class, IAggregateRoot<TKey>
    {
    }
}
