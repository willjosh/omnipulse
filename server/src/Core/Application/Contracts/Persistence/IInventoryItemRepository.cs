using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Enums;

namespace Application.Contracts.Persistence;

public interface IInventoryItemRepository : IGenericRepository<InventoryItem>
{
    // Uniqueness checks
    Task<bool> IsItemNumberUniqueAsync(string itemNumber);
    Task<bool> IsUniversalProductCodeUniqueAsync(string? universalProductCode);
    Task<bool> IsManufacturerPartNumberUniqueAsync(string? manufacturer, string? manufacturerPartNumber); // MPNs are unique within a manufacturer
}
