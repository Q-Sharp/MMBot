using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using TTMMBot.Data.Entities;

namespace TTMMBot.Data
{
    public interface IContext
    {
        bool UseTriggers { get; set; }
        DbSet<Member> Member { get; set; }
        DbSet<Clan> Clan { get; set; }
        DbSet<GlobalSettings> GlobalSettings { get; set; }
        DbSet<Restart> Restart { get; set; }
        DbSet<Vacation> Vacation { get; set; }
        bool TriggersEnabled { get; set; }
        DatabaseFacade Database { get; }
        IModel Model { get; }
        Task MigrateAsync();
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken);
        ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class;
        EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class;
        EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class;
        Task AddRangeAsync(params object[] entities);
        Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken);
    }
}