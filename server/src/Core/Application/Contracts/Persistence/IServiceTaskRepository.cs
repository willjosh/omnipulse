using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IServiceTaskRepository : IGenericRepository<ServiceTask>
{
    /// <param name="name">The name of the service task to check for existence.</param>
    /// <returns>Task result that contains true if a service task with the specified name exists; otherwise, false.</returns>
    Task<bool> DoesNameExistAsync(string name);
    Task<PagedResult<ServiceTask>> GetAllServiceTasksPagedAsync(PaginationParameters parameters);
}