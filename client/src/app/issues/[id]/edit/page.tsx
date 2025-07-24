"use client";
import React, { useEffect, useState } from "react";
import { useRouter, useParams } from "next/navigation";
import IssueHeader from "@/app/_features/issue/components/IssueHeader";
import IssueDetailsForm from "@/app/_features/issue/components/IssueDetailsForm";
import FormContainer from "@/app/_features/shared/form/FormContainer";
import SecondaryButton from "@/app/_features/shared/button/SecondaryButton";
import PrimaryButton from "@/app/_features/shared/button/PrimaryButton";
import { useIssue, useUpdateIssue } from "@/app/_hooks/issue/useIssues";
import { useTechnicians } from "@/app/_hooks/technician/useTechnicians";
import {
  IssueFormState,
  validateIssueForm,
  mapFormToUpdateIssueCommand,
  emptyIssueFormState,
} from "@/app/_utils/issueFormUtils";
import {
  combineDateAndTime,
  extractTimeFromISO,
} from "@/app/_utils/dateTimeUtils";
import IssueResolutionForm from "@/app/_features/issue/components/IssueResolutionForm";
import { IssueStatusEnum } from "@/app/_hooks/issue/issueEnum";

export default function EditIssuePage() {
  const router = useRouter();
  const params = useParams();
  const id = params?.id ? Number(params.id as string) : undefined;

  // Fetch issue data
  const issueId = typeof id === "number" && !isNaN(id) ? id : undefined;
  const { data: issue, isLoading } = useIssue(issueId as number);
  const { technicians } = useTechnicians();
  const { mutate: updateIssue, isPending } = useUpdateIssue();

  // Form state
  const [form, setForm] = useState<IssueFormState>(emptyIssueFormState);
  const [errors, setErrors] = useState<{ [key: string]: string }>({});

  // For Reported Date time picker
  const [reportedTime, setReportedTime] = useState<string>("");

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
        reportedByUserID: issue.reportedByUserID,
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
    if (!validate() || !id) return;

    // Only recombine if the user changed date or time
    let reportedDateToSave = form.reportedDate;
    if (reportedTime && form.reportedDate) {
      const originalTime = extractTimeFromISO(form.reportedDate);
      // Compare only the date part (YYYY-MM-DD)
      const originalDate = form.reportedDate
        ? new Date(form.reportedDate).toISOString().split("T")[0]
        : "";
      const currentDate = form.reportedDate
        ? new Date(form.reportedDate).toISOString().split("T")[0]
        : "";
      if (reportedTime !== originalTime || currentDate !== originalDate) {
        reportedDateToSave = combineDateAndTime(
          form.reportedDate,
          reportedTime,
        );
      }
    }

    const updatedForm = { ...form, reportedDate: reportedDateToSave };

    updateIssue(mapFormToUpdateIssueCommand(updatedForm, id), {
      onSuccess: () => {
        router.push("/issues");
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
            <PrimaryButton onClick={handleSave} disabled={isPending}>
              {isPending ? "Saving..." : "Save Issue"}
            </PrimaryButton>
          </>
        }
      />
      <IssueDetailsForm
        value={form}
        errors={errors}
        onChange={handleFormChange}
        disabled={isPending || isLoading}
        showStatus={true}
        statusEditable={true}
      />
      {/* Conditionally render Resolution Details if status is RESOLVED */}
      {String(form.status) === String(IssueStatusEnum.RESOLVED) && (
        <IssueResolutionForm
          value={form}
          errors={errors}
          onChange={handleFormChange}
          disabled={isPending || isLoading}
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
        <hr className="mb-6" />
        <div className="flex justify-between items-center">
          <SecondaryButton
            onClick={() => router.back()}
            /* disabled={isPending} */
          >
            Cancel
          </SecondaryButton>
          <div className="flex gap-3">
            <PrimaryButton onClick={handleSave} disabled={isPending}>
              {isPending ? "Saving..." : "Save Issue"}
            </PrimaryButton>
          </div>
        </div>
      </div>
    </div>
  );
}
