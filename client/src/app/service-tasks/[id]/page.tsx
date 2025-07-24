"use client";
import React, { useState } from "react";
import { useParams } from "next/navigation";
import FormContainer from "@/app/_features/shared/form/FormContainer";
import DetailFieldRow from "@/app/_features/shared/detail/DetailFieldRow";
import {
  useServiceTask,
  useDeleteServiceTask,
} from "@/app/_hooks/service-task/useServiceTasks";
import { getServiceTaskCategoryLabel } from "@/app/_utils/serviceTaskEnumHelper";
import Loading from "@/app/_features/shared/feedback/Loading";
import EmptyState from "@/app/_features/shared/feedback/EmptyState";
import ServiceTaskHeader from "@/app/_features/service-task/components/ServiceTaskHeader";
import PrimaryButton from "@/app/_features/shared/button/PrimaryButton";
import EditIcon from "@/app/_features/shared/icons/Edit";
import { useRouter } from "next/navigation";
import { useNotification } from "@/app/_features/shared/feedback/NotificationProvider";
import ArchiveIcon from "@/app/_features/shared/icons/Archive";
import ConfirmModal from "@/app/_features/shared/modal/ConfirmModal";

export default function ServiceTaskDetailPage() {
  const params = useParams();
  const id = params.id ? Number(params.id) : undefined;
  const { data: task, isPending, isError } = useServiceTask(id!);
  const router = useRouter();
  const [isArchiveModalOpen, setArchiveModalOpen] = useState(false);
  const deleteServiceTask = useDeleteServiceTask();
  const notify = useNotification();

  if (isPending) {
    return <Loading />;
  }
  if (isError || !task) {
    return (
      <EmptyState
        title="Service Task not found"
        message="The service task you are looking for does not exist or could not be loaded."
      />
    );
  }

  const breadcrumbs = [{ label: "Service Tasks", href: "/service-tasks" }];

  const handleArchive = () => {
    deleteServiceTask.mutate(id!, {
      onSuccess: () => {
        notify("Service task deleted successfully", "success");
        router.push("/service-tasks");
      },
      onError: error => {
        console.error(error);
        notify("Failed to delete service task", "error");
      },
    });
  };

  return (
    <div className="min-h-screen mx-auto bg-gray-50">
      <ServiceTaskHeader
        title={task.name}
        breadcrumbs={breadcrumbs}
        actions={
          <>
            <PrimaryButton
              onClick={() => router.push(`/service-tasks/${id}/edit`)}
            >
              <EditIcon /> Edit
            </PrimaryButton>
            <PrimaryButton
              className="bg-red-600 hover:bg-red-700"
              onClick={() => setArchiveModalOpen(true)}
            >
              <ArchiveIcon /> Delete
            </PrimaryButton>
            <ConfirmModal
              isOpen={isArchiveModalOpen}
              onClose={() => setArchiveModalOpen(false)}
              onConfirm={handleArchive}
              title="Archive Service Task"
              message="Are you sure you want to delete this service task? This action cannot be undone."
              confirmText="Delete"
              cancelText="Cancel"
            />
          </>
        }
      />
      <div className="m-4 px-6 pb-12">
        <FormContainer title="Details" className="max-w-2xl mx-auto">
          <DetailFieldRow label="Name" value={task.name} />
          <DetailFieldRow label="Description" value={task.description || "-"} />
          <DetailFieldRow
            label="Estimated Labour Hours"
            value={task.estimatedLabourHours}
          />
          <DetailFieldRow
            label="Estimated Cost"
            value={`$${(task.estimatedCost ?? 0).toFixed(2)}`}
          />
          <DetailFieldRow
            label="Category"
            value={getServiceTaskCategoryLabel(task.categoryEnum)}
          />
          <DetailFieldRow
            label="Active"
            value={task.isActive ? "Yes" : "No"}
            noBorder
          />
        </FormContainer>
      </div>
    </div>
  );
}
