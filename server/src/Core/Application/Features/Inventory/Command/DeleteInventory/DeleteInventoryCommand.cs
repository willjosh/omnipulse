using System;

using MediatR;

namespace Application.Features.Inventory.Command.DeleteInventory;

public record DeleteInventoryCommand(
    int InventoryID
) : IRequest<int>
{ }