"use client";

import React, { useState, useEffect } from "react";
import { useParams, useRouter } from "next/navigation";
import InspectionFormHeader from "@/features/inspection-form/components/InspectionFormHeader";
import InspectionFormItemDetailsForm, {
  InspectionFormItemDetailsFormValues,
} from "@/features/inspection-form/components/InspectionFormItemDetailsForm";
import { PrimaryButton, SecondaryButton } from "@/components/ui/Button";
import {
  useUpdateInspectionFormItem,
  useInspectionFormItem,
} from "@/features/inspection-form/hooks/useInspectionFormItems";
import { useInspectionForm } from "@/features/inspection-form/hooks/useInspectionForms";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import { InspectionFormItemTypeEnum } from "@/features/inspection-form/types/inspectionFormEnum";

const initialForm: InspectionFormItemDetailsFormValues = {
  itemLabel: "",
  itemDescription: "",
  itemInstructions: "",
  inspectionFormItemTypeEnum: InspectionFormItemTypeEnum.PassFail,
  isRequired: false,
};

export default function EditInspectionFormItemPage() {
  const params = useParams();
  const router = useRouter();
  const notify = useNotification();
  const inspectionFormId = Number(params.id);
  const itemId = Number(params.itemId);

  const [form, setForm] =
    useState<InspectionFormItemDetailsFormValues>(initialForm);
  const [errors, setErrors] = useState<
    Partial<Record<keyof InspectionFormItemDetailsFormValues, string>>
  >({});
  const [isSaving, setIsSaving] = useState(false);

  const { mutate: updateInspectionFormItem, isPending } =
    useUpdateInspectionFormItem();
  const { inspectionForm, isPending: isFormLoading } =
    useInspectionForm(inspectionFormId);
  const {
    inspectionFormItem,
    isPending: isItemLoading,
    isError: isItemError,
  } = useInspectionFormItem(inspectionFormId, itemId);

  // Populate form when item data loads
  useEffect(() => {
    if (inspectionFormItem) {
      setForm({
        itemLabel: inspectionFormItem.itemLabel,
        itemDescription: inspectionFormItem.itemDescription || "",
        itemInstructions: inspectionFormItem.itemInstructions || "",
        inspectionFormItemTypeEnum:
          inspectionFormItem.inspectionFormItemTypeEnum,
        isRequired: inspectionFormItem.isRequired,
      });
    }
  }, [inspectionFormItem]);

  const breadcrumbs = [
    { label: "Inspection Forms", href: "/inspection-forms" },
    {
      label: inspectionForm?.title || "...",
      href: `/inspection-forms/${inspectionFormId}`,
    },
  ];

  const validate = () => {
    const newErrors: typeof errors = {};
    if (!form.itemLabel.trim()) {
      newErrors.itemLabel = "Item label is required.";
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (
    field: keyof InspectionFormItemDetailsFormValues,
    value: any,
  ) => {
    setForm(f => ({ ...f, [field]: value }));
    setErrors(e => ({ ...e, [field]: undefined }));
  };

  const handleCancel = () => {
    router.push(`/inspection-forms/${inspectionFormId}`);
  };

  const handleSave = async () => {
    if (!validate()) return;
    setIsSaving(true);

    updateInspectionFormItem(
      {
        inspectionFormId,
        command: {
          inspectionFormItemID: itemId,
          itemLabel: form.itemLabel.trim(),
          itemDescription: form.itemDescription?.trim() || null,
          itemInstructions: form.itemInstructions?.trim() || null,
          isRequired: form.isRequired,
        },
      },
      {
        onSuccess: () => {
          setIsSaving(false);
          notify("Inspection item updated successfully!", "success");
          router.push(`/inspection-forms/${inspectionFormId}`);
        },
        onError: error => {
          setIsSaving(false);
          console.error("Failed to update inspection item:", error);
          notify("Failed to update inspection item", "error");
        },
      },
    );
  };

  if (isFormLoading || isItemLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (!inspectionForm) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <h2 className="text-2xl font-bold text-gray-900 mb-4">
            Form Not Found
          </h2>
          <p className="text-gray-600 mb-6">
            The inspection form you are looking for does not exist.
          </p>
          <PrimaryButton onClick={() => router.push("/inspection-forms")}>
            Back to Forms
          </PrimaryButton>
        </div>
      </div>
    );
  }

  if (isItemError || !inspectionFormItem) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <h2 className="text-2xl font-bold text-gray-900 mb-4">
            Item Not Found
          </h2>
          <p className="text-gray-600 mb-6">
            The inspection form item you are looking for does not exist.
          </p>
          <PrimaryButton
            onClick={() => router.push(`/inspection-forms/${inspectionFormId}`)}
          >
            Back to Form
          </PrimaryButton>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <InspectionFormHeader
        title={`Edit Item: ${inspectionFormItem.itemLabel}`}
        breadcrumbs={breadcrumbs}
        actions={
          <>
            <SecondaryButton
              onClick={handleCancel}
              disabled={isSaving || isPending}
            >
              Cancel
            </SecondaryButton>
            <PrimaryButton
              onClick={handleSave}
              disabled={isSaving || isPending}
            >
              {isSaving || isPending ? "Updating..." : "Update"}
            </PrimaryButton>
          </>
        }
      />

      <div className="px-6 pb-12 mt-4 max-w-2xl mx-auto">
        <InspectionFormItemDetailsForm
          value={form}
          errors={errors}
          onChange={handleChange}
          disabled={isSaving || isPending}
        />
      </div>

      <div className="max-w-2xl mx-auto w-full mb-12">
        <hr className="mb-6 border-gray-300" />
        <div className="flex justify-between items-center">
          <SecondaryButton
            onClick={handleCancel}
            disabled={isSaving || isPending}
          >
            Cancel
          </SecondaryButton>
          <PrimaryButton onClick={handleSave} disabled={isSaving || isPending}>
            {isSaving || isPending ? "Updating..." : "Update Item"}
          </PrimaryButton>
        </div>
      </div>
    </div>
  );
}
