# Infrastructure Layer

---

The Infrastructure layer provides concrete implementations for external concerns and dependencies defined as abstractions in the Application layer. It implements Clean Architecture principles by handling all external integrations, data persistence, and technical concerns while keeping the core business logic isolated.

---

## Key Principles

- **Dependency Inversion:** Implements interfaces defined in the Application layer
- **External Concerns:** Handles database access, external services, file systems, and third-party integrations
- **Configuration Management:** Manages connection strings, service endpoints, and environment-specific settings
- **Technology Independence:** Keeps framework-specific code isolated from business logic
- **Testability:** Provides abstractions that can be easily mocked for testing

---

## Directory Structure

The Infrastructure layer is organized into two main areas:

### `Infrastructure/Infrastructure/`

External services and cross-cutting concerns:

- `Services/` – External service implementations
  - `AppLogger.cs` – Structured logging implementation
  - `CurrentUserService.cs` – User context management
  - `JwtService.cs` – JWT token generation and validation
  - `HandlebarsPdfGenerator.cs` – PDF generation service
- `BackgroundServices/` – Background task implementations
  - `UpdateServiceReminderStatusBackgroundService.cs` – Automated reminder processing
- `InfrastructureServiceRegistration.cs` – DI container registration

### `Infrastructure/Persistence/`

Data access and database-related concerns:

- `DatabaseContext/` – Entity Framework Core context
  - `OmnipulseDatabaseContext.cs` – Main database context
- `Configs/` – Entity type configurations
  - Entity-specific EF Core configurations (e.g., `VehicleConfiguration.cs`)
- `Repository/` – Repository pattern implementations
  - Concrete repository classes implementing Application layer interfaces
- `Migrations/` – Entity Framework Core database migrations
- `Seeders/` – Database seeding for development and testing
- `PersistenceServerRegistration.cs` – Persistence layer DI registration

---

## Architectural Integration

### Clean Architecture Compliance

The Infrastructure layer adheres to Clean Architecture principles:

1. **Outer Layer:** Contains framework-specific implementations
2. **Dependency Direction:** Depends on Application layer abstractions, never the reverse
3. **Plugin Architecture:** Can be swapped without affecting business logic
4. **Technology Agnostic:** Business logic remains independent of infrastructure choices

### Integration with Application Layer

- **Repository Pattern:** Implements `IRepository<T>` interfaces for data access
- **Service Abstractions:** Provides concrete implementations of service interfaces
- **Logging Abstractions:** Implements `IAppLogger<T>` for structured logging
- **External Services:** Implements interfaces for PDF generation, email, etc.

---

## Persistence Layer Details

### Entity Framework Core Integration

The persistence layer uses Entity Framework Core for data access:

- **Code-First Approach:** Database schema generated from domain entities
- **Migrations:** Version-controlled database schema changes
- **Configurations:** Type-safe entity configuration using Fluent API
- **Seeding:** Automated test data generation for development

### Repository Pattern Implementation

```csharp
public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(OmnipulseDatabaseContext context) : base(context) { }

    // Custom repository methods specific to Vehicle entity
}
```

---

## References

### Entity Framework Core

- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [EF Core Getting Started](https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app)
- [EF Core Performance Best Practices](https://docs.microsoft.com/en-us/ef/core/performance/)
- [Code First Migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Fluent API Configuration](https://docs.microsoft.com/en-us/ef/core/modeling/)
- [EF Core Indexes](https://docs.microsoft.com/en-us/ef/core/modeling/indexes)
- [Connection Pooling](https://docs.microsoft.com/en-us/ef/core/performance/advanced-performance-topics#connection-pooling)

### Clean Architecture

- [Microsoft Clean Architecture Guide](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
- [Clean Architecture by Robert Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Dependency Inversion Principle](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles#dependency-inversion)
- [.NET Application Architecture Guides](https://dotnet.microsoft.com/en-us/learn/dotnet/architecture-guides)

### Repository Pattern

- [Repository Pattern in .NET](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [Generic Repository Pattern](https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application)
- [Repository Pattern Best Practices](https://www.programmingwithwolfgang.com/repository-pattern-net-core/)

### Dependency Injection

- [Dependency Injection in .NET](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [Service Lifetimes](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#service-lifetimes)
- [Service Registration Patterns](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)

### Background Services

- [Background Services in .NET](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services)
- [IHostedService Interface](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.ihostedservice)
- [Background Tasks with Hosted Services](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services#background-tasks-with-hosted-services)

### Logging and Monitoring

- [Logging in .NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/)
- [Structured Logging](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/#log-message-template)
- [Microsoft.Extensions.Logging](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging)
- [High Performance Logging](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/loggermessage)

### Security

- [ASP.NET Core Security Overview](https://docs.microsoft.com/en-us/aspnet/core/security/)
- [JWT Authentication in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn)
- [Connection String Security](https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-strings#security)
- [Data Protection in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/)

### PDF Generation

- [HandlebarsDotNet Documentation](https://github.com/Handlebars-Net/Handlebars.Net)
- [HTML to PDF Conversion](https://github.com/HtmlToImage/HtmlToImage)
- [PDF Generation Best Practices](https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/documents-in-wpf)
