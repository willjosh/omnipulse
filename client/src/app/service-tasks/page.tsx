"use client";
import React, { useState, useMemo } from "react";
import { useRouter } from "next/navigation";
import { DataTable, PaginationControls } from "@/components/ui/Table";
import { PrimaryButton } from "@/components/ui/Button";
import { useServiceTasks } from "@/features/service-task/hooks/useServiceTasks";
import { ServiceTaskWithLabels } from "@/features/service-task/types/serviceTaskType";
import { getServiceTaskCategoryLabel } from "@/features/service-task/utils/serviceTaskEnumHelper";
import { DEFAULT_PAGE_SIZE } from "@/components/ui/Table/constants";
import { FilterBar } from "@/components/ui/Filter";
import { Plus } from "lucide-react";

export default function ServiceTaskListPage() {
  const router = useRouter();

  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [sortBy, setSortBy] = useState("createdat");
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("desc");

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

  const handleSort = (sortKey: string) => {
    if (sortBy === sortKey) {
      setSortOrder(sortOrder === "asc" ? "desc" : "asc");
    } else {
      setSortBy(sortKey);
      setSortOrder("asc");
    }
    setPage(1);
  };

  const serviceTaskTableColumns = [
    {
      key: "name",
      header: "Name",
      width: "250px",
      sortable: true,
      render: (item: ServiceTaskWithLabels) => <div>{item.name}</div>,
    },
    {
      key: "description",
      header: "Description",
      width: "320px",
      sortable: false,
      render: (item: ServiceTaskWithLabels) => (
        <div>{item.description || "-"}</div>
      ),
    },
    {
      key: "category",
      header: "Category",
      sortable: true,
      render: (item: ServiceTaskWithLabels) => (
        <div>{getServiceTaskCategoryLabel(item.categoryEnum)}</div>
      ),
    },
    {
      key: "estimatedlabourhours",
      header: "Estimated Labour Hours",
      width: "200px",
      sortable: true,
      render: (item: ServiceTaskWithLabels) => (
        <div>{item.estimatedLabourHours}</div>
      ),
    },
    {
      key: "estimatedcost",
      header: "Estimated Cost",
      sortable: true,
      render: (item: ServiceTaskWithLabels) => (
        <div>${(item.estimatedCost ?? 0).toFixed(2)}</div>
      ),
    },
  ];

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
    <div className="p-6 w-full max-w-none">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-semibold text-gray-900">Service Tasks</h1>
        <div className="flex items-center gap-2">
          <PrimaryButton onClick={() => router.push("/service-tasks/new")}>
            <Plus size={16} />
            Add Service Task
          </PrimaryButton>
        </div>
      </div>
      <div className="flex items-center justify-between mb-4">
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
        data={serviceTasks}
        getItemId={item => item.id.toString()}
        loading={isPending}
        showActions={false}
        emptyState={emptyState}
        onRowClick={handleRowClick}
        fixedLayout={false}
        onSort={handleSort}
        sortBy={sortBy}
        sortOrder={sortOrder}
      />
    </div>
  );
}
