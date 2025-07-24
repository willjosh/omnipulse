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
    key: "itemName",
    header: "Part",
    sortable: true,
    render: (item: InventoryItemWithLabels) => (
      <div className="text-sm font-medium text-gray-900">{item.itemName}</div>
    ),
  },
  {
    key: "description",
    header: "Description",
    sortable: false,
    render: (item: InventoryItemWithLabels) => (
      <div className="text-sm text-gray-500 max-w-xs truncate">
        {item.description || "No description"}
      </div>
    ),
  },
  {
    key: "categoryLabel",
    header: "Category",
    sortable: true,
    render: (item: InventoryItemWithLabels) => (
      <div className="text-sm text-gray-700">{item.categoryLabel}</div>
    ),
  },
  {
    key: "manufacturer",
    header: "Manufacturer",
    sortable: true,
    render: (item: InventoryItemWithLabels) => (
      <div className="text-sm text-gray-700">{item.manufacturer || "—"}</div>
    ),
  },
  {
    key: "manufacturerPartNumber",
    header: "Manufacturer Part Number",
    sortable: false,
    render: (item: InventoryItemWithLabels) => (
      <div className="text-sm text-gray-700">
        {item.manufacturerPartNumber || "—"}
      </div>
    ),
  },
  {
    key: "unitCost",
    header: "Unit Cost",
    sortable: true,
    render: (item: InventoryItemWithLabels) => (
      <div className="text-sm text-gray-900">
        {item.unitCost
          ? `$${item.unitCost.toFixed(2)}${item.unitCostMeasurementUnitLabel ? ` / ${item.unitCostMeasurementUnitLabel}` : ""}`
          : "—"}
      </div>
    ),
  },
];
