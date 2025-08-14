"use client";

import React, { useState } from "react";
import { useRouter } from "next/navigation";
import WorkOrderHeader from "@/features/work-order/components/WorkOrderHeader";
import WorkOrderDetailsForm from "@/features/work-order/components/WorkOrderDetailsForm";
import WorkOrderIssuesForm from "@/features/work-order/components/WorkOrderIssuesForm";
import WorkOrderLineItemsForm from "@/features/work-order/components/WorkOrderLineItemsForm";
import SecondaryButton from "@/components/ui/Button/SecondaryButton";
import PrimaryButton from "@/components/ui/Button/PrimaryButton";
import { useCreateWorkOrder } from "@/features/work-order/hooks/useWorkOrders";
import { CreateWorkOrderLineItem } from "@/features/work-order/types/workOrderType";
import {
  WorkOrderFormState,
  validateWorkOrderForm,
  mapFormToCreateWorkOrderCommand,
  emptyWorkOrderFormState,
} from "@/features/work-order/utils/workOrderFormUtils";
import { BreadcrumbItem } from "@/components/ui/Layout/Breadcrumbs";
import { getErrorMessage, getErrorFields } from "@/utils/fieldErrorUtils";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";

export default function NewWorkOrderPage() {
  const router = useRouter();
  const notify = useNotification();

  // Form state
  const [form, setForm] = useState<WorkOrderFormState>({
    ...emptyWorkOrderFormState,
  });
  const [errors, setErrors] = useState<{ [key: string]: string }>({});
  const [resetKey, setResetKey] = useState(0);

  // Create work order mutation
  const { mutate: createWorkOrder, isPending } = useCreateWorkOrder();

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
  const toCreateWorkOrderCommand = () => mapFormToCreateWorkOrderCommand(form);

  // Save Work Order handler
  const handleSave = () => {
    if (!validate()) {
      notify("Please fill all required fields", "error");
      return;
    }
    createWorkOrder(toCreateWorkOrderCommand(), {
      onSuccess: () => {
        router.push("/work-orders");
      },
      onError: (error: any) => {
        console.error("Failed to create work order:", error);

        const errorMessage = getErrorMessage(
          error,
          "Failed to create work order. Please check your input and try again.",
        );

        const fieldErrors = getErrorFields(error, [
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
  };

  // Save & Add Another handler
  const handleSaveAndAddAnother = () => {
    if (!validate()) {
      notify("Please fill all required fields", "error");
      return;
    }
    createWorkOrder(toCreateWorkOrderCommand(), {
      onSuccess: () => {
        setForm(emptyWorkOrderFormState);
        setErrors({});
        setResetKey(k => k + 1);
      },
      onError: (error: any) => {
        console.error("Failed to create work order:", error);

        const errorMessage = getErrorMessage(
          error,
          "Failed to create work order. Please check your input and try again.",
        );

        const fieldErrors = getErrorFields(error, [
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
  };

  const breadcrumbs: BreadcrumbItem[] = [
    { label: "Work Orders", href: "/work-orders" },
  ];

  return (
    <div>
      <WorkOrderHeader
        title="New Work Order"
        breadcrumbs={breadcrumbs}
        actions={
          <>
            <SecondaryButton onClick={() => router.back()}>
              Cancel
            </SecondaryButton>
            <PrimaryButton onClick={handleSave} disabled={isPending}>
              {isPending ? "Saving..." : "Save Work Order"}
            </PrimaryButton>
          </>
        }
      />
      <WorkOrderDetailsForm
        key={`details-${resetKey}`}
        value={form}
        errors={errors}
        onChange={handleFormChange}
        disabled={isPending}
        showStatus={false}
        statusEditable={false}
      />

      <WorkOrderIssuesForm
        value={{ issueIdList: form.issueIdList }}
        errors={errors}
        onChange={handleFormChange}
        disabled={isPending}
        vehicleID={form.vehicleID}
      />

      <WorkOrderLineItemsForm
        key={`lineitems-${resetKey}`}
        value={{ workOrderLineItems: form.workOrderLineItems }}
        errors={errors}
        onChange={handleFormChange}
        disabled={isPending}
      />

      {/* Footer Actions */}
      <div className="max-w-4xl mx-auto w-full mt-8 mb-12">
        <hr className="mb-6 border-gray-300" />
        <div className="flex justify-between items-center">
          <SecondaryButton onClick={() => router.back()}>
            Cancel
          </SecondaryButton>
          <div className="flex gap-3">
            <SecondaryButton
              onClick={handleSaveAndAddAnother}
              disabled={isPending}
            >
              {isPending ? "Saving..." : "Save & Add Another"}
            </SecondaryButton>
            <PrimaryButton onClick={handleSave} disabled={isPending}>
              {isPending ? "Saving..." : "Save Work Order"}
            </PrimaryButton>
          </div>
        </div>
      </div>
    </div>
  );
}
