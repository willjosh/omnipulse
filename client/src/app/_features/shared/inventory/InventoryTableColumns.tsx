import { InventoryItemWithLabels } from "@/app/_hooks/inventory-item/inventoryItemType";

interface Column<T> {
  key: keyof T | string;
  header: string;
  render?: (item: T) => React.ReactNode;
  sortable?: boolean;
  width?: string;
}

export const inventoryTableColumns: Column<InventoryItemWithLabels>[] = [
  {
    key: "ItemName",
    header: "Part",
    sortable: true,
    render: (item: InventoryItemWithLabels) => (
      <div className="text-sm font-medium text-gray-900">{item.ItemName}</div>
    ),
  },
  {
    key: "Description",
    header: "Description",
    sortable: false,
    render: (item: InventoryItemWithLabels) => (
      <div className="text-sm text-gray-500 max-w-xs truncate">
        {item.Description || "No description"}
      </div>
    ),
  },
  {
    key: "CategoryLabel",
    header: "Category",
    sortable: true,
    render: (item: InventoryItemWithLabels) => (
      <div className="text-sm text-gray-700">{item.CategoryLabel}</div>
    ),
  },
  {
    key: "Manufacturer",
    header: "Manufacturer",
    sortable: true,
    render: (item: InventoryItemWithLabels) => (
      <div className="text-sm text-gray-700">{item.Manufacturer || "—"}</div>
    ),
  },
  {
    key: "ManufacturerPartNumber",
    header: "Manufacturer Part Number",
    sortable: false,
    render: (item: InventoryItemWithLabels) => (
      <div className="text-sm text-gray-700">
        {item.ManufacturerPartNumber || "—"}
      </div>
    ),
  },
  {
    key: "UnitCost",
    header: "Unit Cost",
    sortable: true,
    render: (item: InventoryItemWithLabels) => (
      <div className="text-sm text-gray-900">
        {item.UnitCost
          ? `$${item.UnitCost.toFixed(2)} ${item.UnitCostMeasurementUnitLabel ? `/ ${item.UnitCostMeasurementUnitLabel}` : ""}`
          : "—"}
      </div>
    ),
  },
];
