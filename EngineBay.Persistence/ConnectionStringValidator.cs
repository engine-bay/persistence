namespace EngineBay.Persistence
{
    using Microsoft.Data.SqlClient;
    using Microsoft.Data.Sqlite;
    using Npgsql;

    public static class ConnectionStringValidator
    {
        public static bool IsValid(DatabaseProviderTypes databaseProviderType, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return false;
            }

            switch (databaseProviderType)
            {
                case DatabaseProviderTypes.InMemory:
                case DatabaseProviderTypes.SQLite:
                    return IsValidSqliteConnectionString(connectionString);
                case DatabaseProviderTypes.SqlServer:
                    return IsValidSqlServerConnectionString(connectionString);
                case DatabaseProviderTypes.Postgres:
                    return IsValidPostgresConnectionString(connectionString);
                default:
                    return false;
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

        private static bool IsValidPostgresConnectionString(string connectionString)
        {
#pragma warning disable CA1031 // We want to catch any kind of configuration exception thrown here and explicitly not re-throw it
            try
            {
                var connection = new NpgsqlConnectionStringBuilder(connectionString);
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
    }
}