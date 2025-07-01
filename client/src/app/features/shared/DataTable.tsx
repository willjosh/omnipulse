import React, { useEffect, useRef } from "react";
import Loading from "./Loading";
import { ActionsColumnHeader, ActionsColumnCell } from "./ActionsColumn";
import { ActionItem } from "./ActionDropdown";

interface Column {
  key: string;
  header: string;
  render?: (item: any) => React.ReactNode;
  sortable?: boolean;
  width?: string;
}

interface DataTableProps {
  data: any[];
  columns: Column[];
  selectedItems: string[];
  onSelectItem: (id: string) => void;
  onSelectAll: () => void;
  onRowClick?: (item: any) => void;
  onSort?: (key: string) => void;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
  loading?: boolean;
  emptyState?: React.ReactNode;
  fixedLayout?: boolean;
  actions?: ActionItem[];
  showActions?: boolean;
}

const DataTable: React.FC<DataTableProps> = ({
  data,
  columns,
  selectedItems,
  onSelectItem,
  onSelectAll,
  onRowClick,
  onSort,
  sortBy,
  sortOrder,
  loading = false,
  emptyState,
  fixedLayout = true,
  actions = [],
  showActions = true,
}) => {
  const scrollContainerRef = useRef<HTMLDivElement>(null);

  const allSelected =
    data.length > 0 && data.every(item => selectedItems.includes(item.id));
  const someSelected = data.some(item => selectedItems.includes(item.id));

  useEffect(() => {
    if (data.length === 0 && scrollContainerRef.current) {
      scrollContainerRef.current.scrollLeft = 0;
    }
  }, [data.length]);

  const handleActionClick = (
    action: ActionItem,
    item: any,
    event: React.MouseEvent,
  ) => {
    event.stopPropagation();
    action.onClick(item);
  };

  const totalColumnWidth = columns.reduce((total, column) => {
    if (column.width) {
      return total + parseInt(column.width);
    }
    return total + 120;
  }, 0);

  const checkboxWidth = 50;
  const actionsWidth = showActions ? 48 : 0;
  const tableWidth = totalColumnWidth + checkboxWidth + actionsWidth;

  if (loading) {
    return <Loading />;
  }

  return (
    <div className="bg-white rounded-lg shadow overflow-hidden">
      <div
        ref={scrollContainerRef}
        className={`h-[500px] relative ${data.length === 0 ? "overflow-hidden" : "overflow-x-auto"}`}
      >
        <table
          className="divide-y divide-gray-200"
          style={{
            tableLayout: fixedLayout ? "fixed" : "auto",
            width: fixedLayout ? `${tableWidth}px` : "100%",
            minWidth: `${tableWidth}px`,
          }}
        >
          <thead className="bg-gray-50 sticky top-0 z-10">
            <tr>
              <th
                className="px-4 py-3 text-left bg-gray-50"
                style={{ width: `${checkboxWidth}px` }}
              >
                <input
                  type="checkbox"
                  className="size-4 text-blue-600 rounded border-gray-300"
                  checked={allSelected}
                  ref={el => {
                    if (el) el.indeterminate = someSelected && !allSelected;
                  }}
                  onChange={onSelectAll}
                />
              </th>
              {columns.map(column => (
                <th
                  key={column.key}
                  className={`px-4 py-3 text-left text-xs font-bold text-gray-500 uppercase tracking-wider bg-gray-50 ${
                    column.sortable && onSort
                      ? "cursor-pointer hover:bg-gray-100"
                      : ""
                  }`}
                  style={{ width: column.width || "120px" }}
                  onClick={() =>
                    column.sortable && onSort && onSort(column.key)
                  }
                >
                  <div className="flex items-center gap-1">
                    <span className="truncate">{column.header}</span>
                    {column.sortable && sortBy === column.key && (
                      <span className="text-blue-600 flex-shrink-0">
                        {sortOrder === "asc" ? "↑" : "↓"}
                      </span>
                    )}
                  </div>
                </th>
              ))}
              {showActions && <ActionsColumnHeader />}
            </tr>
          </thead>
          {data.length > 0 && (
            <tbody className="bg-white divide-y divide-gray-200">
              {data.map(item => (
                <tr
                  key={item.id}
                  onClick={() => onRowClick?.(item)}
                  className="hover:bg-gray-50 cursor-pointer"
                >
                  <td
                    className="px-4 py-3 whitespace-nowrap"
                    style={{ width: `${checkboxWidth}px` }}
                  >
                    <input
                      type="checkbox"
                      className="size-4 text-blue-600 rounded border-gray-300"
                      checked={selectedItems.includes(item.id)}
                      onChange={() => onSelectItem(item.id)}
                      onClick={e => e.stopPropagation()}
                    />
                  </td>
                  {columns.map(column => (
                    <td
                      key={column.key}
                      className="px-4 py-3 text-sm text-gray-900"
                      style={{ width: column.width || "120px" }}
                    >
                      <div className="truncate">
                        {column.render
                          ? column.render(item)
                          : String(item[column.key] || "")}
                      </div>
                    </td>
                  ))}
                  {showActions && (
                    <ActionsColumnCell
                      item={item}
                      actions={actions}
                      onActionClick={handleActionClick}
                    />
                  )}
                </tr>
              ))}
            </tbody>
          )}
        </table>

        {data.length === 0 && (
          <div className="absolute inset-x-0 top-16 bottom-0 flex items-center justify-center">
            <div className="text-center">
              <div className="text-lg text-gray-500 mb-4">{emptyState}</div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default DataTable;
