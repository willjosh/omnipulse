export interface Inventory {
  id: number;
  inventoryItemName: string;
  locationName: string;
  quantityOnHand: number;
  minStockLevel: number;
  maxStockLevel: number;
  needsReorder: boolean;
  lastRestockedDate?: string | null;
  unitCost: number;
}

export interface InventoryFilter {
  PageNumber?: number;
  PageSize?: number;
  Search?: string;
  SortBy?: string;
  SortDescending?: boolean;
}

export interface UpdateInventoryCommand {
  inventoryID: number;
  quantityOnHand: number;
  unitCost: number;
  minStockLevel: number;
  maxStockLevel: number;
  isAdjustment: boolean;
  performedByUserID: string;
}
