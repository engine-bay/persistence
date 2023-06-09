namespace EngineBay.Persistence.Tests
{
    using Humanizer;
    using Microsoft.EntityFrameworkCore;

    public class MockEntity : AuditableModel
    {
        public string? Name { get; set; }

        public static new void CreateDataAnnotations(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<MockEntity>().ToTable(typeof(MockEntity).Name.Pluralize());

            modelBuilder.Entity<MockEntity>().HasKey(x => x.Id);

            modelBuilder.Entity<MockEntity>().Property(x => x.CreatedAt).IsRequired();

            modelBuilder.Entity<MockEntity>().Property(x => x.LastUpdatedAt).IsRequired();

            modelBuilder.Entity<MockEntity>().Property(x => x.CreatedById).IsRequired();

            modelBuilder.Entity<MockEntity>().HasOne(x => x.CreatedBy);

            modelBuilder.Entity<MockEntity>().Property(x => x.LastUpdatedById).IsRequired();

            modelBuilder.Entity<MockEntity>().HasOne(x => x.LastUpdatedBy);

            modelBuilder.Entity<MockEntity>().Property(x => x.Name).IsRequired();
        }
    }
}
