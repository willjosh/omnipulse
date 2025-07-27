# Domain Layer

---

The Domain layer contains the core business logic, entities, and domain knowledge that represent the heart of Omnipulse. It implements Domain-Driven Design (DDD) principles and contains the business rules and domain concepts.

---

## Key Principles

- **Domain-Driven Design:** Entities, value objects, and domain services represent business concepts
- **Rich Domain Model:** Business logic encapsulated within domain entities
- **Aggregate Roots:** Entities that manage consistency boundaries
- **Value Objects:** Immutable objects representing domain concepts
- **Invariants:** Business rules enforced at the domain level

---

## Directory Structure

- `Entities/` – Core domain entities and aggregates
  - `BaseEntity.cs` – Common base class for all entities
  - `Enums/` – Domain value objects and enumerations

---

## Entity Patterns

### Base Entity

Most domain entities inherit from `BaseEntity`, which provides:

- **ID:** Primary key identifier
- **CreatedAt:** Entity creation timestamp
- **UpdatedAt:** Last modification timestamp

**Note:** Xref entities (junction tables) do not inherit from BaseEntity as they form composite primary keys from their foreign keys.

### Navigation Properties

Entities use navigation properties to express relationships:

- **One-to-Many:** `ICollection<T>` for child entities
- **Many-to-One:** Direct entity references
- **Many-to-Many:** Junction entities (Xref classes)

---

## Aggregate Boundaries

Aggregates define consistency boundaries in the domain:

- **Aggregate Root:** Entity that manages the aggregate's consistency
- **Aggregate Members:** Entities that belong to the aggregate
- **Consistency Rules:** Business rules that must be maintained within the aggregate
- **Transaction Boundaries:** Changes to aggregate members are atomic

---

## Development Guidelines

### Adding New Entities

1. **Inherit from BaseEntity:**
   - Provides common ID and timestamp properties
   - Ensures consistent entity structure

2. **Define Business Rules:**
   - Use domain enums for constrained values
   - Implement business invariants

3. **Establish Relationships:**
   - Define navigation properties
   - Use appropriate collection types
   - Consider aggregate boundaries

4. **Add Domain Logic:**
   - Encapsulate business rules in entity methods
   - Implement value objects for complex concepts

---

## References

- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [Aggregate Pattern](https://martinfowler.com/bliki/DDD_Aggregate.html)
- [Value Objects](https://martinfowler.com/bliki/ValueObject.html)
