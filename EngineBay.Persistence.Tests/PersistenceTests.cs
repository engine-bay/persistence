namespace EngineBay.Persistence.Tests
{
    using EngineBay.Persistence.Tests.Mocks;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class PersistenceTests
    {
        private readonly MockModuleDbContext dbContext;

        private readonly ApplicationUser applicationUser;

        private readonly DateTime now = DateTime.UtcNow.AddSeconds(-1);

        public PersistenceTests()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASEPROVIDER, "InMemory");

            var dbContextOptions = new DbContextOptionsBuilder<ModuleWriteDbContext>()
                    .UseInMemoryDatabase(nameof(PersistenceTests))
                    .EnableSensitiveDataLogging()
            .Options;

            this.applicationUser = new MockApplicationUser();
            var currentIdentity = new MockCurrentIdentity(this.applicationUser);
            var timestampIntercetor = new TimestampInterceptor(currentIdentity);

            var context = new MockModuleDbContext(dbContextOptions, timestampIntercetor);
            ArgumentNullException.ThrowIfNull(context);

            this.dbContext = context;
            this.dbContext.Database.EnsureDeleted();
            this.dbContext.Database.EnsureCreated();

            this.dbContext.ApplicationUsers.Add(this.applicationUser);
            this.dbContext.SaveChanges();
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

            var savedMockEntity = this.dbContext.MockEntities.First();

            Assert.True(savedMockEntity.CreatedAt.Ticks > this.now.Ticks);
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

            var savedMockEntity = this.dbContext.MockEntities.First();

            Assert.True(savedMockEntity.LastUpdatedAt.Ticks > this.now.Ticks);
        }

        [Fact]
        public async Task SetsTheLastModifiedByUserId()
        {
            ArgumentNullException.ThrowIfNull(this.dbContext);

            var entity = new MockEntity()
            {
                Name = "Test",
            };

            this.dbContext.MockEntities.Add(entity);

            await this.dbContext.SaveChangesAsync().ConfigureAwait(false);

            var savedMockEntity = this.dbContext.MockEntities.First();

            Assert.Equal(this.applicationUser.Id, savedMockEntity.LastUpdatedById);
        }

        [Fact]
        public async Task SetsTheCreatedByUserId()
        {
            ArgumentNullException.ThrowIfNull(this.dbContext);

            var entity = new MockEntity()
            {
                Name = "Test",
            };

            this.dbContext.MockEntities.Add(entity);

            await this.dbContext.SaveChangesAsync().ConfigureAwait(false);

            var savedMockEntity = this.dbContext.MockEntities.First();

            Assert.Equal(this.applicationUser.Id, savedMockEntity.CreatedById);
        }
    }
}
