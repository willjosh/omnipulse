"use client";
import React, { useEffect, useState } from "react";
import { useRouter, useParams } from "next/navigation";
import IssueHeader from "@/features/issue/components/IssueHeader";
import IssueDetailsForm from "@/features/issue/components/IssueDetailsForm";
import FormContainer from "@/components/ui/Form/FormContainer";
import SecondaryButton from "@/components/ui/Button/SecondaryButton";
import PrimaryButton from "@/components/ui/Button/PrimaryButton";
import { useIssue, useUpdateIssue } from "@/features/issue/hooks/useIssues";
import { useTechnicians } from "@/features/technician/hooks/useTechnicians";
import {
  IssueFormState,
  validateIssueForm,
  mapFormToUpdateIssueCommand,
  emptyIssueFormState,
} from "@/features/issue/utils/issueFormUtils";
import IssueResolutionForm from "@/features/issue/components/IssueResolutionForm";
import { IssueStatusEnum } from "@/features/issue/types/issueEnum";
import { getErrorMessage, getErrorFields } from "@/utils/fieldErrorUtils";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";

export default function EditIssuePage() {
  const router = useRouter();
  const params = useParams();
  const id = params?.id ? Number(params.id as string) : undefined;
  const notify = useNotification();

  // Fetch issue data
  const issueId = typeof id === "number" && !isNaN(id) ? id : undefined;
  const { issue, isPending: isLoadingIssue } = useIssue(issueId as number);
  const { technicians } = useTechnicians();
  const { mutate: updateIssue, isPending: isUpdatingIssue } = useUpdateIssue();

  // Form state
  const [form, setForm] = useState<IssueFormState>(emptyIssueFormState);
  const [errors, setErrors] = useState<{ [key: string]: string }>({});

  // Prefill time fields when issue loads
  useEffect(() => {
    if (issue) {
      setForm({
        vehicleID: issue.vehicleID?.toString() || "",
        priorityLevel: issue.priorityLevel?.toString() || "",
        reportedDate: issue.reportedDate || "",
        title: issue.title || "",
        description: issue.description || "",
        category: issue.category?.toString() || "",
        status: issue.status?.toString() || "1",
        reportedByUserID: issue.reportedByUserID || "",
        resolutionNotes: issue.resolutionNotes || "",
        resolvedDate: issue.resolvedDate || "",
        resolvedByUserID: issue.resolvedByUserID || "",
      });
    }
  }, [issue]);

  // Controlled field change
  const handleFormChange = (field: string, value: string) => {
    setForm(f => ({ ...f, [field]: value }));
    setErrors(e => ({ ...e, [field]: "" }));
  };

  // Validation
  const validate = () => {
    const newErrors = validateIssueForm(form);
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // Save handler
  const handleSave = () => {
    if (!id || !validate()) {
      notify("Please fill all required fields", "error");
      return;
    }

    updateIssue(mapFormToUpdateIssueCommand(form, id), {
      onSuccess: () => {
        router.push("/issues");
      },
      onError: (error: any) => {
        console.error("Failed to update issue:", error);

        // Get dynamic error message from backend
        const errorMessage = getErrorMessage(
          error,
          "Failed to update issue. Please check your input and try again.",
        );

        // Map backend errors to form fields
        const fieldErrors = getErrorFields(error, [
          "issueID",
          "vehicleID",
          "title",
          "description",
          "category",
          "priorityLevel",
          "status",
          "reportedByUserID",
          "reportedDate",
          "resolutionNotes",
          "resolvedDate",
          "resolvedByUserID",
        ]);

        // Set field-specific errors
        const newErrors: { [key: string]: string } = {};
        if (fieldErrors.issueID) {
          newErrors.issueID = "Invalid issue ID";
        }
        if (fieldErrors.vehicleID) {
          newErrors.vehicleID = "Invalid vehicle selection";
        }
        if (fieldErrors.title) {
          newErrors.title = "Invalid title";
        }
        if (fieldErrors.description) {
          newErrors.description = "Invalid description";
        }
        if (fieldErrors.category) {
          newErrors.category = "Invalid category";
        }
        if (fieldErrors.priorityLevel) {
          newErrors.priorityLevel = "Invalid priority level";
        }
        if (fieldErrors.status) {
          newErrors.status = "Invalid status";
        }
        if (fieldErrors.reportedByUserID) {
          newErrors.reportedByUserID = "Invalid reported by user";
        }
        if (fieldErrors.reportedDate) {
          newErrors.reportedDate = "Invalid reported date";
        }
        if (fieldErrors.resolutionNotes) {
          newErrors.resolutionNotes = "Invalid resolution notes";
        }
        if (fieldErrors.resolvedDate) {
          newErrors.resolvedDate = "Invalid resolved date";
        }
        if (fieldErrors.resolvedByUserID) {
          newErrors.resolvedByUserID = "Invalid resolved by user";
        }

        setErrors(newErrors);
        notify(errorMessage, "error");
      },
    });
  };

  const breadcrumbs = [
    { label: "Issues", href: "/issues" },
    { label: `#${id}` },
  ];

  return (
    <div>
      <IssueHeader
        title={`Edit Issue #${id}`}
        breadcrumbs={breadcrumbs}
        actions={
          <>
            <SecondaryButton onClick={() => router.back()}>
              Cancel
            </SecondaryButton>
            <PrimaryButton onClick={handleSave} disabled={isUpdatingIssue}>
              {isUpdatingIssue ? "Saving..." : "Save Issue"}
            </PrimaryButton>
          </>
        }
      />
      <IssueDetailsForm
        value={form}
        errors={errors}
        onChange={handleFormChange}
        disabled={isUpdatingIssue || isLoadingIssue}
        showStatus={true}
        statusEditable={true}
      />
      {/* Conditionally render Resolution Details if status is RESOLVED */}
      {String(form.status) === String(IssueStatusEnum.RESOLVED) && (
        <IssueResolutionForm
          value={form}
          errors={errors}
          onChange={handleFormChange}
          disabled={isUpdatingIssue || isLoadingIssue}
          technicians={technicians}
        />
      )}
      {/* Photos & Documents Row */}
      <div className="max-w-2xl mx-auto w-full mt-6 flex gap-6">
        {/* Photos Section */}
        <FormContainer title="Photos" className="flex-1">
          <div className="flex flex-col items-center justify-center border-2 border-dashed border-gray-300 rounded-lg p-6 bg-gray-50 text-gray-500 cursor-pointer hover:bg-gray-100 transition">
            <input
              type="file"
              accept="image/*"
              multiple
              className="hidden"
              id="photos-upload"
            />
            <label
              htmlFor="photos-upload"
              className="w-full h-full flex flex-col items-center cursor-pointer"
            >
              <span className="mb-2">
                Drag and drop photos here or click to pick files
              </span>
            </label>
          </div>
        </FormContainer>
        {/* Documents Section */}
        <FormContainer title="Documents" className="flex-1">
          <div className="flex flex-col items-center justify-center border-2 border-dashed border-gray-300 rounded-lg p-6 bg-gray-50 text-gray-500 cursor-pointer hover:bg-gray-100 transition">
            <input
              type="file"
              multiple
              className="hidden"
              id="documents-upload"
            />
            <label
              htmlFor="documents-upload"
              className="w-full h-full flex flex-col items-center cursor-pointer"
            >
              <span className="mb-2">
                Drag and drop documents here or click to pick files
              </span>
            </label>
          </div>
        </FormContainer>
      </div>
      {/* Footer Actions */}
      <div className="max-w-2xl mx-auto w-full mt-8 mb-12">
        <hr className="mb-6 border-gray-300" />
        <div className="flex justify-between items-center">
          <SecondaryButton
            onClick={() => router.back()}
            disabled={isUpdatingIssue}
          >
            Cancel
          </SecondaryButton>
          <div className="flex gap-3">
            <PrimaryButton onClick={handleSave} disabled={isUpdatingIssue}>
              {isUpdatingIssue ? "Saving..." : "Save Issue"}
            </PrimaryButton>
          </div>
        </div>
      </div>
    </div>
  );
}
