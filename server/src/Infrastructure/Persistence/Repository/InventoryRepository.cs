using Application.Contracts.Persistence;
using Application.Models.PaginationModels;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
{
    public InventoryRepository(OmnipulseDatabaseContext context) : base(context)
    {
    }

    public async Task<PagedResult<Inventory>> GetAllInventoriesPagedAsync(PaginationParameters parameters)
    {
        var query = _dbSet.AsQueryable();

        // Apply search filter
        query = ApplySearchFilter(query, parameters.Search);

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

        var totalCount = await query.CountAsync();
        var items = await query
            .Include(i => i.InventoryItem)
            .Include(i => i.InventoryItemLocation)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<Inventory>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private static IQueryable<Inventory> ApplySearchFilter(IQueryable<Inventory> query, string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return query;

        string searchPattern = $"%{searchText.Trim().ToLowerInvariant()}%";

        return query.Where(i =>
            EF.Functions.Like(i.InventoryItem.ItemName ?? string.Empty, searchPattern) ||
            EF.Functions.Like(i.InventoryItemLocation != null ? i.InventoryItemLocation.LocationName ?? string.Empty : string.Empty, searchPattern) ||
            EF.Functions.Like(i.MinStockLevel.ToString(), searchPattern) ||
            EF.Functions.Like(i.MinStockLevel.ToString(), searchPattern) ||
            EF.Functions.Like(i.QuantityOnHand.ToString(), searchPattern)
        );
    }

    private static IQueryable<Inventory> ApplySorting(IQueryable<Inventory> query, string? sortBy, bool sortDescending)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return query.OrderByDescending(i => i.ID); // Default sort
        }

        var sortByLower = sortBy.ToLower();
        return sortByLower switch
        {
            "id" => sortDescending
                ? query.OrderByDescending(i => i.ID)
                : query.OrderBy(i => i.ID),
            "quantity" => sortDescending
                ? query.OrderByDescending(i => i.QuantityOnHand)
                : query.OrderBy(i => i.QuantityOnHand),
            "minstocklevel" => sortDescending
                ? query.OrderByDescending(i => i.MinStockLevel)
                : query.OrderBy(i => i.MinStockLevel),
            "maxstocklevel" => sortDescending
                ? query.OrderByDescending(i => i.MaxStockLevel)
                : query.OrderBy(i => i.MaxStockLevel),
            "location" => sortDescending
                ? query.OrderByDescending(i => i.InventoryItemLocation != null ? i.InventoryItemLocation.LocationName : string.Empty)
                : query.OrderBy(i => i.InventoryItemLocation != null ? i.InventoryItemLocation.LocationName : string.Empty),
            "createdat" => sortDescending
                ? query.OrderByDescending(i => i.CreatedAt)
                : query.OrderBy(i => i.CreatedAt),
            "updatedat" => sortDescending
                ? query.OrderByDescending(i => i.UpdatedAt)
                : query.OrderBy(i => i.UpdatedAt),
            _ => query.OrderByDescending(i => i.ID) // Default sort for unrecognized fields
        };
    }

    public async Task<Inventory?> GetInventoryByItemIDAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(i => i.InventoryItemID == id);
    }

    public async Task<Inventory?> GetInventoryWithDetailsAsync(int id)
    {
        return await _dbSet.Include(i => i.InventoryItem)
            .Include(i => i.InventoryItemLocation)
            .FirstOrDefaultAsync(i => i.ID == id);
    }
}