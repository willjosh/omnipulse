import { Inventory } from "../types/inventoryType";

export function formatDate(date?: string | null): string {
  if (!date) return "Unknown";
  const d = new Date(date);
  return isNaN(d.getTime()) ? "Unknown" : d.toLocaleString();
}

export const convertInventoryData = (inventory: Inventory): Inventory => ({
  ...inventory,
  lastRestockedDate: formatDate(inventory.lastRestockedDate),
});
