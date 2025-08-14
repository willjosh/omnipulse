"use client";
import React, { useState } from "react";
import { useRouter } from "next/navigation";
import IssueHeader from "../../../features/issue/components/IssueHeader";
import IssueDetailsForm from "../../../features/issue/components/IssueDetailsForm";
import FormContainer from "../../../components/ui/Form/FormContainer";
import SecondaryButton from "../../../components/ui/Button/SecondaryButton";
import PrimaryButton from "../../../components/ui/Button/PrimaryButton";
import { useCreateIssue } from "@/features/issue/hooks/useIssues";
import {
  IssueFormState,
  validateIssueForm,
  mapFormToCreateIssueCommand,
  emptyIssueFormState,
} from "@/features/issue/utils/issueFormUtils";
import { getErrorMessage, getErrorFields } from "@/utils/fieldErrorUtils";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";

export default function CreateIssueHeaderOnly() {
  const router = useRouter();
  const notify = useNotification();

  // Form state
  const [form, setForm] = useState<IssueFormState>({ ...emptyIssueFormState });
  const [errors, setErrors] = useState<{ [key: string]: string }>({});
  const [resetKey, setResetKey] = useState(0); // for resetting form

  // Create issue mutation
  const { mutate: createIssue, isPending } = useCreateIssue();

  // Validation
  const validate = () => {
    const newErrors = validateIssueForm(form);
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // Controlled field change
  const handleFormChange = (field: string, value: string) => {
    setForm(f => ({ ...f, [field]: value }));
    setErrors(e => ({ ...e, [field]: "" }));
  };

  // Convert form state to CreateIssueCommand
  const toCreateIssueCommand = () => mapFormToCreateIssueCommand(form);

  // Save Issue handler
  const handleSave = () => {
    if (!validate()) {
      notify("Please fill all required fields", "error");
      return;
    }
    createIssue(toCreateIssueCommand(), {
      onSuccess: () => {
        router.push("/issues");
      },
      onError: (error: any) => {
        console.error("Failed to create issue:", error);

        const errorMessage = getErrorMessage(
          error,
          "Failed to create issue. Please check your input and try again.",
        );

        const fieldErrors = getErrorFields(error, [
          "vehicleID",
          "title",
          "description",
          "category",
          "priorityLevel",
          "status",
          "reportedByUserID",
          "reportedDate",
        ]);

        const newErrors: { [key: string]: string } = {};
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

        setErrors(newErrors);
        notify(errorMessage, "error");
      },
    });
  };

  // Save & Add Another handler
  const handleSaveAndAddAnother = () => {
    if (!validate()) {
      notify("Please fill all required fields", "error");
      return;
    }
    createIssue(toCreateIssueCommand(), {
      onSuccess: () => {
        setForm(emptyIssueFormState);
        setResetKey(k => k + 1);
      },
      onError: (error: any) => {
        console.error("Failed to create issue:", error);

        const errorMessage = getErrorMessage(
          error,
          "Failed to create issue. Please check your input and try again.",
        );

        const fieldErrors = getErrorFields(error, [
          "vehicleID",
          "title",
          "description",
          "category",
          "priorityLevel",
          "status",
          "reportedByUserID",
          "reportedDate",
        ]);

        const newErrors: { [key: string]: string } = {};
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

        setErrors(newErrors);
        notify(errorMessage, "error");
      },
    });
  };

  const breadcrumbs = [{ label: "Issues", href: "/issues" }];

  return (
    <div>
      <IssueHeader
        title="New Issue"
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
        key={resetKey}
        value={form}
        errors={errors}
        onChange={handleFormChange}
        disabled={isPending}
        showStatus={false}
        statusEditable={false}
      />
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
            onClick={() => router.back()} /* disabled={isPending} */
          >
            Cancel
          </SecondaryButton>
          <div className="flex gap-3">
            <SecondaryButton
              onClick={handleSaveAndAddAnother}
              disabled={isPending}
            >
              {isPending ? "Saving..." : "Save & Add Another"}
            </SecondaryButton>
            <PrimaryButton onClick={handleSave} disabled={isPending}>
              {isPending ? "Saving..." : "Save Issue"}
            </PrimaryButton>
          </div>
        </div>
      </div>
    </div>
  );
}
