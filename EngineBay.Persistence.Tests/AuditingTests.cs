namespace EngineBay.Persistence.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class AuditingTests
    {
        private MockModuleDbContext? auditedDbContext;

        private MockEntity? mockEntity;

        private DateTime now = DateTime.UtcNow.AddSeconds(-1);

        public AuditingTests()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableConstants.DATABASEPROVIDER, "InMemory");

            var services = new ServiceCollection();

            var databaseConfiguration = new DatabaseConfiguration<ModuleWriteDbContext>();
            databaseConfiguration.RegisterDatabases(services);

            var mockDatabaseConfiguration = new DatabaseConfiguration<MockModuleDbContext>();
            mockDatabaseConfiguration.RegisterDatabases(services);

            var serviceProvider = services.BuildServiceProvider();

            this.auditedDbContext = serviceProvider.GetRequiredService<MockModuleDbContext>();

            this.auditedDbContext.Database.EnsureDeleted();
            this.auditedDbContext.Database.EnsureCreated();

            this.mockEntity = new MockEntity
            {
                Name = "Hello world!",
            };
        }

        [Fact]
        public async void CanSaveChanges()
        {
            if (this.auditedDbContext is null)
            {
                throw new ArgumentException();
            }

            if (this.mockEntity is null)
            {
                throw new ArgumentException();
            }

            var applicationUser = new MockApplicationUser();

            this.auditedDbContext.MockEntities.Add(this.mockEntity);

            await this.auditedDbContext.SaveChangesAsync(applicationUser).ConfigureAwait(false);

            var savedMockEntity = this.auditedDbContext.MockEntities.First();

            Assert.NotNull(savedMockEntity);
        }

        [Fact]
        public async void SetsTheCreatedDate()
        {
            if (this.auditedDbContext is null)
            {
                throw new ArgumentException();
            }

            if (this.mockEntity is null)
            {
                throw new ArgumentException();
            }

            var applicationUser = new MockApplicationUser();

            this.auditedDbContext.MockEntities.Add(this.mockEntity);

            await this.auditedDbContext.SaveChangesAsync(applicationUser).ConfigureAwait(false);

            var savedMockEntity = this.auditedDbContext.MockEntities.First();

            Assert.True(savedMockEntity.CreatedAt.Ticks > this.now.Ticks);
        }

        [Fact]
        public async void SetsTheLastModifiedDate()
        {
            if (this.auditedDbContext is null)
            {
                throw new ArgumentException();
            }

            if (this.mockEntity is null)
            {
                throw new ArgumentException();
            }

            var applicationUser = new MockApplicationUser();

            this.auditedDbContext.MockEntities.Add(this.mockEntity);

            await this.auditedDbContext.SaveChangesAsync(applicationUser).ConfigureAwait(false);

            var savedMockEntity = this.auditedDbContext.MockEntities.First();

            Assert.True(savedMockEntity.LastUpdatedAt.Ticks > this.now.Ticks);
        }

        [Fact]
        public async void SetsTheLastModifiedByUserIdDate()
        {
            if (this.auditedDbContext is null)
            {
                throw new ArgumentException();
            }

            if (this.mockEntity is null)
            {
                throw new ArgumentException();
            }

            var applicationUser = new MockApplicationUser();

            this.auditedDbContext.MockEntities.Add(this.mockEntity);

            await this.auditedDbContext.SaveChangesAsync(applicationUser).ConfigureAwait(false);

            var savedMockEntity = this.auditedDbContext.MockEntities.First();

            Assert.NotNull(savedMockEntity.LastUpdatedById);
        }

        [Fact]
        public async void SetsTheCreatedByUserIdDate()
        {
            if (this.auditedDbContext is null)
            {
                throw new ArgumentException();
            }

            if (this.mockEntity is null)
            {
                throw new ArgumentException();
            }

            var applicationUser = new MockApplicationUser();

            this.auditedDbContext.MockEntities.Add(this.mockEntity);

            await this.auditedDbContext.SaveChangesAsync(applicationUser).ConfigureAwait(false);

            var savedMockEntity = this.auditedDbContext.MockEntities.First();

            Assert.True(savedMockEntity.CreatedById == applicationUser.Id);
        }
    }
}
