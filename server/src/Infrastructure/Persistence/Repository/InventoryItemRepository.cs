using System;

using Application.Contracts.Persistence;
using Application.Models;
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

        // Filtering (search by item number, item name, description, category, manufacturer, manufacturer part number, universal product code, supplier)
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLowerInvariant();

            // Look for matches
            query = query.Where(i =>
                i.ItemNumber.ToLowerInvariant().Contains(search) ||
                i.ItemName.ToLowerInvariant().Contains(search) ||
                i.Description.ToLowerInvariant().Contains(search) ||
                i.Category.ToString().ToLowerInvariant().Contains(search) ||
                i.Manufacturer.ToLowerInvariant().Contains(search) ||
                i.ManufacturerPartNumber.ToLowerInvariant().Contains(search) ||
                i.UniversalProductCode.ToLowerInvariant().Contains(search) ||
                i.Supplier.ToLowerInvariant().Contains(search) ||
                i.WeightKG.ToString().ToLowerInvariant().Contains(search)
            );
        }

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