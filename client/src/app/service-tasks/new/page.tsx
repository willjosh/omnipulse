"use client";
import React, { useState } from "react";
import { useRouter } from "next/navigation";
import ServiceTaskHeader from "@/features/service-task/components/ServiceTaskHeader";
import ServiceTaskDetailsForm, {
  ServiceTaskDetailsFormValues,
} from "@/features/service-task/components/ServiceTaskDetailsForm";
import { PrimaryButton, SecondaryButton } from "@/components/ui/Button";
import { useCreateServiceTask } from "@/features/service-task/hooks/useServiceTasks";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import { getErrorMessage, getErrorFields } from "@/utils/fieldErrorUtils";

const initialForm: ServiceTaskDetailsFormValues = {
  name: "",
  description: "",
  estimatedLabourHours: "",
  estimatedCost: "",
  category: "",
  isActive: true,
};

export default function CreateServiceTaskPage() {
  const router = useRouter();
  const [form, setForm] = useState<ServiceTaskDetailsFormValues>(initialForm);
  const [errors, setErrors] = useState<
    Partial<Record<keyof ServiceTaskDetailsFormValues, string>>
  >({});
  const [isSaving, setIsSaving] = useState(false);
  const [resetKey, setResetKey] = useState(0);

  const { mutate: createServiceTask, isPending } = useCreateServiceTask();
  const notify = useNotification();

  const breadcrumbs = [{ label: "Service Tasks", href: "/service-tasks" }];

  const validate = () => {
    const newErrors: typeof errors = {};
    if (!form.name.trim()) newErrors.name = "Name is required.";
    if (!form.estimatedLabourHours || isNaN(Number(form.estimatedLabourHours)))
      newErrors.estimatedLabourHours = "Estimated Labour Hours is required.";
    if (!form.estimatedCost || isNaN(Number(form.estimatedCost)))
      newErrors.estimatedCost = "Estimated Cost is required.";
    if (!form.category) newErrors.category = "Category is required.";
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (
    field: keyof ServiceTaskDetailsFormValues,
    value: any,
  ) => {
    setForm(f => ({ ...f, [field]: value }));
    setErrors(e => ({ ...e, [field]: undefined }));
  };

  const handleCancel = () => {
    router.push("/service-tasks");
  };

  const handleSave = async () => {
    if (!validate()) {
      notify("Please fill all required fields", "error");
      return;
    }
    setIsSaving(true);
    createServiceTask(
      {
        name: form.name,
        description: form.description,
        estimatedLabourHours: Number(form.estimatedLabourHours),
        estimatedCost: Number(form.estimatedCost),
        category: Number(form.category),
        isActive: form.isActive,
      },
      {
        onSuccess: () => {
          setIsSaving(false);
          notify("Service Task created successfully!", "success");
          router.push("/service-tasks");
        },
        onError: (error: any) => {
          setIsSaving(false);
          console.error("Failed to create service task:", error);

          // Get dynamic error message from backend
          const errorMessage = getErrorMessage(
            error,
            "Failed to create service task. Please check your input and try again.",
          );

          // Map backend errors to form fields
          const fieldErrors = getErrorFields(error, [
            "name",
            "description",
            "estimatedLabourHours",
            "estimatedCost",
            "category",
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
          if (fieldErrors.estimatedLabourHours) {
            newErrors.estimatedLabourHours = "Invalid estimated labour hours";
          }
          if (fieldErrors.estimatedCost) {
            newErrors.estimatedCost = "Invalid estimated cost";
          }
          if (fieldErrors.category) {
            newErrors.category = "Invalid category";
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
    createServiceTask(
      {
        name: form.name,
        description: form.description,
        estimatedLabourHours: Number(form.estimatedLabourHours),
        estimatedCost: Number(form.estimatedCost),
        category: Number(form.category),
        isActive: form.isActive,
      },
      {
        onSuccess: () => {
          setIsSaving(false);
          notify("Service Task created successfully!", "success");
          setForm(initialForm);
          setResetKey(k => k + 1);
        },
        onError: (error: any) => {
          setIsSaving(false);
          console.error("Failed to create service task:", error);

          // Get dynamic error message from backend
          const errorMessage = getErrorMessage(
            error,
            "Failed to create service task. Please check your input and try again.",
          );

          // Map backend errors to form fields
          const fieldErrors = getErrorFields(error, [
            "name",
            "description",
            "estimatedLabourHours",
            "estimatedCost",
            "category",
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
          if (fieldErrors.estimatedLabourHours) {
            newErrors.estimatedLabourHours = "Invalid estimated labour hours";
          }
          if (fieldErrors.estimatedCost) {
            newErrors.estimatedCost = "Invalid estimated cost";
          }
          if (fieldErrors.category) {
            newErrors.category = "Invalid category";
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
      <ServiceTaskHeader
        title="New Service Task"
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
        <ServiceTaskDetailsForm
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
              {isSaving || isPending ? "Saving..." : "Save Service Task"}
            </PrimaryButton>
          </div>
        </div>
      </div>
    </div>
  );
}
