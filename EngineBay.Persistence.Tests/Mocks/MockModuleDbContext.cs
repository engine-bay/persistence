namespace EngineBay.Persistence.Tests
{
    using Microsoft.EntityFrameworkCore;

    public class MockModuleDbContext : ModuleWriteDbContext, IModuleWriteDbContext
    {
        public MockModuleDbContext(DbContextOptions<ModuleWriteDbContext> options)
            : base(options)
        {
        }

        public DbSet<MockEntity> MockEntities { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            MockEntity.CreateDataAnnotations(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ArgumentNullException.ThrowIfNull(optionsBuilder);

            optionsBuilder.AddInterceptors(new TimestampInterceptor());

            base.OnConfiguring(optionsBuilder);
        }
    }
}
