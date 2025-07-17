"use client";
import React, { useState } from "react";
import { useRouter } from "next/navigation";
import ServiceTaskHeader from "@/app/_features/service-task/components/ServiceTaskHeader";
import ServiceTaskDetailsForm, {
  ServiceTaskDetailsFormValues,
} from "@/app/_features/service-task/components/ServiceTaskDetailsForm";
import { PrimaryButton, SecondaryButton } from "@/app/_features/shared/button";
import { useCreateServiceTask } from "@/app/_hooks/service-task/useServiceTask";
import { useNotification } from "@/app/_features/shared/feedback/NotificationProvider";

const initialForm: ServiceTaskDetailsFormValues = {
  Name: "",
  Description: "",
  EstimatedLabourHours: "",
  EstimatedCost: "",
  Category: "",
  IsActive: true,
};

export default function CreateServiceTaskPage() {
  const router = useRouter();
  const [form, setForm] = useState<ServiceTaskDetailsFormValues>(initialForm);
  const [errors, setErrors] = useState<
    Partial<Record<keyof ServiceTaskDetailsFormValues, string>>
  >({});
  const [isSaving, setIsSaving] = useState(false);
  const [resetKey, setResetKey] = useState(0); // for resetting form

  const { mutate: createServiceTask, isPending } = useCreateServiceTask();
  const notify = useNotification();

  const breadcrumbs = [
    { label: "Service Tasks", href: "/service-tasks" },
    { label: "New Service Task" },
  ];

  const validate = () => {
    const newErrors: typeof errors = {};
    if (!form.Name.trim()) newErrors.Name = "Name is required.";
    if (!form.EstimatedLabourHours || isNaN(Number(form.EstimatedLabourHours)))
      newErrors.EstimatedLabourHours = "Estimated Labour Hours is required.";
    if (!form.EstimatedCost || isNaN(Number(form.EstimatedCost)))
      newErrors.EstimatedCost = "Estimated Cost is required.";
    if (!form.Category) newErrors.Category = "Category is required.";
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
    if (!validate()) return;
    setIsSaving(true);
    createServiceTask(
      {
        Name: form.Name,
        Description: form.Description,
        EstimatedLabourHours: Number(form.EstimatedLabourHours),
        EstimatedCost: Number(form.EstimatedCost),
        Category: Number(form.Category),
        IsActive: form.IsActive,
      },
      {
        onSuccess: () => {
          setIsSaving(false);
          notify("Service Task created successfully!", "success");
          router.push("/service-tasks");
        },
        onError: () => setIsSaving(false),
      },
    );
  };

  const handleSaveAndAddAnother = async () => {
    if (!validate()) return;
    setIsSaving(true);
    createServiceTask(
      {
        Name: form.Name,
        Description: form.Description,
        EstimatedLabourHours: Number(form.EstimatedLabourHours),
        EstimatedCost: Number(form.EstimatedCost),
        Category: Number(form.Category),
        IsActive: form.IsActive,
      },
      {
        onSuccess: () => {
          setIsSaving(false);
          notify("Service Task created successfully!", "success");
          setForm(initialForm);
          setResetKey(k => k + 1);
        },
        onError: () => setIsSaving(false),
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
          key={resetKey} // Add key to force re-render and reset form
          value={form}
          errors={errors}
          onChange={handleChange}
          disabled={isSaving || isPending}
        />
      </div>
      {/* Footer Actions */}
      <div className="max-w-2xl mx-auto w-full mt-8 mb-12">
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
