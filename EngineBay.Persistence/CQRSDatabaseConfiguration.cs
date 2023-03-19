namespace EngineBay.Persistence
{
    using LinqKit;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public class CQRSDatabaseConfiguration<TDbContext, TDbQueryContext, TDbWriteContext> : BaseDatabaseConfiguration
        where TDbContext : DbContext
        where TDbQueryContext : DbContext
        where TDbWriteContext : DbContext
    {
        protected override void ConfigureSqlServer(IServiceCollection services, string connectionString)
        {
            // Register a general purpose db context that is not pooled
            services.AddDbContext<TDbContext>(
                options =>
                {
                    options.UseSqlServer(connectionString, options =>
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery).EnableRetryOnFailure())
                    .WithExpressionExpanding();
                });

            // Register a read only optimized db context
            services.AddDbContext<TDbQueryContext>(
                options =>
                {
                    options.UseSqlServer(connectionString, options =>
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery).EnableRetryOnFailure())
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .WithExpressionExpanding();
                });

            // Register a thread safe write optimized db context
            services.AddDbContext<TDbWriteContext>(
                options =>
                {
                    options.UseSqlServer(connectionString, options =>
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery).EnableRetryOnFailure())
                    .WithExpressionExpanding();
                });
        }

        protected override void ConfigurePostgres(IServiceCollection services, string connectionString)
        {
            // Register a general purpose db context that is not pooled
            services.AddDbContext<TDbContext>(
                options =>
                {
                    options.UseNpgsql(connectionString, options =>
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery).EnableRetryOnFailure())

                    .WithExpressionExpanding();
                });

            // Register a read only optimized db context
            services.AddDbContext<TDbQueryContext>(
                options =>
                {
                    options.UseNpgsql(connectionString, options =>
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery).EnableRetryOnFailure())
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .WithExpressionExpanding();
                });

            // Register a thread safe write optimized db context
            services.AddDbContext<TDbWriteContext>(
                options =>
                {
                    options.UseNpgsql(connectionString, options =>
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery).EnableRetryOnFailure())
                    .WithExpressionExpanding();
                });
        }

        protected override void ConfigureInMemory(IServiceCollection services, string connectionString)
        {
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

        protected override void ConfigureSqlite(IServiceCollection services, string connectionString)
        {
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
    }
}