namespace EngineBay.Persistence.Tests
{
    using Xunit;

    public class ConnectionStringValidatorTests
    {
        [Fact]
        public void AnEmptyStringIsNotAValidConnectionString()
        {
            var connectionString = string.Empty;

            var isValid = ConnectionStringValidator.IsValid(DatabaseProviderTypes.SqlServer, connectionString);

            Assert.False(isValid);
        }

        [Fact]
        public void AnInMemorySQLiteConnectionStringIsValid()
        {
            var connectionString = "Filename=:memory:";

            var isValid = ConnectionStringValidator.IsValid(DatabaseProviderTypes.SQLite, connectionString);

            Assert.True(isValid);
        }

        [Fact]
        public void AnFileSystemSQLiteConnectionStringIsValid()
        {
            var connectionString = "Data Source=engine-api.db;";

            var isValid = ConnectionStringValidator.IsValid(DatabaseProviderTypes.SQLite, connectionString);

            Assert.True(isValid);
        }

        [Fact]
        public void ApostgresConnectionStringIsValid()
        {
            var connectionString = "Host=my_host;Database=my_db;Username=my_user;Password=my_pw";

            var isValid = ConnectionStringValidator.IsValid(DatabaseProviderTypes.Postgres, connectionString);

            Assert.True(isValid);
        }

        [Fact]
        public void TheDefaultSqliteConnectionStringIsValid()
        {
            var connectionString = DefaultConnectionStringConstants.DefaultSqliteConnectiontring;

            var isValid = ConnectionStringValidator.IsValid(DatabaseProviderTypes.SQLite, connectionString);

            Assert.True(isValid);
        }

        [Fact]
        public void TheDefaultInMemoryConnectionStringIsValid()
        {
            var connectionString = DefaultConnectionStringConstants.DefaultInMemoryConnectiontring;

            var isValid = ConnectionStringValidator.IsValid(DatabaseProviderTypes.SQLite, connectionString);

            Assert.True(isValid);
        }
    }
}
