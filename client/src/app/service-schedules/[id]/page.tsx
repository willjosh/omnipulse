"use client";
import React from "react";
import { useParams, useRouter } from "next/navigation";
import FormContainer from "@/app/_features/shared/form/FormContainer";
import DetailFieldRow from "@/app/_features/shared/detail/DetailFieldRow";
import { useServiceSchedule } from "@/app/_hooks/service-schedule/useServiceSchedule";
import Loading from "@/app/_features/shared/feedback/Loading";
import EmptyState from "@/app/_features/shared/feedback/EmptyState";
import ServiceScheduleHeader from "@/app/_features/service-schedule/components/ServiceScheduleHeader";
import PrimaryButton from "@/app/_features/shared/button/PrimaryButton";
import EditIcon from "@/app/_features/shared/icons/Edit";
import ArchiveIcon from "@/app/_features/shared/icons/Archive";
import ConfirmModal from "@/app/_features/shared/modal/ConfirmModal";
import { useDeleteServiceSchedule } from "@/app/_hooks/service-schedule/useServiceSchedule";
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
        title={schedule.Name}
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
          <DetailFieldRow label="Name" value={schedule.Name} />
          <DetailFieldRow
            label="Tasks"
            value={
              <div className="flex flex-wrap gap-2 max-w-md break-words justify-end">
                {schedule.ServiceTasks.length > 0 ? (
                  schedule.ServiceTasks.map(task => (
                    <span
                      key={task.id}
                      className="inline-block bg-blue-100 text-blue-800 text-xs font-medium px-3 py-1 rounded-full"
                    >
                      {task.Name}
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
              schedule.TimeIntervalValue && schedule.TimeIntervalUnitLabel ? (
                `${schedule.TimeIntervalValue} ${
                  schedule.TimeIntervalValue === 1
                    ? schedule.TimeIntervalUnitLabel.replace(/s$/, "")
                    : schedule.TimeIntervalUnitLabel
                }`
              ) : schedule.MileageInterval ? (
                `${schedule.MileageInterval} km`
              ) : (
                <span className="text-gray-400">-</span>
              )
            }
          />
          <DetailFieldRow
            label="Buffer"
            value={
              schedule.TimeBufferValue && schedule.TimeBufferUnitLabel ? (
                `${schedule.TimeBufferValue} ${
                  schedule.TimeBufferValue === 1
                    ? schedule.TimeBufferUnitLabel.replace(/s$/, "")
                    : schedule.TimeBufferUnitLabel
                }`
              ) : schedule.MileageBuffer ? (
                `${schedule.MileageBuffer} km`
              ) : (
                <span className="text-gray-400">-</span>
              )
            }
          />
          <DetailFieldRow
            label="First Service"
            value={
              schedule.FirstServiceTimeValue &&
              schedule.FirstServiceTimeUnitLabel ? (
                `${schedule.FirstServiceTimeValue} ${
                  schedule.FirstServiceTimeValue === 1
                    ? schedule.FirstServiceTimeUnitLabel.replace(/s$/, "")
                    : schedule.FirstServiceTimeUnitLabel
                }`
              ) : schedule.FirstServiceMileage ? (
                `${schedule.FirstServiceMileage} km`
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
            value={schedule.IsActive ? "Yes" : "No"}
            noBorder
          />
        </FormContainer>
      </div>
    </div>
  );
}
