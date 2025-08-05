import { Inventory } from "../types/inventoryType";

export const INVENTORY_CONFIG = {
  DEFAULT_PAGE_SIZE: 10,
  MAX_PAGE_SIZE: 100,
  SEARCH_DEBOUNCE_MS: 300,
};

export const INVENTORY_SORT_OPTIONS = [
  { value: "inventoryItemName", label: "Item Name" },
  { value: "locationName", label: "Location" },
  { value: "quantityOnHand", label: "Quantity" },
  { value: "unitCost", label: "Unit Cost" },
  { value: "needsReorder", label: "Reorder Status" },
];

export const getInventoryDisplayName = (inventory: Inventory): string => {
  return `${inventory.inventoryItemName} - ${inventory.locationName}`;
};

export const getStockStatus = (
  inventory: Inventory,
): "low" | "normal" | "high" => {
  if (inventory.quantityOnHand <= inventory.minStockLevel) return "low";
  if (inventory.quantityOnHand >= inventory.maxStockLevel) return "high";
  return "normal";
};

export const getStockStatusColor = (
  status: "low" | "normal" | "high",
): string => {
  switch (status) {
    case "low":
      return "text-red-600";
    case "high":
      return "text-blue-600";
    case "normal":
      return "text-green-600";
    default:
      return "text-gray-600";
  }
};
