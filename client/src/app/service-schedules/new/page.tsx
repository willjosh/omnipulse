"use client";
import React, { useState } from "react";
import { useRouter } from "next/navigation";
import ServiceScheduleHeader from "@/app/_features/service-schedule/components/ServiceScheduleHeader";
import ServiceScheduleDetailsForm, {
  ServiceScheduleDetailsFormValues,
} from "@/app/_features/service-schedule/components/ServiceScheduleDetailsForm";
import { useServiceTasks } from "@/app/_hooks/service-task/useServiceTask";
import { useVehicles } from "@/app/_hooks/vehicle/useVehicles";
import { useCreateServiceSchedule } from "@/app/_hooks/service-schedule/useServiceSchedule";
import { BreadcrumbItem } from "@/app/_features/shared/layout/Breadcrumbs";
import Loading from "@/app/_features/shared/feedback/Loading";
import PrimaryButton from "@/app/_features/shared/button/PrimaryButton";
import SecondaryButton from "@/app/_features/shared/button/SecondaryButton";

const initialForm: ServiceScheduleDetailsFormValues = {
  Name: "",
  TimeIntervalValue: "",
  TimeIntervalUnit: "",
  MileageInterval: "",
  TimeBufferValue: "",
  TimeBufferUnit: "",
  MileageBuffer: "",
  FirstServiceTimeValue: "",
  FirstServiceTimeUnit: "",
  FirstServiceMileage: "",
  ServiceTaskIDs: [],
  IsActive: true,
  ServiceProgramID: "",
};

export default function CreateServiceSchedulePage() {
  const router = useRouter();
  const [form, setForm] =
    useState<ServiceScheduleDetailsFormValues>(initialForm);
  const [errors, setErrors] = useState<
    Partial<Record<keyof ServiceScheduleDetailsFormValues, string>>
  >({});
  const { serviceTasks, isPending: isLoadingTasks } = useServiceTasks({
    pageSize: 100,
  });
  const { vehicles, isLoadingVehicles } = useVehicles({ pageSize: 100 });
  const { mutate: createServiceSchedule, isPending: isCreating } =
    useCreateServiceSchedule();

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
    if (!form.Name.trim()) newErrors.Name = "Name is required.";
    if (!form.ServiceTaskIDs.length)
      newErrors.ServiceTaskIDs = "At least one service task is required.";
    if (!form.ServiceProgramID)
      newErrors.ServiceProgramID = "Service Program is required.";
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = () => {
    if (!validate()) return;
    createServiceSchedule(
      {
        ServiceProgramID: 1, // TODO: Replace with actual program selection
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
          router.push("/service-schedules");
        },
      },
    );
  };

  const handleCancel = () => {
    router.push("/service-schedules");
  };

  const handleSaveAndAddAnother = () => {
    if (!validate()) return;
    createServiceSchedule(
      {
        ServiceProgramID: 1, // TODO: Replace with actual program selection
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
          setForm(initialForm);
          setErrors({});
        },
      },
    );
  };

  if (isLoadingTasks || isLoadingVehicles) {
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
            <PrimaryButton onClick={handleSubmit} disabled={isCreating}>
              {isCreating ? "Saving..." : "Save"}
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
          disabled={isCreating}
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
            <PrimaryButton onClick={handleSubmit} disabled={isCreating}>
              {isCreating ? "Saving..." : "Save"}
            </PrimaryButton>
          </div>
        </div>
      </div>
    </div>
  );
}
