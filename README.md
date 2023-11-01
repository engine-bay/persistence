# EngineBay.Persistence

[![NuGet version](https://badge.fury.io/nu/EngineBay.Persistence.svg)](https://badge.fury.io/nu/EngineBay.Persistence)
[![Maintainability](https://api.codeclimate.com/v1/badges/1b29f03933f09b46893e/maintainability)](https://codeclimate.com/github/engine-bay/persistence/maintainability)
[![Test Coverage](https://api.codeclimate.com/v1/badges/1b29f03933f09b46893e/test_coverage)](https://codeclimate.com/github/engine-bay/persistence/test_coverage)


Persistence module for EngineBay published to [EngineBay.Persistence](https://www.nuget.org/packages/EngineBay.Persistence/) on NuGet.

## About

The persistence module provides base DbContexts that can be inherited by the DbContexts of any module that needs to use a database. In support of [Command and Query Responsibility Segregation (CQRS)](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs), a read-optimised and a write-optimised DbContext are provided - though a general-purpose one is also provided. 

## Usage

