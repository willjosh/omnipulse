"use client";
import React, { useState } from "react";
import { useRouter } from "next/navigation";
import IssueHeader from "../../_features/issue/components/IssueHeader";
import IssueDetailsForm from "../../_features/issue/components/IssueDetailsForm";
import FormContainer from "../../_features/shared/form/FormContainer";
import SecondaryButton from "../../_features/shared/button/SecondaryButton";
import PrimaryButton from "../../_features/shared/button/PrimaryButton";
import { useCreateIssue } from "../../_hooks/issue/useIssues";
import {
  IssueFormState,
  validateIssueForm,
  mapFormToCreateIssueCommand,
  emptyIssueFormState,
} from "../../_utils/issueFormUtils";

export default function CreateIssueHeaderOnly() {
  const router = useRouter();

  // Form state
  const [form, setForm] = useState<IssueFormState>(emptyIssueFormState);
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
    if (!validate()) return;
    createIssue(toCreateIssueCommand(), {
      onSuccess: () => {
        router.push("/issues");
      },
    });
  };

  // Save & Add Another handler
  const handleSaveAndAddAnother = () => {
    if (!validate()) return;
    createIssue(toCreateIssueCommand(), {
      onSuccess: () => {
        setForm({
          VehicleID: "", // Relating an issue to a vehicle
          PriorityLevel: "", // Setting a priority level
          ReportedDate: "", // System auto-generates timestamp
          Title: "",
          Description: "", // Updating a detailed and formatted description
          Category: "", // Selecting an issue category
          Status: "1", // Selecting a status category
          ReportedByUserID: "",
        });
        setResetKey(k => k + 1);
      },
    });
  };

  const breadcrumbs = [
    { label: "Issues", href: "/issues" },
    { label: "New Issue" },
  ];

  return (
    <div>
      <IssueHeader
        title="New Issue"
        breadcrumbs={breadcrumbs}
        onCancel={() => router.back()}
        onSave={handleSave}
        saveText={isPending ? "Saving..." : "Save Issue"}
        isSaving={isPending}
      />
      <IssueDetailsForm
        key={resetKey}
        value={form}
        errors={errors}
        onChange={handleFormChange}
        disabled={isPending}
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
        <hr className="mb-6" />
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
