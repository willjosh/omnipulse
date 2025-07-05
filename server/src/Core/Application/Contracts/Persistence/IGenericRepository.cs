using System;
using System.Linq.Expressions;

using Domain.Entities;

namespace Application.Contracts.Persistence;

/// <summary>
/// Defines a generic repository for performing data access operations on entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of entity managed by the repository, which must inherit from <see cref="BaseEntity"/>.</typeparam>
public interface IGenericRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Retrieves an entity by its ID.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <returns>The entity if found; otherwise, <c>null</c>.</returns>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves all entities.
    /// </summary>
    /// <returns>A read-only list of all entities.</returns>
    Task<IReadOnlyList<T>> GetAllAsync();

    /// <summary>
    /// Finds entities matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The condition to filter entities.</param>
    /// <returns>A read-only list of matching entities.</returns>
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Retrieves the first entity matching the specified predicate if found; otherwise, <c>null</c>.
    /// </summary>
    /// <param name="predicate">The condition to filter entities.</param>
    /// <returns>The first matching entity if found; otherwise, <c>null</c>.</returns>
    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Determines whether an entity with the specified ID exists.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <returns><c>true</c> if the entity exists; otherwise, <c>false</c>.</returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Counts the total number of entities.
    /// </summary>
    /// <returns>The total count of entities.</returns>
    Task<int> CountAsync();

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The added entity.</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Adds a range of entities to the repository.
    /// </summary>
    /// <param name="entities">The collection of entities to add.</param>
    /// <returns>The added entities.</returns>
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(T entity);

    /// <summary>
    /// Deletes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    void Delete(T entity);

    /// <summary>
    /// Deletes an entity with the specified ID from the repository.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    void Delete(int id);

    /// <summary>
    /// Saves all changes made in the repository to the underlying data store.
    /// </summary>
    /// <returns>The number of state entries written to the data store.</returns>
    Task<int> SaveChangesAsync();
}