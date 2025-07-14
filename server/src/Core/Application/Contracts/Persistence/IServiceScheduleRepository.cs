using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IServiceScheduleRepository : IGenericRepository<ServiceSchedule>
{
    /// <summary>
    /// Retrieves all service schedules in the system.
    /// </summary>
    /// <param name="parameters">Pagination parameters.</param>
    /// <returns>A <see cref="PagedResult{ServiceSchedule}"/> containing the paged list of service schedules.</returns>
    Task<PagedResult<ServiceSchedule>> GetAllServiceSchedulesPagedAsync(PaginationParameters parameters);
}