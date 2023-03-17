namespace EngineBay.Persistence
{
    using Microsoft.EntityFrameworkCore;

    public class EngineDb : DbContext, IEngineDb
    {
        public EngineDb(DbContextOptions options)
            : base(options)
        {
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}