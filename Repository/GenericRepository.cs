using Domain.Interface;
using Microsoft.Extensions.Logging;
using Repository.Base;
using Repository.Context;

namespace Repository
{
    public class GenericRepository<Aggregate, TKey> : BaseRepository<Aggregate, TKey>
        where Aggregate : class, IAggregateRoot<TKey>
    {
        public GenericRepository(DatabaseContext context, ILogger<BaseRepository<Aggregate, TKey>> logger) : base(context, logger)
        {
        }
    }
}
