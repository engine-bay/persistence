namespace EngineBay.Persistence
{
    using EngineBay.Core;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class AuditingTests
    {
        private MockEngineDb? auditedDbContext;

        private MockEntity? mockEntity;

        private DateTime now = DateTime.UtcNow.AddSeconds(-1);

        public AuditingTests()
        {
            Environment.SetEnvironmentVariable(ConfigurationConstants.DATABASEPROVIDER, "InMemory");

            var services = new ServiceCollection();

            var databaseConfiguration = new DatabaseConfiguration<EngineWriteDb>();
            databaseConfiguration.RegisterDatabases(services);

            var mockDatabaseConfiguration = new DatabaseConfiguration<MockEngineDb>();
            mockDatabaseConfiguration.RegisterDatabases(services);

            var serviceProvider = services.BuildServiceProvider();

            this.auditedDbContext = serviceProvider.GetRequiredService<MockEngineDb>();

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

            this.auditedDbContext.MockEntities.Add(this.mockEntity);

            await this.auditedDbContext.SaveChangesAsync().ConfigureAwait(false);

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

            this.auditedDbContext.MockEntities.Add(this.mockEntity);

            await this.auditedDbContext.SaveChangesAsync().ConfigureAwait(false);

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

            this.auditedDbContext.MockEntities.Add(this.mockEntity);

            await this.auditedDbContext.SaveChangesAsync().ConfigureAwait(false);

            var savedMockEntity = this.auditedDbContext.MockEntities.First();

            Assert.True(savedMockEntity.LastUpdatedAt.Ticks > this.now.Ticks);
        }

        [Fact]
        public async void CreatedAnAuditEntry()
        {
            if (this.auditedDbContext is null)
            {
                throw new ArgumentException();
            }

            if (this.mockEntity is null)
            {
                throw new ArgumentException();
            }

            this.auditedDbContext.MockEntities.Add(this.mockEntity);

            await this.auditedDbContext.SaveChangesAsync().ConfigureAwait(false);

            var auditEntry = this.auditedDbContext.AuditEntries.First();

            Assert.NotNull(auditEntry);
        }

        [Fact]
        public async void AuditsASeriesofChanges()
        {
            if (this.auditedDbContext is null)
            {
                throw new ArgumentException();
            }

            if (this.mockEntity is null)
            {
                throw new ArgumentException();
            }

            this.auditedDbContext.MockEntities.Add(this.mockEntity);

            await this.auditedDbContext.SaveChangesAsync().ConfigureAwait(false);

            var savedMockEntity = this.auditedDbContext.MockEntities.First();

            savedMockEntity.Name = "Bye world!";

            await this.auditedDbContext.SaveChangesAsync().ConfigureAwait(false);

            this.auditedDbContext.MockEntities.Remove(savedMockEntity);

            await this.auditedDbContext.SaveChangesAsync().ConfigureAwait(false);

            var auditEntries = this.auditedDbContext.AuditEntries.
                Where(x => x.EntityId == savedMockEntity.Id.ToString())
                .OrderBy(x => x.CreatedAt)
                .ToList();

            // we expect there to be three auditing entries
            Assert.Equal(3, auditEntries.Count);

            var firstEntry = auditEntries[0];
            var secondEntry = auditEntries[1];
            var thirdEntry = auditEntries[2];

            // with the following sequence of operations
            Assert.Equal(DatabaseOperationConstants.INSERT, firstEntry.ActionType);
            Assert.Equal(DatabaseOperationConstants.UPDATE, secondEntry.ActionType);
            Assert.Equal(DatabaseOperationConstants.DELETE, thirdEntry.ActionType);
        }
    }
}
