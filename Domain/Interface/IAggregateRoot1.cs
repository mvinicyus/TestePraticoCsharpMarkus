namespace Domain.Interface
{
    public interface IAggregateRoot<out TKey> : IHaveId<TKey>
    {
    }
}
