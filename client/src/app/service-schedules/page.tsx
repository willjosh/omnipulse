"use client";
import React, { useState, useMemo } from "react";
import { useRouter } from "next/navigation";
import { DataTable, PaginationControls } from "@/components/ui/Table";
import { PrimaryButton } from "@/components/ui/Button";
import { useServiceSchedules } from "@/features/service-schedule/hooks/useServiceSchedules";
import { ServiceScheduleWithLabels } from "@/features/service-schedule/types/serviceScheduleType";
import { FilterBar } from "@/components/ui/Filter";
import { Plus } from "lucide-react";
import { DEFAULT_PAGE_SIZE } from "@/components/ui/Table/constants";

export default function ServiceScheduleListPage() {
  const router = useRouter();

  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);

  React.useEffect(() => {
    setPage(1);
  }, [search]);

  const filter = useMemo(
    () => ({ PageNumber: page, PageSize: pageSize, Search: search }),
    [page, pageSize, search],
  );

  const { serviceSchedules, pagination, isPending } =
    useServiceSchedules(filter);

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

  const handleRowClick = (row: ServiceScheduleWithLabels) => {
    router.push(`/service-schedules/${row.id}`);
  };

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
    <div className="p-6 w-full max-w-none">
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
      />
    </div>
  );
}
