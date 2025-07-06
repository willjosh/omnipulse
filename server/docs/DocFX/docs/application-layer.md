# Application Layer

---

It contains:

- **Use Cases / Features** (Commands/Queries, Handlers)
- **DTOs (Data Transfer Objects)**
- **Validation Logic**
- **Mapping Profiles**
- **Abstractions for Services and Repositories**

---

## Key Principles

- **Separation of Concerns:** Keep business logic in the Domain layer. The Application layer should orchestrate, not implement, business rules.
- **CQRS Pattern:** Use Command and Query objects to separate read and write operations.
- **MediatR:** Use MediatR for decoupling request handling (commands/queries) from controllers and services.
- **Validation:** Use FluentValidation for input validation. Validators should be placed alongside their commands/queries.
- **Mapping:** Use AutoMapper for mapping between domain entities and DTOs.

---

## Directory Structure

- `Contracts/` – Abstractions for services, repositories, and logging
  - `Logger/`
  - `Persistence/` - Repository interfaces
- `Exceptions/` – Application-specific exceptions
- `Features/` – Organised by domain (e.g., Vehicles, Users)
  - `Entities` - See [FluentValidation](https://docs.fluentvalidation.net/en/latest/) and [MediatR](https://github.com/LuckyPennySoftware/MediatR)
    - `Command/` – Write operations (POST, PUT, DELETE)
    - `Query/` – Read operations (GET)
- `MappingProfiles/` – AutoMapper profiles. See [AutoMapper](https://docs.automapper.io/en/latest/)
- `Models/` – Application-specific models

---

## Adding New Features

1. **Create a Feature Folder:**
   - Place all commands, queries, handlers, and validators for the feature here.
2. **Define Commands/Queries:**
   - Use records or classes to represent requests.
3. **Implement Handlers:**
   - Use MediatR's `IRequestHandler` or `IRequestHandler<TRequest, TResponse>`.
4. **Add Validators:**
   - Use FluentValidation to enforce input rules.
5. **Update Mapping Profiles:**
   - Add mappings for new DTOs/entities.
6. **Write Unit Tests:**
   - Place tests in the corresponding test project.

---

## References

- [Microsoft Clean Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
