using Domain.Entity.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Data.Common;

namespace Repository.Context
{
    public class DatabaseContext : DbContext
    {
        private DbConnection _connection;

        public IDbContextTransaction transaction { get; private set; }
        private DbTransaction _transaction;
        public DatabaseContext(DbContextOptions<DatabaseContext> opt) : base(opt)
        {
        }

        public async Task BeginTransactionAsync(bool withNolock = false)
        {
            if (withNolock)
            {
                _transaction = await GetConnection().BeginTransactionAsync(IsolationLevel.ReadUncommitted, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                _transaction = await GetConnection().BeginTransactionAsync(IsolationLevel.ReadCommitted, CancellationToken.None).ConfigureAwait(false);
            }
            Database.UseTransaction(_transaction);
        }

        public async Task CommitTransactionAsync()
        {
            await _transaction.CommitAsync().ConfigureAwait(false);
            Dispose();
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null &&
                _transaction.Connection != null &&
                _transaction.Connection.State != ConnectionState.Closed)
            {
                await _transaction.RollbackAsync().ConfigureAwait(false);
            }

            Dispose();
        }

        public DbConnection GetConnection()
        {
            if (_connection?.State == ConnectionState.Open)
            {
                return _connection;
            }

            _connection = this.Database.GetDbConnection();
            if (_connection?.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            return _connection;
        }

        public string getStringConnection
        {
            get
            {
                return Environment.GetEnvironmentVariable("MySqlStringConnection") ?? "";
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(modelBuilder);
            var entities = modelBuilder.Model.GetEntityTypes();
            foreach (var entity in entities)
            {
                var entityObj = modelBuilder.Entity(entity.ClrType);
                var props = entity.ClrType.GetProperties();
                foreach (var prop in props)
                {
                    if (prop.Name == "Id")
                    {
                        entityObj.HasKey(prop.Name);
                        continue;
                    }
                    var under = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    if (under == typeof(bool))
                    {
                        entityObj.Property(prop.Name).HasColumnType("bit");
                    }
                    else if (under == typeof(Guid))
                    {
                        entityObj.Property(prop.Name).HasColumnType("char(38)");
                    }
                }
            }
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<PostEntity> Posts { get; set; }
    }
}
