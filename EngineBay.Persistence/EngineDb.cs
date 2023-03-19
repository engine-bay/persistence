namespace EngineBay.Persistence
{
    using Audit.EntityFramework;
    using Microsoft.EntityFrameworkCore;

    public class EngineDb : AuditDbContext, IEngineDb
    {
        public EngineDb(DbContextOptions options)
            : base(options)
        {
        }

        public void MasterOnModelCreating(ModelBuilder modelBuilder)
        {
            this.OnModelCreating(modelBuilder);
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}