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
            throw new NotImplementedException("Do not call save changes synchronously");
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

            var changeTrackerEntries = this.ChangeTracker.Entries();

            foreach (var entry in changeTrackerEntries)
            {
                // Dot not audit entities that are not tracked, not changed, or not of type IAuditable
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged || !(entry.Entity is BaseModel))
                {
                    continue;
                }

                // don't audit our audits
                if (entry.Entity is AuditEntry)
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

                if (entityId.CurrentValue is null)
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
                    ActionType = entry.State == EntityState.Added ? DatabaseOperationConstants.INSERT : entry.State == EntityState.Deleted ? DatabaseOperationConstants.DELETE : DatabaseOperationConstants.UPDATE,
                    EntityId = entityId.CurrentValue.ToString(),
                    EntityName = entry.Metadata.ClrType.Name,

                    // Username = _username,
                    // TimeStamp = DateTime.UtcNow,
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
            Parallel.ForEach(auditEntries, entry =>
            {
                if (entry.TempProperties is null)
                {
                    throw new ArgumentException("Auditing temporary properties collection was null");
                }

                if (entry.TempChanges is null)
                {
                    throw new ArgumentException("Auditing temporary changes collection was null");
                }

                foreach (var prop in entry.TempProperties)
                {
                    if (prop.CurrentValue is not null)
                    {
                        var currentValue = prop.CurrentValue.ToString();
                        if (prop.Metadata.IsPrimaryKey())
                        {
                            entry.EntityId = prop.CurrentValue.ToString();
                            entry.TempChanges[prop.Metadata.Name] = currentValue;
                        }
                        else
                        {
                            entry.TempChanges[prop.Metadata.Name] = currentValue;
                        }
                    }
                }

                entry.Changes = JsonConvert.SerializeObject(entry.TempChanges, this.serializationSettings);
            });

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