using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IServiceProgramRepository : IGenericRepository<ServiceProgram>
{
    Task<bool> IsNameUniqueAsync(string name);
    Task<PagedResult<ServiceProgram>> GetAllServiceProgramsPagedAsync(PaginationParameters parameters);
}