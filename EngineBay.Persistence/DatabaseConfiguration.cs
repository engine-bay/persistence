namespace EngineBay.Persistence
{
    using LinqKit;
    using Microsoft.Data.SqlClient;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public class DatabaseConfiguration<TDbContext, TDbQueryContext, TDbWriteContext>
        where TDbContext : DbContext
        where TDbQueryContext : DbContext
        where TDbWriteContext : DbContext
    {
        public void RegisterDatabases(IServiceCollection services)
        {
            var databaseProvider = GetDatabaseProvider();
            var connectionString = GetDatabaseConnectionString(databaseProvider);

            switch (databaseProvider)
            {
                case DatabaseProviderTypes.InMemory:
                case DatabaseProviderTypes.SQLite:
                    ConfigureSqlite(services, connectionString);
                    break;
                case DatabaseProviderTypes.SqlServer:
                    ConfigureSqlServer(services, connectionString);
                    break;
                default:
                    throw new ArgumentException($"Unhandled DATABASE_PROVIDER configuration of '{databaseProvider}'.");
            }
        }

        private static bool IsValidSqlServerConnectionString(string connectionString)
        {
#pragma warning disable CA1031 // We want to catch any kind of configuration exception thrown here and explicitly not re-throw it
            try
            {
                var connection = new SqlConnection(connectionString);
                connection.Dispose();
            }
            catch (Exception)
            {
                return false;
            }
#pragma warning restore CA1031

            return true;
        }

        private static bool IsValidSqliteConnectionString(string connectionString)
        {
#pragma warning disable CA1031 // We want to catch any kind of configuration exception thrown here and explicitly not re-throw it
            try
            {
                var connection = new SqliteConnection(connectionString);
                connection.Dispose();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
#pragma warning restore CA1031

        }

        private static void ConfigureSqlServer(IServiceCollection services, string connectionString)
        {
            if (!IsValidSqlServerConnectionString(connectionString))
            {
                throw new ArgumentException("Invalid CONNECTION_STRING configuration.");
            }

            // Register a general purpose db context that is not pooled
            services.AddDbContext<TDbContext>(
                options =>
                {
                    options.UseSqlServer(connectionString, options =>
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .WithExpressionExpanding();
                });

            // Register a read only optimized db context
            services.AddDbContext<TDbQueryContext>(
                options =>
                {
                    options.UseSqlServer(connectionString, options =>
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .WithExpressionExpanding();
                });

            // Register a thread safe write optimized db context
            services.AddDbContext<TDbWriteContext>(
                options =>
                {
                    options.UseSqlServer(connectionString, options =>
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .WithExpressionExpanding();
                });
        }

        private static void ConfigureInMemory(IServiceCollection services, string connectionString)
        {
            if (!IsValidSqliteConnectionString(connectionString))
            {
                throw new ArgumentException("Invalid CONNECTION_STRING configuration.");
            }

#pragma warning disable CA2000 // We explicitly want to keep this conneciton open so that it is re-used each time by the dependency injection. When this connection is closed, the in-memory db is wiped.
            var connection = new SqliteConnection(connectionString);
#pragma warning restore CA2000
            connection.Open();

            // Register a general purpose db context
            services.AddDbContext<TDbContext>(
                options =>
                {
                    options.UseSqlite(connection, options =>
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                        .WithExpressionExpanding();
                }, ServiceLifetime.Singleton);

            // Register a read only optimized db context
            services.AddDbContext<TDbQueryContext>(
                options =>
                {
                    options.UseSqlite(connection, options =>
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .WithExpressionExpanding();
                }, ServiceLifetime.Singleton);

            // Register a thread safe write optimized db context
            services.AddDbContext<TDbWriteContext>(
                options =>
                {
                    options.UseSqlite(connection, options =>
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .WithExpressionExpanding();
                }, ServiceLifetime.Singleton);
        }

        private static void ConfigureSqlite(IServiceCollection services, string connectionString)
        {
            if (!IsValidSqliteConnectionString(connectionString))
            {
                throw new ArgumentException("Invalid CONNECTION_STRING configuration.");
            }

            // Register a general purpose db context
            services.AddDbContext<TDbContext>(
                options =>
                {
                    options.UseSqlite(connectionString, options =>
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                        .WithExpressionExpanding();
                }, ServiceLifetime.Singleton);

            // Register a read only optimized db context
            services.AddDbContext<TDbQueryContext>(
                options =>
                {
                    options.UseSqlite(connectionString, options =>
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .WithExpressionExpanding();
                }, ServiceLifetime.Singleton);

            // Register a thread safe write optimized db context
            services.AddDbContext<TDbWriteContext>(
                options =>
                {
                    options.UseSqlite(connectionString, options =>
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .WithExpressionExpanding();
                }, ServiceLifetime.Singleton);
        }

        private static DatabaseProviderTypes GetDatabaseProvider()
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

        private static string GetDatabaseConnectionString(DatabaseProviderTypes databaseProvider)
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            if (!string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }

            if (databaseProvider == DatabaseProviderTypes.SQLite)
            {
                return "Data Source=engine-api.db;";
            }

            if (databaseProvider == DatabaseProviderTypes.InMemory)
            {
                return "Filename=:memory:";
            }

            throw new ArgumentException("Invalid CONNECTION_STRING configuration.");
        }
    }
}