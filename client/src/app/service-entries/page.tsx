"use client";
import React, { useState, useMemo, useEffect } from "react";
import { useRouter } from "next/navigation";
import { DataTable, PaginationControls } from "@/components/ui/Table";
import { FilterBar } from "@/components/ui/Filter";
import { useMaintenanceHistories } from "@/features/maintenance-history/hooks/useMaintenanceHistory";
import { MaintenanceHistoryWithFormattedDates } from "@/features/maintenance-history/types/maintenanceHistoryType";
import { DEFAULT_PAGE_SIZE } from "@/components/ui/Table/constants";
import { formatEmptyValueWithUnknown } from "@/utils/emptyValueUtils";

export default function ServiceEntriesPage() {
  const router = useRouter();

  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [sortBy, setSortBy] = useState("servicedate");
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("desc");

  useEffect(() => {
    setPage(1);
  }, [search, sortBy, sortOrder]);

  const filter = useMemo(
    () => ({
      PageNumber: page,
      PageSize: pageSize,
      Search: search,
      SortBy: sortBy,
      SortDescending: sortOrder === "desc",
    }),
    [page, pageSize, search, sortBy, sortOrder],
  );

  const { maintenanceHistories, pagination, isPending } =
    useMaintenanceHistories(filter);

  const handleSort = (sortKey: string) => {
    if (sortBy === sortKey) {
      setSortOrder(sortOrder === "asc" ? "desc" : "asc");
    } else {
      setSortBy(sortKey);
      setSortOrder("asc");
    }
    setPage(1);
  };

  const columns = [
    {
      key: "vehicleid",
      header: "Vehicle",
      sortable: true,
      width: "180px",
      render: (item: MaintenanceHistoryWithFormattedDates) => (
        <div>{item.vehicleName}</div>
      ),
    },
    {
      key: "servicedate",
      header: "Service Date",
      sortable: true,
      width: "140px",
      render: (item: MaintenanceHistoryWithFormattedDates) => (
        <div>
          {item.serviceDateFormatted ||
            new Date(item.serviceDate).toLocaleDateString()}
        </div>
      ),
    },
    {
      key: "servicetaskid",
      header: "Service Tasks",
      sortable: true,
      width: "200px",
      render: (item: MaintenanceHistoryWithFormattedDates) => (
        <div>
          {item.serviceTaskName.length > 0 ? (
            item.serviceTaskName.slice(0, 2).map((task, index) => (
              <div key={index} className="text-sm">
                {task}
              </div>
            ))
          ) : (
            <span className="text-gray-400">-</span>
          )}
          {item.serviceTaskName.length > 2 && (
            <div className="text-xs text-gray-500">
              +{item.serviceTaskName.length - 2} more
            </div>
          )}
        </div>
      ),
    },
    {
      key: "technicianid",
      header: "Technician",
      sortable: true,
      width: "150px",
      render: (item: MaintenanceHistoryWithFormattedDates) => (
        <div>{item.technicianName}</div>
      ),
    },
    {
      key: "mileageatservice",
      header: "Mileage",
      sortable: true,
      width: "120px",
      render: (item: MaintenanceHistoryWithFormattedDates) => (
        <div className="text-right">
          {item.mileageAtService.toLocaleString()} km
        </div>
      ),
    },
    {
      key: "cost",
      header: "Cost",
      sortable: true,
      width: "120px",
      render: (item: MaintenanceHistoryWithFormattedDates) => (
        <div className="text-right">{`$${item.cost.toFixed(2)}`}</div>
      ),
    },
    {
      key: "labourhours",
      header: "Labour Hours",
      sortable: true,
      width: "130px",
      render: (item: MaintenanceHistoryWithFormattedDates) => (
        <div className="text-right">{item.labourHours.toFixed(1)}h</div>
      ),
    },
    {
      key: "notes",
      header: "Notes",
      sortable: false,
      width: "200px",
      render: (item: MaintenanceHistoryWithFormattedDates) => (
        <div>
          <div className="truncate max-w-[160px]" title={item.notes || ""}>
            {formatEmptyValueWithUnknown(item.notes)}
          </div>
        </div>
      ),
    },
  ];

  const handleRowClick = (row: MaintenanceHistoryWithFormattedDates) => {
    router.push(`/service-entries/${row.maintenanceHistoryID}`);
  };

  const emptyState = (
    <div className="text-center py-8">
      <p className="text-gray-500 mb-2">
        No maintenance history entries found.
      </p>
      <button
        onClick={() => setSearch("")}
        className="text-blue-600 hover:text-blue-800 text-sm"
      >
        Clear search
      </button>
    </div>
  );

  return (
    <div className="p-6 w-full max-w-none">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-semibold text-gray-900">
          Service Entries
        </h1>
      </div>

      <div className="flex items-end justify-between mb-4">
        <FilterBar
          searchValue={search}
          onSearchChange={setSearch}
          searchPlaceholder="Search"
          onFilterChange={() => {}}
        />
        {pagination && (
          <PaginationControls
            currentPage={pagination.pageNumber}
            totalPages={pagination.totalPages}
            totalItems={pagination.totalCount}
            itemsPerPage={pagination.pageSize}
            onPreviousPage={() => setPage(p => Math.max(1, p - 1))}
            onNextPage={() =>
              setPage(p =>
                pagination && p < pagination.totalPages ? p + 1 : p,
              )
            }
            onPageChange={setPage}
            onPageSizeChange={setPageSize}
          />
        )}
      </div>

      <DataTable
        data={maintenanceHistories}
        columns={columns}
        onRowClick={handleRowClick}
        loading={isPending}
        emptyState={emptyState}
        getItemId={item => item.maintenanceHistoryID.toString()}
        showActions={false}
        fixedLayout={false}
        onSort={handleSort}
        sortBy={sortBy}
        sortOrder={sortOrder}
      />
    </div>
  );
}
