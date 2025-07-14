using Application.Contracts.Persistence;
using Domain.Entities;
using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class ServiceProgramRepository : GenericRepository<ServiceProgram>, IServiceProgramRepository
{
    public ServiceProgramRepository(OmnipulseDatabaseContext context) : base(context) { }
}