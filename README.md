# EngineBay.Persistence

[![NuGet version](https://badge.fury.io/nu/EngineBay.Persistence.svg)](https://badge.fury.io/nu/EngineBay.Persistence)
[![Maintainability](https://api.codeclimate.com/v1/badges/1b29f03933f09b46893e/maintainability)](https://codeclimate.com/github/engine-bay/persistence/maintainability)
[![Test Coverage](https://api.codeclimate.com/v1/badges/1b29f03933f09b46893e/test_coverage)](https://codeclimate.com/github/engine-bay/persistence/test_coverage)


Persistence module for EngineBay published to [EngineBay.Persistence](https://www.nuget.org/packages/EngineBay.Persistence/) on NuGet.

## About

The persistence module provides structures to configure any database connections that modules in your application might need.

The DbContexts from this module should be inherited by the DbContexts of any module that needs to use a database. To support [Command and Query Responsibility Segregation (CQRS)](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs), a read-optimised and a write-optimised DbContext are provided - though a general-purpose one is also provided. 

The [TimestampInterceptor](EngineBay.Persistence/Interceptors/TimestampInterceptor.cs) will add creation and modification timestamps to any model that implements [EngineBay.Core's](https://github.com/engine-bay/core) [BaseModel](https://github.com/engine-bay/core/blob/main/EngineBay.Core/Models/BaseModel.cs).

## Usage

To use this module in another, you will need to create three DbContexts - generic, read, and write - so that they can be registered.

With the currently preferred inheritance structure, your generic context should inherit from [ModuleWriteDbContext](EngineBay.Persistence/DbContexts/ModuleWriteDbContext.cs). Declare all your DbSets in this context. For example:

```cs
namespace EngineBay.Blueprints
{
    using EngineBay.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class BlueprintsDbContext : ModuleWriteDbContext
    {
        public BlueprintsDbContext(DbContextOptions<ModuleWriteDbContext> options)
            : base(options)
        {
        }

        public DbSet<Workbook> Workbooks { get; set; } = null!;

        public DbSet<Blueprint> Blueprints { get; set; } = null!;

        // More DbSets...

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Workbook.CreateDataAnnotations(modelBuilder);
            Blueprint.CreateDataAnnotations(modelBuilder);
            // More annotations...

            base.OnModelCreating(modelBuilder);
        }
    }
}
```

Your read context can inherit from this:

```cs
namespace EngineBay.Blueprints
{
    using EngineBay.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class BlueprintsQueryDbContext : BlueprintsDbContext
    {
        public BlueprintsQueryDbContext(DbContextOptions<ModuleWriteDbContext> options)
            : base(options)
        {
        }
    }
}
```

And, in turn, your write context from the read context:

```cs
namespace EngineBay.Blueprints
{
    using EngineBay.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class BlueprintsWriteDbContext : BlueprintsQueryDbContext
    {
        public BlueprintsWriteDbContext(DbContextOptions<ModuleWriteDbContext> options)
            : base(options)
        {
        }
    }
}
```

If you desire extra functionality for any of the contexts, such as an auditing interceptor from [EngineBay.Auditing](https://github.com/engine-bay/auditing), you can access the options builder with the OnConfiguring method, like this:

```cs
namespace EngineBay.Blueprints
{
    using EngineBay.Auditing;
    using EngineBay.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class BlueprintsWriteDbContext : BlueprintsQueryDbContext
    {
        private readonly IAuditingInterceptor auditingInterceptor;

        public BlueprintsWriteDbContext(DbContextOptions<ModuleWriteDbContext> options, IAuditingInterceptor auditingInterceptor)
            : base(options)
        {
            this.auditingInterceptor = auditingInterceptor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder is null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }

            optionsBuilder.AddInterceptors(this.auditingInterceptor);

            base.OnConfiguring(optionsBuilder);
        }
    }
}
```

When you've created your module's DbContexts, you will need add them to your module's registration class, such as this:

```cs
namespace EngineBay.Blueprints
{
    using EngineBay.Core;
    using EngineBay.Persistence;
    using FluentValidation;

    public class BlueprintsModule : BaseModule
    {
        public override IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
        {
            // Other services registration...

            var databaseConfiguration = new CQRSDatabaseConfiguration<BlueprintsDbContext, BlueprintsQueryDbContext, BlueprintsWriteDbContext>();
            databaseConfiguration.RegisterDatabases(services);

            return services;
        }

        // Other setup methods...
    }
}
```

You will then be able to use your DbContexts freely, as you might any other DbContext, though do try to keep CQRS principals in mind.

### Registration

This module cannot run on its own. You will need to register it and its DbContext in your application to use its functionality. See [EngineBay.CommunityEdition](https://github.com/engine-bay/engine-bay-ce)'s example of [module registration](https://github.com/engine-bay/engine-bay-ce/blob/main/EngineBay.CommunityEdition/Modules/ModuleRegistration.cs) for an example of how to do this.

```cs
        private static IEnumerable<IModule> GetRegisteredModules()
        {
            var modules = new List<IModule>();

            modules.Add(new PersistenceModule());
           // Other modules...

            Console.WriteLine($"Discovered {modules.Count} EngineBay modules");
            return modules;
        }
```

Note that you do not need to register the Persistence DbContexts. The ApplicationUsers DbSet that these contexts provide will be available to any other module's DbContexts by virtue of inheritance.

### Environment Variables

The following environment variables control the database configuration and behavior of EngineBay.

| Environment variable             | Default value |                    Options                    | Description                                                                                                                                                                                                                                                            |
|:---------------------------------|:-------------:|:---------------------------------------------:|:-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `DATABASE_PROVIDER`              |   `SQLite`    | `InMemory`, `SQLite`, `SqlServer`, `Postgres` | The relational database provider to use. Defaults to SQLite when not set.                                                                                                                                                                                              |
| `DATABASE_CONNECTION_STRING`     |    `none`     |                      N/A                      | The connection string to use for the configured `DATABASE_PROVIDER`                                                                                                                                                                                                    |
| `DATABASE_RESET`                 |    `false`    |            `true`, `false`, `none`            | This will ***RESET*** the database, deleting all tables and re-applying database migrations. This is intended for development and testing activities where a deterministic database state is required. Is always `true` when ``DATABASE_PROVIDER` is set to `InMemory` |
| `DATABASE_RESEED`                |    `false`    |            `true`, `false`, `none`            | This will ***RESEED*** the database with initial data. This is intended for development and testing activities where a deterministic database state is required.                                                                                                       |
| `DATABASE_SEED_DATA_PATH`        | `/seed-data`  |               `string`, `none`                | The directory to be used to look for seed data files.                                                                                                                                                                                                                  |
| `DATABASE_EXIT_AFTER_MIGRATIONS` |    `false`    |            `true`, `false`, `none`            | Force shutdown after migrations are completed. This is intended for use in simulating database migrations in CI environments.                                                                                                                                          |
| `DATABASE_EXIT_AFTER_SEEDING`    |    `false`    |            `true`, `false`, `none`            | Force shutdown after database (re)seeding is are completed. This is intended for use in simulating database migrations in CI environments. Only applies if `DATABASE_RESEED` is `true`                                                                                 |

## Dependencies

* [EngineBay.Core](https://github.com/engine-bay/core): Provides several shared classes and base interfaces.
* [EngineBay.Logging](https://github.com/engine-bay/logging): Provides configuration for sensitive data logging