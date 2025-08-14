"use client";
import React, { useState } from "react";
import { useRouter } from "next/navigation";
import InspectionFormHeader from "@/features/inspection-form/components/InspectionFormHeader";
import InspectionFormDetailsForm, {
  InspectionFormDetailsFormValues,
} from "@/features/inspection-form/components/InspectionFormDetailsForm";
import { PrimaryButton, SecondaryButton } from "@/components/ui/Button";
import { useCreateInspectionForm } from "@/features/inspection-form/hooks/useInspectionForms";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import { getErrorMessage, getErrorFields } from "@/utils/fieldErrorUtils";

const initialForm: InspectionFormDetailsFormValues = {
  title: "",
  description: "",
  isActive: true,
};

export default function CreateInspectionFormPage() {
  const router = useRouter();

  const [form, setForm] =
    useState<InspectionFormDetailsFormValues>(initialForm);
  const [errors, setErrors] = useState<
    Partial<Record<keyof InspectionFormDetailsFormValues, string>>
  >({});
  const [isSaving, setIsSaving] = useState(false);
  const [resetKey, setResetKey] = useState(0);

  const { mutate: createInspectionForm, isPending } = useCreateInspectionForm();
  const notify = useNotification();

  const breadcrumbs = [
    { label: "Inspection Forms", href: "/inspection-forms" },
  ];

  const validate = () => {
    const newErrors: typeof errors = {};
    if (!form.title.trim()) newErrors.title = "Title is required.";
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (
    field: keyof InspectionFormDetailsFormValues,
    value: any,
  ) => {
    setForm(f => ({ ...f, [field]: value }));
    setErrors(e => ({ ...e, [field]: undefined }));
  };

  const handleCancel = () => {
    router.push("/inspection-forms");
  };

  const handleSave = async () => {
    if (!validate()) {
      notify("Please fill all required fields", "error");
      return;
    }
    setIsSaving(true);
    createInspectionForm(
      {
        title: form.title,
        description: form.description,
        isActive: form.isActive,
      },
      {
        onSuccess: () => {
          setIsSaving(false);
          notify("Inspection form created successfully!", "success");
          router.push("/inspection-forms");
        },
        onError: (error: any) => {
          setIsSaving(false);
          console.error("Failed to create inspection form:", error);

          // Get dynamic error message from backend
          const errorMessage = getErrorMessage(
            error,
            "Failed to create inspection form. Please check your input and try again.",
          );

          // Map backend errors to form fields
          const fieldErrors = getErrorFields(error, [
            "title",
            "description",
            "isActive",
          ]);

          // Set field-specific errors
          const newErrors: typeof errors = {};
          if (fieldErrors.title) {
            newErrors.title = "Invalid title";
          }
          if (fieldErrors.description) {
            newErrors.description = "Invalid description";
          }
          if (fieldErrors.isActive) {
            newErrors.isActive = "Invalid active status";
          }

          setErrors(newErrors);
          notify(errorMessage, "error");
        },
      },
    );
  };

  const handleSaveAndAddAnother = async () => {
    if (!validate()) {
      notify("Please fill all required fields", "error");
      return;
    }
    setIsSaving(true);
    createInspectionForm(
      {
        title: form.title,
        description: form.description,
        isActive: form.isActive,
      },
      {
        onSuccess: () => {
          setIsSaving(false);
          notify("Inspection form created successfully!", "success");
          setForm(initialForm);
          setResetKey(k => k + 1);
        },
        onError: (error: any) => {
          setIsSaving(false);
          console.error("Failed to create inspection form:", error);

          // Get dynamic error message from backend
          const errorMessage = getErrorMessage(
            error,
            "Failed to create inspection form. Please check your input and try again.",
          );

          // Map backend errors to form fields
          const fieldErrors = getErrorFields(error, [
            "title",
            "description",
            "isActive",
          ]);

          // Set field-specific errors
          const newErrors: typeof errors = {};
          if (fieldErrors.title) {
            newErrors.title = "Invalid title";
          }
          if (fieldErrors.description) {
            newErrors.description = "Invalid description";
          }
          if (fieldErrors.isActive) {
            newErrors.isActive = "Invalid active status";
          }

          setErrors(newErrors);
          notify(errorMessage, "error");
        },
      },
    );
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <InspectionFormHeader
        title="New Inspection Form"
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
        <InspectionFormDetailsForm
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
              {isSaving || isPending ? "Saving..." : "Save Inspection Form"}
            </PrimaryButton>
          </div>
        </div>
      </div>
    </div>
  );
}
