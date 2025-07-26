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
        // Build base query with includes
        var query = _dbSet
            .Include(x => x.Vehicle)
                .ThenInclude(v => v.VehicleGroup)
            .Where(x => x.ServiceProgramID == serviceProgramId);

        // Apply search filter
        query = ApplySearchFilter(query, parameters.Search);

        // Apply sorting
        query = parameters.SortBy?.ToLowerInvariant() switch
        {
            "vehiclename" => parameters.SortDescending ?
                query.OrderByDescending(x => x.Vehicle.Name) :
                query.OrderBy(x => x.Vehicle.Name),
            "make" => parameters.SortDescending ?
                query.OrderByDescending(x => x.Vehicle.Make) :
                query.OrderBy(x => x.Vehicle.Make),
            "model" => parameters.SortDescending ?
                query.OrderByDescending(x => x.Vehicle.Model) :
                query.OrderBy(x => x.Vehicle.Model),
            "vehiclegroup" => parameters.SortDescending ?
                query.OrderByDescending(x => x.Vehicle.VehicleGroup.Name) :
                query.OrderBy(x => x.Vehicle.VehicleGroup.Name),
            "addedat" => parameters.SortDescending ?
                query.OrderByDescending(x => x.AddedAt) :
                query.OrderBy(x => x.AddedAt),
            _ => query.OrderBy(x => x.Vehicle.Name) // Default Sorting = Vehicle Name
        };

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<XrefServiceProgramVehicle>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private static IQueryable<XrefServiceProgramVehicle> ApplySearchFilter(IQueryable<XrefServiceProgramVehicle> query, string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return query;

        string searchPattern = $"%{searchText.Trim().ToLowerInvariant()}%";

        return query.Where(x =>
            EF.Functions.Like(x.Vehicle.Name, searchPattern) ||
            EF.Functions.Like(x.Vehicle.Make, searchPattern) ||
            EF.Functions.Like(x.Vehicle.Model, searchPattern) ||
            EF.Functions.Like(x.Vehicle.VehicleType.ToString(), searchPattern) ||
            EF.Functions.Like(x.Vehicle.VehicleGroup.Name, searchPattern)
        );
    }

    public async Task<PagedResult<Vehicle>> GetAllServiceProgramVehiclesPagedAsync(int serviceProgramID, PaginationParameters parameters)
    {
        var query = _dbContext.XrefServiceProgramVehicles
            .Where(x => x.ServiceProgramID == serviceProgramID)
            .Include(x => x.Vehicle)
                .ThenInclude(v => v.VehicleGroup)
            .Select(x => x.Vehicle);

        // Apply search filter
        query = ApplyVehicleSearchFilter(query, parameters.Search);

        // Apply sorting
        query = parameters.SortBy?.ToLowerInvariant() switch
        {
            "name" or "vehiclename" => parameters.SortDescending ?
                query.OrderByDescending(v => v.Name) :
                query.OrderBy(v => v.Name),
            "make" => parameters.SortDescending ?
                query.OrderByDescending(v => v.Make) :
                query.OrderBy(v => v.Make),
            "model" => parameters.SortDescending ?
                query.OrderByDescending(v => v.Model) :
                query.OrderBy(v => v.Model),
            "vehiclegroup" => parameters.SortDescending ?
                query.OrderByDescending(v => v.VehicleGroup.Name) :
                query.OrderBy(v => v.VehicleGroup.Name),
            _ => query.OrderBy(v => v.Name) // Default Sorting = Vehicle Name
        };

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<Vehicle>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private static IQueryable<Vehicle> ApplyVehicleSearchFilter(IQueryable<Vehicle> query, string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return query;

        string searchPattern = $"%{searchText.Trim().ToLowerInvariant()}%";

        return query.Where(v =>
            EF.Functions.Like(v.Name, searchPattern) ||
            EF.Functions.Like(v.Make, searchPattern) ||
            EF.Functions.Like(v.Model, searchPattern) ||
            EF.Functions.Like(v.VehicleType.ToString(), searchPattern) ||
            EF.Functions.Like(v.VehicleGroup.Name, searchPattern)
        );
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