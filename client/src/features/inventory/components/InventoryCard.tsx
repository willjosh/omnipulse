import { Inventory } from "../types/inventoryType";
import { getStockStatus, getStockStatusColor } from "../config/inventoryConfig";
import { formatEmptyValueWithUnknown } from "@/utils/emptyValueUtils";

interface InventoryCardProps {
  inventory: Inventory;
  onClick?: () => void;
}

export function InventoryCard({ inventory, onClick }: InventoryCardProps) {
  const stockStatus = getStockStatus(inventory);
  const statusColor = getStockStatusColor(stockStatus);

  return (
    <div
      className="bg-white rounded-lg shadow-md p-4 border border-gray-200 hover:shadow-lg transition-shadow cursor-pointer"
      onClick={onClick}
    >
      <div className="flex justify-between items-start mb-2">
        <h3 className="text-lg font-semibold text-gray-900">
          {inventory.inventoryItemName}
        </h3>
        {inventory.needsReorder && (
          <span className="bg-red-100 text-red-800 text-xs font-medium px-2.5 py-0.5 rounded">
            Reorder
          </span>
        )}
      </div>

      <p className="text-sm text-gray-600 mb-3">{inventory.locationName}</p>

      <div className="grid grid-cols-2 gap-4 text-sm">
        <div>
          <span className="text-gray-500">Quantity:</span>
          <span className={`ml-1 font-medium ${statusColor}`}>
            {inventory.quantityOnHand}
          </span>
        </div>
        <div>
          <span className="text-gray-500">Unit Cost:</span>
          <span className="ml-1 font-medium">
            ${inventory.unitCost.toFixed(2)}
          </span>
        </div>
        <div>
          <span className="text-gray-500">Min Level:</span>
          <span className="ml-1">{inventory.minStockLevel}</span>
        </div>
        <div>
          <span className="text-gray-500">Max Level:</span>
          <span className="ml-1">{inventory.maxStockLevel}</span>
        </div>
      </div>

      {inventory.lastRestockedDate && (
        <div className="mt-3 pt-3 border-t border-gray-100">
          <span className="text-xs text-gray-500">
            Last restocked:{" "}
            {formatEmptyValueWithUnknown(inventory.lastRestockedDate)}
          </span>
        </div>
      )}
    </div>
  );
}
