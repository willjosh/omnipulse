using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IServiceScheduleRepository : IGenericRepository<ServiceSchedule>
{
    Task<PagedResult<ServiceSchedule>> GetAllServiceSchedulePagedAsync(PaginationParameters parameters);
}