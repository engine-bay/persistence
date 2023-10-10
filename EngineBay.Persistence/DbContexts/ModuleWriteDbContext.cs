namespace EngineBay.Persistence
{
    using EngineBay.Core;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class ModuleWriteDbContext : ModuleDbContext, IModuleWriteDbContext
    {
        private JsonSerializerSettings serializationSettings;

        private bool auditingEnabled;

        public ModuleWriteDbContext(DbContextOptions<ModuleWriteDbContext> options)
            : base(options)
        {
            this.serializationSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };

            this.auditingEnabled = BaseDatabaseConfiguration.IsAuditingEnabled();
        }

        /// <inheritdoc/>
        public override int SaveChanges()
        {
            this.SetTimeStamps();

            return base.SaveChanges();
        }

        public int SaveChanges(ApplicationUser user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            this.SetAuditedTimeStamps(user);

            if (!this.auditingEnabled)
            {
                return base.SaveChanges();
            }

            // Save current entity
            var result = base.SaveChanges();

            return result;
        }

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            this.SetAuditedTimeStamps(user);

            if (!this.auditingEnabled)
            {
                return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            // Save current entity
            var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc/>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.SetTimeStamps();

            return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        private void SetAuditedTimeStamps(ApplicationUser user)
        {
            var entries = this.ChangeTracker
                .Entries()
                .Where(e => e.Entity is AuditableModel && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

            Parallel.ForEach(entries, entityEntry =>
            {
                ((AuditableModel)entityEntry.Entity).LastUpdatedAt = DateTime.UtcNow;
                ((AuditableModel)entityEntry.Entity).LastUpdatedById = user.Id;
                ((AuditableModel)entityEntry.Entity).LastUpdatedBy = user;

                if (entityEntry.State == EntityState.Added)
                {
                    ((AuditableModel)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                    ((AuditableModel)entityEntry.Entity).CreatedById = user.Id;
                    ((AuditableModel)entityEntry.Entity).CreatedBy = user;
                }
            });
        }

        private void SetTimeStamps()
        {
            var entries = this.ChangeTracker
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