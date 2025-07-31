using System;

using MediatR;

namespace Application.Features.Inventory.Command.CreateInventory;

public record CreateInventoryCommand(
    int InventoryItemID,
    int InventoryItemLocationID,
    int QuantityOnHand,
    int MinStockLevel,
    int MaxStockLevel,
    int ReorderPoint,
    decimal UnitCost
) : IRequest<int>
{ }