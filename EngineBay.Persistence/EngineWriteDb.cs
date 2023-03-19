namespace EngineBay.Persistence
{
    using EngineBay.Core;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class EngineWriteDb : EngineDb, IEngineQueryDb, IEngineWriteDb
    {
        private JsonSerializerSettings serializationSettings;

        public EngineWriteDb(DbContextOptions<EngineWriteDb> options)
            : base(options)
        {
            this.serializationSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
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

            // Get audit entries
            var auditEntries = this.OnBeforeSaveChanges();

            // Save current entity
            var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // Save audit entries
            await this.OnAfterSaveChangesAsync(auditEntries).ConfigureAwait(false);

            return result;
        }

        private List<AuditEntry> OnBeforeSaveChanges()
        {
            this.ChangeTracker.DetectChanges();
            var entries = new List<AuditEntry>();

            foreach (var entry in this.ChangeTracker.Entries())
            {
                // Dot not audit entities that are not tracked, not changed, or not of type IAuditable
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged || !(entry.Entity is BaseModel))
                {
                    continue;
                }

                if (entry.Properties is null)
                {
                    throw new ArgumentException();
                }

                var entityId = entry.Properties.Single(p => p.Metadata.IsPrimaryKey());

                if (entityId is null)
                {
                    throw new ArgumentException();
                }

                var changes = entry.Properties.Select(p => new { p.Metadata.Name, p.CurrentValue });

                if (changes is null)
                {
                    throw new ArgumentException();
                }

                var auditEntry = new AuditEntry
                {
                    ActionType = entry.State == EntityState.Added ? "INSERT" : entry.State == EntityState.Deleted ? "DELETE" : "UPDATE",
                    EntityId = entityId.ToString(),
                    EntityName = entry.Metadata.ClrType.Name,

                    // Username = _username,
                    // TimeStamp = DateTime.UtcNow,

                    // TempProperties are properties that are only generated on save, e.g. ID's
                    // These properties will be set correctly after the audited entity has been saved
                    TempChanges = changes.ToDictionary(i => i.Name, i => i.CurrentValue),
                    TempProperties = entry.Properties.Where(p => p.IsTemporary).ToList(),
                };

                entries.Add(auditEntry);
            }

            return entries;
        }

        private Task OnAfterSaveChangesAsync(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
            {
                return Task.CompletedTask;
            }

            // For each temporary property in each audit entry - update the value in the audit entry to the actual (generated) value
            foreach (var entry in auditEntries)
            {
                if (entry.TempProperties is null)
                {
                    throw new ArgumentException("Auditing temporary properties collection was null");
                }

                var changes = new Dictionary<string, object?>();

                foreach (var prop in entry.TempProperties)
                {
                    if (prop.CurrentValue is not null)
                    {
                        var currentValue = prop.CurrentValue.ToString();
                        if (prop.Metadata.IsPrimaryKey())
                        {
                            entry.EntityId = prop.CurrentValue.ToString();
                            changes[prop.Metadata.Name] = currentValue;
                        }
                        else
                        {
                            changes[prop.Metadata.Name] = currentValue;
                        }
                    }
                }

                entry.Changes = JsonConvert.SerializeObject(changes, this.serializationSettings);
            }

            this.AuditEntries.AddRange(auditEntries);
            return this.SaveChangesAsync();
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
                ((BaseModel)entityEntry.Entity).LastUpdatedAt = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseModel)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}