namespace EngineBay.Persistence
{
    using Microsoft.EntityFrameworkCore;

    public class ModuleDbContext : DbContext, IModuleDbContext
    {
        public ModuleDbContext(DbContextOptions<ModuleDbContext> options)
            : base(options)
        {
        }

        protected ModuleDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<AuditEntry> AuditEntries { get; set; } = null!;

        public DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;

        public void MasterOnModelCreating(ModelBuilder modelBuilder)
        {
            this.OnModelCreating(modelBuilder);
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            AuditEntry.CreateDataAnnotations(modelBuilder);
            ApplicationUser.CreateDataAnnotations(modelBuilder);
        }
    }
}