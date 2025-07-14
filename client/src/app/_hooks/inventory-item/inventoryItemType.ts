import {
  InventoryItemCategoryEnum,
  InventoryItemUnitCostMeasurementUnitEnum,
} from "./inventoryItemEnum";

export interface InventoryItem {
  id: number;
  ItemNumber: string;
  ItemName: string;
  Description?: string | null;
  Category?: InventoryItemCategoryEnum | null;
  Manufacturer?: string | null;
  ManufacturerPartNumber?: string | null;
  UniversalProductCode?: string | null;
  UnitCost?: number | null;
  UnitCostMeasurementUnit?: InventoryItemUnitCostMeasurementUnitEnum | null;
  Supplier?: string | null;
  WeightKG?: number | null;
  IsActive: boolean;
}

export interface InventoryItemWithLabels
  extends Omit<InventoryItem, "Category" | "UnitCostMeasurementUnit"> {
  Category: number;
  CategoryLabel: string;
  CategoryEnum: InventoryItemCategoryEnum;
  UnitCostMeasurementUnit: number;
  UnitCostMeasurementUnitLabel: string;
  UnitCostMeasurementUnitEnum: InventoryItemUnitCostMeasurementUnitEnum;
}

export interface CreateInventoryItemCommand {
  ItemNumber: string;
  ItemName: string;
  Description?: string | null;
  Category?: InventoryItemCategoryEnum | null;
  Manufacturer?: string | null;
  ManufacturerPartNumber?: string | null;
  UniversalProductCode?: string | null;
  UnitCost?: number | null;
  UnitCostMeasurementUnit?: InventoryItemUnitCostMeasurementUnitEnum | null;
  Supplier?: string | null;
  WeightKG?: number | null;
  IsActive: boolean;
}

export interface UpdateInventoryItemCommand {
  id: number;
  ItemNumber: string;
  ItemName: string;
  Description?: string | null;
  Category?: InventoryItemCategoryEnum | null;
  Manufacturer?: string | null;
  ManufacturerPartNumber?: string | null;
  UniversalProductCode?: string | null;
  UnitCost?: number | null;
  UnitCostMeasurementUnit?: InventoryItemUnitCostMeasurementUnitEnum | null;
  Supplier?: string | null;
  WeightKG?: number | null;
  IsActive: boolean;
}

export interface InventoryItemFilter {
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
  search?: string;
}
