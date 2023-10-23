namespace EngineBay.Persistence.Tests
{
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class PersistenceTests
    {
        private readonly MockModuleDbContext dbContext;

        private readonly DateTime now = DateTime.UtcNow.AddSeconds(-1);

        public PersistenceTests()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASEPROVIDER, "InMemory");

            var dbContextOptions = new DbContextOptionsBuilder<ModuleWriteDbContext>()
                .UseInMemoryDatabase(nameof(PersistenceTests))
                .EnableSensitiveDataLogging()
                .Options;

            var context = new MockModuleDbContext(dbContextOptions);
            ArgumentNullException.ThrowIfNull(context);

            this.dbContext = context;
            this.dbContext.Database.EnsureDeleted();
            this.dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task CanSaveChanges()
        {
            ArgumentNullException.ThrowIfNull(this.dbContext);

            var entity = new MockEntity()
            {
                Name = "Test",
            };

            this.dbContext.MockEntities.Add(entity);

            await this.dbContext.SaveChangesAsync().ConfigureAwait(false);

            var savedMockEntity = this.dbContext.MockEntities.First();

            Assert.NotNull(savedMockEntity);
        }

        [Fact]
        public async Task SetsTheCreatedDate()
        {
            ArgumentNullException.ThrowIfNull(this.dbContext);

            var entity = new MockEntity()
            {
                Name = "Test",
            };

            this.dbContext.MockEntities.Add(entity);

            await this.dbContext.SaveChangesAsync().ConfigureAwait(false);

            Assert.True(entity.CreatedAt.Ticks > this.now.Ticks);
        }

        [Fact]
        public async Task SetsTheLastModifiedDate()
        {
            ArgumentNullException.ThrowIfNull(this.dbContext);

            var entity = new MockEntity()
            {
                Name = "Test",
            };

            this.dbContext.MockEntities.Add(entity);

            await this.dbContext.SaveChangesAsync().ConfigureAwait(false);

            Assert.True(entity.LastUpdatedAt.Ticks > this.now.Ticks);
        }
    }
}
