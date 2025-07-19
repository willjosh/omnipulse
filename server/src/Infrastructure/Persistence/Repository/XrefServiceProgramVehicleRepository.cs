using Application.Contracts.Persistence;
using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class XrefServiceProgramVehicleRepository : IXrefServiceProgramVehicleRepository
{
    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<XrefServiceProgramVehicle> _dbSet;

    public XrefServiceProgramVehicleRepository(OmnipulseDatabaseContext context)
    {
        _dbContext = context;
        _dbSet = _dbContext.Set<XrefServiceProgramVehicle>();
    }

    public async Task<List<XrefServiceProgramVehicle>> GetByServiceProgramIDAsync(int serviceProgramId)
    {
        return await _dbSet.Where(x => x.ServiceProgramID == serviceProgramId).ToListAsync();
    }

    public async Task<PagedResult<XrefServiceProgramVehicle>> GetAllByServiceProgramIDPagedAsync(int serviceProgramId, PaginationParameters parameters)
    {
        var baseQuery = BuildBaseQuery(serviceProgramId);
        var filteredQuery = ApplyXrefSearchFilter(baseQuery, parameters.Search);
        var sortedQuery = ApplyXrefSorting(filteredQuery, parameters.SortBy, parameters.SortDescending);

        return await ExecutePaginatedQuery(sortedQuery, parameters);
    }

    public async Task<PagedResult<Vehicle>> GetAllServiceProgramVehiclesPagedAsync(int serviceProgramId, PaginationParameters parameters)
    {
        var baseQuery = BuildBaseQuery(serviceProgramId).Select(x => x.Vehicle);
        var filteredQuery = ApplyVehicleSearchFilter(baseQuery, parameters.Search);
        var sortedQuery = ApplyVehicleSorting(filteredQuery, parameters.SortBy, parameters.SortDescending);

        return await ExecutePaginatedQuery(sortedQuery, parameters);
    }

    private IQueryable<XrefServiceProgramVehicle> BuildBaseQuery(int serviceProgramId)
    {
        return _dbSet
            .Include(x => x.Vehicle)
                .ThenInclude(v => v.VehicleGroup)
            .Where(x => x.ServiceProgramID == serviceProgramId);
    }

    private static IQueryable<XrefServiceProgramVehicle> ApplyXrefSearchFilter(IQueryable<XrefServiceProgramVehicle> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return query;

        var searchLower = search.ToLowerInvariant();
        return query.Where(x =>
            x.Vehicle.Name.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ||
            x.Vehicle.Make.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ||
            x.Vehicle.Model.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ||
            x.Vehicle.VehicleType.ToString().Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ||
            x.Vehicle.VehicleGroup.Name.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase)
        );
    }

    private static IQueryable<Vehicle> ApplyVehicleSearchFilter(IQueryable<Vehicle> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return query;

        var searchLower = search.ToLowerInvariant();
        return query.Where(v =>
            v.Name.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ||
            v.Make.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ||
            v.Model.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ||
            v.VehicleType.ToString().Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ||
            v.VehicleGroup.Name.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase)
        );
    }

    private static IQueryable<XrefServiceProgramVehicle> ApplyXrefSorting(IQueryable<XrefServiceProgramVehicle> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "vehiclename" => sortDescending ?
                query.OrderByDescending(x => x.Vehicle.Name) :
                query.OrderBy(x => x.Vehicle.Name),
            "make" => sortDescending ?
                query.OrderByDescending(x => x.Vehicle.Make) :
                query.OrderBy(x => x.Vehicle.Make),
            "model" => sortDescending ?
                query.OrderByDescending(x => x.Vehicle.Model) :
                query.OrderBy(x => x.Vehicle.Model),
            "vehiclegroup" => sortDescending ?
                query.OrderByDescending(x => x.Vehicle.VehicleGroup.Name) :
                query.OrderBy(x => x.Vehicle.VehicleGroup.Name),
            "addedat" => sortDescending ?
                query.OrderByDescending(x => x.AddedAt) :
                query.OrderBy(x => x.AddedAt),
            _ => query.OrderBy(x => x.Vehicle.Name) // Default Sorting = Vehicle Name
        };
    }

    private static IQueryable<Vehicle> ApplyVehicleSorting(IQueryable<Vehicle> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "name" or "vehiclename" => sortDescending ?
                query.OrderByDescending(v => v.Name) :
                query.OrderBy(v => v.Name),
            "make" => sortDescending ?
                query.OrderByDescending(v => v.Make) :
                query.OrderBy(v => v.Make),
            "model" => sortDescending ?
                query.OrderByDescending(v => v.Model) :
                query.OrderBy(v => v.Model),
            "vehiclegroup" => sortDescending ?
                query.OrderByDescending(v => v.VehicleGroup.Name) :
                query.OrderBy(v => v.VehicleGroup.Name),
            _ => query.OrderBy(v => v.Name) // Default Sorting = Vehicle Name
        };
    }

    private static async Task<PagedResult<T>> ExecutePaginatedQuery<T>(IQueryable<T> query, PaginationParameters parameters)
    {
        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    public async Task<List<XrefServiceProgramVehicle>> GetByVehicleIDAsync(int vehicleId)
    {
        return await _dbSet.Where(x => x.VehicleID == vehicleId).ToListAsync();
    }

    public async Task<bool> ExistsAsync(int serviceProgramId, int vehicleId)
    {
        return await _dbSet.AnyAsync(x => x.ServiceProgramID == serviceProgramId && x.VehicleID == vehicleId);
    }

    public async Task AddAsync(XrefServiceProgramVehicle xref)
    {
        await _dbSet.AddAsync(xref);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<XrefServiceProgramVehicle> xrefs)
    {
        await _dbSet.AddRangeAsync(xrefs);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(int serviceProgramId, int vehicleId)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(x => x.ServiceProgramID == serviceProgramId && x.VehicleID == vehicleId);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RemoveAllForServiceProgramAsync(int serviceProgramId)
    {
        var xrefs = await _dbSet.Where(x => x.ServiceProgramID == serviceProgramId).ToListAsync();
        if (xrefs.Count != 0)
        {
            _dbSet.RemoveRange(xrefs);
            await _dbContext.SaveChangesAsync();
        }
    }
}