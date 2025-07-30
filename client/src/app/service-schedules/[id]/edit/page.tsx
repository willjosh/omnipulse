"use client";
import React, { useState, useEffect } from "react";
import { useRouter, useParams } from "next/navigation";
import ServiceScheduleHeader from "@/features/service-schedule/components/ServiceScheduleHeader";
import ServiceScheduleDetailsForm, {
  ServiceScheduleDetailsFormValues,
} from "@/features/service-schedule/components/ServiceScheduleDetailsForm";
import { PrimaryButton, SecondaryButton } from "@/components/ui/Button";
import {
  useServiceSchedule,
  useUpdateServiceSchedule,
} from "@/features/service-schedule/hooks/useServiceSchedules";
import { useServiceTasks } from "@/features/service-task/hooks/useServiceTasks";
import { useVehicles } from "@/features/vehicle/hooks/useVehicles";
import { useServicePrograms } from "@/features/service-program/hooks/useServicePrograms";
import { BreadcrumbItem } from "@/components/ui/Layout/Breadcrumbs";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";

export default function EditServiceSchedulePage() {
  const router = useRouter();
  const params = useParams();
  const id = params.id ? Number(params.id) : undefined;
  const { serviceSchedule, isPending, isError } = useServiceSchedule(id!);
  const { mutate: updateServiceSchedule, isPending: isUpdating } =
    useUpdateServiceSchedule();
  const { serviceTasks, isPending: isLoadingTasks } = useServiceTasks({
    PageSize: 100,
  });
  const { vehicles, isPending: isLoadingVehicles } = useVehicles({
    PageSize: 100,
  });
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
    if (serviceSchedule) {
      setForm({
        name: serviceSchedule.name,
        timeIntervalValue: serviceSchedule.timeIntervalValue?.toString() || "",
        timeIntervalUnit: serviceSchedule.timeIntervalUnit || "",
        mileageInterval: serviceSchedule.mileageInterval?.toString() || "",
        timeBufferValue: serviceSchedule.timeBufferValue?.toString() || "",
        timeBufferUnit: serviceSchedule.timeBufferUnit || "",
        mileageBuffer: serviceSchedule.mileageBuffer?.toString() || "",
        firstServiceTimeValue:
          serviceSchedule.firstServiceTimeValue?.toString() || "",
        firstServiceTimeUnit: serviceSchedule.firstServiceTimeUnit || "",
        firstServiceMileage:
          serviceSchedule.firstServiceMileage?.toString() || "",
        serviceTaskIDs: serviceSchedule.serviceTasks.map(task => task.id),
        isActive: serviceSchedule.isActive,
        serviceProgramID: serviceSchedule.serviceProgramID,
      });
    }
  }, [serviceSchedule]);

  const breadcrumbs: BreadcrumbItem[] = [
    { label: "Service Schedules", href: "/service-schedules" },
    { label: serviceSchedule?.name || "...", href: `/service-schedules/${id}` },
  ];

  const validate = () => {
    const newErrors: typeof errors = {};
    if (!form?.name?.trim()) newErrors.name = "Name is required.";
    if (!form?.serviceTaskIDs.length)
      newErrors.serviceTaskIDs = "At least one service task is required.";
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
          showServiceProgram={false}
        />
      </div>
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
