"use client";
import React, { useState, useMemo } from "react";
import { useRouter } from "next/navigation";
import { DataTable, PaginationControls } from "@/components/ui/Table";
import { PrimaryButton } from "@/components/ui/Button";
import { useServiceTasks } from "@/features/service-task/hooks/useServiceTasks";
import { ServiceTaskWithLabels } from "@/features/service-task/types/serviceTaskType";
import { getServiceTaskCategoryLabel } from "@/features/service-task/utils/serviceTaskEnumHelper";
import { DEFAULT_PAGE_SIZE } from "@/components/ui/Table/constants";
import { serviceTaskTableColumns } from "@/features/service-task/config/ServiceTaskTableColumns";
import { FilterBar } from "@/components/ui/Filter";
import { Plus } from "lucide-react";
import {
  Listbox,
  ListboxButton,
  ListboxOptions,
  ListboxOption,
} from "@headlessui/react";
import { ChevronDown } from "lucide-react";
import { serviceTaskSortOptions } from "@/features/service-task/config/serviceTaskOptions";

export default function ServiceTaskListPage() {
  const router = useRouter();

  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [sortBy, setSortBy] = useState("Name");
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");

  React.useEffect(() => {
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

  const { serviceTasks, pagination, isPending } = useServiceTasks(filter);

  console.log(serviceTasks);

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

  const handlePreviousPage = () => setPage(p => Math.max(1, p - 1));
  const handleNextPage = () =>
    setPage(p => (pagination && p < pagination.totalPages ? p + 1 : p));
  const handlePageChange = (p: number) => setPage(p);
  const handlePageSizeChange = (size: number) => {
    setPageSize(size);
    setPage(1);
  };

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
    <div className="p-6 w-full max-w-none min-h-screen">
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
