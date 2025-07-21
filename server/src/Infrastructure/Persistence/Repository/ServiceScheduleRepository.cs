using Application.Contracts.Persistence;
using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class ServiceScheduleRepository : GenericRepository<ServiceSchedule>, IServiceScheduleRepository
{
    public ServiceScheduleRepository(OmnipulseDatabaseContext context) : base(context) { }

    public async Task<List<ServiceSchedule>> GetAllByServiceProgramIDAsync(int serviceProgramID)
    {
        return await _dbSet
            .Where(ss => ss.ServiceProgramID == serviceProgramID)
            .ToListAsync();
    }

    public async Task<List<ServiceSchedule>> GetAllWithServiceTasksByServiceProgramIDAsync(int serviceProgramID)
    {
        return await _dbSet
            .Include(ss => ss.XrefServiceScheduleServiceTasks)
                .ThenInclude(xref => xref.ServiceTask)
            .Where(ss => ss.ServiceProgramID == serviceProgramID)
            .ToListAsync();
    }

    public async Task<PagedResult<ServiceSchedule>> GetAllServiceSchedulesPagedAsync(PaginationParameters parameters)
    {
        return await BuildPagedServiceScheduleQuery(parameters);
    }

    public async Task<PagedResult<ServiceSchedule>> GetAllByServiceProgramIDPagedAsync(int serviceProgramId, PaginationParameters parameters)
    {
        return await BuildPagedServiceScheduleQuery(parameters, serviceProgramId);
    }

    private async Task<PagedResult<ServiceSchedule>> BuildPagedServiceScheduleQuery(PaginationParameters parameters, int? serviceProgramID = null)
    {
        var query = _dbSet
            .Include(ss => ss.ServiceProgram)
            .Include(ss => ss.XrefServiceScheduleServiceTasks)
                .ThenInclude(xref => xref.ServiceTask)
            .AsQueryable();

        if (serviceProgramID.HasValue)
        {
            query = query.Where(ss => ss.ServiceProgramID == serviceProgramID.Value);
        }

        // Filtering (search)
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLowerInvariant();
            query = query.Where(ss =>
                ss.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase) ||
                ss.ServiceProgram.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)
            );
        }

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

        // Total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<ServiceSchedule>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private static IQueryable<ServiceSchedule> ApplySorting(IQueryable<ServiceSchedule> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "name" => sortDescending ? query.OrderByDescending(ss => ss.Name) : query.OrderBy(ss => ss.Name),
            "isactive" => sortDescending ? query.OrderByDescending(ss => ss.IsActive) : query.OrderBy(ss => ss.IsActive),
            "createdat" => sortDescending ? query.OrderByDescending(ss => ss.CreatedAt) : query.OrderBy(ss => ss.CreatedAt),
            "updatedat" => sortDescending ? query.OrderByDescending(ss => ss.UpdatedAt) : query.OrderBy(ss => ss.UpdatedAt),
            _ => query.OrderBy(ss => ss.ID)
        };
    }
}