namespace EngineBay.Persistence
{
    public static class DatabaseConfigurationConstants
    {
        public const string DefaultSqliteConnectiontring = "Data Source=enginebay.db;Cache=Shared";
        public const string DefaultInMemoryConnectiontring = "DataSource=file::memory:?cache=shared";
    }
}