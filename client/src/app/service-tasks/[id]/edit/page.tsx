"use client";
import React, { useState, useEffect } from "react";
import { useRouter, useParams } from "next/navigation";
import ServiceTaskHeader from "@/app/_features/service-task/components/ServiceTaskHeader";
import ServiceTaskDetailsForm, {
  ServiceTaskDetailsFormValues,
} from "@/app/_features/service-task/components/ServiceTaskDetailsForm";
import { PrimaryButton, SecondaryButton } from "@/app/_features/shared/button";
import {
  useServiceTask,
  useUpdateServiceTask,
} from "@/app/_hooks/service-task/useServiceTasks";
import { useNotification } from "@/app/_features/shared/feedback/NotificationProvider";

export default function EditServiceTaskPage() {
  const router = useRouter();
  const params = useParams();
  const id = params.id ? Number(params.id) : undefined;
  const { data: task, isPending, isError } = useServiceTask(id!);
  const { mutate: updateServiceTask, isPending: isUpdating } =
    useUpdateServiceTask();
  const notify = useNotification();

  const [form, setForm] = useState<ServiceTaskDetailsFormValues | null>(null);
  const [errors, setErrors] = useState<
    Partial<Record<keyof ServiceTaskDetailsFormValues, string>>
  >({});

  useEffect(() => {
    if (task) {
      setForm({
        name: task.name,
        description: task.description || "",
        estimatedLabourHours: task.estimatedLabourHours.toString(),
        estimatedCost: task.estimatedCost.toString(),
        category: task.categoryEnum,
        isActive: task.isActive,
      });
    }
  }, [task]);

  const breadcrumbs = [
    { label: "Service Tasks", href: "/service-tasks" },
    { label: task?.name || "...", href: `/service-tasks/${id}` },
  ];

  const validate = () => {
    const newErrors: typeof errors = {};
    if (!form?.name?.trim()) newErrors.name = "Name is required.";
    if (!form?.estimatedLabourHours || isNaN(Number(form.estimatedLabourHours)))
      newErrors.estimatedLabourHours = "Estimated Labour Hours is required.";
    if (!form?.estimatedCost || isNaN(Number(form.estimatedCost)))
      newErrors.estimatedCost = "Estimated Cost is required.";
    if (!form?.category) newErrors.category = "Category is required.";
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (
    field: keyof ServiceTaskDetailsFormValues,
    value: any,
  ) => {
    setForm(f => (f ? { ...f, [field]: value } : f));
    setErrors(e => ({ ...e, [field]: undefined }));
  };

  const handleCancel = () => {
    router.push(`/service-tasks/${id}`);
  };

  const handleSave = async () => {
    if (!validate() || !form) return;
    updateServiceTask(
      {
        ServiceTaskID: id!,
        name: form.name,
        description: form.description,
        estimatedLabourHours: Number(form.estimatedLabourHours),
        estimatedCost: Number(form.estimatedCost),
        category: Number(form.category),
        isActive: form.isActive,
      },
      {
        onSuccess: () => {
          notify("Service Task updated successfully!", "success");
          router.push(`/service-tasks/${id}`);
        },
      },
    );
  };

  if (isPending || !form) return <div className="p-8">Loading...</div>;
  if (isError)
    return <div className="p-8 text-red-500">Failed to load service task.</div>;

  return (
    <div className="min-h-screen bg-gray-50">
      <ServiceTaskHeader
        title={"Edit Service Task"}
        breadcrumbs={breadcrumbs}
        actions={
          <>
            <SecondaryButton onClick={handleCancel} disabled={isUpdating}>
              Cancel
            </SecondaryButton>
            <PrimaryButton onClick={handleSave} disabled={isUpdating}>
              {isUpdating ? "Saving..." : "Save"}
            </PrimaryButton>
          </>
        }
      />
      <div className="px-6 pb-12 mt-4 max-w-2xl mx-auto">
        <ServiceTaskDetailsForm
          value={form}
          errors={errors}
          onChange={handleChange}
          disabled={isUpdating}
          showIsActive
        />
      </div>
      {/* Footer Actions */}
      <div className="max-w-2xl mx-auto w-full mb-12">
        <hr className="mb-6 border-gray-300" />
        <div className="flex justify-between items-center">
          <SecondaryButton onClick={handleCancel} disabled={isUpdating}>
            Cancel
          </SecondaryButton>
          <PrimaryButton onClick={handleSave} disabled={isUpdating}>
            {isUpdating ? "Saving..." : "Save Service Task"}
          </PrimaryButton>
        </div>
      </div>
    </div>
  );
}
