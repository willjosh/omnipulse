"use client";
import React, { useState, useMemo } from "react";
import { useRouter } from "next/navigation";
import { DataTable, PaginationControls } from "@/app/_features/shared/table";
import { PrimaryButton } from "@/app/_features/shared/button";
import { FilterBar } from "@/app/_features/shared/filter";
import { useServiceTasks } from "@/app/_hooks/service-task/useServiceTask";
import { ServiceTaskWithLabels } from "@/app/_hooks/service-task/serviceTaskType";
import { getServiceTaskCategoryLabel } from "@/app/_utils/serviceTaskEnumHelper";

const DEFAULT_PAGE_SIZE = 10;

const columns = [
  { key: "Name", header: "Name", width: "250px" },
  { key: "Description", header: "Description", width: "320px" },
  { key: "EstimatedLabourHours", header: "Est. Labour Hours", width: "200px" },
  { key: "EstimatedCost", header: "Est. Cost" },
  { key: "CategoryLabel", header: "Category" },
  { key: "IsActive", header: "Active" },
];

export default function ServiceTaskListPage() {
  const router = useRouter();
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);

  const filter = useMemo(
    () => ({ page, pageSize, search }),
    [page, pageSize, search],
  );

  const { serviceTasks, pagination, isPending } = useServiceTasks(filter);

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

  // Reset page when search changes
  React.useEffect(() => {
    setPage(1);
  }, [search]);

  // Handler for row click to navigate to details page (to be implemented)
  const handleRowClick = (row: any) => {
    router.push(`/service-tasks/${row.id}`);
  };

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
      <div className="flex items-center justify-between mb-2">
        <h1 className="text-2xl font-semibold text-gray-900">Service Tasks</h1>
        <PrimaryButton onClick={() => router.push("/service-tasks/new")}>
          + Add Service Task
        </PrimaryButton>
      </div>
      <div className="flex items-center justify-between mb-4">
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
        columns={columns}
        data={tableData}
        selectedItems={[]}
        onSelectItem={() => {}}
        onSelectAll={() => {}}
        getItemId={item => item.id}
        loading={isPending}
        showActions={false}
        emptyState={emptyState}
        onRowClick={handleRowClick}
      />
    </div>
  );
}
