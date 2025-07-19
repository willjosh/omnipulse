using Application.Contracts.Persistence;
using Application.Features.ServicePrograms.Query.GetAllServiceProgramVehicle;
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
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var searchLower = parameters.Search.ToLowerInvariant();
            query = query.Where(x =>
                x.Vehicle.Name.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ||
                x.Vehicle.Make.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ||
                x.Vehicle.Model.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ||
                x.Vehicle.VehicleType.ToString().Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ||
                x.Vehicle.VehicleGroup.Name.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase)
            );
        }

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

    public async Task<PagedResult<Vehicle>> GetAllServiceProgramVehiclesPagedAsync(int serviceProgramID, PaginationParameters parameters)
    {
        var query = _dbContext.XrefServiceProgramVehicles
            .Where(x => x.ServiceProgramID == serviceProgramID)
            .Include(x => x.Vehicle)
                .ThenInclude(v => v.VehicleGroup)
            .Select(x => x.Vehicle);

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var searchLower = parameters.Search.ToLowerInvariant();
            query = query.Where(v =>
                v.Name.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ||
                v.Make.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ||
                v.Model.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ||
                v.VehicleType.ToString().Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ||
                v.VehicleGroup.Name.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase)
            );
        }

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

    public async Task<PagedResult<XrefServiceProgramVehicleDTO>> GetServiceProgramVehiclesWithMetadataPagedAsync(int serviceProgramID, PaginationParameters parameters)
    {
        var query = _dbContext.XrefServiceProgramVehicles
            .Where(x => x.ServiceProgramID == serviceProgramID)
            .Include(x => x.Vehicle)
                .ThenInclude(v => v.VehicleGroup)
            .Select(x => new XrefServiceProgramVehicleDTO
            {
                ServiceProgramID = x.ServiceProgramID,
                VehicleID = x.Vehicle.ID,
                VehicleName = x.Vehicle.Name,
                AddedAt = x.AddedAt
            });

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var searchLower = parameters.Search.ToLowerInvariant();
            query = query.Where(x =>
                x.VehicleName.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase)
            );
        }

        // Apply sorting
        query = parameters.SortBy?.ToLowerInvariant() switch
        {
            "vehiclename" or "name" => parameters.SortDescending ?
                query.OrderByDescending(x => x.VehicleName) :
                query.OrderBy(x => x.VehicleName),
            "addedat" => parameters.SortDescending ?
                query.OrderByDescending(x => x.AddedAt) :
                query.OrderBy(x => x.AddedAt),
            _ => query.OrderBy(x => x.VehicleName) // Default Sorting = Vehicle Name
        };

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<XrefServiceProgramVehicleDTO>
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