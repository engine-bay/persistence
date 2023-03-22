namespace EngineBay.Persistence
{
    using Microsoft.Extensions.DependencyInjection;

    public abstract class BaseDatabaseConfiguration
    {
        public static DatabaseProviderTypes GetDatabaseProvider()
        {
            var databaseProviderEnvironmentVariable = Environment.GetEnvironmentVariable(ConfigurationConstants.DATABASEPROVIDER);

            if (string.IsNullOrEmpty(databaseProviderEnvironmentVariable))
            {
                Console.WriteLine($"Warning: {ConfigurationConstants.DATABASEPROVIDER} not configured, using SQLite database.");
                return DatabaseProviderTypes.SQLite;
            }

            var databaseProvider = (DatabaseProviderTypes)Enum.Parse(typeof(DatabaseProviderTypes), databaseProviderEnvironmentVariable);

            if (!Enum.IsDefined(typeof(DatabaseProviderTypes), databaseProvider) | databaseProvider.ToString().Contains(',', StringComparison.InvariantCulture))
            {
                Console.WriteLine($"Warning: '{databaseProviderEnvironmentVariable}' is not a valid {ConfigurationConstants.DATABASEPROVIDER} configuration option. Valid options are: ");
                foreach (string name in Enum.GetNames(typeof(DatabaseProviderTypes)))
                {
                    Console.Write(name);
                    Console.Write(", ");
                }

                throw new ArgumentException($"Invalid {ConfigurationConstants.DATABASEPROVIDER} configuration.");
            }

            return databaseProvider;
        }

        public static bool IsAuditingEnabled()
        {
            var auditingEnabledEnvironmentVariable = Environment.GetEnvironmentVariable(ConfigurationConstants.DATABASEAUDITINGENABLED);

            if (string.IsNullOrEmpty(auditingEnabledEnvironmentVariable))
            {
                return true;
            }

            bool auditingEnabled;
            if (bool.TryParse(auditingEnabledEnvironmentVariable, out auditingEnabled))
            {
                if (!auditingEnabled)
                {
                    Console.WriteLine($"Warning: Auditing has been disabled by {ConfigurationConstants.DATABASEAUDITINGENABLED} configuration.");
                }

                return auditingEnabled;
            }

            throw new ArgumentException($"Invalid {ConfigurationConstants.DATABASEAUDITINGENABLED} configuration.");
        }

        public void RegisterDatabases(IServiceCollection services)
        {
            var databaseProvider = GetDatabaseProvider();
            var connectionString = GetDatabaseConnectionString(databaseProvider);

            if (!ConnectionStringValidator.IsValid(databaseProvider, connectionString))
            {
                throw new ArgumentException($"Invalid {ConfigurationConstants.DATABASECONNECTIONSTRING} configuration.");
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
                case DatabaseProviderTypes.Postgres:
                    this.ConfigurePostgres(services, connectionString);
                    break;
                default:
                    throw new ArgumentException($"Unhandled {ConfigurationConstants.DATABASEPROVIDER} configuration of '{databaseProvider}'.");
            }
        }

        protected virtual void ConfigureSqlServer(IServiceCollection services, string connectionString)
        {
            throw new NotImplementedException();
        }

        protected virtual void ConfigurePostgres(IServiceCollection services, string connectionString)
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

        public static string GetDatabaseConnectionString(DatabaseProviderTypes databaseProvider)
        {
            if (databaseProvider == DatabaseProviderTypes.InMemory)
            {
                return DatabaseConfigurationConstants.DefaultInMemoryConnectiontring;
            }

            var connectionString = Environment.GetEnvironmentVariable(ConfigurationConstants.DATABASECONNECTIONSTRING);

            if (!string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }

            if (databaseProvider == DatabaseProviderTypes.SQLite)
            {
                return DatabaseConfigurationConstants.DefaultSqliteConnectiontring;
            }

            throw new ArgumentException($"Invalid {ConfigurationConstants.DATABASECONNECTIONSTRING} configuration.");
        }
    }
}