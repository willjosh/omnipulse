"use client";
import React from "react";
import { useParams, useRouter } from "next/navigation";
import FormContainer from "@/app/_features/shared/form/FormContainer";
import DetailFieldRow from "@/app/_features/shared/detail/DetailFieldRow";
import { useServiceSchedule } from "@/app/_hooks/service-schedule/useServiceSchedules";
import Loading from "@/app/_features/shared/feedback/Loading";
import EmptyState from "@/app/_features/shared/feedback/EmptyState";
import ServiceScheduleHeader from "@/app/_features/service-schedule/components/ServiceScheduleHeader";
import PrimaryButton from "@/app/_features/shared/button/PrimaryButton";
import EditIcon from "@/app/_features/shared/icons/Edit";
import ArchiveIcon from "@/app/_features/shared/icons/Archive";
import ConfirmModal from "@/app/_features/shared/modal/ConfirmModal";
import { useDeleteServiceSchedule } from "@/app/_hooks/service-schedule/useServiceSchedules";
import { useNotification } from "@/app/_features/shared/feedback/NotificationProvider";
import { BreadcrumbItem } from "@/app/_features/shared/layout/Breadcrumbs";

export default function ServiceScheduleDetailPage() {
  const params = useParams();
  const id = params.id ? Number(params.id) : undefined;
  const { data: schedule, isPending, isError } = useServiceSchedule(id!);
  const router = useRouter();
  const { mutate: deleteServiceSchedule, isPending: isDeleting } =
    useDeleteServiceSchedule();
  const notify = useNotification();
  const [isDeleteModalOpen, setDeleteModalOpen] = React.useState(false);

  if (isPending) {
    return <Loading />;
  }
  if (isError || !schedule) {
    return (
      <EmptyState
        title="Service Schedule not found"
        message="The service schedule you are looking for does not exist or could not be loaded."
      />
    );
  }

  const breadcrumbs: BreadcrumbItem[] = [
    { label: "Service Schedules", href: "/service-schedules" },
  ];

  const handleDelete = async () => {
    deleteServiceSchedule(id!, {
      onSuccess: () => {
        notify("Service schedule deleted successfully", "success");
        router.push("/service-schedules");
      },
      onError: () => {
        notify("Failed to delete service schedule", "error");
      },
    });
  };

  return (
    <div className="min-h-screen mx-auto bg-gray-50">
      <ServiceScheduleHeader
        title={schedule.name}
        breadcrumbs={breadcrumbs}
        actions={
          <>
            <PrimaryButton
              onClick={() => router.push(`/service-schedules/${id}/edit`)}
            >
              <EditIcon /> Edit
            </PrimaryButton>
            <PrimaryButton
              className="bg-red-600 hover:bg-red-700 ml-2"
              onClick={() => setDeleteModalOpen(true)}
              disabled={isDeleting}
            >
              <ArchiveIcon /> Delete
            </PrimaryButton>
            <ConfirmModal
              isOpen={isDeleteModalOpen}
              onClose={() => setDeleteModalOpen(false)}
              onConfirm={handleDelete}
              title="Delete Service Schedule"
              message="Are you sure you want to delete this service schedule? This action cannot be undone."
              confirmText="Delete"
              cancelText="Cancel"
            />
          </>
        }
      />
      <div className="m-4 px-6 pb-12">
        <FormContainer title="Details" className="max-w-2xl mx-auto">
          <DetailFieldRow label="Name" value={schedule.name} />
          <DetailFieldRow
            label="Tasks"
            value={
              <div className="flex flex-wrap gap-2 max-w-md break-words justify-end">
                {schedule.serviceTasks.length > 0 ? (
                  schedule.serviceTasks.map(task => (
                    <span
                      key={task.id}
                      className="inline-block bg-blue-100 text-blue-800 text-xs font-medium px-3 py-1 rounded-full"
                    >
                      {task.name}
                    </span>
                  ))
                ) : (
                  <span className="text-gray-400">-</span>
                )}
              </div>
            }
          />
          <DetailFieldRow
            label="Frequency"
            value={
              schedule.timeIntervalValue && schedule.timeIntervalUnitLabel ? (
                `${schedule.timeIntervalValue} ${
                  schedule.timeIntervalValue === 1
                    ? schedule.timeIntervalUnitLabel.replace(/s$/, "")
                    : schedule.timeIntervalUnitLabel
                }`
              ) : schedule.mileageInterval ? (
                `${schedule.mileageInterval} km`
              ) : (
                <span className="text-gray-400">-</span>
              )
            }
          />
          <DetailFieldRow
            label="Buffer"
            value={
              schedule.timeBufferValue && schedule.timeBufferUnitLabel ? (
                `${schedule.timeBufferValue} ${
                  schedule.timeBufferValue === 1
                    ? schedule.timeBufferUnitLabel.replace(/s$/, "")
                    : schedule.timeBufferUnitLabel
                }`
              ) : schedule.mileageBuffer ? (
                `${schedule.mileageBuffer} km`
              ) : (
                <span className="text-gray-400">-</span>
              )
            }
          />
          <DetailFieldRow
            label="First Service"
            value={
              schedule.firstServiceTimeValue &&
              schedule.firstServiceTimeUnitLabel ? (
                `${schedule.firstServiceTimeValue} ${
                  schedule.firstServiceTimeValue === 1
                    ? schedule.firstServiceTimeUnitLabel.replace(/s$/, "")
                    : schedule.firstServiceTimeUnitLabel
                }`
              ) : schedule.firstServiceMileage ? (
                `${schedule.firstServiceMileage} km`
              ) : (
                <span className="text-gray-400">-</span>
              )
            }
          />
          {/*
          <DetailFieldRow
            label="First Service Date"
            value={
              schedule.FirstServiceDate
                ? new Date(schedule.FirstServiceDate).toLocaleDateString()
                : <span className="text-gray-400">-</span>
            }
          />
          */}
          <DetailFieldRow
            label="Active"
            value={schedule.isActive ? "Yes" : "No"}
            noBorder
          />
        </FormContainer>
      </div>
    </div>
  );
}
