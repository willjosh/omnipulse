"use client";

import React, { useState, useMemo } from "react";
import { useRouter } from "next/navigation";
import { DataTable, PaginationControls } from "@/components/ui/Table";
import { PrimaryButton } from "@/components/ui/Button";
import { FilterBar } from "@/components/ui/Filter";
import { Plus, Trash2 } from "lucide-react";
import {
  useWorkOrders,
  useDeleteWorkOrder,
} from "@/features/work-order/hooks/useWorkOrders";
import { WorkOrderWithLabels } from "@/features/work-order/types/workOrderType";
import { DEFAULT_PAGE_SIZE } from "@/components/ui/Table/constants";
import { ConfirmModal } from "@/components/ui/Modal";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";

export default function WorkOrdersPage() {
  const router = useRouter();
  const notify = useNotification();
  const { mutate: deleteWorkOrder, isPending: isDeleting } =
    useDeleteWorkOrder();

  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [sortBy, setSortBy] = useState("id");
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");
  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    workOrder: WorkOrderWithLabels | null;
  }>({ isOpen: false, workOrder: null });

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

  const handleSort = (sortKey: string) => {
    if (sortBy === sortKey) {
      setSortOrder(sortOrder === "asc" ? "desc" : "asc");
    } else {
      setSortBy(sortKey);
      setSortOrder("asc");
    }
    setPage(1);
  };

  const workOrderTableColumns = [
    {
      key: "title",
      header: "Title & Description",
      width: "300px",
      sortable: false,
      render: (item: WorkOrderWithLabels) => (
        <div>
          <div className="font-medium text-gray-900">{item.title}</div>
          {item.description && (
            <div className="text-sm text-gray-500 mt-1">{item.description}</div>
          )}
        </div>
      ),
    },
    {
      key: "vehicleid",
      header: "Vehicle",
      width: "200px",
      sortable: false,
      render: (item: WorkOrderWithLabels) => <div>{item.vehicleName}</div>,
    },
    {
      key: "workordertype",
      header: "Type",
      width: "120px",
      sortable: true,
      render: (item: WorkOrderWithLabels) => (
        <div>{item.workOrderTypeLabel}</div>
      ),
    },
    {
      key: "priority",
      header: "Priority",
      width: "100px",
      sortable: true,
      render: (item: WorkOrderWithLabels) => (
        <div>{item.priorityLevelLabel}</div>
      ),
    },
    {
      key: "status",
      header: "Status",
      width: "120px",
      sortable: true,
      render: (item: WorkOrderWithLabels) => <div>{item.statusLabel}</div>,
    },
    {
      key: "assignedtouserid",
      header: "Assigned To",
      width: "150px",
      sortable: false,
      render: (item: WorkOrderWithLabels) => (
        <div>{item.assignedToUserName}</div>
      ),
    },
    {
      key: "scheduledstartdate",
      header: "Scheduled Start",
      width: "150px",
      sortable: true,
      render: (item: WorkOrderWithLabels) => (
        <div>{item.scheduledStartDate}</div>
      ),
    },
    {
      key: "actualstartdate",
      header: "Actual Start",
      width: "150px",
      sortable: true,
      render: (item: WorkOrderWithLabels) => <div>{item.actualStartDate}</div>,
    },
    {
      key: "startodometer",
      header: "Start Odometer",
      width: "120px",
      sortable: true,
      render: (item: WorkOrderWithLabels) => <div>{item.startOdometer}</div>,
    },
    {
      key: "endodometer",
      header: "End Odometer",
      width: "120px",
      sortable: false,
      render: (item: WorkOrderWithLabels) => <div>{item.endOdometer}</div>,
    },
    {
      key: "totalcost",
      header: "Total Cost",
      width: "100px",
      sortable: false,
      render: (item: WorkOrderWithLabels) => <div>{item.totalCost}</div>,
    },
    {
      key: "totallaborcost",
      header: "Labor Cost",
      width: "100px",
      sortable: false,
      render: (item: WorkOrderWithLabels) => <div>{item.totalLaborCost}</div>,
    },
    {
      key: "totalitemcost",
      header: "Item Cost",
      width: "100px",
      sortable: false,
      render: (item: WorkOrderWithLabels) => <div>{item.totalItemCost}</div>,
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

  const handleRowClick = (workOrder: WorkOrderWithLabels) => {
    router.push(`/work-orders/${workOrder.id}`);
  };

  const handleDeleteWorkOrder = async () => {
    if (!confirmModal.workOrder) return;

    deleteWorkOrder(confirmModal.workOrder.id, {
      onSuccess: () => {
        notify("Work order deleted successfully", "success");
        setConfirmModal({ isOpen: false, workOrder: null });
      },
      onError: error => {
        console.error("Failed to delete work order:", error);
        notify("Failed to delete work order", "error");
        setConfirmModal({ isOpen: false, workOrder: null });
      },
    });
  };

  const workOrderActions = useMemo(
    () => [
      {
        key: "delete",
        label: "Delete",
        icon: <Trash2 size={16} />,
        variant: "danger" as const,
        onClick: (workOrder: WorkOrderWithLabels) => {
          setConfirmModal({ isOpen: true, workOrder });
        },
      },
    ],
    [],
  );

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
    <div className="p-6 w-full max-w-none">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-semibold text-gray-900">Work Orders</h1>
        <PrimaryButton onClick={() => router.push("/work-orders/new")}>
          <Plus size={16} />
          Add Work Order
        </PrimaryButton>
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
        columns={workOrderTableColumns}
        data={workOrders}
        selectedItems={[]}
        onSelectItem={() => {}}
        onSelectAll={() => {}}
        getItemId={item => item.id.toString()}
        loading={isPending}
        showActions={true}
        actions={workOrderActions}
        emptyState={emptyState}
        onRowClick={handleRowClick}
        fixedLayout={false}
        onSort={handleSort}
        sortBy={sortBy}
        sortOrder={sortOrder}
      />

      <ConfirmModal
        isOpen={confirmModal.isOpen}
        onClose={() =>
          !isDeleting && setConfirmModal({ isOpen: false, workOrder: null })
        }
        onConfirm={handleDeleteWorkOrder}
        title="Delete Work Order"
        message={
          confirmModal.workOrder
            ? `Are you sure you want to delete "${confirmModal.workOrder.title}"? This action cannot be undone.`
            : ""
        }
        confirmText={isDeleting ? "Deleting..." : "Delete"}
      />
    </div>
  );
}
