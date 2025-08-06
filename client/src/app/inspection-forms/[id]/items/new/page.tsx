"use client";

import React, { useState } from "react";
import { useParams, useRouter } from "next/navigation";
import InspectionFormHeader from "@/features/inspection-form/components/InspectionFormHeader";
import InspectionFormItemDetailsForm, {
  InspectionFormItemDetailsFormValues,
} from "@/features/inspection-form/components/InspectionFormItemDetailsForm";
import { PrimaryButton, SecondaryButton } from "@/components/ui/Button";
import { useCreateInspectionFormItem } from "@/features/inspection-form/hooks/useInspectionFormItems";
import { useInspectionForm } from "@/features/inspection-form/hooks/useInspectionForms";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import { InspectionFormItemTypeEnum } from "@/features/inspection-form/types/inspectionFormEnum";
import Loading from "@/components/ui/Feedback/Loading";

const initialForm: InspectionFormItemDetailsFormValues = {
  itemLabel: "",
  itemDescription: "",
  itemInstructions: "",
  inspectionFormItemTypeEnum: InspectionFormItemTypeEnum.PassFail,
  isRequired: false,
};

export default function AddInspectionItemPage() {
  const params = useParams();
  const router = useRouter();
  const notify = useNotification();
  const inspectionFormId = Number(params.id);

  const [form, setForm] =
    useState<InspectionFormItemDetailsFormValues>(initialForm);
  const [errors, setErrors] = useState<
    Partial<Record<keyof InspectionFormItemDetailsFormValues, string>>
  >({});
  const [isSaving, setIsSaving] = useState(false);
  const [resetKey, setResetKey] = useState(0);

  const { mutate: createInspectionFormItem, isPending } =
    useCreateInspectionFormItem();
  const { inspectionForm, isPending: isFormLoading } =
    useInspectionForm(inspectionFormId);

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

    createInspectionFormItem(
      {
        inspectionFormID: inspectionFormId,
        itemLabel: form.itemLabel.trim(),
        itemDescription: form.itemDescription?.trim() || null,
        itemInstructions: form.itemInstructions?.trim() || null,
        inspectionFormItemTypeEnum: form.inspectionFormItemTypeEnum,
        isRequired: form.isRequired,
      },
      {
        onSuccess: () => {
          setIsSaving(false);
          notify("Inspection item created successfully!", "success");
          router.push(`/inspection-forms/${inspectionFormId}`);
        },
        onError: error => {
          setIsSaving(false);
          console.error("Failed to create inspection item:", error);
          notify("Failed to create inspection item", "error");
        },
      },
    );
  };

  const handleSaveAndAddAnother = async () => {
    if (!validate()) return;
    setIsSaving(true);

    createInspectionFormItem(
      {
        inspectionFormID: inspectionFormId,
        itemLabel: form.itemLabel.trim(),
        itemDescription: form.itemDescription?.trim() || null,
        itemInstructions: form.itemInstructions?.trim() || null,
        inspectionFormItemTypeEnum: form.inspectionFormItemTypeEnum,
        isRequired: form.isRequired,
      },
      {
        onSuccess: () => {
          setIsSaving(false);
          notify("Inspection item created successfully!", "success");
          setForm(initialForm);
          setResetKey(k => k + 1);
        },
        onError: error => {
          setIsSaving(false);
          console.error("Failed to create inspection item:", error);
          notify("Failed to create inspection item", "error");
        },
      },
    );
  };

  if (isFormLoading) {
    return <Loading />;
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

  return (
    <div className="min-h-screen bg-gray-50">
      <InspectionFormHeader
        title="Add Inspection Item"
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
              {isSaving || isPending ? "Saving..." : "Save"}
            </PrimaryButton>
          </>
        }
      />

      <div className="px-6 pb-12 mt-4 max-w-2xl mx-auto">
        <InspectionFormItemDetailsForm
          key={resetKey}
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
          <div className="flex gap-3">
            <SecondaryButton
              onClick={handleSaveAndAddAnother}
              disabled={isSaving || isPending}
            >
              {isSaving || isPending ? "Saving..." : "Save & Add Another"}
            </SecondaryButton>
            <PrimaryButton
              onClick={handleSave}
              disabled={isSaving || isPending}
            >
              {isSaving || isPending ? "Saving..." : "Save Item"}
            </PrimaryButton>
          </div>
        </div>
      </div>
    </div>
  );
}
