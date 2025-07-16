"use client";
import React, { useState, useMemo } from "react";
import { useRouter } from "next/navigation";
import { DataTable, PaginationControls } from "@/app/_features/shared/table";
import { PrimaryButton } from "@/app/_features/shared/button";
import { useServiceTasks } from "@/app/_hooks/service-task/useServiceTask";
import { ServiceTaskWithLabels } from "@/app/_hooks/service-task/serviceTaskType";
import { getServiceTaskCategoryLabel } from "@/app/_utils/serviceTaskEnumHelper";
import { DEFAULT_PAGE_SIZE } from "@/app/_features/shared/table/constants";
import { serviceTaskTableColumns } from "@/app/_features/service-task/components/ServiceTaskTableColumns";
import { FilterBar } from "@/app/_features/shared/filter";
import { Plus } from "lucide-react";

export default function ServiceTaskListPage() {
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
    () => ({ page, pageSize, search }),
    [page, pageSize, search],
  );

  // Fetch data using the filter
  const { serviceTasks, pagination, isPending } = useServiceTasks(filter);

  // Transform data for the table
  const tableData = useMemo(
    () =>
      serviceTasks.map((task: ServiceTaskWithLabels) => ({
        id: task.id.toString(),
        Name: task.Name,
        Description: task.Description || "-",
        EstimatedLabourHours: task.EstimatedLabourHours,
        EstimatedCost: `$${task.EstimatedCost.toFixed(2)}`,
        CategoryLabel: getServiceTaskCategoryLabel(task.CategoryEnum),
        IsActive: task.IsActive ? "Yes" : "No",
      })),
    [serviceTasks],
  );

  // Pagination handlers
  const handlePreviousPage = () => setPage(p => Math.max(1, p - 1));
  const handleNextPage = () =>
    setPage(p => (pagination && p < pagination.totalPages ? p + 1 : p));
  const handlePageChange = (p: number) => setPage(p);
  const handlePageSizeChange = (size: number) => {
    setPageSize(size);
    setPage(1);
  };

  // Row click handler
  const handleRowClick = (row: any) => {
    router.push(`/service-tasks/${row.id}`);
  };

  // Empty state
  const emptyState = (
    <div className="text-center py-8">
      <p className="text-gray-500 mb-2">No service tasks found.</p>
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
        <h1 className="text-2xl font-semibold text-gray-900">Service Tasks</h1>
        <PrimaryButton
          onClick={() => router.push("/service-tasks/new")}
          className="flex items-center justify-center text-center"
        >
          <div className="flex items-center justify-center">
            <Plus className="w-5 h-5" />
            <span className="ml-2 flex items-center">Add Service Task</span>
          </div>
        </PrimaryButton>
      </div>
      <div className="flex items-center justify-between mb-6">
        <FilterBar
          searchValue={search}
          onSearchChange={setSearch}
          searchPlaceholder="Search service tasks"
          onFilterChange={() => {}}
        />
        {pagination && (
          <PaginationControls
            currentPage={pagination.pageNumber}
            totalPages={pagination.totalPages}
            totalItems={pagination.totalCount}
            itemsPerPage={pagination.pageSize}
            onPreviousPage={handlePreviousPage}
            onNextPage={handleNextPage}
            onPageChange={handlePageChange}
            onPageSizeChange={handlePageSizeChange}
            className="ml-auto"
          />
        )}
      </div>
      <DataTable
        columns={serviceTaskTableColumns}
        data={tableData}
        selectedItems={[]}
        onSelectItem={() => {}}
        onSelectAll={() => {}}
        getItemId={item => item.id}
        loading={isPending}
        showActions={false}
        emptyState={emptyState}
        onRowClick={handleRowClick}
        fixedLayout={false}
      />
    </div>
  );
}
