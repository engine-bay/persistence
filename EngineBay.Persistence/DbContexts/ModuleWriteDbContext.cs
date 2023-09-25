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

            // Get audit entries
            var auditEntries = this.OnBeforeSaveChanges(user);

            // Save current entity
            var result = base.SaveChanges();

            // Save audit entries
            this.OnAfterSaveChanges(auditEntries);

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

            // Get audit entries
            var auditEntries = this.OnBeforeSaveChanges(user);

            // Save current entity
            var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // Save audit entries
            await this.OnAfterSaveChangesAsync(auditEntries).ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc/>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.SetTimeStamps();

            return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        private List<AuditEntry> OnBeforeSaveChanges(ApplicationUser user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            this.ChangeTracker.DetectChanges();
            var entries = new List<AuditEntry>();

            var changeTrackerEntries = this.ChangeTracker.Entries();

            foreach (var entry in changeTrackerEntries)
            {
                // Dot not audit entities that are not tracked, not changed, or not of type AuditableModel
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged || !(entry.Entity is AuditableModel))
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
                    throw new ArgumentException("Auditing change tracker entry properties were null");
                }

                var entityId = entry.Properties.Single(p => p.Metadata.IsPrimaryKey());

                if (entityId is null)
                {
                    throw new ArgumentException("Auditing change tracker entry entityId was null");
                }

                if (entityId.CurrentValue is null)
                {
                    throw new ArgumentException("Auditing change tracker entry current value was null");
                }

                var changes = entry.Properties.Select(p => new { p.Metadata.Name, p.CurrentValue });

                if (changes is null)
                {
                    throw new ArgumentException("Auditing change tracker entry changes were null");
                }

                var auditEntry = new AuditEntry
                {
                    ActionType = entry.State == EntityState.Added ? DatabaseOperationConstants.INSERT : entry.State == EntityState.Deleted ? DatabaseOperationConstants.DELETE : DatabaseOperationConstants.UPDATE,
                    EntityId = entityId.CurrentValue.ToString(),
                    EntityName = entry.Metadata.ClrType.Name,
                    ApplicationUserId = user.Id,
                    ApplicationUserName = user.Username,
                    TempChanges = changes.ToDictionary(i => i.Name, i => i.CurrentValue),
                    TempProperties = entry.Properties.Where(p => p.IsTemporary).ToList(),
                };

                entries.Add(auditEntry);
            }

            return entries;
        }

        private List<AuditEntry> UpdateEntryChanges(List<AuditEntry> auditEntries)
        {
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

            return auditEntries;
        }

        private Task OnAfterSaveChangesAsync(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
            {
                return Task.CompletedTask;
            }

            var updatedAuditEntries = this.UpdateEntryChanges(auditEntries);

            this.AuditEntries.AddRange(updatedAuditEntries);
            return this.SaveChangesAsync();
        }

        private void OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
            {
                return;
            }

            var updatedAuditEntries = this.UpdateEntryChanges(auditEntries);

            this.AuditEntries.AddRange(updatedAuditEntries);
            this.SaveChanges();
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