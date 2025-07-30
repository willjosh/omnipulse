"use client";
import React from "react";
import { useParams, useRouter } from "next/navigation";
import FormContainer from "@/components/ui/Form/FormContainer";
import DetailFieldRow from "@/components/ui/Detail/DetailFieldRow";
import { useServiceSchedule } from "@/features/service-schedule/hooks/useServiceSchedules";
import Loading from "@/components/ui/Feedback/Loading";
import EmptyState from "@/components/ui/Feedback/EmptyState";
import ServiceScheduleHeader from "@/features/service-schedule/components/ServiceScheduleHeader";
import PrimaryButton from "@/components/ui/Button/PrimaryButton";
import EditIcon from "@/components/ui/Icons/Edit";
import ArchiveIcon from "@/components/ui/Icons/Archive";
import ConfirmModal from "@/components/ui/Modal/ConfirmModal";
import { useDeleteServiceSchedule } from "@/features/service-schedule/hooks/useServiceSchedules";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import { BreadcrumbItem } from "@/components/ui/Layout/Breadcrumbs";

export default function ServiceScheduleDetailPage() {
  const params = useParams();
  const id = params.id ? Number(params.id) : undefined;
  const { serviceSchedule, isPending, isError } = useServiceSchedule(id!);
  const router = useRouter();
  const { mutate: deleteServiceSchedule, isPending: isDeleting } =
    useDeleteServiceSchedule();
  const notify = useNotification();
  const [isDeleteModalOpen, setDeleteModalOpen] = React.useState(false);

  if (isPending) {
    return <Loading />;
  }
  if (isError || !serviceSchedule) {
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
        title={serviceSchedule.name}
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
          <DetailFieldRow label="Name" value={serviceSchedule.name} />
          <DetailFieldRow
            label="Tasks"
            value={
              <div className="flex flex-wrap gap-2 max-w-md break-words justify-end">
                {serviceSchedule.serviceTasks.length > 0 ? (
                  serviceSchedule.serviceTasks.map(task => (
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
              serviceSchedule.timeIntervalValue &&
              serviceSchedule.timeIntervalUnitLabel ? (
                `${serviceSchedule.timeIntervalValue} ${
                  serviceSchedule.timeIntervalValue === 1
                    ? serviceSchedule.timeIntervalUnitLabel.replace(/s$/, "")
                    : serviceSchedule.timeIntervalUnitLabel
                }`
              ) : serviceSchedule.mileageInterval ? (
                `${serviceSchedule.mileageInterval} km`
              ) : (
                <span className="text-gray-400">-</span>
              )
            }
          />
          <DetailFieldRow
            label="Buffer"
            value={
              serviceSchedule.timeBufferValue &&
              serviceSchedule.timeBufferUnitLabel ? (
                `${serviceSchedule.timeBufferValue} ${
                  serviceSchedule.timeBufferValue === 1
                    ? serviceSchedule.timeBufferUnitLabel.replace(/s$/, "")
                    : serviceSchedule.timeBufferUnitLabel
                }`
              ) : serviceSchedule.mileageBuffer ? (
                `${serviceSchedule.mileageBuffer} km`
              ) : (
                <span className="text-gray-400">-</span>
              )
            }
          />
          <DetailFieldRow
            label="First Service"
            value={
              serviceSchedule.firstServiceTimeValue &&
              serviceSchedule.firstServiceTimeUnitLabel ? (
                `${serviceSchedule.firstServiceTimeValue} ${
                  serviceSchedule.firstServiceTimeValue === 1
                    ? serviceSchedule.firstServiceTimeUnitLabel.replace(
                        /s$/,
                        "",
                      )
                    : serviceSchedule.firstServiceTimeUnitLabel
                }`
              ) : serviceSchedule.firstServiceMileage ? (
                `${serviceSchedule.firstServiceMileage} km`
              ) : (
                <span className="text-gray-400">-</span>
              )
            }
          />
          {/*
          <DetailFieldRow
            label="First Service Date"
              serviceSchedule.firstServiceDate
                ? new Date(serviceSchedule.firstServiceDate).toLocaleDateString()
                ? new Date(schedule.FirstServiceDate).toLocaleDateString()
                : <span className="text-gray-400">-</span>
            }
          */}
          <DetailFieldRow
            label="Active"
            value={serviceSchedule.isActive ? "Yes" : "No"}
            noBorder
          />
        </FormContainer>
      </div>
    </div>
  );
}
