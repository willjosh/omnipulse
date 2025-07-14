using Application.Contracts.Persistence;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class ServiceProgramRepository : GenericRepository<ServiceProgram>, IServiceProgramRepository
{
    public ServiceProgramRepository(OmnipulseDatabaseContext context) : base(context) { }

    public async Task<bool> IsNameUniqueAsync(string name)
    {
        return !await _dbSet.AnyAsync(sp => sp.Name == name);
    }
}