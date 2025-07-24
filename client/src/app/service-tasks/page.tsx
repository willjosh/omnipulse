"use client";
import React, { useState, useMemo } from "react";
import { useRouter } from "next/navigation";
import { DataTable, PaginationControls } from "@/app/_features/shared/table";
import { PrimaryButton } from "@/app/_features/shared/button";
import { useServiceTasks } from "@/app/_hooks/service-task/useServiceTasks";
import { ServiceTaskWithLabels } from "@/app/_hooks/service-task/serviceTaskType";
import { getServiceTaskCategoryLabel } from "@/app/_utils/serviceTaskEnumHelper";
import { DEFAULT_PAGE_SIZE } from "@/app/_features/shared/table/constants";
import { serviceTaskTableColumns } from "@/app/_features/service-task/components/ServiceTaskTableColumns";
import { FilterBar } from "@/app/_features/shared/filter";
import { Plus } from "lucide-react";
import {
  Listbox,
  ListboxButton,
  ListboxOptions,
  ListboxOption,
} from "@headlessui/react";
import { ChevronDown } from "lucide-react";
import { serviceTaskSortOptions } from "@/app/_utils/serviceTaskOptions";

export default function ServiceTaskListPage() {
  const router = useRouter();

  // UI state: search, pagination, etc.
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [sortBy, setSortBy] = useState("Name");
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");

  // Reset page to 1 when search changes
  React.useEffect(() => {
    setPage(1);
  }, [search, sortBy, sortOrder]);

  // Compose filter object for data fetching
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

  // Fetch data using the filter
  const { serviceTasks, pagination, isPending } = useServiceTasks(filter);

  console.log(serviceTasks);

  // Transform data for the table
  const tableData = useMemo(
    () =>
      serviceTasks.map((task: ServiceTaskWithLabels) => ({
        id: task.id.toString(),
        name: task.name,
        description: task.description || "-",
        estimatedLabourHours: task.estimatedLabourHours,
        estimatedCost: `$${(task.estimatedCost ?? 0).toFixed(2)}`,
        categoryLabel: getServiceTaskCategoryLabel(task.categoryEnum),
        isActive: task.isActive ? "Yes" : "No",
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
        <div className="flex items-center gap-2">
          <Listbox
            value={`${sortBy}-${sortOrder}`}
            onChange={val => {
              const [field, order] = val.split("-");
              setSortBy(field);
              setSortOrder(order as "asc" | "desc");
            }}
          >
            {({ open }) => (
              <div className="relative">
                <ListboxButton className="flex items-center justify-between px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 focus:outline-none focus:ring-2 transition-colors h-9 min-w-[80px] max-w-[200px]">
                  <span className="truncate">Sort by:</span>
                  <span className="mx-3 font-semibold truncate">
                    {serviceTaskSortOptions.find(
                      (o: { value: string; label: string }) =>
                        o.value === `${sortBy}-${sortOrder}`,
                    )?.label || "Sort"}
                  </span>
                  <ChevronDown className="w-4 h-4 ml-auto" />
                </ListboxButton>
                <ListboxOptions className="absolute right-0 mt-2 w-40 bg-white rounded-lg shadow-lg border border-gray-200 z-20 focus:outline-none">
                  {serviceTaskSortOptions.map(
                    (opt: { value: string; label: string }) => (
                      <ListboxOption
                        key={opt.value}
                        value={opt.value}
                        className={({
                          active,
                          selected,
                        }: {
                          active: boolean;
                          selected: boolean;
                        }) =>
                          `w-full text-left px-4 py-2 text-sm cursor-pointer transition-colors ${
                            selected
                              ? "bg-blue-100 text-blue-700 font-semibold"
                              : active
                                ? "bg-blue-50 text-blue-700"
                                : "text-gray-700"
                          }`
                        }
                      >
                        {opt.label}
                      </ListboxOption>
                    ),
                  )}
                </ListboxOptions>
              </div>
            )}
          </Listbox>
          <PrimaryButton onClick={() => router.push("/service-tasks/new")}>
            <div className="flex items-center justify-center">
              <Plus className="w-5 h-5" />
              <span className="ml-2 flex items-center">Add Service Task</span>
            </div>
          </PrimaryButton>
        </div>
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
        onSort={key => {
          if (sortBy === key) {
            setSortOrder(sortOrder === "asc" ? "desc" : "asc");
          } else {
            setSortBy(key);
            setSortOrder("asc");
          }
        }}
        sortBy={sortBy}
        sortOrder={sortOrder}
      />
    </div>
  );
}
