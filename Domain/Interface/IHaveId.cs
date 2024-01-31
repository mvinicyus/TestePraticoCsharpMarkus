namespace Domain.Interface
{
    public interface IHaveId<out TKey>
    {
        TKey Id { get; }
    }
}
