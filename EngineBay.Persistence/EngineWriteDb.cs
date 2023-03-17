namespace EngineBay.Persistence
{
    using EngineBay.Core;
    using Microsoft.EntityFrameworkCore;

    public class EngineWriteDb : EngineDb, IEngineQueryDb, IEngineWriteDb
    {
        public EngineWriteDb(DbContextOptions<EngineWriteDb> options)
            : base(options)
        {
        }

        /// <inheritdoc/>
        public override int SaveChanges()
        {
            this.SetTimeStamps();

            return base.SaveChanges();
        }

        /// <inheritdoc/>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.SetTimeStamps();

            return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        private void SetTimeStamps()
        {
            var entries = this.ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseModel && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseModel)entityEntry.Entity).LastUpdatedAt = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseModel)entityEntry.Entity).CreatedAt = DateTime.Now;
                }
            }
        }
    }
}