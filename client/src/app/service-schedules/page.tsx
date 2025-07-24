"use client";
import React, { useState, useMemo } from "react";
import { useRouter } from "next/navigation";
import { DataTable, PaginationControls } from "@/app/_features/shared/table";
import { PrimaryButton } from "@/app/_features/shared/button";
import { useServiceSchedules } from "@/app/_hooks/service-schedule/useServiceSchedules";
import { ServiceScheduleWithLabels } from "@/app/_hooks/service-schedule/serviceScheduleType";
import { FilterBar } from "@/app/_features/shared/filter";
import { Plus } from "lucide-react";
import { DEFAULT_PAGE_SIZE } from "@/app/_features/shared/table/constants";

export default function ServiceScheduleListPage() {
  const router = useRouter();

  // UI state: search, pagination, etc.
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);

  // Reset page to 1 when search changes
  React.useEffect(() => {
    setPage(1);
  }, [search]);

  // Compose filter object for data fetching
  const filter = useMemo(
    () => ({ PageNumber: page, PageSize: pageSize, Search: search }),
    [page, pageSize, search],
  );

  // Fetch data using the filter
  const { serviceSchedules, pagination, isPending } =
    useServiceSchedules(filter);

  // Table columns
  const columns = [
    { key: "name", header: "Name", sortable: true, width: "200px" },
    {
      key: "serviceTasks",
      header: "Tasks",
      render: (item: ServiceScheduleWithLabels) => (
        <div>
          {item.serviceTasks.map(task => (
            <div key={task.id}>{task.name}</div>
          ))}
        </div>
      ),
      width: "220px",
    },
    {
      key: "timeIntervalValue",
      header: "Frequency",
      render: (item: ServiceScheduleWithLabels) =>
        item.timeIntervalValue && item.timeIntervalUnitLabel
          ? `${item.timeIntervalValue} ${item.timeIntervalUnitLabel}`
          : item.mileageInterval
            ? `${item.mileageInterval} km`
            : "-",
      width: "140px",
    },
    {
      key: "isActive",
      header: "Active",
      render: (item: ServiceScheduleWithLabels) =>
        item.isActive ? "Yes" : "No",
      width: "80px",
    },
  ];

  // Row click handler
  const handleRowClick = (row: ServiceScheduleWithLabels) => {
    router.push(`/service-schedules/${row.id}`);
  };

  // Empty state
  const emptyState = (
    <div className="text-center py-8">
      <p className="text-gray-500 mb-2">No service schedules found.</p>
      <button
        onClick={() => setSearch("")}
        className="text-blue-600 hover:text-blue-800 text-sm"
      >
        Clear search
      </button>
    </div>
  );

  return (
    <div className="p-6 w-[1260px] min-h-screen mx-auto">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-semibold text-gray-900">
          Service Schedules
        </h1>
        <PrimaryButton onClick={() => router.push("/service-schedules/new")}>
          <Plus size={18} className="mr-2" />
          Add Service Schedule
        </PrimaryButton>
      </div>
      <div className="flex items-end justify-between mb-4">
        <FilterBar
          searchValue={search}
          onSearchChange={setSearch}
          searchPlaceholder="Search service schedules..."
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
        data={serviceSchedules}
        columns={columns}
        selectedItems={[]}
        onSelectItem={() => {}}
        onSelectAll={() => {}}
        onRowClick={handleRowClick}
        loading={isPending}
        emptyState={emptyState}
        getItemId={item => item.id.toString()}
        showActions={false}
        fixedLayout={false}
        // Make table take full width
        // style={{ width: '100%' }}
      />
    </div>
  );
}
