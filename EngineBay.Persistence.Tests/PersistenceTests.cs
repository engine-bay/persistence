namespace EngineBay.Persistence.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class PersistenceTests
    {
        private readonly MockModuleDbContext? auditedDbContext;

        private readonly MockEntity? mockEntity;

        public PersistenceTests()
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
        public async Task CanSaveChanges()
        {
            ArgumentNullException.ThrowIfNull(this.auditedDbContext);
            ArgumentNullException.ThrowIfNull(this.mockEntity);

            this.auditedDbContext.MockEntities.Add(this.mockEntity);

            await this.auditedDbContext.SaveChangesAsync().ConfigureAwait(false);

            var savedMockEntity = this.auditedDbContext.MockEntities.First();

            Assert.NotNull(savedMockEntity);
        }
    }
}
