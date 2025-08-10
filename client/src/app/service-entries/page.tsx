"use client";
import React, { useState, useMemo, useEffect } from "react";
import { useRouter } from "next/navigation";
import { DataTable, PaginationControls } from "@/components/ui/Table";
import { PrimaryButton } from "@/components/ui/Button";
import { FilterBar } from "@/components/ui/Filter";
import {
  Plus,
  Calendar,
  Car,
  Wrench,
  DollarSign,
  Clock,
  FileText,
} from "lucide-react";
import { useMaintenanceHistories } from "@/features/maintenance-history/hooks/useMaintenanceHistory";
import { MaintenanceHistoryWithFormattedDates } from "@/features/maintenance-history/types/maintenanceHistoryType";
import { DEFAULT_PAGE_SIZE } from "@/components/ui/Table/constants";

export default function ServiceEntriesPage() {
  const router = useRouter();

  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);

  useEffect(() => {
    setPage(1);
  }, [search]);

  const filter = useMemo(
    () => ({ PageNumber: page, PageSize: pageSize, Search: search }),
    [page, pageSize, search],
  );

  const { maintenanceHistories, pagination, isPending } =
    useMaintenanceHistories(filter);

  const columns = [
    {
      key: "vehicleName",
      header: "Vehicle",
      sortable: true,
      width: "180px",
      render: (item: MaintenanceHistoryWithFormattedDates) => (
        <div className="flex items-center">
          <Car size={16} className="mr-2 text-gray-500" />
          {item.vehicleName}
        </div>
      ),
    },
    {
      key: "serviceDate",
      header: "Service Date",
      sortable: true,
      width: "140px",
      render: (item: MaintenanceHistoryWithFormattedDates) => (
        <div className="flex items-center">
          <Calendar size={16} className="mr-2 text-gray-500" />
          {item.serviceDateFormatted ||
            new Date(item.serviceDate).toLocaleDateString()}
        </div>
      ),
    },
    {
      key: "serviceTasks",
      header: "Service Tasks",
      width: "200px",
      render: (item: MaintenanceHistoryWithFormattedDates) => (
        <div className="flex items-center">
          <Wrench size={16} className="mr-2 text-gray-500" />
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
        </div>
      ),
    },
    {
      key: "technicianName",
      header: "Technician",
      width: "150px",
      render: (item: MaintenanceHistoryWithFormattedDates) => (
        <div className="flex items-center">
          <Wrench size={16} className="mr-2 text-gray-500" />
          {item.technicianName}
        </div>
      ),
    },
    {
      key: "mileageAtService",
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
        <div className="flex items-center justify-end">
          <DollarSign size={16} className="mr-1 text-gray-500" />
          {`$${item.cost.toFixed(2)}`}
        </div>
      ),
    },
    {
      key: "labourHours",
      header: "Labour Hours",
      sortable: true,
      width: "130px",
      render: (item: MaintenanceHistoryWithFormattedDates) => (
        <div className="flex items-center justify-end">
          <Clock size={16} className="mr-2 text-gray-500" />
          {item.labourHours.toFixed(1)}h
        </div>
      ),
    },
    {
      key: "notes",
      header: "Notes",
      width: "200px",
      render: (item: MaintenanceHistoryWithFormattedDates) => (
        <div className="flex items-center">
          <FileText size={16} className="mr-2 text-gray-500" />
          <div className="truncate max-w-[160px]" title={item.notes || ""}>
            {item.notes || "-"}
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
        <PrimaryButton onClick={() => router.push("/service-entries/new")}>
          <Plus size={18} className="mr-2" />
          Add Service Entry
        </PrimaryButton>
      </div>

      <div className="flex items-end justify-between mb-4">
        <FilterBar
          searchValue={search}
          onSearchChange={setSearch}
          searchPlaceholder="Search service entries..."
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
        selectedItems={[]}
        onSelectItem={() => {}}
        onSelectAll={() => {}}
        onRowClick={handleRowClick}
        loading={isPending}
        emptyState={emptyState}
        getItemId={item => item.maintenanceHistoryID.toString()}
        showActions={false}
        fixedLayout={false}
      />
    </div>
  );
}
