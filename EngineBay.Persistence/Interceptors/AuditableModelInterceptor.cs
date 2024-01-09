namespace EngineBay.Persistence
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using EngineBay.Core;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;

    public class AuditableModelInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentIdentity currentIdentity;

        public AuditableModelInterceptor(ICurrentIdentity currentIdentity)
        {
            this.currentIdentity = currentIdentity;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ArgumentNullException.ThrowIfNull(eventData);

            this.SetAuditedModelProperties(eventData);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(eventData);

            this.SetAuditedModelProperties(eventData);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void SetAuditedModelProperties(DbContextEventData eventData)
        {
            if (eventData.Context is null)
            {
                throw new ArgumentException("Event data has no context");
            }

            var entries = eventData.Context.ChangeTracker
                .Entries()
                .Where(e => e.Entity is AuditableModel && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

            Parallel.ForEach(entries, entityEntry =>
            {
                ((AuditableModel)entityEntry.Entity).LastUpdatedById = this.currentIdentity.UserId;
                if (entityEntry.State == EntityState.Added)
                {
                    ((AuditableModel)entityEntry.Entity).CreatedById = this.currentIdentity.UserId;
                }
            });
        }
    }
}
