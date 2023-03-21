namespace EngineBay.Persistence
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class ModuleDbContext : IdentityDbContext<ApplicationUser>, IModuleDbContext
    {
        public ModuleDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<AuditEntry> AuditEntries { get; set; } = null!;

        public void MasterOnModelCreating(ModelBuilder builder)
        {
            this.OnModelCreating(builder);
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}