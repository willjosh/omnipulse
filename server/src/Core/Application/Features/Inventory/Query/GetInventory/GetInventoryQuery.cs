using System;

using MediatR;

namespace Application.Features.Inventory.Query.GetInventory;

public record GetInventoryQuery(int InventoryID) : IRequest<InventoryDetailDTO> { }