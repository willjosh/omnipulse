import React, { useState, useMemo, useEffect } from "react";
import FormContainer from "../../../components/ui/Form/FormContainer";
import FormField from "../../../components/ui/Form/FormField";
import {
  Combobox,
  ComboboxInput,
  ComboboxButton,
  ComboboxOptions,
  ComboboxOption,
} from "@headlessui/react";
import {
  getCategoryOptions,
  getPriorityOptions,
  getStatusOptions,
} from "@/features/issue/config/issueOptions";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { Autocomplete, TextField } from "@mui/material";
import { useVehicles } from "../../vehicle/hooks/useVehicles";
import {
  getTimeOptions,
  combineDateAndTimeLocal,
  extractTimeFromISO,
} from "@/utils/dateTimeUtils";
import { IssueDetailsFormProps } from "@/features/issue/types/issueFormType";
import { VehicleOption } from "@/features/issue/types/issueFormType";
import { VehicleWithLabels } from "@/features/vehicle/types/vehicleType";

const IssueDetailsForm: React.FC<IssueDetailsFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
  showStatus = true,
  statusEditable = true,
}) => {
  const timeOptions = getTimeOptions();
  // Search/filter state for vehicle dropdown
  const [vehicleSearch, setVehicleSearch] = useState("");
  const { vehicles, isPending: isLoadingVehicles } = useVehicles();
  const vehicleOptions = useMemo(
    () =>
      vehicles.map((v: VehicleWithLabels) => ({
        value: v.id.toString(),
        label: v.name,
      })),
    [vehicles],
  );

  const filteredVehicles = useMemo(() => {
    if (!vehicleSearch) return vehicleOptions;
    const searchLower = vehicleSearch.toLowerCase();
    return vehicleOptions.filter((v: VehicleOption) =>
      v.label.toLowerCase().includes(searchLower),
    );
  }, [vehicleSearch, vehicleOptions]);
  const selectedVehicle: VehicleOption | null =
    vehicleOptions.find((v: VehicleOption) => v.value === value.vehicleID) ||
    null;

  // Local state for time selection
  const [reportedTime, setReportedTime] = useState<string>("");

  // Helper function to parse formatted dates
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

  // Prefill reportedTime when value.ReportedDate changes
  useEffect(() => {
    setReportedTime(extractTimeFromISO(value.reportedDate));
  }, [value.reportedDate]);

  return (
    <FormContainer title="Details" className="mt-6 max-w-2xl mx-auto w-full">
      {/* Asset (Vehicle) Dropdown with search */}
      <FormField label="Asset" required error={errors.vehicleID}>
        <Combobox
          value={selectedVehicle}
          onChange={v => v && onChange("vehicleID", v.value)}
          disabled={disabled || isLoadingVehicles}
        >
          <div className="relative">
            <ComboboxInput
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              displayValue={(vehicle: VehicleOption | null) =>
                vehicle?.label || ""
              }
              onChange={e => setVehicleSearch(e.target.value)}
              placeholder="Search vehicles..."
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
              {isLoadingVehicles ? (
                <div className="px-4 py-2 text-gray-500">Loading...</div>
              ) : filteredVehicles.length === 0 ? (
                <div className="px-4 py-2 text-gray-500">
                  No vehicles found.
                </div>
              ) : (
                filteredVehicles.map((opt: VehicleOption) => (
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
                ))
              )}
            </ComboboxOptions>
          </div>
        </Combobox>
      </FormField>
      <FormField label="Priority Level" required error={errors.priorityLevel}>
        <Combobox
          value={
            getPriorityOptions().find(
              opt => opt.value === value.priorityLevel,
            ) || null
          }
          onChange={opt => opt && onChange("priorityLevel", opt.value)}
          disabled={disabled}
        >
          <div className="relative">
            <ComboboxInput
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              displayValue={(opt: { value: string; label: string } | null) =>
                opt?.label || ""
              }
              placeholder="Select priority..."
              disabled={disabled}
              readOnly
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
              {getPriorityOptions().map(opt => (
                <ComboboxOption
                  key={opt.value}
                  value={opt}
                  className={({ active, selected }) =>
                    `cursor-pointer select-none px-4 py-2 flex items-center ${active ? "bg-blue-100" : ""}`
                  }
                >
                  {({ selected }) => (
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
      </FormField>
      {/* Status Dropdown */}
      {showStatus && (
        <FormField label="Status" required error={errors.status}>
          <Combobox
            value={
              getStatusOptions().find(opt => opt.value === value.status) || null
            }
            onChange={opt => opt && onChange("status", opt.value)}
            disabled={disabled || !statusEditable}
          >
            <div className="relative">
              <ComboboxInput
                className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                displayValue={(opt: { value: string; label: string } | null) =>
                  opt?.label || ""
                }
                placeholder="Select status..."
                disabled={disabled || !statusEditable}
                readOnly
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
                {getStatusOptions().map(opt => (
                  <ComboboxOption
                    key={opt.value}
                    value={opt}
                    className={({ active, selected }) =>
                      `cursor-pointer select-none px-4 py-2 flex items-center ${active ? "bg-blue-100" : ""}`
                    }
                  >
                    {({ selected }) => (
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
        </FormField>
      )}
      {/* Category Dropdown */}
      <FormField label="Category" required error={errors.category}>
        <Combobox
          value={
            getCategoryOptions().find(opt => opt.value === value.category) ||
            null
          }
          onChange={opt => opt && onChange("category", opt.value)}
          disabled={disabled}
        >
          <div className="relative">
            <ComboboxInput
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              displayValue={(opt: { value: string; label: string } | null) =>
                opt?.label || ""
              }
              placeholder="Select category..."
              disabled={disabled}
              readOnly // disables typing, dropdown only
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
              {getCategoryOptions().map(opt => (
                <ComboboxOption
                  key={opt.value}
                  value={opt}
                  className={({ active, selected }) =>
                    `cursor-pointer select-none px-4 py-2 flex items-center ${active ? "bg-blue-100" : ""}`
                  }
                >
                  {({ selected }) => (
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
      </FormField>
      <FormField label="Reported Date" error={errors.reportedDate}>
        <div className="flex">
          <div className="w-1/3 mr-4">
            <LocalizationProvider dateAdapter={AdapterDateFns}>
              <DatePicker
                value={parseFormattedDate(value.reportedDate)}
                onChange={date => {
                  let newTime = reportedTime;
                  if (!newTime) {
                    newTime = timeOptions[0];
                    setReportedTime(newTime);
                  }
                  const iso = combineDateAndTimeLocal(
                    date ? date.toISOString() : "",
                    newTime,
                  );
                  onChange("reportedDate", iso);
                }}
                slotProps={{ textField: { size: "small" } }}
                disabled={disabled}
              />
            </LocalizationProvider>
          </div>
          <div className="w-1/3">
            <Autocomplete
              options={timeOptions}
              value={reportedTime}
              onChange={(_e, newValue) => {
                setReportedTime(newValue || "");
                const iso = combineDateAndTimeLocal(
                  value.reportedDate,
                  newValue || "",
                );
                onChange("reportedDate", iso);
              }}
              renderInput={params => (
                <TextField {...params} placeholder="Select time" size="small" />
              )}
              disabled={disabled}
              ListboxProps={{ style: { maxHeight: 200, overflowY: "auto" } }}
            />
          </div>
        </div>
      </FormField>
      <FormField label="Summary" required error={errors.title}>
        <input
          type="text"
          value={value.title}
          onChange={e => onChange("title", e.target.value)}
          placeholder="Enter summary"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white h-[40px]"
          disabled={disabled}
        />
      </FormField>
      {/* Description Field (Rich Text Editor placeholder) */}
      <FormField label="Description" error={errors.description}>
        {/* TODO: Replace with a real rich text editor (e.g., React Quill, Slate, Tiptap) */}
        <textarea
          value={value.description}
          onChange={e => onChange("description", e.target.value)}
          placeholder="Describe the issue in detail..."
          className="w-full border border-gray-300 rounded-3xl px-3 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white min-h-[100px] resize-y"
          disabled={disabled}
        />
      </FormField>
    </FormContainer>
  );
};

export default IssueDetailsForm;
