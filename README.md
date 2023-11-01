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



### Registration



### Environment Variables



## Dependencies