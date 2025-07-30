using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IInventoryItemRepository : IGenericRepository<InventoryItem>
{
    // Uniqueness checks
    Task<bool> IsItemNumberUniqueAsync(string itemNumber);
    Task<bool> IsUniversalProductCodeUniqueAsync(string? universalProductCode);
    Task<bool> IsManufacturerPartNumberUniqueAsync(string? manufacturer, string? manufacturerPartNumber); // MPNs are unique within a manufacturer

    // Paged Results
    Task<PagedResult<InventoryItem>> GetAllInventoryItemsPagedAsync(PaginationParameters parameters);
}