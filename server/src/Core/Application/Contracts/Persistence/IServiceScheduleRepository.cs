using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IServiceScheduleRepository : IGenericRepository<ServiceSchedule>
{
    /// <summary>
    /// Retrieves all service schedules for a given service program by ID.
    /// </summary>
    /// <param name="serviceProgramID">The ID of the <see cref="ServiceProgram"/>.</param>
    /// <returns>A list of <see cref="ServiceSchedule"/> entities for the given service program.</returns>
    Task<List<ServiceSchedule>> GetAllByServiceProgramIDAsync(int serviceProgramID);

    /// <summary>
    /// Retrieves all service schedules for a given service program.
    /// </summary>
    /// <param name="serviceProgramID">The ID of the <see cref="ServiceProgram"/>.</param>
    /// <returns>A list of <see cref="ServiceSchedule"/> entities for the given service program.</returns>
    Task<List<ServiceSchedule>> GetAllWithServiceTasksByServiceProgramIDAsync(int serviceProgramID);

    /// <summary>
    /// Retrieves all service schedules in the system.
    /// </summary>
    /// <param name="parameters">Pagination parameters.</param>
    /// <returns>A <see cref="PagedResult{ServiceSchedule}"/> containing the paged list of service schedules.</returns>
    Task<PagedResult<ServiceSchedule>> GetAllServiceSchedulesPagedAsync(PaginationParameters parameters);

    /// <summary>
    /// Retrieves all service schedules for a given service program, paged.
    /// </summary>
    /// <param name="serviceProgramID">The ID of the service program.</param>
    /// <param name="parameters">The pagination parameters.</param>
    /// <returns>A paged result containing <see cref="ServiceSchedule"/> entities for the given service program.</returns>
    Task<PagedResult<ServiceSchedule>> GetAllByServiceProgramIDPagedAsync(int serviceProgramID, PaginationParameters parameters);

    /// <summary>
    /// Retrieves a single service schedule by ID with its associated service tasks.
    /// </summary>
    /// <param name="serviceScheduleID">The ID of the service schedule.</param>
    /// <returns>A <see cref="ServiceSchedule"/> entity with its service tasks included, or null if not found.</returns>
    Task<ServiceSchedule?> GetByIdWithServiceTasksAsync(int serviceScheduleID);
}