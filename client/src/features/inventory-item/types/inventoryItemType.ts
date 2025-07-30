import {
  InventoryItemCategoryEnum,
  InventoryItemUnitCostMeasurementUnitEnum,
} from "./inventoryItemEnum";

export interface InventoryItem {
  id: number;
  itemNumber: string;
  itemName: string;
  description?: string | null;
  category?: InventoryItemCategoryEnum | null;
  manufacturer?: string | null;
  manufacturerPartNumber?: string | null;
  universalProductCode?: string | null;
  unitCost?: number | null;
  unitCostMeasurementUnit?: InventoryItemUnitCostMeasurementUnitEnum | null;
  supplier?: string | null;
  weightKG?: number | null;
  isActive: boolean;
}

export interface InventoryItemWithLabels
  extends Omit<InventoryItem, "category" | "unitCostMeasurementUnit"> {
  category: number;
  categoryLabel: string;
  categoryEnum: InventoryItemCategoryEnum;
  unitCostMeasurementUnit: number;
  unitCostMeasurementUnitLabel: string;
  unitCostMeasurementUnitEnum: InventoryItemUnitCostMeasurementUnitEnum;
}

export interface CreateInventoryItemCommand {
  itemNumber: string;
  itemName: string;
  description?: string | null;
  category?: InventoryItemCategoryEnum | null;
  manufacturer?: string | null;
  manufacturerPartNumber?: string | null;
  universalProductCode?: string | null;
  unitCost?: number | null;
  unitCostMeasurementUnit?: InventoryItemUnitCostMeasurementUnitEnum | null;
  supplier?: string | null;
  weightKG?: number | null;
  isActive: boolean;
}

export interface UpdateInventoryItemCommand {
  inventoryItemID: number;
  itemNumber: string;
  itemName: string;
  description?: string | null;
  category?: InventoryItemCategoryEnum | null;
  manufacturer?: string | null;
  manufacturerPartNumber?: string | null;
  universalProductCode?: string | null;
  unitCost?: number | null;
  unitCostMeasurementUnit?: InventoryItemUnitCostMeasurementUnitEnum | null;
  supplier?: string | null;
  weightKG?: number | null;
  isActive: boolean;
}

export interface InventoryItemFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
}
