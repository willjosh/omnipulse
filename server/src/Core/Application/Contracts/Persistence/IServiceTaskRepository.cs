using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IServiceTaskRepository : IGenericRepository<ServiceTask>
{
    /// <param name="name">The name of the service task to check for existence.</param>
    /// <returns>Task result that contains true if a service task with the specified name exists; otherwise, false.</returns>
    Task<bool> DoesNameExistAsync(string name);

    /// <summary>
    /// Checks if a service task with the specified name exists, excluding the service task with the given ID.
    /// This is useful for update operations where you want to check for duplicates but allow the current entity to keep its name.
    /// </summary>
    /// <param name="name">The name of the service task to check for existence.</param>
    /// <param name="excludeServiceTaskId">The ID of the service task to exclude from the check.</param>
    /// <returns>Task result that contains true if a service task with the specified name exists (excluding the specified ID); otherwise, false.</returns>
    Task<bool> DoesNameExistExcludingIdAsync(string name, int excludeServiceTaskId);

    Task<PagedResult<ServiceTask>> GetAllServiceTasksPagedAsync(PaginationParameters parameters);
}