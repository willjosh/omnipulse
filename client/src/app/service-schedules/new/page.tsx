"use client";
import React, { useState, useEffect, Suspense } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import ServiceScheduleHeader from "@/features/service-schedule/components/ServiceScheduleHeader";
import ServiceScheduleDetailsForm, {
  ServiceScheduleDetailsFormValues,
} from "@/features/service-schedule/components/ServiceScheduleDetailsForm";
import { useServiceTasks } from "@/features/service-task/hooks/useServiceTasks";
import { useVehicles } from "@/features/vehicle/hooks/useVehicles";
import { useCreateServiceSchedule } from "@/features/service-schedule/hooks/useServiceSchedules";
import { useServicePrograms } from "@/features/service-program/hooks/useServicePrograms";
import { BreadcrumbItem } from "@/components/ui/Layout/Breadcrumbs";
import Loading from "@/components/ui/Feedback/Loading";
import PrimaryButton from "@/components/ui/Button/PrimaryButton";
import SecondaryButton from "@/components/ui/Button/SecondaryButton";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import { getErrorMessage, getErrorFields } from "@/utils/fieldErrorUtils";

const initialForm: ServiceScheduleDetailsFormValues = {
  name: "",
  timeIntervalValue: "",
  timeIntervalUnit: "",
  mileageInterval: "",
  timeBufferValue: "",
  timeBufferUnit: "",
  mileageBuffer: "",
  firstServiceDate: "",
  firstServiceMileage: "",
  serviceTaskIDs: [],
  scheduleType: 1, // Default to TIME-based
  serviceProgramID: "",
};

function CreateServiceScheduleForm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const serviceProgramId = searchParams.get("serviceProgramId");
  const notify = useNotification();

  const [form, setForm] =
    useState<ServiceScheduleDetailsFormValues>(initialForm);
  const [errors, setErrors] = useState<
    Partial<Record<keyof ServiceScheduleDetailsFormValues, string>>
  >({});
  const [formError, setFormError] = useState<string | null>(null);
  const { serviceTasks, isPending: isLoadingTasks } = useServiceTasks({
    PageSize: 100,
  });
  const { vehicles, isPending: isLoadingVehicles } = useVehicles({
    PageSize: 100,
  });
  const { servicePrograms, isPending: isLoadingPrograms } = useServicePrograms({
    PageSize: 100,
  });
  const { mutate: createServiceSchedule, isPending: isCreating } =
    useCreateServiceSchedule();

  useEffect(() => {
    if (serviceProgramId) {
      setForm(prev => ({ ...prev, serviceProgramID: serviceProgramId }));
    }
  }, [serviceProgramId]);

  const breadcrumbs: BreadcrumbItem[] = [
    { label: "Service Schedules", href: "/service-schedules" },
  ];

  const handleChange = (
    field: keyof ServiceScheduleDetailsFormValues,
    value: any,
  ) => {
    setForm(f => ({ ...f, [field]: value }));
    setErrors(e => ({ ...e, [field]: undefined }));
  };

  const validate = () => {
    const newErrors: typeof errors = {};
    setFormError(null);
    if (!form.name.trim()) newErrors.name = "Name is required.";
    if (!form.serviceTaskIDs.length)
      newErrors.serviceTaskIDs = "At least one service task is required.";
    if (!form.serviceProgramID)
      newErrors.serviceProgramID = "Service Program is required.";
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleCancel = () => {
    if (serviceProgramId) {
      router.push(`/service-programs/${serviceProgramId}`);
    } else {
      router.push("/service-schedules");
    }
  };

  const handleSaveAndAddAnother = () => {
    if (!validate()) {
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

          // Store without Z suffix to treat as local time, not UTC
          formattedFirstServiceDate = `${year}-${month}-${day}T${String(hours).padStart(2, "0")}:${String(minutes).padStart(2, "0")}:00.000`;
        }
      } catch (error) {
        console.error("Error formatting date:", error);
      }
    }

    const payload: any = {
      serviceProgramID: Number(form.serviceProgramID),
      name: form.name,
      serviceTaskIDs: form.serviceTaskIDs.map((id: any) => Number(id)),
      scheduleType: form.scheduleType,
    };
    if (form.timeIntervalValue)
      payload.timeIntervalValue = Number(form.timeIntervalValue);
    if (form.timeIntervalUnit)
      payload.timeIntervalUnit = Number(form.timeIntervalUnit);
    if (form.timeBufferValue)
      payload.timeBufferValue = Number(form.timeBufferValue);
    if (form.timeBufferUnit)
      payload.timeBufferUnit = Number(form.timeBufferUnit);
    if (form.mileageInterval)
      payload.mileageInterval = Number(form.mileageInterval);
    if (form.mileageBuffer) payload.mileageBuffer = Number(form.mileageBuffer);
    if (formattedFirstServiceDate)
      payload.firstServiceDate = formattedFirstServiceDate;
    if (form.firstServiceMileage)
      payload.firstServiceMileage = Number(form.firstServiceMileage);
    createServiceSchedule(payload, {
      onSuccess: () => {
        notify("Service schedule created successfully!", "success");
        setForm(initialForm);
        setErrors({});
        if (serviceProgramId) {
          router.push(`/service-programs/${serviceProgramId}`);
        }
      },
      onError: (error: any) => {
        console.error("Failed to create service schedule:", error);

        // Get dynamic error message from backend
        const errorMessage = getErrorMessage(
          error,
          "Failed to create service schedule. Please check your input and try again.",
        );

        // Map backend errors to form fields
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

        // Set field-specific errors
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
    });
  };

  const handleSave = () => {
    if (!validate()) {
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

          // Store without Z suffix to treat as local time, not UTC
          formattedFirstServiceDate = `${year}-${month}-${day}T${String(hours).padStart(2, "0")}:${String(minutes).padStart(2, "0")}:00.000`;
        }
      } catch (error) {
        console.error("Error formatting date:", error);
      }
    }

    const payload: any = {
      serviceProgramID: Number(form.serviceProgramID),
      name: form.name,
      serviceTaskIDs: form.serviceTaskIDs.map((id: any) => Number(id)),
      scheduleType: form.scheduleType,
    };
    if (form.timeIntervalValue)
      payload.timeIntervalValue = Number(form.timeIntervalValue);
    if (form.timeIntervalUnit)
      payload.timeIntervalUnit = Number(form.timeIntervalUnit);
    if (form.timeBufferValue)
      payload.timeBufferValue = Number(form.timeBufferValue);
    if (form.timeBufferUnit)
      payload.timeBufferUnit = Number(form.timeBufferUnit);
    if (form.mileageInterval)
      payload.mileageInterval = Number(form.mileageInterval);
    if (form.mileageBuffer) payload.mileageBuffer = Number(form.mileageBuffer);
    if (formattedFirstServiceDate)
      payload.firstServiceDate = formattedFirstServiceDate;
    if (form.firstServiceMileage)
      payload.firstServiceMileage = Number(form.firstServiceMileage);
    createServiceSchedule(payload, {
      onSuccess: () => {
        notify("Service schedule created successfully!", "success");
        if (serviceProgramId) {
          router.push(`/service-programs/${serviceProgramId}`);
        } else {
          router.push("/service-schedules");
        }
      },
      onError: (error: any) => {
        console.error("Failed to create service schedule:", error);

        // Get dynamic error message from backend
        const errorMessage = getErrorMessage(
          error,
          "Failed to create service schedule. Please check your input and try again.",
        );

        // Map backend errors to form fields
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

        // Set field-specific errors
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
    });
  };

  if (isLoadingTasks || isLoadingVehicles || isLoadingPrograms) {
    return <Loading />;
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <ServiceScheduleHeader
        title="New Service Schedule"
        breadcrumbs={breadcrumbs}
        actions={
          <>
            <SecondaryButton onClick={handleCancel} disabled={isCreating}>
              Cancel
            </SecondaryButton>
            <PrimaryButton onClick={handleSave} disabled={isCreating}>
              {isCreating ? "Saving..." : "Save"}
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
          disabled={isCreating}
          showScheduleType={true}
          showServiceProgram={!serviceProgramId}
        />
      </div>
      <div className="max-w-2xl mx-auto w-full mb-12">
        <hr className="mb-6 border-gray-300" />
        <div className="flex justify-between items-center">
          <SecondaryButton onClick={handleCancel} disabled={isCreating}>
            Cancel
          </SecondaryButton>
          <div className="flex gap-3">
            <SecondaryButton
              onClick={handleSaveAndAddAnother}
              disabled={isCreating}
            >
              {isCreating ? "Saving..." : "Save & Add Another"}
            </SecondaryButton>
            <PrimaryButton onClick={handleSave} disabled={isCreating}>
              {isCreating ? "Saving..." : "Save"}
            </PrimaryButton>
          </div>
        </div>
      </div>
    </div>
  );
}

export default function CreateServiceSchedulePage() {
  return (
    <Suspense fallback={<Loading />}>
      <CreateServiceScheduleForm />
    </Suspense>
  );
}
