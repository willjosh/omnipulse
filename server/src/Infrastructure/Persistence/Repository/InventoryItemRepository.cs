using Application.Contracts.Persistence;
using Application.Models.PaginationModels;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class InventoryItemRepository : GenericRepository<InventoryItem>, IInventoryItemRepository
{
    public InventoryItemRepository(OmnipulseDatabaseContext context) : base(context) { }

    public async Task<PagedResult<InventoryItem>> GetAllInventoryItemsPagedAsync(PaginationParameters parameters)
    {
        var query = _dbSet.AsQueryable();

        // Apply search filter
        query = ApplySearchFilter(query, parameters.Search);

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

        // get total count before pagination
        var totalCount = await query.CountAsync();

        // apply pagination
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<InventoryItem>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private static IQueryable<InventoryItem> ApplySearchFilter(IQueryable<InventoryItem> query, string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return query;

        string searchPattern = $"%{searchText.Trim().ToLowerInvariant()}%";

        return query.Where(i =>
            EF.Functions.Like(i.ItemNumber, searchPattern) ||
            EF.Functions.Like(i.ItemName, searchPattern) ||
            EF.Functions.Like(i.Description ?? string.Empty, searchPattern) ||
            (i.Category.HasValue && EF.Functions.Like(i.Category.Value.ToString(), searchPattern)) ||
            EF.Functions.Like(i.Manufacturer ?? string.Empty, searchPattern) ||
            EF.Functions.Like(i.ManufacturerPartNumber ?? string.Empty, searchPattern) ||
            EF.Functions.Like(i.UniversalProductCode ?? string.Empty, searchPattern) ||
            EF.Functions.Like(i.Supplier ?? string.Empty, searchPattern) ||
            (i.WeightKG.HasValue && EF.Functions.Like(i.WeightKG.Value.ToString(), searchPattern))
        );
    }

    public async Task<bool> IsItemNumberUniqueAsync(string itemNumber)
    {
        if (string.IsNullOrWhiteSpace(itemNumber))
            return false;
        return !await _dbSet.AnyAsync(i => i.ItemNumber.ToLower() == itemNumber.ToLower());
    }

    public async Task<bool> IsManufacturerPartNumberUniqueAsync(string? manufacturer, string? manufacturerPartNumber)
    {
        if (string.IsNullOrWhiteSpace(manufacturer) || string.IsNullOrWhiteSpace(manufacturerPartNumber))
            return false;
        return !await _dbSet.AnyAsync(i =>
            i.Manufacturer != null && i.ManufacturerPartNumber != null &&
            i.Manufacturer.ToLower() == manufacturer.ToLower() &&
            i.ManufacturerPartNumber.ToLower() == manufacturerPartNumber.ToLower()
        );
    }

    public async Task<bool> IsUniversalProductCodeUniqueAsync(string? universalProductCode)
    {
        if (string.IsNullOrWhiteSpace(universalProductCode))
            return false;
        return !await _dbSet.AnyAsync(i =>
            i.UniversalProductCode != null &&
            i.UniversalProductCode.ToLower() == universalProductCode.ToLower()
        );
    }

    private IQueryable<InventoryItem> ApplySorting(IQueryable<InventoryItem> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "itemnumber" => sortDescending ? query.OrderByDescending(i => i.ItemNumber) : query.OrderBy(i => i.ItemNumber),
            "itemname" => sortDescending ? query.OrderByDescending(i => i.ItemName) : query.OrderBy(i => i.ItemName),
            "description" => sortDescending ? query.OrderByDescending(i => i.Description) : query.OrderBy(i => i.Description),
            "category" => sortDescending ? query.OrderByDescending(i => i.Category.ToString()) : query.OrderBy(i => i.Category.ToString()),
            "manufacturer" => sortDescending ? query.OrderByDescending(i => i.Manufacturer) : query.OrderBy(i => i.Manufacturer),
            "manufacturerpartnumber" => sortDescending ? query.OrderByDescending(i => i.ManufacturerPartNumber) : query.OrderBy(i => i.ManufacturerPartNumber),
            "universalproductcode" => sortDescending ? query.OrderByDescending(i => i.UniversalProductCode) : query.OrderBy(i => i.UniversalProductCode),
            "supplier" => sortDescending ? query.OrderByDescending(i => i.Supplier) : query.OrderBy(i => i.Supplier),
            "weightkg" => sortDescending ? query.OrderByDescending(i => i.WeightKG) : query.OrderBy(i => i.WeightKG),
            "unitcost" => sortDescending ? query.OrderByDescending(i => i.UnitCost) : query.OrderBy(i => i.UnitCost),
            "unitcostmeasurementunit" => sortDescending ? query.OrderByDescending(i => i.UnitCostMeasurementUnit.ToString()) : query.OrderBy(i => i.UnitCostMeasurementUnit.ToString()),
            "createdat" => sortDescending ? query.OrderByDescending(i => i.CreatedAt) : query.OrderBy(i => i.CreatedAt),
            "updatedat" => sortDescending ? query.OrderByDescending(i => i.UpdatedAt) : query.OrderBy(i => i.UpdatedAt),
            _ => sortDescending ? query.OrderByDescending(i => i.ItemNumber) : query.OrderBy(i => i.ItemNumber)
        };
    }
}