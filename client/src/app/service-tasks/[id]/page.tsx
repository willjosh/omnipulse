"use client";
import React, { useState } from "react";
import { useParams } from "next/navigation";
import FormContainer from "@/components/ui/Form/FormContainer";
import DetailFieldRow from "@/components/ui/Detail/DetailFieldRow";
import {
  useServiceTask,
  useDeleteServiceTask,
} from "@/features/service-task/hooks/useServiceTasks";
import { getServiceTaskCategoryLabel } from "@/features/service-task/utils/serviceTaskEnumHelper";
import Loading from "@/components/ui/Feedback/Loading";
import EmptyState from "@/components/ui/Feedback/EmptyState";
import ServiceTaskHeader from "@/features/service-task/components/ServiceTaskHeader";
import PrimaryButton from "@/components/ui/Button/PrimaryButton";
import EditIcon from "@/components/ui/Icons/Edit";
import { useRouter } from "next/navigation";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import ArchiveIcon from "@/components/ui/Icons/Archive";
import ConfirmModal from "@/components/ui/Modal/ConfirmModal";

export default function ServiceTaskDetailPage() {
  const params = useParams();
  const id = params.id ? Number(params.id) : undefined;
  const { serviceTask, isPending, isError } = useServiceTask(id!);
  const router = useRouter();
  const [isArchiveModalOpen, setArchiveModalOpen] = useState(false);
  const deleteServiceTask = useDeleteServiceTask();
  const notify = useNotification();

  if (isPending) {
    return <Loading />;
  }
  if (isError || !serviceTask) {
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
        title={serviceTask.name}
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
          <DetailFieldRow label="Name" value={serviceTask.name} />
          <DetailFieldRow
            label="Description"
            value={serviceTask.description || "-"}
          />
          <DetailFieldRow
            label="Estimated Labour Hours"
            value={serviceTask.estimatedLabourHours}
          />
          <DetailFieldRow
            label="Estimated Cost"
            value={`$${(serviceTask.estimatedCost ?? 0).toFixed(2)}`}
          />
          <DetailFieldRow
            label="Category"
            value={getServiceTaskCategoryLabel(serviceTask.categoryEnum)}
          />
          <DetailFieldRow
            label="Active"
            value={serviceTask.isActive ? "Yes" : "No"}
            noBorder
          />
        </FormContainer>
      </div>
    </div>
  );
}
