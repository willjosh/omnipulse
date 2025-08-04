using System;

using MediatR;

namespace Application.Features.Inventory.Command.UpdateInventory;

public record UpdateInventoryCommand(
    int InventoryID,
    int QuantityOnHand,
    decimal UnitCost,
    int MinStockLevel,
    int MaxStockLevel,
    bool IsAdjustment,
    string PerformedByUserID
) : IRequest<int>
{ }