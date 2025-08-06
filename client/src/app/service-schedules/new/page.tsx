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
    if (form.firstServiceDate) payload.firstServiceDate = form.firstServiceDate;
    if (form.firstServiceMileage)
      payload.firstServiceMileage = Number(form.firstServiceMileage);
    createServiceSchedule(payload, {
      onSuccess: () => {
        setForm(initialForm);
        setErrors({});
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
