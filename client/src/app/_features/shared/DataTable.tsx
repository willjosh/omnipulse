import React from "react";
import Loading from "./Loading";

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
}) => {
  const allSelected =
    data.length > 0 && data.every(item => selectedItems.includes(item.id));
  const someSelected = data.some(item => selectedItems.includes(item.id));

  if (loading) {
    return <Loading />;
  }

  return (
    <div className="bg-white rounded-lg shadow overflow-hidden">
      <div className="overflow-x-auto">
        <table className="w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-4 py-3 text-left">
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
                  className={`px-4 py-3 text-left text-xs font-bold text-gray-500 uppercase tracking-wider ${
                    column.sortable && onSort
                      ? "cursor-pointer hover:bg-gray-100"
                      : ""
                  }`}
                  style={{ width: column.width }}
                  onClick={() =>
                    column.sortable && onSort && onSort(column.key)
                  }
                >
                  <div className="flex items-center gap-1">
                    {column.header}
                    {column.sortable && sortBy === column.key && (
                      <span className="text-blue-600">
                        {sortOrder === "asc" ? "↑" : "↓"}
                      </span>
                    )}
                  </div>
                </th>
              ))}
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {data.map(item => (
              <tr
                key={item.id}
                onClick={() => onRowClick?.(item)}
                className="hover:bg-gray-50 cursor-pointer"
              >
                <td className="px-4 py-3 whitespace-nowrap">
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
                    className="px-2 py-3 whitespace-nowrap text-sm text-gray-900"
                  >
                    {column.render
                      ? column.render(item)
                      : String(item[column.key] || "")}
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {data.length === 0 && emptyState && (
        <div className="px-6 py-12 text-center">{emptyState}</div>
      )}
    </div>
  );
};

export default DataTable;
