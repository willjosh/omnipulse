using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IServiceProgramRepository : IGenericRepository<ServiceProgram>
{
    Task<bool> IsNameUniqueAsync(string name);
}