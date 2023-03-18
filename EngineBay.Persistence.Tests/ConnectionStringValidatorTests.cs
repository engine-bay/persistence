namespace EngineBay.Persistence
{
    using System.Data;
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
        public void TheDefaultSqliteConnectionStringIsValid()
        {
            var connectionString = DatabaseConfigurationConstants.DefaultSqliteConnectiontring;

            var isValid = ConnectionStringValidator.IsValid(DatabaseProviderTypes.SQLite, connectionString);

            Assert.True(isValid);
        }

        [Fact]
        public void TheDefaultInMemoryConnectionStringIsValid()
        {
            var connectionString = DatabaseConfigurationConstants.DefaultInMemoryConnectiontring;

            var isValid = ConnectionStringValidator.IsValid(DatabaseProviderTypes.SQLite, connectionString);

            Assert.True(isValid);
        }
    }
}
