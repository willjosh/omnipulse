"use client";

import React, { useState, useEffect } from "react";
import { useRouter, useParams } from "next/navigation";
import WorkOrderHeader from "@/features/work-order/components/WorkOrderHeader";
import WorkOrderDetailsForm from "@/features/work-order/components/WorkOrderDetailsForm";
import WorkOrderIssuesForm from "@/features/work-order/components/WorkOrderIssuesForm";
import WorkOrderLineItemsForm from "@/features/work-order/components/WorkOrderLineItemsForm";
import SecondaryButton from "@/components/ui/Button/SecondaryButton";
import PrimaryButton from "@/components/ui/Button/PrimaryButton";
import {
  useWorkOrder,
  useUpdateWorkOrder,
  useCompleteWorkOrder,
} from "@/features/work-order/hooks/useWorkOrders";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import { CreateWorkOrderLineItem } from "@/features/work-order/types/workOrderType";
import {
  WorkOrderFormState,
  validateWorkOrderForm,
  mapFormToCreateWorkOrderCommand,
  emptyWorkOrderFormState,
} from "@/features/work-order/utils/workOrderFormUtils";
import { BreadcrumbItem } from "@/components/ui/Layout/Breadcrumbs";
import { getErrorMessage, getErrorFields } from "@/utils/fieldErrorUtils";

export default function EditWorkOrderPage() {
  const router = useRouter();
  const params = useParams();
  const workOrderId = Number(params.id);

  // Form state
  const [form, setForm] = useState<WorkOrderFormState>({
    ...emptyWorkOrderFormState,
  });
  const [errors, setErrors] = useState<{ [key: string]: string }>({});
  const [resetKey, setResetKey] = useState(0);

  // Fetch work order data
  const { workOrder, isPending: isLoading } = useWorkOrder(workOrderId);

  // Update work order mutation
  const { mutate: updateWorkOrder, isPending: isUpdating } =
    useUpdateWorkOrder();

  // Complete work order mutation
  const { mutateAsync: completeWorkOrder, isPending: isCompleting } =
    useCompleteWorkOrder();
  const notify = useNotification();

  // Load work order data into form when data is available
  useEffect(() => {
    if (workOrder) {
      const formData: WorkOrderFormState = {
        title: workOrder.title,
        description: workOrder.description,
        vehicleID: workOrder.vehicleID,
        workOrderType: workOrder.workOrderTypeEnum,
        priorityLevel: workOrder.priorityLevelEnum,
        status: workOrder.statusEnum,
        assignedToUserID: workOrder.assignedToUserID,
        scheduledStartDate: workOrder.scheduledStartDate,
        actualStartDate: workOrder.actualStartDate,
        scheduledCompletionDate: workOrder.scheduledCompletionDate,
        actualCompletionDate: workOrder.actualCompletionDate,
        startOdometer: workOrder.startOdometer,
        endOdometer: workOrder.endOdometer,
        issueIdList: workOrder.issueIDs,
        workOrderLineItems: workOrder.workOrderLineItems.map(item => ({
          itemType: item.itemTypeEnum,
          quantity: item.quantity,
          description: item.description,
          inventoryItemID: item.inventoryItemID,
          assignedToUserID: item.assignedToUserID,
          serviceTaskID: item.serviceTaskID,
          unitPrice: item.unitPrice,
          hourlyRate: item.hourlyRate,
          laborHours: item.laborHours,
        })) as CreateWorkOrderLineItem[],
      };
      setForm(formData);
    }
  }, [workOrder]);

  // Validation
  const validate = () => {
    const newErrors = validateWorkOrderForm(form);
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // Controlled field change
  const handleFormChange = (
    field: string,
    value: string | number | null | number[] | CreateWorkOrderLineItem[],
  ) => {
    setForm(f => ({ ...f, [field]: value }));
    setErrors(e => ({ ...e, [field]: "" }));
  };

  // Convert form state to CreateWorkOrderCommand
  const toUpdateWorkOrderCommand = () => ({
    workOrderID: workOrderId,
    ...mapFormToCreateWorkOrderCommand(form),
  });

  // Save Work Order handler
  const handleSave = async () => {
    if (!validate()) {
      notify("Please fill all required fields", "error");
      return;
    }

    // Check if status is being changed to "Completed"
    const isCompleting = form.status === 5; // WorkOrderStatusEnum.COMPLETED

    try {
      if (isCompleting) {
        // First complete the work order
        await completeWorkOrder(workOrderId);
        notify("Work order completed successfully!", "success");
      }

      // Then update the work order with all other changes
      updateWorkOrder(toUpdateWorkOrderCommand(), {
        onSuccess: () => {
          notify("Work order updated successfully!", "success");
          router.push(`/work-orders/${workOrderId}`);
        },
        onError: (error: any) => {
          console.error("Failed to update work order:", error);

          const errorMessage = getErrorMessage(
            error,
            "Failed to update work order. Please check your input and try again.",
          );

          const fieldErrors = getErrorFields(error, [
            "workOrderID",
            "vehicleID",
            "assignedToUserID",
            "title",
            "description",
            "workOrderType",
            "priorityLevel",
            "status",
            "scheduledStartDate",
            "actualStartDate",
            "scheduledCompletionDate",
            "actualCompletionDate",
            "startOdometer",
            "endOdometer",
            "issueIdList",
            "workOrderLineItems",
          ]);

          const newErrors: { [key: string]: string } = {};
          if (fieldErrors.workOrderID) {
            newErrors.workOrderID = "Invalid work order ID";
          }
          if (fieldErrors.vehicleID) {
            newErrors.vehicleID = "Invalid vehicle selection";
          }
          if (fieldErrors.assignedToUserID) {
            newErrors.assignedToUserID = "Invalid assigned user";
          }
          if (fieldErrors.title) {
            newErrors.title = "Invalid title";
          }
          if (fieldErrors.description) {
            newErrors.description = "Invalid description";
          }
          if (fieldErrors.workOrderType) {
            newErrors.workOrderType = "Invalid work order type";
          }
          if (fieldErrors.priorityLevel) {
            newErrors.priorityLevel = "Invalid priority level";
          }
          if (fieldErrors.status) {
            newErrors.status = "Invalid status";
          }
          if (fieldErrors.scheduledStartDate) {
            newErrors.scheduledStartDate = "Invalid scheduled start date";
          }
          if (fieldErrors.actualStartDate) {
            newErrors.actualStartDate = "Invalid actual start date";
          }
          if (fieldErrors.scheduledCompletionDate) {
            newErrors.scheduledCompletionDate =
              "Invalid scheduled completion date";
          }
          if (fieldErrors.actualCompletionDate) {
            newErrors.actualCompletionDate = "Invalid actual completion date";
          }
          if (fieldErrors.startOdometer) {
            newErrors.startOdometer = "Invalid start odometer";
          }
          if (fieldErrors.endOdometer) {
            newErrors.endOdometer = "Invalid end odometer";
          }
          if (fieldErrors.issueIdList) {
            newErrors.issueIdList = "Invalid issue selection";
          }
          if (fieldErrors.workOrderLineItems) {
            newErrors.workOrderLineItems = "Invalid line items";
          }

          setErrors(newErrors);
          notify(errorMessage, "error");
        },
      });
    } catch (error: any) {
      console.error("Error saving work order:", error);

      const errorMessage = getErrorMessage(
        error,
        "Failed to save work order. Please check your input and try again.",
      );

      const fieldErrors = getErrorFields(error, ["workOrderID", "status"]);

      const newErrors: { [key: string]: string } = {};
      if (fieldErrors.workOrderID) {
        newErrors.workOrderID = "Invalid work order ID";
      }
      if (fieldErrors.status) {
        newErrors.status = "Invalid status";
      }

      setErrors(newErrors);
      notify(errorMessage, "error");
    }
  };

  // Handle loading and error states
  if (isLoading) {
    return <div>Loading work order...</div>;
  }

  if (!workOrder) {
    return <div>Work order not found</div>;
  }

  // Prevent editing completed work orders
  if (workOrder.status === 5) {
    // WorkOrderStatusEnum.COMPLETED
    return (
      <div className="min-h-screen bg-gray-50">
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <p className="text-red-600 mb-2">
              Cannot edit completed work orders
            </p>
            <SecondaryButton
              onClick={() => router.push(`/work-orders/${workOrderId}`)}
              className="mt-4"
            >
              Back to Work Order
            </SecondaryButton>
          </div>
        </div>
      </div>
    );
  }

  const breadcrumbs: BreadcrumbItem[] = [
    { label: "Work Orders", href: "/work-orders" },
    { label: workOrder.title, href: `/work-orders/${workOrderId}` },
  ];

  return (
    <div>
      <WorkOrderHeader
        title={`Edit Work Order - ${workOrder.title}`}
        breadcrumbs={breadcrumbs}
        actions={
          <>
            <SecondaryButton onClick={() => router.back()}>
              Cancel
            </SecondaryButton>
            <PrimaryButton
              onClick={handleSave}
              disabled={isUpdating || isCompleting}
            >
              {isUpdating || isCompleting ? "Saving..." : "Save Changes"}
            </PrimaryButton>
          </>
        }
      />
      <WorkOrderDetailsForm
        key={`details-${resetKey}`}
        value={form}
        errors={errors}
        onChange={handleFormChange}
        disabled={isUpdating || isCompleting}
        showStatus={true}
        statusEditable={true}
      />

      <WorkOrderIssuesForm
        value={{ issueIdList: form.issueIdList }}
        errors={errors}
        onChange={handleFormChange}
        disabled={isUpdating || isCompleting}
        vehicleID={form.vehicleID}
      />

      <WorkOrderLineItemsForm
        key={`lineitems-${resetKey}`}
        value={{ workOrderLineItems: form.workOrderLineItems }}
        errors={errors}
        onChange={handleFormChange}
        disabled={isUpdating || isCompleting}
      />

      {/* Footer Actions */}
      <div className="max-w-4xl mx-auto w-full mt-8 mb-12">
        <hr className="mb-6 border-gray-300" />
        <div className="flex justify-between items-center">
          <SecondaryButton onClick={() => router.back()}>
            Cancel
          </SecondaryButton>
          <div className="flex gap-3">
            <PrimaryButton
              onClick={handleSave}
              disabled={isUpdating || isCompleting}
            >
              {isUpdating || isCompleting ? "Saving..." : "Save Changes"}
            </PrimaryButton>
          </div>
        </div>
      </div>
    </div>
  );
}
