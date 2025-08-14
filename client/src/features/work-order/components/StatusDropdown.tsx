import React, { useState, useEffect } from "react";
import { ChevronDown, Check } from "lucide-react";
import { WorkOrderStatusEnum } from "../types/workOrderEnum";
import { getWorkOrderStatusLabel } from "../utils/workOrderEnumHelper";
import { useCompleteWorkOrder } from "../hooks/useWorkOrders";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import SecondaryButton from "@/components/ui/Button/SecondaryButton";

interface StatusDropdownProps {
  currentStatus: number;
  workOrderId: number;
  onStatusChange?: (newStatus: number) => void;
  onUpdateWorkOrder?: (newStatus: number) => void;
  disabled?: boolean;
}

const StatusDropdown: React.FC<StatusDropdownProps> = ({
  currentStatus,
  workOrderId,
  onStatusChange,
  onUpdateWorkOrder,
  disabled = false,
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const [selectedStatus, setSelectedStatus] = useState(currentStatus);
  const { mutateAsync: completeWorkOrder, isPending: isCompleting } =
    useCompleteWorkOrder();
  const notify = useNotification();

  // Sync local state with prop when currentStatus changes
  useEffect(() => {
    setSelectedStatus(currentStatus);
  }, [currentStatus]);

  const statusOptions = [
    { value: WorkOrderStatusEnum.CREATED, label: "Created" },
    { value: WorkOrderStatusEnum.ASSIGNED, label: "Assigned" },
    { value: WorkOrderStatusEnum.IN_PROGRESS, label: "In Progress" },
    { value: WorkOrderStatusEnum.WAITING_PARTS, label: "Waiting Parts" },
    { value: WorkOrderStatusEnum.COMPLETED, label: "Completed" },
    { value: WorkOrderStatusEnum.CANCELLED, label: "Cancelled" },
    { value: WorkOrderStatusEnum.ON_HOLD, label: "On Hold" },
  ];

  const getStatusColor = (status: number) => {
    switch (status) {
      case WorkOrderStatusEnum.CREATED:
      case WorkOrderStatusEnum.ASSIGNED:
        return "bg-green-500";
      case WorkOrderStatusEnum.IN_PROGRESS:
      case WorkOrderStatusEnum.WAITING_PARTS:
        return "bg-orange-500";
      case WorkOrderStatusEnum.COMPLETED:
        return "bg-gray-500";
      case WorkOrderStatusEnum.CANCELLED:
        return "bg-red-500";
      case WorkOrderStatusEnum.ON_HOLD:
        return "bg-orange-500";
      default:
        return "bg-gray-500";
    }
  };

  const handleStatusChange = async (newStatus: number) => {
    if (disabled || isCompleting) return;

    try {
      if (newStatus === WorkOrderStatusEnum.COMPLETED) {
        // Call the complete work order API
        await completeWorkOrder(workOrderId);
        notify("Work order completed successfully!", "success");
        setSelectedStatus(newStatus);
        onStatusChange?.(newStatus);
      } else {
        // For other status changes, call the update API
        if (onUpdateWorkOrder) {
          setSelectedStatus(newStatus);
          onStatusChange?.(newStatus);
          onUpdateWorkOrder(newStatus);
        }
      }

      setIsOpen(false);
    } catch (error: any) {
      console.error("Error updating work order status:", error);

      // Handle specific error cases based on the API response
      let errorMessage = "Failed to update work order status.";

      if (error?.response?.status === 400) {
        errorMessage =
          "Work order is not ready for completion or validation failed.";
      } else if (error?.response?.status === 404) {
        errorMessage = "Work order not found.";
      } else if (error?.response?.status === 409) {
        errorMessage = "Work order is already completed.";
      } else if (error?.response?.data?.detail) {
        errorMessage = error.response.data.detail;
      }

      notify(errorMessage, "error");
      setSelectedStatus(currentStatus);
    }
  };

  const currentStatusLabel = getWorkOrderStatusLabel(selectedStatus);

  return (
    <div className="relative">
      <SecondaryButton
        onClick={() => !disabled && setIsOpen(!isOpen)}
        disabled={disabled || isCompleting}
        className="flex items-center gap-2 px-3 py-1.5"
      >
        <div
          className={`w-2 h-2 rounded-full ${getStatusColor(selectedStatus)}`}
        />
        <span className="text-gray-900">{currentStatusLabel}</span>
        <ChevronDown className="w-4 h-4 text-gray-500" />
        {isCompleting && (
          <div className="animate-spin rounded-full h-3 w-3 border-b border-blue-600" />
        )}
      </SecondaryButton>

      {isOpen && !disabled && (
        <div className="absolute top-full left-0 mt-1 w-48 bg-white border border-gray-200 rounded-md shadow-lg z-10">
          <div className="py-1">
            {statusOptions.map(option => (
              <button
                key={option.value}
                onClick={() => handleStatusChange(option.value)}
                disabled={isCompleting}
                className={`w-full flex items-center gap-2 px-3 py-2 text-sm text-left hover:bg-gray-50 ${
                  selectedStatus === option.value
                    ? "bg-blue-50 text-blue-700"
                    : "text-gray-900"
                } ${isCompleting ? "cursor-not-allowed opacity-50" : "cursor-pointer"}`}
              >
                <div
                  className={`w-2 h-2 rounded-full ${getStatusColor(option.value)}`}
                />
                <span className="flex-1">{option.label}</span>
                {selectedStatus === option.value && (
                  <Check className="w-4 h-4 text-blue-600" />
                )}
              </button>
            ))}
          </div>
        </div>
      )}

      {isOpen && (
        <div className="fixed inset-0 z-0" onClick={() => setIsOpen(false)} />
      )}
    </div>
  );
};

export default StatusDropdown;
