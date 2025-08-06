import { Inventory } from "../types/inventoryType";

interface Column<T> {
  key: keyof T | string;
  header: string;
  render?: (item: T) => React.ReactNode;
  sortable?: boolean;
  width?: string;
}

export const inventoryTableColumns: Column<Inventory>[] = [
  {
    key: "id",
    header: "Item",
    sortable: true,
    width: "200px",
    render: (item: Inventory) => (
      <div className="text-sm font-medium text-gray-900">
        {item.inventoryItemName}
      </div>
    ),
  },
  {
    key: "location",
    header: "Location",
    sortable: true,
    width: "150px",
    render: (item: Inventory) => (
      <div className="text-sm text-gray-700">{item.locationName}</div>
    ),
  },
  {
    key: "quantity",
    header: "Quantity",
    sortable: true,
    width: "120px",
    render: (item: Inventory) => {
      let colorClass = "text-gray-900 ";
      if (item.quantityOnHand <= item.minStockLevel) {
        colorClass = "text-red-600";
      } else if (item.quantityOnHand >= item.maxStockLevel) {
        colorClass = "text-blue-600";
      } else {
        colorClass = "text-green-600";
      }

      return (
        <div className={`text-sm ${colorClass}`}>{item.quantityOnHand}</div>
      );
    },
  },
  {
    key: "minstocklevel",
    header: "Min Level",
    sortable: true,
    width: "100px",
    render: (item: Inventory) => (
      <div className="text-sm text-gray-700">{item.minStockLevel}</div>
    ),
  },
  {
    key: "maxstocklevel",
    header: "Max Level",
    sortable: true,
    width: "100px",
    render: (item: Inventory) => (
      <div className="text-sm text-gray-700">{item.maxStockLevel}</div>
    ),
  },
  {
    key: "needsReorder",
    header: "Status",
    sortable: false,
    width: "120px",
    render: (item: Inventory) => (
      <div>
        {item.needsReorder ? (
          <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-100 text-red-800">
            Reorder
          </span>
        ) : (
          <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
            Stocked
          </span>
        )}
      </div>
    ),
  },
  {
    key: "unitCost",
    header: "Unit Cost",
    sortable: false,
    width: "120px",
    render: (item: Inventory) => (
      <div className="text-sm text-gray-900">${item.unitCost.toFixed(2)}</div>
    ),
  },
  {
    key: "lastRestockedDate",
    header: "Last Restocked",
    sortable: false,
    width: "150px",
    render: (item: Inventory) => (
      <div className="text-sm text-gray-700">
        {item.lastRestockedDate && item.lastRestockedDate !== "Unknown"
          ? item.lastRestockedDate
          : "—"}
      </div>
    ),
  },
];
