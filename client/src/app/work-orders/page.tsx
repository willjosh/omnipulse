"use client";

import React, { useState, useMemo } from "react";
import { useRouter } from "next/navigation";
import { DataTable, PaginationControls } from "@/components/ui/Table";
import { PrimaryButton } from "@/components/ui/Button";
import { FilterBar } from "@/components/ui/Filter";
import { Plus } from "lucide-react";
import { useWorkOrders } from "@/features/work-order/hooks/useWorkOrders";
import { WorkOrderWithLabels } from "@/features/work-order/types/workOrderType";
import { DEFAULT_PAGE_SIZE } from "@/components/ui/Table/constants";
import { workOrderTableColumns } from "@/features/work-order/config/workOrderTableColumns";

export default function WorkOrdersPage() {
  const router = useRouter();

  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [sortBy, setSortBy] = useState("Title");
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

  const { workOrders, pagination, isPending } = useWorkOrders(filter);

  const tableData = useMemo(
    () =>
      workOrders.map((workOrder: WorkOrderWithLabels) => ({
        id: workOrder.id.toString(),
        title: workOrder.title,
        vehicleName: workOrder.vehicleName,
        workOrderTypeLabel: workOrder.workOrderTypeLabel,
        priorityLevelLabel: workOrder.priorityLevelLabel,
        statusLabel: workOrder.statusLabel,
        assignedToUserName: workOrder.assignedToUserName,
        scheduledStartDate: workOrder.scheduledStartDate || "Not scheduled",
        actualStartDate: workOrder.actualStartDate || "Not started",
        startOdometer: workOrder.startOdometer,
        endOdometer: workOrder.endOdometer || "—",
        totalCost: workOrder.totalCost
          ? `$${workOrder.totalCost.toFixed(2)}`
          : "—",
        totalLaborCost: workOrder.totalLaborCost
          ? `$${workOrder.totalLaborCost.toFixed(2)}`
          : "—",
        totalItemCost: workOrder.totalItemCost
          ? `$${workOrder.totalItemCost.toFixed(2)}`
          : "—",
        description: workOrder.description || "—",
      })),
    [workOrders],
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
    router.push(`/work-orders/${row.id}`);
  };

  const emptyState = (
    <div className="text-center py-8">
      <p className="text-gray-500 mb-2">No work orders found.</p>
      <p className="text-gray-400 mb-4">
        Work Orders are used to plan and complete service needed for a
        particular vehicle.
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
    <div className="p-6 w-[1260px] min-h-screen mx-auto">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-semibold text-gray-900">Work Orders</h1>
        <PrimaryButton onClick={() => router.push("/work-orders/new")}>
          <div className="flex items-center justify-center">
            <Plus className="w-5 h-5" />
            <span className="ml-2 flex items-center">Add Work Order</span>
          </div>
        </PrimaryButton>
      </div>

      <div className="flex items-center justify-between mb-6">
        <FilterBar
          searchValue={search}
          onSearchChange={setSearch}
          searchPlaceholder="Search work orders"
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
        columns={workOrderTableColumns}
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
