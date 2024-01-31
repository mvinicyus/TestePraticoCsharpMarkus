using Domain.Interface;

namespace Domain.Entity.Base
{
    public abstract class AggregateRoot<TKey> : IAggregateRoot<TKey>, IAggregateRoot, IHaveId<TKey>//incluir no git
    {
        public TKey Id { get; protected set; }
        protected AggregateRoot()
        {
            Id = default(TKey);
        }
    }
}
