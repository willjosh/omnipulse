"use client";
import React, { useState } from "react";
import { useRouter } from "next/navigation";
import ServiceProgramHeader from "@/features/service-program/components/ServiceProgramHeader";
import ServiceProgramDetailsForm, {
  ServiceProgramDetailsFormValues,
} from "@/features/service-program/components/ServiceProgramDetailsForm";
import { PrimaryButton, SecondaryButton } from "@/components/ui/Button";
import { useCreateServiceProgram } from "@/features/service-program/hooks/useServicePrograms";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import { getErrorMessage, getErrorFields } from "@/utils/fieldErrorUtils";

const initialForm: ServiceProgramDetailsFormValues = {
  name: "",
  description: "",
};

export default function CreateServiceProgramPage() {
  const router = useRouter();
  const notify = useNotification();
  const [form, setForm] =
    useState<ServiceProgramDetailsFormValues>(initialForm);
  const [errors, setErrors] = useState<
    Partial<Record<keyof ServiceProgramDetailsFormValues, string>>
  >({});
  const [isSaving, setIsSaving] = useState(false);
  const { mutate: createServiceProgram, isPending } = useCreateServiceProgram();

  const breadcrumbs = [
    { label: "Service Programs", href: "/service-programs" },
  ];

  const validate = () => {
    const newErrors: typeof errors = {};
    if (!form.name.trim()) newErrors.name = "Name is required.";
    if (form.name.length > 200)
      newErrors.name = "Name must not exceed 200 characters.";
    if (form.description && form.description.length > 500)
      newErrors.description = "Description must not exceed 500 characters.";
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (
    field: keyof ServiceProgramDetailsFormValues,
    value: any,
  ) => {
    setForm(f => ({ ...f, [field]: value }));
    setErrors(e => ({ ...e, [field]: undefined }));
  };

  const handleCancel = () => {
    router.push("/service-programs");
  };

  const handleSave = async () => {
    if (!validate()) {
      notify("Please fill all required fields", "error");
      return;
    }
    setIsSaving(true);
    createServiceProgram(
      { name: form.name, description: form.description, isActive: true },
      {
        onSuccess: () => {
          setIsSaving(false);
          notify("Service Program created successfully!", "success");
          router.push("/service-programs");
        },
        onError: (error: any) => {
          setIsSaving(false);
          console.error("Failed to create service program:", error);

          // Get dynamic error message from backend
          const errorMessage = getErrorMessage(
            error,
            "Failed to create service program. Please check your input and try again.",
          );

          // Map backend errors to form fields
          const fieldErrors = getErrorFields(error, [
            "name",
            "description",
            "isActive",
          ]);

          // Set field-specific errors
          const newErrors: typeof errors = {};
          if (fieldErrors.name) {
            newErrors.name = "Invalid name";
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
      <ServiceProgramHeader
        title="New Service Program"
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
        <ServiceProgramDetailsForm
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
            {isSaving || isPending ? "Saving..." : "Save"}
          </PrimaryButton>
        </div>
      </div>
    </div>
  );
}
