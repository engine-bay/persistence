namespace EngineBay.Persistence
{
    using System;
    using EngineBay.Core;
    using Humanizer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    public class AuditEntry : BaseModel
    {
        public string? EntityName { get; set; }

        public string? ActionType { get; set; }

        // public string Username { get; set; }
        public string? EntityId { get; set; }

#pragma warning disable CA2227 // These properties are not read only as they are set during the auditing process.
        public Dictionary<string, object?>? Changes { get; set; }

        public ICollection<PropertyEntry>? TempProperties { get; set; }

#pragma warning restore CA2227
        public static new void CreateDataAnnotations(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<AuditEntry>().ToTable(typeof(AuditEntry).Name.Pluralize());

            modelBuilder.Entity<AuditEntry>().HasKey(x => x.Id);

            modelBuilder.Entity<AuditEntry>().Property(x => x.CreatedAt).IsRequired();

            modelBuilder.Entity<AuditEntry>().Property(x => x.LastUpdatedAt).IsRequired();

            modelBuilder.Entity<AuditEntry>().Property(x => x.EntityName).IsRequired();

            modelBuilder.Entity<AuditEntry>().Property(x => x.ActionType).IsRequired();

            modelBuilder.Entity<AuditEntry>().Property(x => x.EntityId).IsRequired();

            modelBuilder.Entity<AuditEntry>().Ignore(x => x.TempProperties);
        }
    }
}