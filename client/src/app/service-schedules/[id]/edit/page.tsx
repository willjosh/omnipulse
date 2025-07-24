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
} from "@/app/_hooks/service-schedule/useServiceSchedules";
import { useServiceTasks } from "@/app/_hooks/service-task/useServiceTasks";
import { useVehicles } from "@/app/_hooks/vehicle/useVehicles";
import { useServicePrograms } from "@/app/_hooks/service-program/useServicePrograms";
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
    PageSize: 100,
  });
  const { vehicles, isLoadingVehicles } = useVehicles({ PageSize: 100 });
  const { servicePrograms, isPending: isLoadingPrograms } = useServicePrograms({
    PageSize: 100,
  });
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
        name: schedule.name,
        timeIntervalValue: schedule.timeIntervalValue?.toString() || "",
        timeIntervalUnit: schedule.timeIntervalUnit || "",
        mileageInterval: schedule.mileageInterval?.toString() || "",
        timeBufferValue: schedule.timeBufferValue?.toString() || "",
        timeBufferUnit: schedule.timeBufferUnit || "",
        mileageBuffer: schedule.mileageBuffer?.toString() || "",
        firstServiceTimeValue: schedule.firstServiceTimeValue?.toString() || "",
        firstServiceTimeUnit: schedule.firstServiceTimeUnit || "",
        firstServiceMileage: schedule.firstServiceMileage?.toString() || "",
        serviceTaskIDs: schedule.serviceTasks.map(task => task.id),
        isActive: schedule.isActive,
        serviceProgramID: schedule.serviceProgramID,
      });
    }
  }, [schedule]);

  const breadcrumbs: BreadcrumbItem[] = [
    { label: "Service Schedules", href: "/service-schedules" },
    { label: schedule?.name || "...", href: `/service-schedules/${id}` },
  ];

  const validate = () => {
    const newErrors: typeof errors = {};
    if (!form?.name?.trim()) newErrors.name = "Name is required.";
    if (!form?.serviceTaskIDs.length)
      newErrors.serviceTaskIDs = "At least one service task is required.";
    if (!form?.serviceProgramID)
      newErrors.serviceProgramID = "Service Program is required.";
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
        serviceScheduleID: id!,
        serviceProgramID: Number(form.serviceProgramID),
        name: form.name,
        serviceTaskIDs: form.serviceTaskIDs,
        timeIntervalValue: form.timeIntervalValue
          ? Number(form.timeIntervalValue)
          : undefined,
        timeIntervalUnit: form.timeIntervalUnit
          ? Number(form.timeIntervalUnit)
          : undefined,
        timeBufferValue: form.timeBufferValue
          ? Number(form.timeBufferValue)
          : undefined,
        timeBufferUnit: form.timeBufferUnit
          ? Number(form.timeBufferUnit)
          : undefined,
        mileageInterval: form.mileageInterval
          ? Number(form.mileageInterval)
          : undefined,
        mileageBuffer: form.mileageBuffer
          ? Number(form.mileageBuffer)
          : undefined,
        firstServiceTimeValue: form.firstServiceTimeValue
          ? Number(form.firstServiceTimeValue)
          : undefined,
        firstServiceTimeUnit: form.firstServiceTimeUnit
          ? Number(form.firstServiceTimeUnit)
          : undefined,
        firstServiceMileage: form.firstServiceMileage
          ? Number(form.firstServiceMileage)
          : undefined,
        isActive: form.isActive,
      },
      {
        onSuccess: () => {
          notify("Service Schedule updated successfully!", "success");
          router.push(`/service-schedules/${id}`);
        },
      },
    );
  };

  if (
    isPending ||
    isLoadingTasks ||
    isLoadingVehicles ||
    isLoadingPrograms ||
    !form
  )
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
          availableServicePrograms={servicePrograms}
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
