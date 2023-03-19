namespace EngineBay.Persistence
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class EngineDb : DbContext, IEngineDb
    {
        public EngineDb(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<AuditEntry> AuditEntries { get; set; } = null!;

        public void MasterOnModelCreating(ModelBuilder modelBuilder)
        {
            this.OnModelCreating(modelBuilder);
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<AuditEntry>().Property(auditEntry => auditEntry.Changes).HasConversion(
                value => JsonConvert.SerializeObject(value),
                serializedValue => JsonConvert.DeserializeObject<Dictionary<string, object?>>(serializedValue));

            base.OnModelCreating(modelBuilder);
        }
    }
}