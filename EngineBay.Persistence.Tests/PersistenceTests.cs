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

            this.dbContext = context;
        }

        [Fact]
        public async Task CanSaveChanges()
        {
            var entity = new MockEntity()
            {
                Name = "Test",
            };

            this.dbContext.MockEntities.Add(entity);

            await this.dbContext.SaveChangesAsync();

            var savedMockEntity = this.dbContext.MockEntities.First();

            Assert.NotNull(savedMockEntity);
        }

        [Fact]
        public async Task SetsTheCreatedDate()
        {
            var entity = new MockEntity()
            {
                Name = "Test",
            };

            this.dbContext.MockEntities.Add(entity);

            await this.dbContext.SaveChangesAsync();

            Assert.True(entity.CreatedAt.Ticks > this.now.Ticks);
        }

        [Fact]
        public async Task SetsTheLastModifiedDate()
        {
            var entity = new MockEntity()
            {
                Name = "Test",
            };

            this.dbContext.MockEntities.Add(entity);

            await this.dbContext.SaveChangesAsync();

            Assert.True(entity.LastUpdatedAt.Ticks > this.now.Ticks);
        }
    }
}
