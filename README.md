# PostgreSQL-And-MongoDB-EFCore-Provider
If you interested in working with both PostgreSQL and MongoDB using Entity Framework Core in your ASP.NET Core project. Here's how you can integrate both databases using `Npgsql.EntityFrameworkCore.PostgreSQL` for PostgreSQL and `MongoDB.EntityFrameworkCore` for MongoDB. You can use more database providers if you want.

[Database Providers] : https://learn.microsoft.com/en-us/ef/core/providers/?tabs=dotnet-core-cli

To use MongoDB with Entity Framework Core (EF Core) in an ASP.NET Core application, you can leverage the MongoDB.EntityFrameworkCore provider. This package integrates MongoDB as a backend database while using familiar EF Core APIs.

[MongoDB] NuGet : https://www.nuget.org/packages/MongoDB.EntityFrameworkCore <br/>
[NpgSql] Nuget : https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL

Install the necessary NuGet packages for both MongoDB and PostgreSQL.
```console
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package MongoDB.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

Some api request and response for this project. Here are some examples

[POST] : [https://localhost:7245/api/user]
```json
{
  "id": "a26f2f74-b619-4905-b763-0dcd81e8c0cd",
  "name": "raihan nishat",
  "email": "raihan-nishat.dll@example.net",
  "address": {
    "street": "4",
    "city": "dhaka",
    "state": "bd",
    "zip": "1206"
  },
  "contacts": [
    {
      "type": "personal",
      "number": "+88 01710512211"
    }
  ]
}
```

[GET] : [https://localhost:7245/api/user/a26f2f74-b619-4905-b763-0dcd81e8c0cd/contacts]
```json
[
  {
    "type": "personal",
    "number": "+88 01710512211"
  }
]
```

[GET] : [https://localhost:7245/api/user/a26f2f74-b619-4905-b763-0dcd81e8c0cd/address]
```json
{
  "street": "4",
  "city": "dhaka",
  "state": "bd",
  "zip": "1206"
}
```


## OOP Principles and Considerations
### [1] Dependency Injection
Using IRepositoryFactory to resolve repositories dynamically can be seen as a pragmatic approach to avoid manually managing repository instances.

### [2] Single Responsibility Principle
The UnitOfWork class should manage the database context and coordinate transactions. By resolving repositories dynamically from the repository factory.

### [3] Explicit Dependencies Principle
Explicitly injecting dependencies into the UnitOfWork constructor is generally preferred. This approach makes the class's dependencies clear and improves testability.

## Dynamic entity configuration
MongoDbContext and PostgreSqlDbContext classes, which inherits from DbContext, override the OnModelCreating method to dynamically configure entity mappings based on the types that implement IEntity. 

## Unit of Work Pattern Rules
The UnitOfWork pattern is designed to maintain a list of objects affected by a business transaction and coordinate the writing out of changes and the resolution of concurrency problems. The way you've implemented the UnitOfWork class can work, but there are a few considerations regarding object-oriented programming (OOP) principles and the traditional intent of the UnitOfWork pattern. The traditional UnitOfWork pattern involves Keeping track of changes to objects and coordinating the persistence of those changes. Managing the lifecycle of repositories. 

## Key Points
### [1] Clearing Repositories
Since those repositories don't need explicit disposal, simply clearing the _repositories dictionary as you currently do is sufficient.

### [2] Disposing the DbContext
_context.Dispose(); is correctly used to dispose of the database context's connection.

### [3] Suppressing Finalization
GC.SuppressFinalize(this); is correctly used to prevent the finalizer from running since the cleanup is already handled.

## Minimal Api
Using static methods for mapping minimal API endpoints in ASP.NET Core has several advantages. Here’s a detailed explanation of why static methods are useful in this context.

### [1] No Instance Required
Static methods belong to the type itself rather than to instances of the type. This means you don't need to create an instance of the endpoint class to map the routes. This simplifies the code and reduces the overhead associated with object instantiation.

### [2] Simplifies Endpoint Registration
By using static methods, you can directly call the methods without needing to manage the lifecycle of the endpoint class. This is particularly useful for setting up minimal APIs where you want to keep the setup code concise and straightforward.

### [3] Thread Safety
Static methods are inherently thread-safe when they do not modify any shared state. Since the methods used for mapping endpoints typically only configure routing and do not maintain any state, they are safe to use in a multi-threaded environment like a web server.

### [4] Clear Separation of Concerns
Using static methods can help to clearly separate the endpoint definitions from other application logic. This can make your code easier to understand and maintain, as the endpoint definitions are isolated in a dedicated static class.
