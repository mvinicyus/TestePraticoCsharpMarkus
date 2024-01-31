using Domain.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Context;

namespace Repository.Base
{
    public abstract class BaseRepository<TAggregate, TKey> : IRepository<TAggregate, TKey>, IDisposable
        where TAggregate : class, IAggregateRoot<TKey>
    {
        protected readonly ILogger<BaseRepository<TAggregate, TKey>> _logger;
        protected DatabaseContext Context { get; }
        public DbSet<TAggregate> DbSet { get; }

        public BaseRepository(DatabaseContext context, ILogger<BaseRepository<TAggregate, TKey>> logger)
        {
            _logger = logger;
            Context = context;
            DbSet = Context.Set<TAggregate>();
        }
        public async Task BeginTransactionAsync(bool withNolock = false)
        {
            if (withNolock)
            {
                await Context
                    .BeginTransactionAsync(withNolock)
                    .ConfigureAwait(false);
            }
            else
            {
                await Context
                    .BeginTransactionAsync(withNolock)
                    .ConfigureAwait(false);
            }
        }

        public async Task<int> CommitTransactionAsync()
        {
            var changes = await Context.SaveChangesAsync().ConfigureAwait(false);
            await Context.CommitTransactionAsync().ConfigureAwait(false);
            this.Dispose();
            return changes;
        }

        public async Task RollbackTransactionAsync()
        {
            await Context.RollbackTransactionAsync().ConfigureAwait(false);
        }

        public async Task<int> SaveChangesASync()
        {
            return await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public virtual async Task SaveAsync(TAggregate aggregate)
        {
            try
            {
                await DbSet.AddAsync(aggregate);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao salvar objeto no repositório",
                    nameof(SaveAsync),
                    typeof(BaseRepository<,>).FullName,
                    new
                    {
                        TAggregate = typeof(TAggregate).FullName,
                        TKey = typeof(TKey).FullName,
                        aggregate
                    });
                throw;
            }
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
