namespace EngineBay.Persistence.Tests
{
    using EngineBay.Core;
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

            var applicationUser = new ApplicationUser();

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

            var applicationUser = new ApplicationUser();

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

            var applicationUser = new ApplicationUser();

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

            var applicationUser = new ApplicationUser();

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

            var applicationUser = new ApplicationUser();

            this.auditedDbContext.MockEntities.Add(this.mockEntity);

            await this.auditedDbContext.SaveChangesAsync(applicationUser).ConfigureAwait(false);

            var savedMockEntity = this.auditedDbContext.MockEntities.First();

            Assert.True(savedMockEntity.CreatedById == applicationUser.Id);
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

            var applicationUser = new ApplicationUser();

            this.auditedDbContext.MockEntities.Add(this.mockEntity);

            await this.auditedDbContext.SaveChangesAsync(applicationUser).ConfigureAwait(false);

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

            var applicationUser = new ApplicationUser();

            this.auditedDbContext.MockEntities.Add(this.mockEntity);

            await this.auditedDbContext.SaveChangesAsync(applicationUser).ConfigureAwait(false);

            var savedMockEntity = this.auditedDbContext.MockEntities.First();

            savedMockEntity.Name = "Bye world!";

            await this.auditedDbContext.SaveChangesAsync(applicationUser).ConfigureAwait(false);

            this.auditedDbContext.MockEntities.Remove(savedMockEntity);

            await this.auditedDbContext.SaveChangesAsync(applicationUser).ConfigureAwait(false);

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
