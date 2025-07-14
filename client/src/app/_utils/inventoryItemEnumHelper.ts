import {
  InventoryItemCategoryEnum,
  InventoryItemUnitCostMeasurementUnitEnum,
} from "@/app/_hooks/inventory-item/inventoryItemEnum";

export function getInventoryItemCategoryLabel(
  category?: InventoryItemCategoryEnum | 0 | null,
): string {
  switch (category) {
    case InventoryItemCategoryEnum.ENGINE:
      return "Engine";
    case InventoryItemCategoryEnum.TRANSMISSION:
      return "Transmission";
    case InventoryItemCategoryEnum.BRAKES:
      return "Brakes";
    case InventoryItemCategoryEnum.TIRES:
      return "Tires";
    case InventoryItemCategoryEnum.ELECTRICAL:
      return "Electrical";
    case InventoryItemCategoryEnum.BODY:
      return "Body";
    case InventoryItemCategoryEnum.INTERIOR:
      return "Interior";
    case InventoryItemCategoryEnum.FLUIDS:
      return "Fluids";
    case InventoryItemCategoryEnum.FILTERS:
      return "Filters";
    default:
      return "Unknown";
  }
}

export function getInventoryItemUnitCostMeasurementUnitLabel(
  unit?: InventoryItemUnitCostMeasurementUnitEnum | 0 | null,
): string {
  switch (unit) {
    case InventoryItemUnitCostMeasurementUnitEnum.Unit:
      return "Unit";
    case InventoryItemUnitCostMeasurementUnitEnum.Litre:
      return "Litre";
    case InventoryItemUnitCostMeasurementUnitEnum.Gram:
      return "Gram";
    case InventoryItemUnitCostMeasurementUnitEnum.Kilogram:
      return "Kilogram";
    case InventoryItemUnitCostMeasurementUnitEnum.Metre:
      return "Metre";
    case InventoryItemUnitCostMeasurementUnitEnum.SquareMetre:
      return "Square Metre";
    case InventoryItemUnitCostMeasurementUnitEnum.CubicMetre:
      return "Cubic Metre";
    case InventoryItemUnitCostMeasurementUnitEnum.Box:
      return "Box";
    default:
      return "Unknown";
  }
}
