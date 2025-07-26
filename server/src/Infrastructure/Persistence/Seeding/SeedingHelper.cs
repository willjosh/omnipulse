using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Persistence.DatabaseContext;

namespace Persistence.Seeding;

public static class SeedingHelper
{
    /// <summary>
    /// Retrieves an entity of type <typeparamref name="T"/> at the specified logical index (with modulo wrapping).
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="context">Database context</param>
    /// <param name="index">The desired index (will wrap with modulo if out of range)</param>
    /// <param name="logger">Logger</param>
    /// <returns>The entity at the calculated index, or null if no entities exist</returns>
    public static T? GetEntityByIndex<T>(OmnipulseDatabaseContext context, int index, ILogger logger) where T : class
    {
        var entitySet = context.Set<T>().AsNoTracking();

        int count = entitySet.Count();
        if (count == 0)
        {
            logger.LogWarning("No {EntityType} found. Cannot get entity at index {Index}.", typeof(T).Name, index);
            return null;
        }

        int actualIndex = ((index % count) + count) % count;

        return entitySet
            .TagWith($"{nameof(GetEntityByIndex)}<{typeof(T).Name}>(index: {index})")
            .Skip(actualIndex)
            .FirstOrDefault();
    }

    /// <summary>
    /// Gets all entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="context">Database context</param>
    /// <param name="logger">Logger</param>
    /// <returns>List of entities, or empty list if none exist</returns>
    public static List<T> GetEntities<T>(OmnipulseDatabaseContext context, ILogger logger) where T : class
    {
        var entities = context
            .Set<T>()
            .AsNoTracking()
            .TagWith($"{nameof(GetEntities)}<{typeof(T).Name}>")
            .ToList();

        if (entities.Count == 0) logger.LogWarning("No {EntityType} found.", typeof(T).Name);

        return entities;
    }

    /// <summary>
    /// Projects all entities of type <typeparamref name="T"/> to a desired shape using the given <paramref name="selector"/>.
    /// </summary>
    /// <typeparam name="T">The source entity type</typeparam>
    /// <typeparam name="TResult">The projection result type</typeparam>
    /// <param name="context">Database context</param>
    /// <param name="selector">Projection function (e.g., entity => entity.Id)</param>
    /// <param name="logger">Logger instance</param>
    /// <returns>List of projected results</returns>
    public static List<TResult> ProjectEntities<T, TResult>(
        OmnipulseDatabaseContext context,
        Expression<Func<T, TResult>> selector,
        ILogger logger) where T : class
    {
        var projected = context.Set<T>()
            .AsNoTracking()
            .TagWith($"{nameof(ProjectEntities)}<{typeof(T).Name}, {typeof(TResult).Name}>")
            .Select(selector)
            .ToList();

        if (projected.Count == 0)
        {
            logger.LogWarning("No {EntityType} found when projecting entities.", typeof(T).Name);
        }

        return projected;
    }

    /// <summary>
    /// Checks if entities exist.
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="context">Database context</param>
    /// <param name="logger">Logger</param>
    /// <returns>True if entities exist, false otherwise</returns>
    public static bool CheckEntitiesExist<T>(OmnipulseDatabaseContext context, ILogger logger) where T : class
    {
        if (context.Set<T>().AsNoTracking().Take(1).Any()) return true;

        logger.LogWarning("No {EntityType} found.", typeof(T).Name);
        return false;
    }
}