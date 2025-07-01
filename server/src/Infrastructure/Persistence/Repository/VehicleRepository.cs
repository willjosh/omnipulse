using Application.Contracts.Persistence;
using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;
using Domain.Entities.Enums;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(OmnipulseDatabaseContext context) : base(context) { }

    public async Task<PagedResult<Vehicle>> GetAllVehiclesPagedAsync(PaginationParameters parameters)
    {
        var query = _dbSet.AsQueryable();

        query = query
            .Include(v => v.User)
            .Include(v => v.VehicleGroup);

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLowerInvariant();

            // Look for matches
            query = query.
                Where(v =>
                    v.Name.ToLowerInvariant().Contains(search) ||
                    v.Make.ToLowerInvariant().Contains(search) ||
                    v.Model.ToLowerInvariant().Contains(search) ||
                    v.VIN.ToLowerInvariant().Contains(search) ||
                    v.Year.ToString().ToLowerInvariant().Contains(search) ||
                    v.Location.ToLowerInvariant().Contains(search) ||
                    v.LicensePlate.ToLowerInvariant().Contains(search)
                );
        }

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

        // get total count before pagination
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

    public async Task<Vehicle?> GetVehicleWithDetailsAsync(int vehicleId)
    {
        return await _dbSet
            .Include(v => v.User)
            .Include(v => v.VehicleGroup)
            .FirstOrDefaultAsync(v => v.ID == vehicleId);
    }

    public Task<bool> LicensePlateExistAsync(string licensePlate)
    {
        return _dbSet.AnyAsync(v => v.LicensePlate == licensePlate);
    }

    public async Task VehicleDeactivateAsync(int VehicleID)
    {
        var vehicle = await _dbSet.FindAsync(VehicleID);
        if (vehicle != null)
        {
            vehicle.Status = VehicleStatusEnum.INACTIVE;
        }
    }

    public async Task<bool> VinExistAsync(string VIN)
    {
        return await _dbSet.AnyAsync(v => v.VIN == VIN);
    }

    // Helper method to apply sorting based on parameters
    private IQueryable<Vehicle> ApplySorting(IQueryable<Vehicle> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "name" => sortDescending ?
                query.OrderByDescending(v => v.Name) :
                query.OrderBy(v => v.Name),
            "make" => sortDescending ?
                query.OrderByDescending(v => v.Make) :
                query.OrderBy(v => v.Make),
            "model" => sortDescending ?
                query.OrderByDescending(v => v.Model) :
                query.OrderBy(v => v.Model),
            "year" => sortDescending ?
                query.OrderByDescending(v => v.Year) :
                query.OrderBy(v => v.Year),
            "status" => sortDescending ?
                query.OrderByDescending(v => v.Status) :
                query.OrderBy(v => v.Status),
            "purchaseprice" => sortDescending ?
                query.OrderByDescending(v => v.PurchasePrice) :
                query.OrderBy(v => v.PurchasePrice),
            "mileage" => sortDescending ?
                query.OrderByDescending(v => v.Mileage) :
                query.OrderBy(v => v.Mileage),
            "enginehours" => sortDescending ?
                query.OrderByDescending(v => v.EngineHours) :
                query.OrderBy(v => v.EngineHours),
            "createdat" => sortDescending ?
                query.OrderByDescending(v => v.CreatedAt) :
                query.OrderBy(v => v.CreatedAt),
            "purchasedate" => sortDescending ?
                query.OrderByDescending(v => v.PurchaseDate) :
                query.OrderBy(v => v.PurchaseDate),
            "fuelcapacity" => sortDescending ?
                query.OrderByDescending(v => v.FuelCapacity) :
                query.OrderBy(v => v.FuelCapacity),
            "location" => sortDescending ?
                query.OrderByDescending(v => v.Location) :
                query.OrderBy(v => v.Location),

            _ => query.OrderBy(v => v.ID) // Default sorting by ID
        };
    }
}