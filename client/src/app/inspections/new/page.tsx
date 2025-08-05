"use client";

import React, { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { useParams } from "next/navigation";
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

interface FormErrors {
  details: Partial<Record<keyof InspectionDetailsFormValues, string>>;
  odometer: Partial<Record<keyof InspectionOdometerFormValues, string>>;
  checklist: Partial<Record<string, string>>;
  signOff: Partial<Record<keyof InspectionSignOffFormValues, string>>;
}

const InspectionCreationPage: React.FC = () => {
  const router = useRouter();
  const params = useParams();
  const notify = useNotification();

  // Get inspection form ID from URL params
  const inspectionFormId = params?.formId
    ? parseInt(params.formId as string)
    : null;

  // Form state
  const [detailsForm, setDetailsForm] = useState<InspectionDetailsFormValues>({
    vehicleID: 0,
    technicianID: "",
  });

  const [odometerForm, setOdometerForm] =
    useState<InspectionOdometerFormValues>({
      odometerReading: 0,
      voidOdometer: false,
      photoFile: null,
    });

  const [checklistForm, setChecklistForm] =
    useState<InspectionItemChecklistFormValues>({ items: [] });

  const [signOffForm, setSignOffForm] = useState<InspectionSignOffFormValues>({
    vehicleConditionOK: false,
    vehicleConditionRemarks: "",
    showVehicleConditionRemarks: false,
    driverSignature: "",
    signatureRemarks: "",
    showSignatureRemarks: false,
  });

  const [errors, setErrors] = useState<FormErrors>({
    details: {},
    odometer: {},
    checklist: {},
    signOff: {},
  });

  // API hooks
  const { inspectionForm, isPending: isLoadingForm } = useInspectionForm(
    inspectionFormId || 0,
  );

  const { inspectionFormItems, isPending: isLoadingItems } =
    useInspectionFormItems(inspectionFormId || 0);

  const createInspectionMutation = useCreateInspection();

  // Initialize checklist items when form items are loaded
  useEffect(() => {
    if (inspectionFormItems && inspectionFormItems.length > 0) {
      const initialItems = inspectionFormItems.map((item: any) => ({
        id: item.id,
        itemLabel: item.itemLabel,
        itemDescription: item.itemDescription || undefined,
        itemInstructions: item.itemInstructions || undefined,
        passed: null as boolean | null,
        comment: "",
        showRemarks: false,
      }));

      setChecklistForm(prev => ({ ...prev, items: initialItems }));
    }
  }, [inspectionFormItems]);

  // Form change handlers
  const handleDetailsChange = (
    field: keyof InspectionDetailsFormValues,
    value: any,
  ) => {
    setDetailsForm(prev => ({ ...prev, [field]: value }));
    // Clear error when user starts typing
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
    if (errors.odometer[field]) {
      setErrors(prev => ({
        ...prev,
        odometer: { ...prev.odometer, [field]: "" },
      }));
    }
  };

  const handleChecklistChange = (field: string, value: any) => {
    setChecklistForm(prev => ({ ...prev, [field]: value }));
  };

  const handleSignOffChange = (
    field: keyof InspectionSignOffFormValues,
    value: any,
  ) => {
    setSignOffForm(prev => ({ ...prev, [field]: value }));
    if (errors.signOff[field]) {
      setErrors(prev => ({
        ...prev,
        signOff: { ...prev.signOff, [field]: "" },
      }));
    }
  };

  // Validation
  const validateForm = (): boolean => {
    const newErrors: FormErrors = {
      details: {},
      odometer: {},
      checklist: {},
      signOff: {},
    };

    // Details validation
    if (!detailsForm.vehicleID) {
      newErrors.details.vehicleID = "Vehicle is required";
    }
    if (!detailsForm.technicianID) {
      newErrors.details.technicianID = "Technician is required";
    }

    // Odometer validation
    if (!odometerForm.odometerReading && !odometerForm.voidOdometer) {
      newErrors.odometer.odometerReading =
        "Odometer reading is required unless voided";
    }

    // Checklist validation
    const uncompletedItems = checklistForm.items.filter(
      item => item.passed === null,
    );
    if (uncompletedItems.length > 0) {
      newErrors.checklist.items = `Please complete all inspection items (${uncompletedItems.length} remaining)`;
    }

    // Sign-off validation
    if (!signOffForm.vehicleConditionOK) {
      newErrors.signOff.vehicleConditionOK =
        "Vehicle condition must be confirmed";
    }
    if (!signOffForm.driverSignature) {
      newErrors.signOff.driverSignature = "Driver signature is required";
    }

    setErrors(newErrors);
    return (
      Object.keys(newErrors.details).length === 0 &&
      Object.keys(newErrors.odometer).length === 0 &&
      Object.keys(newErrors.checklist).length === 0 &&
      Object.keys(newErrors.signOff).length === 0
    );
  };

  // Submit handler
  const handleSubmit = async () => {
    if (!validateForm() || !inspectionFormId) {
      notify("Please fix all validation errors", "error");
      return;
    }

    try {
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
        inspectionStartTime: new Date().toISOString(),
        inspectionEndTime: new Date().toISOString(),
        odometerReading: odometerForm.voidOdometer
          ? null
          : odometerForm.odometerReading,
        vehicleCondition: signOffForm.vehicleConditionOK
          ? VehicleConditionEnum.Excellent
          : VehicleConditionEnum.NotSafeToOperate,
        notes:
          signOffForm.vehicleConditionRemarks ||
          signOffForm.signatureRemarks ||
          null,
        inspectionItems,
      };

      await createInspectionMutation.mutateAsync(command);
      notify("Inspection created successfully", "success");
      router.push("/inspections");
    } catch (error) {
      console.error("Error creating inspection:", error);
      notify("Failed to create inspection", "error");
    }
  };

  const handleCancel = () => {
    router.push("/inspections");
  };

  // Loading states
  if (isLoadingForm || isLoadingItems) {
    return (
      <div className="min-h-screen bg-gray-50">
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
            <p className="mt-2 text-gray-600">Loading inspection form...</p>
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

  const actions = (
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

        <InspectionItemChecklistForm
          value={checklistForm}
          errors={errors.checklist}
          onChange={handleChecklistChange}
          disabled={createInspectionMutation.isPending}
        />

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

export default InspectionCreationPage;
