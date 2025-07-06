using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IServiceTaskRepository : IGenericRepository<ServiceTask>
{
    public Task<bool> IsNameUniqueAsync(string name);
    public Task<PagedResult<ServiceTask>> GetAllServiceTasksPagedAsync(PaginationParameters parameters);
}