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
import { getErrorMessage, getErrorFields } from "@/utils/fieldErrorUtils";

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
  const [formError, setFormError] = useState<string | null>(null);

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
        firstServiceDate: serviceSchedule.firstServiceDate || "",
        firstServiceMileage:
          serviceSchedule.firstServiceMileage?.toString() || "",
        serviceTaskIDs: serviceSchedule.serviceTasks.map(task => task.id),
        scheduleType: serviceSchedule.scheduleType,
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
    setFormError(null);

    if (!form?.name?.trim()) newErrors.name = "Name is required.";
    if (!form?.serviceTaskIDs.length)
      newErrors.serviceTaskIDs = "At least one service task is required.";

    // Validate based on the original schedule type
    const originalScheduleType = serviceSchedule?.scheduleType;
    const isTimeBased = originalScheduleType === 1; // ServiceScheduleTypeEnum.TIME
    const isMileageBased = originalScheduleType === 2; // ServiceScheduleTypeEnum.MILEAGE

    // For time-based schedules, we need time interval values
    // For mileage-based schedules, we need mileage interval values
    let hasValidConfiguration = false;

    if (isTimeBased) {
      // For time-based schedules, check if we have valid interval values
      // Note: timeIntervalValue can be "0" (valid) or "" (invalid)
      const hasTimeInterval =
        form?.timeIntervalValue !== undefined && form?.timeIntervalValue !== "";
      const hasTimeUnit =
        form?.timeIntervalUnit !== undefined && form?.timeIntervalUnit !== "";
      hasValidConfiguration = Boolean(hasTimeInterval && hasTimeUnit);
    } else if (isMileageBased) {
      const hasMileageInterval =
        form?.mileageInterval !== undefined && form?.mileageInterval !== "";
      hasValidConfiguration = Boolean(hasMileageInterval);
    }

    if (!hasValidConfiguration) {
      setFormError(
        `Invalid configuration for ${isTimeBased ? "time-based" : "mileage-based"} schedule. Please ensure all required fields are filled.`,
      );
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0 && hasValidConfiguration;
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
    if (!validate() || !form) {
      notify("Please fill all required fields", "error");
      return;
    }

    // Ensure the date is properly formatted as ISO string before saving
    let formattedFirstServiceDate = form.firstServiceDate;
    if (form.firstServiceDate && form.firstServiceDate !== "") {
      try {
        const date = new Date(form.firstServiceDate);
        if (!isNaN(date.getTime())) {
          // Preserve the local time by creating a new ISO string
          const year = date.getFullYear();
          const month = String(date.getMonth() + 1).padStart(2, "0");
          const day = String(date.getDate()).padStart(2, "0");
          const hours = date.getHours();
          const minutes = date.getMinutes();

          formattedFirstServiceDate = `${year}-${month}-${day}T${String(hours).padStart(2, "0")}:${String(minutes).padStart(2, "0")}:00.000`;
        }
      } catch (error) {
        console.error("Error formatting date:", error);
      }
    }
    updateServiceSchedule(
      {
        serviceScheduleID: id!,
        serviceProgramID: Number(form.serviceProgramID),
        name: form.name,
        serviceTaskIDs: form.serviceTaskIDs,
        timeIntervalValue: form.timeIntervalValue
          ? Number(form.timeIntervalValue)
          : null,
        timeIntervalUnit: form.timeIntervalUnit
          ? Number(form.timeIntervalUnit)
          : null,
        timeBufferValue: form.timeBufferValue
          ? Number(form.timeBufferValue)
          : null,
        timeBufferUnit: form.timeBufferUnit
          ? Number(form.timeBufferUnit)
          : null,
        mileageInterval: form.mileageInterval
          ? Number(form.mileageInterval)
          : null,
        mileageBuffer: form.mileageBuffer ? Number(form.mileageBuffer) : null,
        firstServiceDate: formattedFirstServiceDate || null,
        firstServiceMileage: form.firstServiceMileage
          ? Number(form.firstServiceMileage)
          : null,
      },
      {
        onSuccess: () => {
          notify("Service Schedule updated successfully!", "success");
          router.push(`/service-schedules/${id}`);
        },
        onError: (error: any) => {
          console.error("Failed to update service schedule:", error);

          const errorMessage = getErrorMessage(
            error,
            "Failed to update service schedule. Please check your input and try again.",
          );

          const fieldErrors = getErrorFields(error, [
            "serviceProgramID",
            "name",
            "serviceTaskIDs",
            "timeIntervalValue",
            "timeIntervalUnit",
            "timeBufferValue",
            "timeBufferUnit",
            "mileageInterval",
            "mileageBuffer",
            "firstServiceDate",
            "firstServiceMileage",
          ]);

          const newErrors: typeof errors = {};
          if (fieldErrors.serviceProgramID) {
            newErrors.serviceProgramID = "Invalid service program";
          }
          if (fieldErrors.name) {
            newErrors.name = "Invalid name";
          }
          if (fieldErrors.serviceTaskIDs) {
            newErrors.serviceTaskIDs = "Invalid service task selection";
          }
          if (fieldErrors.timeIntervalValue) {
            newErrors.timeIntervalValue = "Invalid time interval value";
          }
          if (fieldErrors.timeIntervalUnit) {
            newErrors.timeIntervalUnit = "Invalid time interval unit";
          }
          if (fieldErrors.timeBufferValue) {
            newErrors.timeBufferValue = "Invalid time buffer value";
          }
          if (fieldErrors.timeBufferUnit) {
            newErrors.timeBufferUnit = "Invalid time buffer unit";
          }
          if (fieldErrors.mileageInterval) {
            newErrors.mileageInterval = "Invalid mileage interval";
          }
          if (fieldErrors.mileageBuffer) {
            newErrors.mileageBuffer = "Invalid mileage buffer";
          }
          if (fieldErrors.firstServiceDate) {
            newErrors.firstServiceDate = "Invalid first service date";
          }
          if (fieldErrors.firstServiceMileage) {
            newErrors.firstServiceMileage = "Invalid first service mileage";
          }

          setErrors(newErrors);
          notify(errorMessage, "error");
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
        {formError && (
          <div className="mb-4 text-red-600 text-sm font-medium">
            {formError}
          </div>
        )}
        <ServiceScheduleDetailsForm
          value={form}
          errors={errors}
          onChange={handleChange}
          availableServiceTasks={serviceTasks}
          availableVehicles={vehicles}
          availableServicePrograms={servicePrograms}
          disabled={isUpdating}
          showScheduleType={false}
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
