"use client";
import React, { useState, useMemo, useEffect } from "react";
import { useRouter } from "next/navigation";
import { DataTable, PaginationControls } from "@/components/ui/Table";
import { FilterBar } from "@/components/ui/Filter";
import { formatEmptyValueWithUnknown } from "@/utils/emptyValueUtils";
import ModalPortal from "@/components/ui/Modal/ModalPortal";
import {
  useServiceReminders,
  useAddWorkOrderToServiceReminder,
} from "../hooks/useServiceReminders";
import { useWorkOrders } from "@/features/work-order/hooks/useWorkOrders";
import { ServiceReminderWithLabels } from "../types/serviceReminderType";
import { WorkOrderWithLabels } from "@/features/work-order/types/workOrderType";
import { DEFAULT_PAGE_SIZE } from "@/components/ui/Table/constants";
import { AlertTriangle, Clock, CheckCircle, XCircle, Plus } from "lucide-react";
import {
  ServiceReminderActionType,
  SERVICE_REMINDER_ACTION_CONFIG,
} from "../config/serviceReminderActions";

const ServiceReminderList: React.FC = () => {
  const router = useRouter();
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [sortBy, setSortBy] = useState("duedate");
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");
  const [showWorkOrderModal, setShowWorkOrderModal] = useState(false);
  const [selectedReminder, setSelectedReminder] =
    useState<ServiceReminderWithLabels | null>(null);

  useEffect(() => {
    setPage(1);
  }, [search, sortBy, sortOrder]);

  const filters = useMemo(
    () => ({
      PageNumber: page,
      PageSize: pageSize,
      SortBy: sortBy,
      SortDescending: sortOrder === "desc",
      Search: search,
    }),
    [page, pageSize, sortBy, sortOrder, search],
  );

  const { serviceReminders, pagination, isPending, isError, error } =
    useServiceReminders(filters);
  const { workOrders } = useWorkOrders({ PageSize: 100 });
  const {
    mutateAsync: addWorkOrderToServiceReminder,
    isPending: isAddingWorkOrder,
  } = useAddWorkOrderToServiceReminder();

  const getStatusIcon = (status: number) => {
    switch (status) {
      case 1: // UPCOMING
        return <Clock size={16} className="text-blue-600" />;
      case 2: // DUE_SOON
        return <AlertTriangle size={16} className="text-yellow-600" />;
      case 3: // OVERDUE
        return <XCircle size={16} className="text-red-600" />;
      case 4: // COMPLETED
        return <CheckCircle size={16} className="text-green-600" />;
      default:
        return <Clock size={16} className="text-gray-600" />;
    }
  };

  const serviceReminderColumns = [
    {
      key: "vehiclename",
      header: "Vehicle",
      sortable: true,
      width: "200px",
      render: (reminder: ServiceReminderWithLabels) => (
        <div>
          <div className="font-medium">{reminder.vehicleName}</div>
        </div>
      ),
    },
    {
      key: "servicerogramname",
      header: "Service Program",
      sortable: false,
      width: "200px",
      render: (reminder: ServiceReminderWithLabels) => (
        <div className="font-medium">
          {formatEmptyValueWithUnknown(reminder.serviceProgramName)}
        </div>
      ),
    },
    {
      key: "serviceschedulename",
      header: "Service Schedule",
      sortable: false,
      width: "200px",
      render: (reminder: ServiceReminderWithLabels) => (
        <div className="font-medium">{reminder.serviceScheduleName}</div>
      ),
    },
    {
      key: "status",
      header: "Status",
      sortable: true,
      width: "150px",
      render: (reminder: ServiceReminderWithLabels) => (
        <div className="flex items-center gap-2">
          {getStatusIcon(reminder.status)}
          <span className="text-sm font-medium">{reminder.statusLabel}</span>
        </div>
      ),
    },
    {
      key: "scheduletype",
      header: "Schedule Type",
      sortable: false,
      width: "150px",
      render: (reminder: ServiceReminderWithLabels) => (
        <span className="px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
          {reminder.scheduleTypeLabel}
        </span>
      ),
    },
    {
      key: "duedate",
      header: "Due Date",
      sortable: true,
      width: "180px",
      render: (reminder: ServiceReminderWithLabels) => (
        <div>
          {reminder.dueDate ? (
            <div className="font-medium">
              {new Date(reminder.dueDate).toLocaleDateString()}
            </div>
          ) : (
            <div className="text-gray-400">
              {formatEmptyValueWithUnknown(reminder.dueDate)}
            </div>
          )}
          {reminder.daysUntilDue !== null &&
            reminder.daysUntilDue !== undefined && (
              <div
                className={`text-sm ${
                  reminder.daysUntilDue < 0
                    ? "text-red-600"
                    : reminder.daysUntilDue <= 7
                      ? "text-yellow-600"
                      : "text-gray-500"
                }`}
              >
                {reminder.daysUntilDue < 0
                  ? `${Math.abs(reminder.daysUntilDue)} days overdue`
                  : reminder.daysUntilDue === 0
                    ? "Due today"
                    : `${reminder.daysUntilDue} days left`}
              </div>
            )}
        </div>
      ),
    },
    {
      key: "duemileage",
      header: "Due Mileage",
      sortable: true,
      width: "180px",
      render: (reminder: ServiceReminderWithLabels) => (
        <div>
          {reminder.dueMileage ? (
            <div className="font-medium">
              {reminder.dueMileage.toLocaleString()} km
            </div>
          ) : (
            <div className="text-gray-400">
              {formatEmptyValueWithUnknown(reminder.dueMileage)}
            </div>
          )}
          {reminder.mileageVariance !== null &&
            reminder.mileageVariance !== undefined && (
              <div
                className={`text-sm ${
                  reminder.mileageVariance > 0
                    ? "text-red-600"
                    : reminder.mileageVariance > -500
                      ? "text-yellow-600"
                      : "text-gray-500"
                }`}
              >
                {reminder.mileageVariance > 0
                  ? `${reminder.mileageVariance.toLocaleString()} km overdue`
                  : `${Math.abs(reminder.mileageVariance).toLocaleString()} km left`}
              </div>
            )}
        </div>
      ),
    },
    {
      key: "currentmileage",
      header: "Current Mileage",
      sortable: false,
      width: "150px",
      render: (reminder: ServiceReminderWithLabels) => (
        <span className="text-sm">
          {reminder.currentMileage.toLocaleString()} km
        </span>
      ),
    },
    {
      key: "taskcount",
      header: "Tasks",
      sortable: false,
      width: "100px",
      render: (reminder: ServiceReminderWithLabels) => (
        <div className="text-center">
          <div className="font-medium">{reminder.taskCount}</div>
          <div className="text-xs text-gray-500">tasks</div>
        </div>
      ),
    },
    {
      key: "totalestimatedcost",
      header: "Estimated Cost",
      sortable: false,
      width: "120px",
      render: (reminder: ServiceReminderWithLabels) => (
        <div className="text-right font-medium">
          ${reminder.totalEstimatedCost.toFixed(2)}
        </div>
      ),
    },
    {
      key: "totalestimatedlabourhours",
      header: "Estimated Labour Hours",
      sortable: false,
      width: "120px",
      render: (reminder: ServiceReminderWithLabels) => (
        <div className="text-center font-medium">
          {reminder.totalEstimatedLabourHours}h
        </div>
      ),
    },
  ];

  const handleSort = (sortKey: string) => {
    if (sortBy === sortKey) {
      setSortOrder(sortOrder === "asc" ? "desc" : "asc");
    } else {
      setSortBy(sortKey);
      setSortOrder("asc");
    }
    setPage(1);
  };

  const handlePageChange = (newPage: number) => {
    setPage(newPage);
  };

  const handlePageSizeChange = (newPageSize: number) => {
    setPageSize(newPageSize);
    setPage(1);
  };

  const handleRowClick = (reminder: ServiceReminderWithLabels) => {
    // Navigate to service schedule details page
    router.push(`/service-schedules/${reminder.serviceScheduleID}`);
  };

  const handleAddWorkOrder = (reminder: ServiceReminderWithLabels) => {
    setSelectedReminder(reminder);
    setShowWorkOrderModal(true);
  };

  const handleWorkOrderSelect = async (workOrder: WorkOrderWithLabels) => {
    if (!selectedReminder) return;

    try {
      await addWorkOrderToServiceReminder({
        serviceReminderId: selectedReminder.id,
        workOrderId: workOrder.id,
      });

      setShowWorkOrderModal(false);
      setSelectedReminder(null);
    } catch (error) {
      console.error("Error adding work order to service reminder:", error);
    }
  };

  const serviceReminderActions = [
    {
      key: ServiceReminderActionType.ADD_WORK_ORDER,
      label:
        SERVICE_REMINDER_ACTION_CONFIG[ServiceReminderActionType.ADD_WORK_ORDER]
          .label,
      variant:
        SERVICE_REMINDER_ACTION_CONFIG[ServiceReminderActionType.ADD_WORK_ORDER]
          .variant,
      icon: <Plus size={16} />,
      onClick: handleAddWorkOrder,
    },
  ];

  const emptyState = (
    <div className="text-center py-8">
      <p className="text-gray-500 mb-2">No service reminders found.</p>
      <button
        onClick={() => setSearch("")}
        className="text-blue-600 hover:text-blue-800 text-sm"
      >
        Clear search
      </button>
    </div>
  );

  if (isError) {
    return (
      <div className="p-6 w-full max-w-none">
        <div className="text-center py-10 text-red-600">
          Error loading service reminders: {error?.message || "Unknown error"}
        </div>
      </div>
    );
  }

  return (
    <div className="p-6 w-full max-w-none">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-semibold text-gray-900">
          Service Reminders
        </h1>
      </div>

      <div className="flex items-end justify-between mb-4">
        <FilterBar
          searchValue={search}
          onSearchChange={setSearch}
          searchPlaceholder="Search"
          onFilterChange={() => {}}
        />

        <PaginationControls
          currentPage={page}
          totalPages={pagination?.totalPages || 0}
          totalItems={pagination?.totalCount || 0}
          itemsPerPage={pageSize}
          onNextPage={() => handlePageChange(page + 1)}
          onPreviousPage={() => handlePageChange(page - 1)}
          onPageChange={setPage}
          onPageSizeChange={handlePageSizeChange}
        />
      </div>

      <DataTable<ServiceReminderWithLabels>
        data={serviceReminders || []}
        columns={serviceReminderColumns}
        onRowClick={handleRowClick}
        actions={serviceReminderActions}
        showActions={true}
        fixedLayout={false}
        loading={isPending}
        getItemId={reminder => reminder.id.toString()}
        emptyState={emptyState}
        onSort={handleSort}
        sortBy={sortBy}
        sortOrder={sortOrder}
      />

      {/* Work Order Selection Modal */}
      <ModalPortal isOpen={showWorkOrderModal}>
        <div className="fixed inset-0 backdrop-brightness-50 bg-opacity-50 flex items-center justify-center z-[100]">
          <div className="bg-white rounded-lg shadow-xl max-w-4xl w-full mx-4 max-h-[80vh] overflow-hidden">
            {/* Header */}
            <div className="px-6 py-4 border-b border-gray-200">
              <div className="flex items-center justify-between">
                <div>
                  <h2 className="text-lg font-semibold text-gray-900">
                    Select Work Order
                    {isAddingWorkOrder && (
                      <span className="ml-2 text-sm text-blue-600">
                        Adding...
                      </span>
                    )}
                  </h2>
                  {selectedReminder && (
                    <p className="text-sm text-gray-500 mt-1">
                      For {selectedReminder.vehicleName} -{" "}
                      {selectedReminder.serviceScheduleName}
                    </p>
                  )}
                </div>
                <button
                  onClick={() => setShowWorkOrderModal(false)}
                  className="text-gray-400 hover:text-gray-600"
                >
                  <svg
                    className="w-6 h-6"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth="2"
                      d="M6 18L18 6M6 6l12 12"
                    />
                  </svg>
                </button>
              </div>
            </div>

            {/* Content */}
            <div className="px-6 py-4 overflow-y-auto max-h-[60vh]">
              <div className="mb-4">
                <h3 className="text-sm font-medium text-gray-700 mb-2">
                  Available Work Orders for {selectedReminder?.vehicleName}
                </h3>
                <div className="space-y-2">
                  {(() => {
                    // Filter work orders to only show vehicles with the same vehicle ID
                    const filteredWorkOrders = workOrders.filter(
                      workOrder =>
                        workOrder.vehicleID === selectedReminder?.vehicleID,
                    );

                    if (filteredWorkOrders.length > 0) {
                      return filteredWorkOrders.map(workOrder => (
                        <div
                          key={workOrder.id}
                          onClick={() =>
                            !isAddingWorkOrder &&
                            handleWorkOrderSelect(workOrder)
                          }
                          className={`p-3 border border-gray-200 rounded-lg transition-colors ${
                            isAddingWorkOrder
                              ? "opacity-50 cursor-not-allowed"
                              : "hover:bg-gray-50 cursor-pointer"
                          }`}
                        >
                          <div className="flex items-center justify-between">
                            <div>
                              <div className="font-medium text-gray-900">
                                {workOrder.title}
                              </div>
                              {workOrder.description && (
                                <div className="text-sm text-gray-500 mt-1">
                                  {workOrder.description}
                                </div>
                              )}
                              <div className="text-sm text-gray-500 mt-1">
                                Vehicle: {workOrder.vehicleName} | Status:{" "}
                                {workOrder.statusLabel}
                              </div>
                            </div>
                            <div className="text-right">
                              <div className="text-sm font-medium text-gray-900">
                                ${workOrder.totalCost?.toFixed(2) || "0.00"}
                              </div>
                              <div className="text-xs text-gray-500">
                                {workOrder.workOrderLineItems.length} items
                              </div>
                            </div>
                          </div>
                        </div>
                      ));
                    } else {
                      return (
                        <div className="text-center py-8">
                          <p className="text-gray-500 mb-2">
                            No work orders found for{" "}
                            {selectedReminder?.vehicleName}.
                          </p>
                          <button
                            onClick={() => router.push("/work-orders/new")}
                            className="text-blue-600 hover:text-blue-700 font-medium"
                          >
                            Create New Work Order
                          </button>
                        </div>
                      );
                    }
                  })()}
                </div>
              </div>
            </div>

            {/* Footer */}
            <div className="px-6 py-4 border-t border-gray-200 bg-gray-50 rounded-b-lg">
              <div className="flex justify-between">
                <button
                  onClick={() => router.push("/work-orders/new")}
                  className="text-blue-600 hover:text-blue-700 font-medium"
                >
                  Create New Work Order
                </button>
                <button
                  onClick={() => setShowWorkOrderModal(false)}
                  className="text-blue-600 hover:text-blue-700 font-medium"
                >
                  Cancel
                </button>
              </div>
            </div>
          </div>
        </div>
      </ModalPortal>
    </div>
  );
};

export default ServiceReminderList;
