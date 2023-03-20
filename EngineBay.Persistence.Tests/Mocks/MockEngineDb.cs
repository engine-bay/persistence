namespace EngineBay.Persistence
{
    using Microsoft.EntityFrameworkCore;

    public class MockEngineDb : EngineWriteDb, IEngineWriteDb
    {
        public MockEngineDb(DbContextOptions<EngineWriteDb> options)
       : base(options)
        {
        }

        public DbSet<MockEntity> MockEntities { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            MockEntity.CreateDataAnnotations(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }
    }
}
