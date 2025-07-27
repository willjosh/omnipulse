# API Layer

---

The API layer serves as the entry point for all HTTP requests to the system. It provides RESTful endpoints that follow CQRS patterns and integrate with the Application layer through MediatR.

---

## Key Principles

- **RESTful Design:** Follow REST conventions with proper HTTP methods and status codes
- **CQRS Integration:** Use MediatR to delegate requests to appropriate Application layer handlers
- **Comprehensive Documentation:** XML comments for Swagger/OpenAPI documentation
- **Consistent Error Handling:** Standardised error responses with proper HTTP status codes
- **Logging:** Structured logging for all API operations
- **Validation:** Input validation through Application layer validators
- **Security:** CORS configuration and authentication/authorization setup

---

## Directory Structure

- `Program.cs` – Application bootstrap and configuration
- `appsettings.json` – Default configuration settings
- `appsettings.Development.json` – Development environment configuration
- `Api.http` – HTTP client test file for manual API testing
- `Controllers/` – HTTP endpoint controllers organised by domain
- `Properties/` – Launch settings and configuration
  - `launchSettings.json` – Debug and launch configuration

---

## Architectural Integration

### CQRS Pattern Implementation

The API layer implements CQRS (Command Query Responsibility Segregation) through:

- **Commands:** POST, PUT, DELETE operations that modify state
- **Queries:** GET operations that retrieve data
- **MediatR:** Mediator pattern for decoupling controllers from business logic

### Controller Responsibilities

Controllers in the API layer are thin and focused on:

1. **HTTP Protocol Handling:** Route requests, validate HTTP semantics
2. **Request Delegation:** Send commands/queries to Application layer via MediatR
3. **Response Formatting:** Convert Application layer responses to HTTP responses
4. **Error Translation:** Map Application exceptions to appropriate HTTP status codes

### Integration with Application Layer

- Controllers receive commands/queries from HTTP requests
- MediatR routes these to appropriate Application layer handlers
- Handlers execute business logic and return results
- Controllers format results as HTTP responses

---

## Controller Patterns

### Base Structure

All controllers follow a consistent pattern with dependency injection of `IMediator` and `ILogger`. The standard attributes ensure consistent routing and content negotiation.

---

## Configuration Patterns

### Service Registration

The API layer registers services from the following architectural layers:

- **Application Layer:** CQRS handlers, validators, mappers
- **Infrastructure Layer:** Database context, repositories, external integrations, logging

### Environment-Specific Configuration

- **Development:** Swagger UI, detailed error messages, CORS for localhost
- **Production:** Error handling middleware, restricted CORS policies

### Documentation Configuration

- XML comments are automatically included in Swagger documentation
- Controller and action documentation is generated from XML comments
- Test and coverage files are excluded from documentation generation

---

## Security

### CORS Policies

- **Development:** Permissive policies for local development
- **Production:** Restricted policies with specific allowed origins

### Authentication/Authorization

- ASP.NET Core Identity integration for user management
- JWT token support for API authentication
- Role-based authorization for endpoint access control

### Input Validation

- All input is validated through Application layer validators
- FluentValidation provides comprehensive validation rules
- Validation failures return appropriate HTTP status codes

---

## Development Workflow

### Adding New Controllers

1. **Create Controller Class:**
   - Inherit from ASP.NET Core's `ControllerBase`
   - Add standard routing attributes
   - Inject `IMediator` and `ILogger`

2. **Implement Endpoints:**
   - Follow REST conventions
   - Use appropriate HTTP methods
   - Add comprehensive XML documentation
   - Implement consistent error handling

3. **Create Application Layer Components:**
   - Commands and queries for business operations
   - Handlers for business logic execution
   - Validators for input validation

4. **Write Tests:**
   - Unit tests for controller logic
   - Integration tests for full request/response cycle

### Testing Strategies

- **Unit Testing:** Test controller logic in isolation
- **Integration Testing:** Test full request/response cycle
- **Manual Testing:** Use Swagger UI and HTTP client files
- **API Testing:** Automated tests for endpoint behavior

---

## Performance Considerations

### Async/Await Pattern

All controller actions are asynchronous to support:

- Non-blocking I/O operations
- Scalable request handling
- Proper resource utilization

### Cancellation Support

Controllers support cancellation tokens for:

- Request timeout handling
- Graceful shutdown support
- Resource cleanup

---

## References

- [ASP.NET Core Web API](https://learn.microsoft.com/en-us/aspnet/core/web-api/)
- [REST API Design Best Practices](https://learn.microsoft.com/en-us/azure/architecture/best-practices/api-design)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [CQRS Pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs)
