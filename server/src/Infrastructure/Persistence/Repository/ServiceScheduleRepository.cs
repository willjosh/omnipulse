using Application.Contracts.Persistence;

using Domain.Entities;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class ServiceScheduleRepository : GenericRepository<ServiceSchedule>, IServiceScheduleRepository
{
    public ServiceScheduleRepository(OmnipulseDatabaseContext context) : base(context) { }
}