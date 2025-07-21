using Application.Contracts.Persistence;

using Domain.Entities;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class InventoryItemRepository : GenericRepository<InventoryItem>, IInventoryItemRepository
{
    public InventoryItemRepository(OmnipulseDatabaseContext context) : base(context) { }

    public Task<bool> IsItemNumberUniqueAsync(string itemNumber)
        => throw new NotImplementedException();

    public Task<bool> IsUniversalProductCodeUniqueAsync(string? universalProductCode)
        => throw new NotImplementedException();

    public Task<bool> IsManufacturerPartNumberUniqueAsync(string? manufacturer, string? manufacturerPartNumber)
        => throw new NotImplementedException();
}