using System;
using Domain.Entities.Enums;
using MediatR;

namespace Application.Features.InventoryItems.Command.UpdateInventoryItem;

public record UpdateInventoryItemCommand(
    int InventoryItemID,
    string ItemNumber,
    string ItemName,
    string? Description,
    InventoryItemCategoryEnum? Category,
    string? Manufacturer,
    string? ManufacturerPartNumber,
    string? UniversalProductCode,
    decimal? UnitCost,
    InventoryItemUnitCostMeasurementUnitEnum? UnitCostMeasurementUnit,
    string? Supplier,
    double? WeightKG,
    bool IsActive
) : IRequest<int>;
