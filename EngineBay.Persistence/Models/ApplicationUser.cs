namespace EngineBay.Persistence
{
    using Humanizer;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationUser : AuditableModel
    {
        public ApplicationUser(string username)
        {
            this.Username = username;
        }

        public string Username { get; set; }

        public static new void CreateDataAnnotations(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<ApplicationUser>().ToTable(typeof(ApplicationUser).Name.Pluralize());

            modelBuilder.Entity<ApplicationUser>().HasKey(x => x.Id);

            modelBuilder.Entity<ApplicationUser>().Property(x => x.CreatedAt).IsRequired();

            modelBuilder.Entity<ApplicationUser>().Property(x => x.LastUpdatedAt).IsRequired();

            modelBuilder.Entity<ApplicationUser>().Property(x => x.CreatedById);

            modelBuilder.Entity<ApplicationUser>().Ignore(x => x.CreatedBy);

            modelBuilder.Entity<ApplicationUser>().Property(x => x.LastUpdatedById);

            modelBuilder.Entity<ApplicationUser>().Ignore(x => x.LastUpdatedBy);

            modelBuilder.Entity<ApplicationUser>().Property(x => x.Username).IsRequired();

            modelBuilder.Entity<ApplicationUser>().HasIndex(x => x.Username).IsUnique();
        }
    }
}