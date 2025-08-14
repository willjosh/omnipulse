"use client";
import React, { useState, useEffect } from "react";
import { useParams, useRouter } from "next/navigation";
import ServiceProgramHeader from "@/features/service-program/components/ServiceProgramHeader";
import ServiceProgramDetailsForm, {
  ServiceProgramDetailsFormValues,
} from "@/features/service-program/components/ServiceProgramDetailsForm";
import { PrimaryButton, SecondaryButton } from "@/components/ui/Button";
import { Loading } from "@/components/ui/Feedback";
import { EmptyState } from "@/components/ui/Feedback";
import {
  useServiceProgram,
  useUpdateServiceProgram,
} from "@/features/service-program/hooks/useServicePrograms";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import { getErrorMessage, getErrorFields } from "@/utils/fieldErrorUtils";

export default function EditServiceProgramPage() {
  const params = useParams();
  const router = useRouter();
  const id = params.id ? Number(params.id) : undefined;
  const notify = useNotification();

  const [form, setForm] = useState<ServiceProgramDetailsFormValues>({
    name: "",
    description: "",
    isActive: true,
  });
  const [errors, setErrors] = useState<
    Partial<Record<keyof ServiceProgramDetailsFormValues, string>>
  >({});
  const [isSaving, setIsSaving] = useState(false);

  const {
    serviceProgram,
    isPending: isLoadingProgram,
    isError: isProgramError,
  } = useServiceProgram(id!);
  const { mutate: updateServiceProgram, isPending: isUpdating } =
    useUpdateServiceProgram();

  // Populate form when service program data is loaded
  useEffect(() => {
    if (serviceProgram) {
      setForm({
        name: serviceProgram.name,
        description: serviceProgram.description || "",
        isActive: serviceProgram.isActive,
      });
    }
  }, [serviceProgram]);

  const breadcrumbs = [
    { label: "Service Programs", href: "/service-programs" },
    {
      label: serviceProgram?.name || "Service Program",
      href: `/service-programs/${id}`,
    },
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
    router.push(`/service-programs/${id}`);
  };

  const handleSave = async () => {
    if (!validate() || !id || !serviceProgram) {
      notify("Please fill all required fields", "error");
      return;
    }
    setIsSaving(true);

    updateServiceProgram(
      {
        serviceProgramID: id,
        name: form.name,
        description: form.description,
        isActive: form.isActive ?? true,
      },
      {
        onSuccess: () => {
          setIsSaving(false);
          notify("Service Program updated successfully!", "success");
          router.push(`/service-programs/${id}`);
        },
        onError: (error: any) => {
          setIsSaving(false);
          console.error("Failed to update service program:", error);

          const errorMessage = getErrorMessage(
            error,
            "Failed to update service program. Please check your input and try again.",
          );

          const fieldErrors = getErrorFields(error, [
            "name",
            "description",
            "isActive",
          ]);

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

  if (isLoadingProgram) {
    return <Loading />;
  }

  if (isProgramError || !serviceProgram) {
    return (
      <EmptyState
        title="Service Program not found"
        message="The service program you are trying to edit does not exist or could not be loaded."
      />
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <ServiceProgramHeader
        title="Edit Service Program"
        breadcrumbs={breadcrumbs}
        actions={
          <>
            <SecondaryButton
              onClick={handleCancel}
              disabled={isSaving || isUpdating}
            >
              Cancel
            </SecondaryButton>
            <PrimaryButton
              onClick={handleSave}
              disabled={isSaving || isUpdating}
            >
              {isSaving || isUpdating ? "Saving..." : "Save"}
            </PrimaryButton>
          </>
        }
      />
      <div className="px-6 pb-12 mt-4 max-w-2xl mx-auto">
        <ServiceProgramDetailsForm
          value={form}
          errors={errors}
          onChange={handleChange}
          disabled={isSaving || isUpdating}
          showIsActive={false}
        />
      </div>
      {/* Footer Actions */}
      <div className="max-w-2xl mx-auto w-full mb-12">
        <hr className="mb-6 border-gray-300" />
        <div className="flex justify-between items-center">
          <SecondaryButton
            onClick={handleCancel}
            disabled={isSaving || isUpdating}
          >
            Cancel
          </SecondaryButton>
          <PrimaryButton onClick={handleSave} disabled={isSaving || isUpdating}>
            {isSaving || isUpdating ? "Saving..." : "Save"}
          </PrimaryButton>
        </div>
      </div>
    </div>
  );
}
