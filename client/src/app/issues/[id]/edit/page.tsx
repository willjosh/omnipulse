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
  Combobox,
  ComboboxInput,
  ComboboxButton,
  ComboboxOptions,
  ComboboxOption,
} from "@headlessui/react";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { Autocomplete, TextField } from "@mui/material";
import { getTimeOptions, combineDateAndTime } from "@/app/_utils/dateTimeUtils";

// Utility to extract time in HH:mm from ISO string
function extractTimeFromISO(isoString: string | null | undefined): string {
  if (!isoString) return "";
  const date = new Date(isoString);
  if (isNaN(date.getTime())) return "";
  const hours = String(date.getHours()).padStart(2, "0");
  const minutes = String(date.getMinutes()).padStart(2, "0");
  return `${hours}:${minutes}`;
}

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

  // Prefill time fields when issue loads
  useEffect(() => {
    if (issue) {
      setForm({
        VehicleID: issue.VehicleID?.toString() || "",
        PriorityLevel: issue.PriorityLevel?.toString() || "",
        ReportedDate: issue.ReportedDate || "",
        Title: issue.Title || "",
        Description: issue.Description || "",
        Category: issue.Category?.toString() || "",
        Status: issue.Status?.toString() || "1",
        ReportedByUserID: issue.ReportedByUserID || "",
        ResolutionNotes: issue.ResolutionNotes || "",
        ResolvedDate: issue.ResolvedDate || "",
        ResolvedByUserID: issue.ResolvedByUserID || "",
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
    let reportedDateToSave = form.ReportedDate;
    if (reportedTime && form.ReportedDate) {
      const originalTime = extractTimeFromISO(form.ReportedDate);
      // Compare only the date part (YYYY-MM-DD)
      const originalDate = form.ReportedDate
        ? new Date(form.ReportedDate).toISOString().split("T")[0]
        : "";
      const currentDate = form.ReportedDate
        ? new Date(form.ReportedDate).toISOString().split("T")[0]
        : "";
      if (reportedTime !== originalTime || currentDate !== originalDate) {
        reportedDateToSave = combineDateAndTime(
          form.ReportedDate,
          reportedTime,
        );
      }
    }

    const updatedForm = { ...form, ReportedDate: reportedDateToSave };

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

  // For Reported Date time picker
  const [reportedTime, setReportedTime] = useState<string>("");
  // For Resolved Date time picker
  const timeOptions = getTimeOptions();
  const [resolvedTime, setResolvedTime] = useState<string>("");
  // For Resolved By search
  const [resolvedBySearch, setResolvedBySearch] = useState("");
  const resolvedByOptions = technicians.map((t: any) => ({
    value: t.id,
    label: `${t.FirstName} ${t.LastName}`,
  }));
  const filteredResolvedBy = resolvedBySearch
    ? resolvedByOptions.filter((u: any) =>
        u.label.toLowerCase().includes(resolvedBySearch.toLowerCase()),
      )
    : resolvedByOptions;
  const resolvedByUser =
    resolvedByOptions.find((u: any) => u.value === form.ResolvedByUserID) ||
    null;

  return (
    <div>
      <IssueHeader
        title={`Edit Issue #${id}`}
        breadcrumbs={breadcrumbs}
        onCancel={() => router.back()}
        onSave={handleSave}
        saveText={isPending ? "Saving..." : "Save Issue"}
        isSaving={isPending}
      />
      <IssueDetailsForm
        value={form}
        errors={errors}
        onChange={handleFormChange}
        disabled={isPending || isLoading}
      />
      {/* Resolution Details Section */}
      <FormContainer
        title="Resolution Details"
        className="max-w-2xl mx-auto w-full mt-6"
      >
        {/* Resolution Notes - same as Description */}
        <div className="mb-4">
          <label className="block text-sm font-medium mb-1">
            Resolution Notes
          </label>
          <textarea
            value={form.ResolutionNotes}
            onChange={e => handleFormChange("ResolutionNotes", e.target.value)}
            placeholder="Describe the resolution in detail..."
            className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white min-h-[100px] resize-y"
            disabled={isPending || isLoading}
          />
        </div>
        {/* Resolved Date - same as Reported Date */}
        <div className="mb-4">
          <label className="block text-sm font-medium mb-1">
            Resolved Date
          </label>
          <div className="flex">
            <div className="w-1/3 mr-4">
              <LocalizationProvider dateAdapter={AdapterDateFns}>
                <DatePicker
                  value={form.ResolvedDate ? new Date(form.ResolvedDate) : null}
                  onChange={date => {
                    let newTime = resolvedTime;
                    if (!newTime) {
                      newTime = timeOptions[0];
                      setResolvedTime(newTime);
                    }
                    const iso = combineDateAndTime(
                      date ? date.toISOString() : "",
                      newTime,
                    );
                    handleFormChange("ResolvedDate", iso);
                  }}
                  slotProps={{
                    textField: {
                      size: "small",
                      error: !!errors.ResolvedDate,
                      helperText: errors.ResolvedDate || undefined,
                    },
                  }}
                  disabled={isPending || isLoading}
                />
              </LocalizationProvider>
            </div>
            <div className="w-1/3">
              <Autocomplete
                options={timeOptions}
                value={resolvedTime}
                onChange={(_e, newValue) => {
                  setResolvedTime((newValue ?? "") as string);
                  const iso = combineDateAndTime(
                    form.ResolvedDate ?? "",
                    (newValue ?? "") as string,
                  );
                  handleFormChange("ResolvedDate", iso);
                }}
                renderInput={params => (
                  <TextField
                    {...params}
                    placeholder="Select time"
                    size="small"
                  />
                )}
                disabled={isPending || isLoading}
                ListboxProps={{ style: { maxHeight: 200, overflowY: "auto" } }}
              />
            </div>
          </div>
        </div>
        {/* Resolved By - same as Reported By */}
        <div className="mb-2">
          <label className="block text-sm font-medium mb-1">Resolved By</label>
          <Combobox
            value={resolvedByUser}
            onChange={u => u && handleFormChange("ResolvedByUserID", u.value)}
            disabled={isPending || isLoading}
          >
            <div className="relative">
              <ComboboxInput
                className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                displayValue={(user: { value: string; label: string } | null) =>
                  user?.label || ""
                }
                onChange={e =>
                  setResolvedBySearch(String(e.target.value ?? ""))
                }
                placeholder="Search users..."
                disabled={isPending || isLoading}
              />
              <ComboboxButton className="absolute inset-y-0 right-0 flex items-center pr-2">
                <svg
                  className="h-5 w-5 text-gray-400"
                  viewBox="0 0 20 20"
                  fill="none"
                  stroke="currentColor"
                >
                  <path
                    d="M7 7l3-3 3 3m0 6l-3 3-3-3"
                    strokeWidth="1.5"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  />
                </svg>
              </ComboboxButton>
              <ComboboxOptions className="absolute z-10 mt-1 w-full bg-white border border-gray-200 rounded-3xl shadow-lg max-h-60 overflow-auto">
                {filteredResolvedBy.length === 0 && (
                  <div className="px-4 py-2 text-gray-500">No users found.</div>
                )}
                {filteredResolvedBy.map((opt: any) => (
                  <ComboboxOption
                    key={opt.value}
                    value={opt}
                    className={({ active, selected }: any) =>
                      `cursor-pointer select-none px-4 py-2 flex items-center ${active ? "bg-blue-100" : ""}`
                    }
                  >
                    {({ selected }: any) => (
                      <>
                        <span className="flex-1">{opt.label}</span>
                        {selected && (
                          <svg
                            className="h-5 w-5 text-blue-600 ml-2"
                            fill="none"
                            stroke="currentColor"
                            viewBox="0 0 24 24"
                          >
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              strokeWidth="2"
                              d="M5 13l4 4L19 7"
                            />
                          </svg>
                        )}
                      </>
                    )}
                  </ComboboxOption>
                ))}
              </ComboboxOptions>
            </div>
          </Combobox>
        </div>
      </FormContainer>
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
