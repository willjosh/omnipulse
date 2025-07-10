using Application.Contracts.Persistence;
using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class ServiceScheduleRepository : GenericRepository<ServiceSchedule>, IServiceScheduleRepository
{
    public ServiceScheduleRepository(OmnipulseDatabaseContext context) : base(context) { }

    public Task<PagedResult<ServiceSchedule>> GetAllServiceSchedulePagedAsync(PaginationParameters parameters)
    {
        throw new NotImplementedException();
    }
}