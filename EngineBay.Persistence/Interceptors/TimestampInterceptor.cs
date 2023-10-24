namespace EngineBay.Persistence
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using EngineBay.Core;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;

    public class TimestampInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ArgumentNullException.ThrowIfNull(eventData);

            TimestampInterceptor.SetAuditedTimestamps(eventData);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(eventData);

            TimestampInterceptor.SetAuditedTimestamps(eventData);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void SetAuditedTimestamps(DbContextEventData eventData)
        {
            ArgumentNullException.ThrowIfNull(eventData.Context);

            var entries = eventData.Context.ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseModel && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

            Parallel.ForEach(entries, entityEntry =>
            {
                ((BaseModel)entityEntry.Entity).LastUpdatedAt = DateTime.UtcNow;
                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseModel)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                }
            });
        }
    }
}
