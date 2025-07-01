using System;
using System.Linq.Expressions;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IGenericRepository<T> where T : BaseEntity
{
    // Basic Get
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> GetAllAsync();

    // Find with conditions
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    // Check operations
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();

    // CRUD operations
    Task<T> AddAsync(T entity);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    void Delete(T entity);
    void Delete(int id);

    // Save changes
    Task<int> SaveChangesAsync();
}