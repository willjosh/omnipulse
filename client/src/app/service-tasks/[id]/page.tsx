"use client";
import React from "react";
import { useParams } from "next/navigation";
import FormContainer from "@/app/_features/shared/form/FormContainer";
import DetailFieldRow from "@/app/_features/shared/detail/DetailFieldRow";
import { useServiceTask } from "@/app/_hooks/service-task/useServiceTask";
import { getServiceTaskCategoryLabel } from "@/app/_utils/serviceTaskEnumHelper";
import Loading from "@/app/_features/shared/feedback/Loading";
import EmptyState from "@/app/_features/shared/feedback/EmptyState";
import ServiceTaskHeader from "@/app/_features/service-task/components/ServiceTaskHeader";

export default function ServiceTaskDetailPage() {
  const params = useParams();
  const id = params.id ? Number(params.id) : undefined;
  const { data: task, isPending, isError } = useServiceTask(id!);

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

  const breadcrumbs = [
    { label: "Service Tasks", href: "/service-tasks" },
    { label: task.Name },
  ];

  return (
    <div className="min-h-screen mx-auto bg-gray-50">
      <ServiceTaskHeader
        title={task.Name}
        breadcrumbs={breadcrumbs}
        actions={null}
      />
      <div className="m-4 px-6 pb-12">
        <FormContainer title="Details" className="max-w-2xl mx-auto">
          <DetailFieldRow label="Name" value={task.Name} />
          <DetailFieldRow label="Description" value={task.Description || "-"} />
          <DetailFieldRow
            label="Estimated Labour Hours"
            value={task.EstimatedLabourHours}
          />
          <DetailFieldRow
            label="Estimated Cost"
            value={`$${task.EstimatedCost.toFixed(2)}`}
          />
          <DetailFieldRow
            label="Category"
            value={getServiceTaskCategoryLabel(task.CategoryEnum)}
          />
          <DetailFieldRow
            label="Active"
            value={task.IsActive ? "Yes" : "No"}
            noBorder
          />
        </FormContainer>
      </div>
    </div>
  );
}
