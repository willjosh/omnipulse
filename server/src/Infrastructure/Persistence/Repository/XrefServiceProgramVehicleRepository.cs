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

    private static IQueryable<XrefServiceProgramVehicle> ApplySorting(IQueryable<XrefServiceProgramVehicle> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "vehiclename" => sortDescending ?
                query.OrderByDescending(x => x.Vehicle.Name) :
                query.OrderBy(x => x.Vehicle.Name),
            "addedat" => sortDescending ?
                query.OrderByDescending(x => x.AddedAt) :
                query.OrderBy(x => x.AddedAt),
            _ => query.OrderBy(x => x.Vehicle.Name) // Default Sorting = Vehicle Name
        };
    }

    public async Task<PagedResult<XrefServiceProgramVehicle>> GetAllByServiceProgramIDPagedAsync(int serviceProgramId, PaginationParameters parameters)
    {
        var query = _dbSet
            .Include(x => x.Vehicle)
                .ThenInclude(v => v.VehicleGroup)
            .Where(x => x.ServiceProgramID == serviceProgramId)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLowerInvariant();
            query = query.Where(x =>
                x.Vehicle.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase) ||
                x.Vehicle.Make.Contains(search, StringComparison.InvariantCultureIgnoreCase) ||
                x.Vehicle.Model.Contains(search, StringComparison.InvariantCultureIgnoreCase) ||
                x.Vehicle.VehicleType.ToString().Contains(search, StringComparison.InvariantCultureIgnoreCase) ||
                x.Vehicle.VehicleGroup.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)
            );
        }

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

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