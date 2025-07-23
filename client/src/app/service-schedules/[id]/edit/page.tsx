"use client";
import React, { useState, useEffect } from "react";
import { useRouter, useParams } from "next/navigation";
import ServiceScheduleHeader from "@/app/_features/service-schedule/components/ServiceScheduleHeader";
import ServiceScheduleDetailsForm, {
  ServiceScheduleDetailsFormValues,
} from "@/app/_features/service-schedule/components/ServiceScheduleDetailsForm";
import { PrimaryButton, SecondaryButton } from "@/app/_features/shared/button";
import {
  useServiceSchedule,
  useUpdateServiceSchedule,
} from "@/app/_hooks/service-schedule/useServiceSchedule";
import { useServiceTasks } from "@/app/_hooks/service-task/useServiceTask";
import { useVehicles } from "@/app/_hooks/vehicle/useVehicles";
import { BreadcrumbItem } from "@/app/_features/shared/layout/Breadcrumbs";
import { useNotification } from "@/app/_features/shared/feedback/NotificationProvider";

export default function EditServiceSchedulePage() {
  const router = useRouter();
  const params = useParams();
  const id = params.id ? Number(params.id) : undefined;
  const { data: schedule, isPending, isError } = useServiceSchedule(id!);
  const { mutate: updateServiceSchedule, isPending: isUpdating } =
    useUpdateServiceSchedule();
  const { serviceTasks, isPending: isLoadingTasks } = useServiceTasks({
    pageSize: 100,
  });
  const { vehicles, isLoadingVehicles } = useVehicles({ pageSize: 100 });
  const notify = useNotification();

  const [form, setForm] = useState<ServiceScheduleDetailsFormValues | null>(
    null,
  );
  const [errors, setErrors] = useState<
    Partial<Record<keyof ServiceScheduleDetailsFormValues, string>>
  >({});

  useEffect(() => {
    if (schedule) {
      setForm({
        Name: schedule.Name,
        TimeIntervalValue: schedule.TimeIntervalValue?.toString() || "",
        TimeIntervalUnit: schedule.TimeIntervalUnit || "",
        MileageInterval: schedule.MileageInterval?.toString() || "",
        TimeBufferValue: schedule.TimeBufferValue?.toString() || "",
        TimeBufferUnit: schedule.TimeBufferUnit || "",
        MileageBuffer: schedule.MileageBuffer?.toString() || "",
        FirstServiceTimeValue: schedule.FirstServiceTimeValue?.toString() || "",
        FirstServiceTimeUnit: schedule.FirstServiceTimeUnit || "",
        FirstServiceMileage: schedule.FirstServiceMileage?.toString() || "",
        ServiceTaskIDs: schedule.ServiceTasks.map(task => task.id),
        IsActive: schedule.IsActive,
        ServiceProgramID: schedule.ServiceProgramID,
      });
    }
  }, [schedule]);

  const breadcrumbs: BreadcrumbItem[] = [
    { label: "Service Schedules", href: "/service-schedules" },
    { label: schedule?.Name || "...", href: `/service-schedules/${id}` },
  ];

  const validate = () => {
    const newErrors: typeof errors = {};
    if (!form?.Name?.trim()) newErrors.Name = "Name is required.";
    if (!form?.ServiceTaskIDs.length)
      newErrors.ServiceTaskIDs = "At least one service task is required.";
    if (!form?.ServiceProgramID)
      newErrors.ServiceProgramID = "Service Program is required.";
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (
    field: keyof ServiceScheduleDetailsFormValues,
    value: any,
  ) => {
    setForm(f => (f ? { ...f, [field]: value } : f));
    setErrors(e => ({ ...e, [field]: undefined }));
  };

  const handleCancel = () => {
    router.push(`/service-schedules/${id}`);
  };

  const handleSave = async () => {
    if (!validate() || !form) return;
    updateServiceSchedule(
      {
        ServiceScheduleID: id!,
        ServiceProgramID: Number(form.ServiceProgramID),
        Name: form.Name,
        ServiceTaskIDs: form.ServiceTaskIDs,
        TimeIntervalValue: form.TimeIntervalValue
          ? Number(form.TimeIntervalValue)
          : undefined,
        TimeIntervalUnit: form.TimeIntervalUnit
          ? Number(form.TimeIntervalUnit)
          : undefined,
        TimeBufferValue: form.TimeBufferValue
          ? Number(form.TimeBufferValue)
          : undefined,
        TimeBufferUnit: form.TimeBufferUnit
          ? Number(form.TimeBufferUnit)
          : undefined,
        MileageInterval: form.MileageInterval
          ? Number(form.MileageInterval)
          : undefined,
        MileageBuffer: form.MileageBuffer
          ? Number(form.MileageBuffer)
          : undefined,
        FirstServiceTimeValue: form.FirstServiceTimeValue
          ? Number(form.FirstServiceTimeValue)
          : undefined,
        FirstServiceTimeUnit: form.FirstServiceTimeUnit
          ? Number(form.FirstServiceTimeUnit)
          : undefined,
        FirstServiceMileage: form.FirstServiceMileage
          ? Number(form.FirstServiceMileage)
          : undefined,
        IsActive: form.IsActive,
      },
      {
        onSuccess: () => {
          notify("Service Schedule updated successfully!", "success");
          router.push(`/service-schedules/${id}`);
        },
      },
    );
  };

  if (isPending || isLoadingTasks || isLoadingVehicles || !form)
    return <div className="p-8">Loading...</div>;
  if (isError)
    return (
      <div className="p-8 text-red-500">Failed to load service schedule.</div>
    );

  return (
    <div className="min-h-screen bg-gray-50">
      <ServiceScheduleHeader
        title={"Edit Service Schedule"}
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
        <ServiceScheduleDetailsForm
          value={form}
          errors={errors}
          onChange={handleChange}
          availableServiceTasks={serviceTasks}
          availableVehicles={vehicles}
          disabled={isUpdating}
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
            {isUpdating ? "Saving..." : "Save Service Schedule"}
          </PrimaryButton>
        </div>
      </div>
    </div>
  );
}
