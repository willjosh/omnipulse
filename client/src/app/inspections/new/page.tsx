"use client";

import React, { useState, useEffect, Suspense } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import InspectionHeader from "@/features/inspection/components/InspectionHeader";
import InspectionDetailsForm, {
  InspectionDetailsFormValues,
} from "@/features/inspection/components/InspectionDetailsForm";
import InspectionOdometerForm, {
  InspectionOdometerFormValues,
} from "@/features/inspection/components/InspectionOdometerForm";
import InspectionItemChecklistForm, {
  InspectionItemChecklistFormValues,
} from "@/features/inspection/components/InspectionItemChecklistForm";
import InspectionSignOffForm, {
  InspectionSignOffFormValues,
} from "@/features/inspection/components/InspectionSignOffForm";
import { PrimaryButton, SecondaryButton } from "@/components/ui/Button";
import { useCreateInspection } from "@/features/inspection/hooks/useInspections";
import { useInspectionForm } from "@/features/inspection-form/hooks/useInspectionForms";
import { useInspectionFormItems } from "@/features/inspection-form/hooks/useInspectionFormItems";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import { VehicleConditionEnum } from "@/features/inspection/types/inspectionEnum";
import { CreateInspectionCommand } from "@/features/inspection/types/inspectionType";
import { getErrorMessage, getErrorFields } from "@/utils/fieldErrorUtils";

interface FormErrors {
  details: Partial<Record<keyof InspectionDetailsFormValues, string>>;
  odometer: Partial<Record<keyof InspectionOdometerFormValues, string>>;
  checklist: Partial<Record<string, string>>;
  signOff: Partial<Record<keyof InspectionSignOffFormValues, string>>;
}

const InspectionCreationContent: React.FC = () => {
  const router = useRouter();
  const searchParams = useSearchParams();
  const notify = useNotification();

  // Get inspection form ID from URL query params
  const inspectionFormId = Number(searchParams.get("formId"));

  // Form state
  const [detailsForm, setDetailsForm] = useState<InspectionDetailsFormValues>({
    vehicleID: 0,
    technicianID: "",
  });

  const [odometerForm, setOdometerForm] =
    useState<InspectionOdometerFormValues>({ odometerReading: null });

  const [checklistForm, setChecklistForm] =
    useState<InspectionItemChecklistFormValues>({ items: [] });

  const [signOffForm, setSignOffForm] = useState<InspectionSignOffFormValues>({
    vehicleCondition: VehicleConditionEnum.Excellent,
    notes: null,
  });

  const [errors, setErrors] = useState<FormErrors>({
    details: {},
    odometer: {},
    checklist: {},
    signOff: {},
  });

  // Timer state for inspection start and end times
  const [inspectionStartTime, setInspectionStartTime] = useState<string | null>(
    null,
  );
  const [inspectionEndTime, setInspectionEndTime] = useState<string | null>(
    null,
  );

  // API hooks - only enable when we have a valid form ID
  const {
    inspectionForm,
    isPending: isLoadingForm,
    isError: isFormError,
    error: formError,
  } = useInspectionForm(inspectionFormId);

  const {
    inspectionFormItems,
    isPending: isLoadingItems,
    isError: isItemsError,
    error: itemsError,
  } = useInspectionFormItems(inspectionFormId);

  // Add error handling for missing or invalid form ID
  if (!inspectionFormId || isNaN(inspectionFormId)) {
    return (
      <div className="min-h-screen bg-gray-50">
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <p className="text-gray-600">
              Invalid or missing inspection form ID
            </p>
            <SecondaryButton
              onClick={() => router.push("/inspections")}
              className="mt-4"
            >
              Back to Inspections
            </SecondaryButton>
          </div>
        </div>
      </div>
    );
  }

  const createInspectionMutation = useCreateInspection();

  // Initialize inspection items when form items are loaded
  useEffect(() => {
    if (inspectionFormItems && inspectionFormItems.length > 0) {
      const initialItems = inspectionFormItems.map((item: any) => ({
        id: item.id,
        itemLabel: item.itemLabel,
        itemDescription: item.itemDescription || undefined,
        itemInstructions: item.itemInstructions || undefined,
        isRequired: item.isRequired || false,
        passed: null as boolean | null,
        comment: "",
        showRemarks: false,
      }));

      // Only update if the items are different
      setChecklistForm(prev => {
        const currentItemIds = prev.items.map(item => item.id).sort();
        const newItemIds = initialItems.map(item => item.id).sort();

        if (JSON.stringify(currentItemIds) === JSON.stringify(newItemIds)) {
          return prev; // Don't update if items are the same
        }
        return { ...prev, items: initialItems };
      });
    } else if (inspectionFormItems && inspectionFormItems.length === 0) {
      // Only update if the current items array is not already empty
      setChecklistForm(prev => {
        if (prev.items.length === 0) {
          return prev; // Don't update if already empty
        }
        return { ...prev, items: [] };
      });
    }
  }, [inspectionFormItems]);

  // Set inspection start time when component mounts (when user clicks "Start Inspection")
  useEffect(() => {
    if (!inspectionStartTime) {
      setInspectionStartTime(new Date().toISOString());
    }
  }, [inspectionStartTime]);

  // Validation
  const validateForm = (): boolean => {
    const newErrors: FormErrors = {
      details: {},
      odometer: {},
      checklist: {},
      signOff: {},
    };

    if (!detailsForm.vehicleID) {
      newErrors.details.vehicleID = "Vehicle is required";
    }
    if (!detailsForm.technicianID) {
      newErrors.details.technicianID = "Technician is required";
    }

    // Validate vehicle condition (required)
    if (
      signOffForm.vehicleCondition === undefined ||
      signOffForm.vehicleCondition === null
    ) {
      newErrors.signOff.vehicleCondition = "Vehicle condition is required";
    }

    // Validate required inspection items
    checklistForm.items.forEach(item => {
      if (item.isRequired && item.passed === null) {
        newErrors.checklist[`item-${item.id}`] =
          `${item.itemLabel} is required`;
      }
    });

    setErrors(newErrors);
    return (
      Object.keys(newErrors.details).length === 0 &&
      Object.keys(newErrors.signOff).length === 0 &&
      Object.keys(newErrors.checklist).length === 0
    );
  };

  // Form change handlers
  const handleDetailsChange = (
    field: keyof InspectionDetailsFormValues,
    value: any,
  ) => {
    setDetailsForm(prev => ({ ...prev, [field]: value }));
    if (errors.details[field]) {
      setErrors(prev => ({
        ...prev,
        details: { ...prev.details, [field]: "" },
      }));
    }
  };

  const handleOdometerChange = (
    field: keyof InspectionOdometerFormValues,
    value: any,
  ) => {
    setOdometerForm(prev => ({ ...prev, [field]: value }));
  };

  const handleChecklistChange = (field: string, value: any) => {
    setChecklistForm(prev => ({ ...prev, [field]: value }));
  };

  const handleSignOffChange = (
    field: keyof InspectionSignOffFormValues,
    value: any,
  ) => {
    setSignOffForm(prev => ({ ...prev, [field]: value }));
  };

  // Submit handler
  const handleSubmit = async () => {
    if (!validateForm() || !inspectionFormId) {
      notify("Please fill all required fields", "error");
      return;
    }

    try {
      // Set the inspection end time when submitting
      const endTime = new Date().toISOString();
      setInspectionEndTime(endTime);

      const inspectionItems = checklistForm.items
        .filter(item => item.passed !== null)
        .map(item => ({
          inspectionFormItemID: item.id,
          passed: item.passed!,
          comment: item.comment || null,
        }));

      const command: CreateInspectionCommand = {
        inspectionFormID: inspectionFormId,
        vehicleID: detailsForm.vehicleID,
        technicianID: detailsForm.technicianID,
        inspectionStartTime: inspectionStartTime || new Date().toISOString(),
        inspectionEndTime: endTime,
        odometerReading: odometerForm.odometerReading,
        vehicleCondition: signOffForm.vehicleCondition,
        notes: signOffForm.notes,
        inspectionItems: inspectionItems,
      };

      await createInspectionMutation.mutateAsync(command);
      notify("Inspection created successfully", "success");
      router.push("/inspections");
    } catch (error: any) {
      console.error("Error creating inspection:", error);

      // Get dynamic error message from backend
      const errorMessage = getErrorMessage(
        error,
        "Failed to create inspection. Please check your input and try again.",
      );

      // Map backend errors to form fields
      const fieldErrors = getErrorFields(error, [
        "inspectionFormID",
        "vehicleID",
        "technicianID",
        "inspectionStartTime",
        "inspectionEndTime",
        "odometerReading",
        "vehicleCondition",
        "notes",
        "inspectionItems",
      ]);

      // Set field-specific errors
      const newErrors: FormErrors = {
        details: {},
        odometer: {},
        checklist: {},
        signOff: {},
      };

      // Map backend field names to form sections
      if (fieldErrors.vehicleID) {
        newErrors.details.vehicleID = "Invalid vehicle selection";
      }
      if (fieldErrors.technicianID) {
        newErrors.details.technicianID = "Invalid technician selection";
      }
      if (fieldErrors.odometerReading) {
        newErrors.odometer.odometerReading = "Invalid odometer reading";
      }
      if (fieldErrors.vehicleCondition) {
        newErrors.signOff.vehicleCondition = "Invalid vehicle condition";
      }
      if (fieldErrors.inspectionItems) {
        // Handle inspection items errors - this could be for missing required items
        checklistForm.items.forEach(item => {
          if (item.isRequired && item.passed === null) {
            newErrors.checklist[`item-${item.id}`] =
              `${item.itemLabel} is required`;
          }
        });
      }

      setErrors(newErrors);
      notify(errorMessage, "error");
    }
  };

  const handleCancel = () => {
    router.push("/inspections");
  };

  // Error states
  if (isFormError || isItemsError) {
    return (
      <div className="min-h-screen bg-gray-50">
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <p className="text-red-600 mb-2">Error loading inspection form</p>
            {formError && (
              <p className="text-sm text-gray-500 mb-2">
                Form Error: {formError.message}
              </p>
            )}
            {itemsError && (
              <p className="text-sm text-gray-500 mb-2">
                Items Error: {itemsError.message}
              </p>
            )}
            <SecondaryButton
              onClick={() => router.push("/inspections")}
              className="mt-4"
            >
              Back to Inspections
            </SecondaryButton>
          </div>
        </div>
      </div>
    );
  }

  // Loading states
  if (isLoadingForm || isLoadingItems) {
    return (
      <div className="min-h-screen bg-gray-50">
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
            <p className="mt-2 text-gray-600">
              {isLoadingForm
                ? "Loading inspection form..."
                : "Loading inspection items..."}
            </p>
            <p className="text-sm text-gray-500 mt-2">
              Form ID: {inspectionFormId}
            </p>
          </div>
        </div>
      </div>
    );
  }

  if (!inspectionForm) {
    return (
      <div className="min-h-screen bg-gray-50">
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <p className="text-gray-600">Inspection form not found</p>
            <SecondaryButton
              onClick={() => router.push("/inspections")}
              className="mt-4"
            >
              Back to Inspections
            </SecondaryButton>
          </div>
        </div>
      </div>
    );
  }

  const breadcrumbs = [
    { label: "Inspections", href: "/inspections" },
    { label: "New Inspection", href: "#" },
  ];

  // Calculate inspection duration
  const getInspectionDuration = () => {
    if (!inspectionStartTime) return null;

    const startTime = new Date(inspectionStartTime);
    const endTime = inspectionEndTime
      ? new Date(inspectionEndTime)
      : new Date();
    const durationMs = endTime.getTime() - startTime.getTime();

    const minutes = Math.floor(durationMs / (1000 * 60));
    const seconds = Math.floor((durationMs % (1000 * 60)) / 1000);

    return `${minutes}:${seconds.toString().padStart(2, "0")}`;
  };

  const actions = (
    <div className="flex items-center gap-4">
      {/* Timer display */}
      {inspectionStartTime && (
        <div className="text-sm text-gray-600">
          <span className="font-medium">Duration:</span>{" "}
          {getInspectionDuration()}
        </div>
      )}

      <div className="flex gap-2">
        <SecondaryButton
          onClick={handleCancel}
          disabled={createInspectionMutation.isPending}
        >
          Cancel
        </SecondaryButton>
        <PrimaryButton
          onClick={handleSubmit}
          disabled={createInspectionMutation.isPending}
        >
          {createInspectionMutation.isPending
            ? "Creating..."
            : "Create Inspection"}
        </PrimaryButton>
      </div>
    </div>
  );

  return (
    <div className="min-h-screen bg-gray-50">
      <InspectionHeader
        title={`New Inspection - ${inspectionForm.title}`}
        breadcrumbs={breadcrumbs}
        actions={actions}
      />

      <div className="max-w-4xl mx-auto px-6 py-8 space-y-6">
        <InspectionDetailsForm
          value={detailsForm}
          errors={errors.details}
          onChange={handleDetailsChange}
          disabled={createInspectionMutation.isPending}
        />

        <InspectionOdometerForm
          value={odometerForm}
          errors={errors.odometer}
          onChange={handleOdometerChange}
          disabled={createInspectionMutation.isPending}
        />

        {/* Inspection Items - Only show if there are items */}
        {checklistForm.items.length > 0 && (
          <InspectionItemChecklistForm
            value={checklistForm}
            errors={errors.checklist}
            onChange={handleChecklistChange}
            disabled={createInspectionMutation.isPending}
          />
        )}

        <InspectionSignOffForm
          value={signOffForm}
          errors={errors.signOff}
          onChange={handleSignOffChange}
          disabled={createInspectionMutation.isPending}
        />
      </div>
    </div>
  );
};

const InspectionCreationPage: React.FC = () => {
  return (
    <Suspense
      fallback={
        <div className="min-h-screen bg-gray-50">
          <div className="flex items-center justify-center h-64">
            <div className="text-center">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
              <p className="mt-2 text-gray-600">Loading inspection form...</p>
            </div>
          </div>
        </div>
      }
    >
      <InspectionCreationContent />
    </Suspense>
  );
};

export default InspectionCreationPage;
