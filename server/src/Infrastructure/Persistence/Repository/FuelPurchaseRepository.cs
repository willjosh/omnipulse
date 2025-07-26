using Application.Contracts.Persistence;
using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class FuelPurchasesRepository : GenericRepository<FuelPurchase>, IFuelPurchaseRepository
{
    public FuelPurchasesRepository(OmnipulseDatabaseContext context) : base(context) { }

    public async Task<bool> IsValidOdometerReading(int vehicleId, double odometerReading)
    {
        var vehicle = await _dbSet.FindAsync(vehicleId);
        return vehicle == null || odometerReading >= vehicle.OdometerReading;
    }

    public async Task<bool> IsReceiptNumberUniqueAsync(string receiptNumber)
    {
        return !await _dbSet.AnyAsync(fp => fp.ReceiptNumber == receiptNumber);
    }

    // Helper method to apply sorting based on parameters
    private IQueryable<FuelPurchase> ApplySorting(IQueryable<FuelPurchase> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "VehicleId" => sortDescending ?
                query.OrderByDescending(fp => fp.VehicleId) :
                query.OrderBy(fp => fp.VehicleId),
            "PurchasedByUserId" => sortDescending ?
                query.OrderByDescending(fp => fp.PurchasedByUserId) :
                query.OrderBy(fp => fp.PurchasedByUserId),
            "PurchaseDate" => sortDescending ?
                query.OrderByDescending(fp => fp.PurchaseDate) :
                query.OrderBy(fp => fp.PurchaseDate),
            "OdometerReading" => sortDescending ?
                query.OrderByDescending(fp => fp.OdometerReading) :
                query.OrderBy(fp => fp.OdometerReading),
            "Volume" => sortDescending ?
                query.OrderByDescending(fp => fp.Volume) :
                query.OrderBy(fp => fp.Volume),
            "PricePerUnit" => sortDescending ?
                query.OrderByDescending(fp => fp.PricePerUnit) :
                query.OrderBy(fp => fp.PricePerUnit),
            "TotalCost" => sortDescending ?
                query.OrderByDescending(fp => fp.TotalCost) :
                query.OrderBy(fp => fp.TotalCost),
            "FuelStation" => sortDescending ?
                query.OrderByDescending(fp => fp.FuelStation) :
                query.OrderBy(fp => fp.FuelStation),
            "ReceiptNumber" => sortDescending ?
                query.OrderByDescending(fp => fp.ReceiptNumber) :
                query.OrderBy(fp => fp.ReceiptNumber),
            _ => query.OrderBy(fp => fp.ID) // Default sorting by ID
        };
    }
    public async Task<PagedResult<FuelPurchase>> GetAllFuelPurchasesPagedAsync(PaginationParameters parameters)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLowerInvariant();

            // Look for matches
            query = query.
                Where(fp =>
                    fp.ReceiptNumber.ToLowerInvariant().Contains(search) ||
                    fp.PurchasedByUserId.ToLowerInvariant().Contains(search) ||
                    fp.FuelStation.ToLowerInvariant().Contains(search)
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

        return new PagedResult<FuelPurchase>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }
}