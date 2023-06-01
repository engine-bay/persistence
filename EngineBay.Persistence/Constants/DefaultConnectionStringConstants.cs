namespace EngineBay.Persistence
{
    public static class DefaultConnectionStringConstants
    {
        public const string DefaultSqliteConnectiontring = "Data Source=enginebay.sqlite;Cache=Shared";
        public const string DefaultInMemoryConnectiontring = "DataSource=file::memory:?cache=shared";
    }
}