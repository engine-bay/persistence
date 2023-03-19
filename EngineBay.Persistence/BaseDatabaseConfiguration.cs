namespace EngineBay.Persistence
{
    using Microsoft.Extensions.DependencyInjection;

    public abstract class BaseDatabaseConfiguration
    {
        public static DatabaseProviderTypes GetDatabaseProvider()
        {
            var databaseProviderEnvironmentVariable = Environment.GetEnvironmentVariable("DATABASE_PROVIDER");

            if (string.IsNullOrEmpty(databaseProviderEnvironmentVariable))
            {
                Console.WriteLine("Warning: DATABASE_PROVIDER not configured, using SQLite database.");
                return DatabaseProviderTypes.SQLite;
            }

            var databaseProvider = (DatabaseProviderTypes)Enum.Parse(typeof(DatabaseProviderTypes), databaseProviderEnvironmentVariable);

            if (!Enum.IsDefined(typeof(DatabaseProviderTypes), databaseProvider) | databaseProvider.ToString().Contains(',', StringComparison.InvariantCulture))
            {
                Console.WriteLine($"Warning: '{databaseProviderEnvironmentVariable}' is not a valid DATABASE_PROVIDER configuration option. Valid options are: ");
                foreach (string name in Enum.GetNames(typeof(DatabaseProviderTypes)))
                {
                    Console.Write(name);
                    Console.Write(", ");
                }

                throw new ArgumentException("Invalid DATABASE_PROVIDER configuration.");
            }

            return databaseProvider;
        }

        public void RegisterDatabases(IServiceCollection services)
        {
            var databaseProvider = GetDatabaseProvider();
            var connectionString = GetDatabaseConnectionString(databaseProvider);

            if (!ConnectionStringValidator.IsValid(databaseProvider, connectionString))
            {
                throw new ArgumentException("Invalid CONNECTION_STRING configuration.");
            }

            switch (databaseProvider)
            {
                case DatabaseProviderTypes.InMemory:
                    this.ConfigureInMemory(services, connectionString);
                    break;
                case DatabaseProviderTypes.SQLite:
                    this.ConfigureSqlite(services, connectionString);
                    break;
                case DatabaseProviderTypes.SqlServer:
                    this.ConfigureSqlServer(services, connectionString);
                    break;
                default:
                    throw new ArgumentException($"Unhandled DATABASE_PROVIDER configuration of '{databaseProvider}'.");
            }
        }

        protected virtual void ConfigureSqlServer(IServiceCollection services, string connectionString)
        {
            throw new NotImplementedException();
        }

        protected virtual void ConfigureInMemory(IServiceCollection services, string connectionString)
        {
            throw new NotImplementedException();
        }

        protected virtual void ConfigureSqlite(IServiceCollection services, string connectionString)
        {
            throw new NotImplementedException();
        }

        private static string GetDatabaseConnectionString(DatabaseProviderTypes databaseProvider)
        {
            if (databaseProvider == DatabaseProviderTypes.InMemory)
            {
                return DatabaseConfigurationConstants.DefaultInMemoryConnectiontring;
            }

            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            if (!string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }

            if (databaseProvider == DatabaseProviderTypes.SQLite)
            {
                return DatabaseConfigurationConstants.DefaultSqliteConnectiontring;
            }

            throw new ArgumentException("Invalid CONNECTION_STRING configuration.");
        }
    }
}