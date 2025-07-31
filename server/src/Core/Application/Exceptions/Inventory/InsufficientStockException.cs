using System;

namespace Application.Exceptions.Inventory;

public class InsufficientStockException : Exception
{
    public InsufficientStockException(int inventoryItemId, int requestedQuantity)
        : base($"Not enough stock for inventory item ID {inventoryItemId}. Requested: {requestedQuantity}")
    {
    }
}