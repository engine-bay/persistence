namespace EngineBay.Persistence
{
    using Microsoft.Extensions.DependencyInjection;

    public abstract class BaseDatabaseConfiguration
    {
        public static DatabaseProviderTypes GetDatabaseProvider()
        {
            var databaseProviderEnvironmentVariable = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.DATABASEPROVIDER);

            if (string.IsNullOrEmpty(databaseProviderEnvironmentVariable))
            {
                Console.WriteLine($"Warning: {EnvironmentVariableConstants.DATABASEPROVIDER} not configured, using {DatabaseProviderTypes.SQLite} database.");
                return DatabaseProviderTypes.SQLite;
            }

            var databaseProvider = (DatabaseProviderTypes)Enum.Parse(typeof(DatabaseProviderTypes), databaseProviderEnvironmentVariable);

            if (!Enum.IsDefined(typeof(DatabaseProviderTypes), databaseProvider) || databaseProvider.ToString().Contains(',', StringComparison.InvariantCulture))
            {
                Console.WriteLine($"Warning: '{databaseProviderEnvironmentVariable}' is not a valid {EnvironmentVariableConstants.DATABASEPROVIDER} configuration option. Valid options are: ");
                foreach (string name in Enum.GetNames(typeof(DatabaseProviderTypes)))
                {
                    Console.Write(name);
                    Console.Write(", ");
                }

                throw new ArgumentException($"Invalid {EnvironmentVariableConstants.DATABASEPROVIDER} configuration.");
            }

            return databaseProvider;
        }

        public static bool IsDatabaseReset()
        {
            var databaseProvider = GetDatabaseProvider();
            if (databaseProvider == DatabaseProviderTypes.InMemory)
            {
                Console.WriteLine($"Warning: {EnvironmentVariableConstants.DATABASEPROVIDER} was set to '{DatabaseProviderTypes.InMemory}', setting {EnvironmentVariableConstants.DATABASERESET} to 'true'.");
                return true;
            }

            var databaseResetString = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.DATABASERESET);

            if (string.IsNullOrEmpty(databaseResetString))
            {
                return false;
            }

            if (string.Equals(databaseResetString, "TRUE", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Warning: {EnvironmentVariableConstants.DATABASERESET} was set to 'true', this will RESET the database to default. I hope you know what you're doing...");
                return true;
            }

            return false;
        }

        public static bool IsDatabaseReseeded()
        {
            var databaseReseedString = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.DATABASERESEED);

            if (string.IsNullOrEmpty(databaseReseedString))
            {
                return false;
            }

            if (string.Equals(databaseReseedString, "TRUE", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Warning: {EnvironmentVariableConstants.DATABASERESEED} was set to 'true', this will attempt to insert default data into the database. I hope you know what you're doing...");
                return true;
            }

            return false;
        }

        public static bool ShouldExitAfterMigrations()
        {
            var exitAfterMigrationsString = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.DATABASEEXITAFTERMIGRATIONS);

            if (string.IsNullOrEmpty(exitAfterMigrationsString))
            {
                return false;
            }

            if (string.Equals(exitAfterMigrationsString, "TRUE", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Warning: {EnvironmentVariableConstants.DATABASEEXITAFTERMIGRATIONS} was set to 'true', this will exit the process once database migrations have been applied.");
                return true;
            }

            return false;
        }

        public static bool ShouldExitAfterSeeding()
        {
            var exitAfterSeedingString = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.DATABASEEXITAFTERSEEDING);

            if (string.IsNullOrEmpty(exitAfterSeedingString))
            {
                return false;
            }

            if (string.Equals(exitAfterSeedingString, "TRUE", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Warning: {EnvironmentVariableConstants.DATABASEEXITAFTERSEEDING} was set to 'true', this will exit the process once database seed data has been inserted.");
                return true;
            }

            return false;
        }

        public static string GetDatabaseConnectionString()
        {
            var databaseProvider = GetDatabaseProvider();
            if (databaseProvider == DatabaseProviderTypes.InMemory)
            {
                return DefaultConnectionStringConstants.DefaultInMemoryConnectiontring;
            }

            var connectionString = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.DATABASECONNECTIONSTRING);

            if (!string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }

            if (databaseProvider == DatabaseProviderTypes.SQLite)
            {
                return DefaultConnectionStringConstants.DefaultSqliteConnectiontring;
            }

            throw new ArgumentException($"Invalid {EnvironmentVariableConstants.DATABASECONNECTIONSTRING} configuration.");
        }

        public static bool IsDetailedDatabaseLoggingEnabled()
        {
            var loggingLevelEnvironmentVariable = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.LOGGINGLEVEL);

            if (string.IsNullOrEmpty(loggingLevelEnvironmentVariable))
            {
                return false;
            }

            var loggingLevel = (LogLevel)Enum.Parse(typeof(LogLevel), loggingLevelEnvironmentVariable);

            if (!Enum.IsDefined(typeof(LogLevel), loggingLevel) | loggingLevel.ToString().Contains(',', StringComparison.InvariantCulture))
            {
                Console.WriteLine($"Warning: '{loggingLevelEnvironmentVariable}' is not a valid {EnvironmentVariableConstants.LOGGINGLEVEL} configuration option. Valid options are: ");
                foreach (string name in Enum.GetNames(typeof(LogLevel)))
                {
                    Console.Write(name);
                    Console.Write(", ");
                }

                throw new ArgumentException($"Invalid {EnvironmentVariableConstants.LOGGINGLEVEL} configuration.");
            }

            return loggingLevel == LogLevel.Trace;
        }

        public void RegisterDatabases(IServiceCollection services)
        {
            var databaseProvider = GetDatabaseProvider();
            var connectionString = GetDatabaseConnectionString();

            if (!ConnectionStringValidator.IsValid(databaseProvider, connectionString))
            {
                throw new ArgumentException($"Invalid {EnvironmentVariableConstants.DATABASECONNECTIONSTRING} configuration.");
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
                    throw new ArgumentException($"Unhandled {EnvironmentVariableConstants.DATABASEPROVIDER} configuration of '{databaseProvider}'.");
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
    }
}