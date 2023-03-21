namespace EngineBay.Persistence
{
    using LinqKit;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public class DatabaseConfiguration<TDbContext> : BaseDatabaseConfiguration
        where TDbContext : DbContext
    {
        protected override void ConfigureSqlServer(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<TDbContext>(
                options =>
                {
                    options.UseSqlServer(connectionString, options =>
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .WithExpressionExpanding();
                });
        }

        protected override void ConfigurePostgres(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<TDbContext>(
                options =>
                {
                    options.UseNpgsql(connectionString, options =>
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .WithExpressionExpanding();
                });
        }

        protected override void ConfigureInMemory(IServiceCollection services, string connectionString)
        {
#pragma warning disable CA2000 // We explicitly want to keep this conneciton open so that it is re-used each time by the dependency injection. When this connection is closed, the in-memory db is wiped.
            var connection = new SqliteConnection(connectionString);
#pragma warning restore CA2000
            connection.Open();

            services.AddDbContext<TDbContext>(
                options =>
                {
                    options.UseSqlite(connection, options =>
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                        .WithExpressionExpanding();
                });
        }

        protected override void ConfigureSqlite(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<TDbContext>(
                options =>
                {
                    options.UseSqlite(connectionString, options =>
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                        .WithExpressionExpanding();
                });
        }
    }
}