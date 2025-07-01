using System;
using Domain.Entities.Enums;
using MediatR;

namespace Application.Features.InventoryItems.Command.CreateInventoryItem;

public record CreateInventoryItemCommand(
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
