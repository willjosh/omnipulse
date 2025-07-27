"use client";
import React, { useState, useEffect, Suspense } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import ServiceScheduleHeader from "@/app/_features/service-schedule/components/ServiceScheduleHeader";
import ServiceScheduleDetailsForm, {
  ServiceScheduleDetailsFormValues,
} from "@/app/_features/service-schedule/components/ServiceScheduleDetailsForm";
import { useServiceTasks } from "@/app/_hooks/service-task/useServiceTasks";
import { useVehicles } from "@/app/_hooks/vehicle/useVehicles";
import { useCreateServiceSchedule } from "@/app/_hooks/service-schedule/useServiceSchedules";
import { useServicePrograms } from "@/app/_hooks/service-program/useServicePrograms";
import { BreadcrumbItem } from "@/app/_features/shared/layout/Breadcrumbs";
import Loading from "@/app/_features/shared/feedback/Loading";
import PrimaryButton from "@/app/_features/shared/button/PrimaryButton";
import SecondaryButton from "@/app/_features/shared/button/SecondaryButton";

const initialForm: ServiceScheduleDetailsFormValues = {
  name: "",
  timeIntervalValue: "",
  timeIntervalUnit: "",
  mileageInterval: "",
  timeBufferValue: "",
  timeBufferUnit: "",
  mileageBuffer: "",
  firstServiceTimeValue: "",
  firstServiceTimeUnit: "",
  firstServiceMileage: "",
  serviceTaskIDs: [],
  isActive: true,
  serviceProgramID: "",
};

function CreateServiceScheduleForm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const serviceProgramId = searchParams.get("serviceProgramId");

  const [form, setForm] =
    useState<ServiceScheduleDetailsFormValues>(initialForm);
  const [errors, setErrors] = useState<
    Partial<Record<keyof ServiceScheduleDetailsFormValues, string>>
  >({});
  const [formError, setFormError] = useState<string | null>(null);
  const { serviceTasks, isPending: isLoadingTasks } = useServiceTasks({
    PageSize: 100,
  });
  const { vehicles, isLoadingVehicles } = useVehicles({ PageSize: 100 });
  const { servicePrograms, isPending: isLoadingPrograms } = useServicePrograms({
    PageSize: 100,
  });
  const { mutate: createServiceSchedule, isPending: isCreating } =
    useCreateServiceSchedule();

  // Pre-fill service program ID if provided in URL
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
    // Recurrence validation
    const hasTimeRecurrence = form.timeIntervalValue && form.timeIntervalUnit;
    const hasMileageRecurrence = form.mileageInterval;
    if (!hasTimeRecurrence && !hasMileageRecurrence) {
      setFormError(
        "At least one recurrence option must be provided: time-based (Time Interval & Unit) or mileage-based (Mileage Interval).",
      );
    }
    setErrors(newErrors);
    return (
      Object.keys(newErrors).length === 0 &&
      (hasTimeRecurrence || hasMileageRecurrence)
    );
  };

  const handleCancel = () => {
    // If we came from a service program page, go back there
    if (serviceProgramId) {
      router.push(`/service-programs/${serviceProgramId}`);
    } else {
      router.push("/service-schedules");
    }
  };

  const handleSaveAndAddAnother = () => {
    if (!validate()) return;
    const payload: any = {
      serviceProgramID: Number(form.serviceProgramID),
      name: form.name,
      serviceTaskIDs: form.serviceTaskIDs.map((id: any) => Number(id)),
      isActive: form.isActive,
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
    if (form.firstServiceTimeValue)
      payload.firstServiceTimeValue = Number(form.firstServiceTimeValue);
    if (form.firstServiceTimeUnit)
      payload.firstServiceTimeUnit = Number(form.firstServiceTimeUnit);
    if (form.firstServiceMileage)
      payload.firstServiceMileage = Number(form.firstServiceMileage);
    createServiceSchedule(payload, {
      onSuccess: () => {
        setForm(initialForm);
        setErrors({});
        // If we came from a service program page, go back there
        if (serviceProgramId) {
          router.push(`/service-programs/${serviceProgramId}`);
        }
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
            <PrimaryButton
              onClick={handleSaveAndAddAnother}
              disabled={isCreating}
            >
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
          showServiceProgram={!serviceProgramId}
        />
      </div>
      {/* Footer Actions */}
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
            <PrimaryButton
              onClick={handleSaveAndAddAnother}
              disabled={isCreating}
            >
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
