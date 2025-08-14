import React, { useState, useEffect, useMemo } from "react";
import FormContainer from "../../../components/ui/Form/FormContainer";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { Autocomplete, TextField } from "@mui/material";
import {
  Combobox,
  ComboboxInput,
  ComboboxButton,
  ComboboxOptions,
  ComboboxOption,
} from "@headlessui/react";
import {
  getTimeOptions,
  combineDateAndTimeLocal,
  extractTimeFromISO,
} from "@/utils/dateTimeUtils";

interface IssueResolutionFormProps {
  value: {
    resolutionNotes?: string;
    resolvedDate?: string;
    resolvedByUserID?: string;
  };
  errors: { [key: string]: string };
  onChange: (field: string, value: string) => void;
  disabled?: boolean;
  technicians: Array<{ id: string; firstName: string; lastName: string }>;
}

const IssueResolutionForm: React.FC<IssueResolutionFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
  technicians,
}) => {
  const timeOptions = getTimeOptions();
  const [resolvedTime, setResolvedTime] = useState<string>("");
  const [resolvedBySearch, setResolvedBySearch] = useState("");

  const parseFormattedDate = (
    formattedDate: string | null | undefined,
  ): Date | null => {
    if (!formattedDate) return null;
    const isoDate = new Date(formattedDate);
    if (!isNaN(isoDate.getTime())) {
      return isoDate;
    }
    try {
      const parsed = new Date(formattedDate);
      if (!isNaN(parsed.getTime())) {
        return parsed;
      }
    } catch (e) {
      console.warn("Failed to parse formatted date:", formattedDate);
    }
    return null;
  };

  useEffect(() => {
    const extractedTime = extractTimeFromISO(value.resolvedDate);
    if (extractedTime && extractedTime !== resolvedTime) {
      setResolvedTime(extractedTime);
    }
  }, [value.resolvedDate, resolvedTime]);

  const resolvedByOptions = useMemo(
    () =>
      technicians.map(t => ({
        value: String(t.id),
        label: `${t.firstName} ${t.lastName}`,
      })),
    [technicians],
  );
  const filteredResolvedBy = useMemo(
    () =>
      resolvedBySearch
        ? resolvedByOptions.filter(u =>
            u.label.toLowerCase().includes(resolvedBySearch.toLowerCase()),
          )
        : resolvedByOptions,
    [resolvedBySearch, resolvedByOptions],
  );
  const [selectedResolvedBy, setSelectedResolvedBy] = useState<{
    value: string;
    label: string;
  } | null>(null);

  useEffect(() => {
    const found =
      resolvedByOptions.find(u => u.value === String(value.resolvedByUserID)) ||
      null;
    if (found?.value !== selectedResolvedBy?.value) {
      setSelectedResolvedBy(found);
    }
  }, [resolvedByOptions, value.resolvedByUserID, selectedResolvedBy?.value]);

  return (
    <FormContainer
      title="Resolution Details"
      className="max-w-2xl mx-auto w-full mt-6"
    >
      {/* Resolution Notes */}
      <div className="mb-4">
        <label className="block text-sm font-medium mb-1">
          Resolution Notes
        </label>
        <textarea
          value={value.resolutionNotes}
          onChange={e => onChange("resolutionNotes", e.target.value)}
          placeholder="Describe the resolution in detail..."
          className={`w-full border rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white min-h-[100px] resize-y ${
            errors.resolutionNotes ? "border-red-500" : "border-gray-300"
          }`}
          disabled={disabled}
        />
        {errors.resolutionNotes && (
          <p className="text-red-500 text-sm mt-1">{errors.resolutionNotes}</p>
        )}
      </div>
      {/* Resolved Date */}
      <div className="mb-4">
        <label className="block text-sm font-medium mb-1">Resolved Date</label>
        <div className="flex">
          <div className="w-1/3 mr-4">
            <LocalizationProvider dateAdapter={AdapterDateFns}>
              <DatePicker
                value={parseFormattedDate(value.resolvedDate)}
                onChange={date => {
                  let newTime = resolvedTime;
                  if (!newTime) {
                    newTime = timeOptions[0];
                    setResolvedTime(newTime);
                  }
                  const iso = combineDateAndTimeLocal(
                    date ? date.toISOString() : "",
                    newTime,
                  );
                  onChange("resolvedDate", iso);
                }}
                slotProps={{
                  textField: {
                    size: "small",
                    error: !!errors.resolvedDate,
                    helperText: errors.resolvedDate,
                  },
                }}
                disabled={disabled}
              />
            </LocalizationProvider>
          </div>
          <div className="w-1/3">
            <Autocomplete
              options={timeOptions}
              value={resolvedTime}
              onChange={(_e, newValue) => {
                setResolvedTime(newValue || "");
                const iso = combineDateAndTimeLocal(
                  value.resolvedDate ?? "",
                  newValue || "",
                );
                onChange("resolvedDate", iso);
              }}
              renderInput={params => (
                <TextField {...params} placeholder="Select time" size="small" />
              )}
              disabled={disabled}
              ListboxProps={{ style: { maxHeight: 200, overflowY: "auto" } }}
            />
          </div>
        </div>
        {errors.resolvedDate && (
          <p className="text-red-500 text-sm mt-1">{errors.resolvedDate}</p>
        )}
      </div>
      {/* Resolved By */}
      <div className="mb-2">
        <label className="block text-sm font-medium mb-1">Resolved By</label>
        <Combobox
          value={selectedResolvedBy}
          onChange={u => {
            setSelectedResolvedBy(u);
            if (u) onChange("resolvedByUserID", u.value);
          }}
          disabled={disabled}
        >
          <div className="relative">
            <ComboboxInput
              className={`w-full border rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white ${
                errors.resolvedByUserID ? "border-red-500" : "border-gray-300"
              }`}
              displayValue={(user: { value: string; label: string } | null) =>
                user?.label || ""
              }
              onChange={e => setResolvedBySearch(String(e.target.value ?? ""))}
              placeholder="Search users..."
              disabled={disabled}
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
              {filteredResolvedBy.map(opt => (
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
        {errors.resolvedByUserID && (
          <p className="text-red-500 text-sm mt-1">{errors.resolvedByUserID}</p>
        )}
      </div>
    </FormContainer>
  );
};

export default IssueResolutionForm;
