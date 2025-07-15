import React, { useState, useMemo, useEffect } from "react";
import FormContainer from "../../shared/form/FormContainer";
import FormField from "../../shared/form/FormField";
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
} from "@/app/_utils/issueOptions";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { Autocomplete, TextField } from "@mui/material";
import { useTechnicians } from "../../../_hooks/technician/useTechnicians";
import { useVehicles } from "../../../_hooks/vehicle/useVehicles";
import { getTimeOptions, combineDateAndTime } from "@/app/_utils/dateTimeUtils";
import { IssueDetailsFormProps } from "@/app/_types/issueTypes";
import { VehicleOption } from "@/app/_types/vehicleTypes";

const IssueDetailsForm: React.FC<IssueDetailsFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
}) => {
  const timeOptions = getTimeOptions();
  // Search/filter state for vehicle dropdown
  const [vehicleSearch, setVehicleSearch] = useState("");
  const { vehicles, isLoadingVehicles } = useVehicles();
  const vehicleOptions = useMemo(
    () => vehicles.map(v => ({ value: v.id.toString(), label: v.Name })),
    [vehicles],
  );

  const filteredVehicles = useMemo(() => {
    if (!vehicleSearch) return vehicleOptions;
    const searchLower = vehicleSearch.toLowerCase();
    return vehicleOptions.filter(v =>
      v.label.toLowerCase().includes(searchLower),
    );
  }, [vehicleSearch, vehicleOptions]);
  const selectedVehicle: VehicleOption | null =
    vehicleOptions.find(v => v.value === value.VehicleID) || null;

  // Fetch technicians for Reported By dropdown
  const { technicians, isPending: isLoadingTechnicians } = useTechnicians();
  const usersList = technicians.map(t => ({
    value: t.id,
    label: `${t.FirstName} ${t.LastName}`,
  }));
  const [userSearch, setUserSearch] = useState("");
  const filteredUsers = useMemo(() => {
    if (!userSearch) return usersList;
    const searchLower = userSearch.toLowerCase();
    return usersList.filter(u => u.label.toLowerCase().includes(searchLower));
  }, [userSearch, usersList]);
  const selectedUser =
    usersList.find(u => u.value === value.ReportedByUserID) || null;

  // Local state for time selection
  const [reportedTime, setReportedTime] = useState<string>("");

  // Utility to extract time in HH:mm from ISO string
  function extractTimeFromISO(isoString: string | null | undefined): string {
    if (!isoString) return "";
    const date = new Date(isoString);
    if (isNaN(date.getTime())) return "";
    const hours = String(date.getHours()).padStart(2, "0");
    const minutes = String(date.getMinutes()).padStart(2, "0");
    return `${hours}:${minutes}`;
  }

  // Prefill reportedTime when value.ReportedDate changes
  useEffect(() => {
    setReportedTime(extractTimeFromISO(value.ReportedDate));
  }, [value.ReportedDate]);

  return (
    <FormContainer title="Details" className="mt-6 max-w-2xl mx-auto w-full">
      {/* Asset (Vehicle) Dropdown with search */}
      <FormField label="Asset" required error={errors.VehicleID}>
        <Combobox
          value={selectedVehicle}
          onChange={v => v && onChange("VehicleID", v.value)}
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
                filteredVehicles.map(opt => (
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
      <FormField label="Priority Level" required error={errors.PriorityLevel}>
        <Combobox
          value={
            getPriorityOptions().find(
              opt => opt.value === value.PriorityLevel,
            ) || null
          }
          onChange={opt => opt && onChange("PriorityLevel", opt.value)}
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
      {/* Category Dropdown */}
      <FormField label="Category" required error={errors.Category}>
        <Combobox
          value={
            getCategoryOptions().find(opt => opt.value === value.Category) ||
            null
          }
          onChange={opt => opt && onChange("Category", opt.value)}
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
      <FormField label="Reported Date" error={errors.ReportedDate}>
        <div className="flex">
          <div className="w-1/3 mr-4">
            <LocalizationProvider dateAdapter={AdapterDateFns}>
              <DatePicker
                value={
                  value.ReportedDate &&
                  !isNaN(new Date(value.ReportedDate).getTime())
                    ? new Date(value.ReportedDate)
                    : null
                }
                onChange={date => {
                  let newTime = reportedTime;
                  if (!newTime) {
                    newTime = timeOptions[0];
                    setReportedTime(newTime);
                  }
                  const iso = combineDateAndTime(
                    date ? date.toISOString() : "",
                    newTime,
                  );
                  onChange("ReportedDate", iso);
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
                const iso = combineDateAndTime(
                  value.ReportedDate,
                  newValue || "",
                );
                onChange("ReportedDate", iso);
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
      <FormField label="Summary" required error={errors.Title}>
        <input
          type="text"
          value={value.Title}
          onChange={e => onChange("Title", e.target.value)}
          placeholder="Enter summary"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white h-[40px]"
          disabled={disabled}
        />
      </FormField>
      {/* Description Field (Rich Text Editor placeholder) */}
      <FormField label="Description" error={errors.Description}>
        {/* TODO: Replace with a real rich text editor (e.g., React Quill, Slate, Tiptap) */}
        <textarea
          value={value.Description}
          onChange={e => onChange("Description", e.target.value)}
          placeholder="Describe the issue in detail..."
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white min-h-[100px] resize-y"
          disabled={disabled}
        />
      </FormField>
      <FormField label="Reported By" required error={errors.ReportedByUserID}>
        <Combobox
          value={selectedUser}
          onChange={u => u && onChange("ReportedByUserID", u.value)}
          disabled={disabled || isLoadingTechnicians}
        >
          <div className="relative">
            <ComboboxInput
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              displayValue={(user: { value: string; label: string } | null) =>
                user?.label || ""
              }
              onChange={e => setUserSearch(e.target.value)}
              placeholder="Search users..."
              disabled={disabled || isLoadingTechnicians}
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
              {filteredUsers.length === 0 && (
                <div className="px-4 py-2 text-gray-500">No users found.</div>
              )}
              {filteredUsers.map(opt => (
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
      </FormField>
    </FormContainer>
  );
};

export default IssueDetailsForm;
