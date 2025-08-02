import React, { useState, useMemo, useEffect } from "react";
import FormContainer from "@/components/ui/Form/FormContainer";
import FormField from "@/components/ui/Form/FormField";
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
import { useVehicles } from "@/features/vehicle/hooks/useVehicles";
import { useTechnicians } from "@/features/technician/hooks/useTechnicians";
import {
  getTimeOptions,
  combineDateAndTime,
  extractTimeFromISO,
} from "@/utils/dateTimeUtils";
import {
  WorkTypeEnum,
  PriorityLevelEnum,
  WorkOrderStatusEnum,
} from "../types/workOrderEnum";
import { VehicleWithLabels } from "@/features/vehicle/types/vehicleType";

export interface WorkOrderDetailsFormValues {
  // Basic Details
  title: string;
  description?: string | null;
  vehicleID: number;
  workOrderType: WorkTypeEnum;
  priorityLevel: PriorityLevelEnum;
  status: WorkOrderStatusEnum;
  assignedToUserID: string;

  // Scheduling
  scheduledStartDate?: string | null;
  actualStartDate?: string | null;
  scheduledCompletionDate?: string | null;
  actualCompletionDate?: string | null;

  // Odometer
  startOdometer: number | null;
  endOdometer?: number | null;
}

interface WorkOrderDetailsFormProps {
  value: WorkOrderDetailsFormValues;
  errors: Partial<Record<keyof WorkOrderDetailsFormValues, string>>;
  onChange: (
    field: keyof WorkOrderDetailsFormValues,
    value: string | number | null,
  ) => void;
  disabled?: boolean;
  showStatus?: boolean;
  statusEditable?: boolean;
}

const workTypeOptions = [
  { value: WorkTypeEnum.SCHEDULED, label: "Scheduled" },
  { value: WorkTypeEnum.UNSCHEDULED, label: "Unscheduled" },
];

const priorityLevelOptions = [
  { value: PriorityLevelEnum.LOW, label: "Low" },
  { value: PriorityLevelEnum.MEDIUM, label: "Medium" },
  { value: PriorityLevelEnum.HIGH, label: "High" },
  { value: PriorityLevelEnum.CRITICAL, label: "Critical" },
];

const statusOptions = [
  { value: WorkOrderStatusEnum.CREATED, label: "Created" },
  { value: WorkOrderStatusEnum.ASSIGNED, label: "Assigned" },
  { value: WorkOrderStatusEnum.IN_PROGRESS, label: "In Progress" },
  { value: WorkOrderStatusEnum.WAITING_PARTS, label: "Waiting for Parts" },
  { value: WorkOrderStatusEnum.COMPLETED, label: "Completed" },
  { value: WorkOrderStatusEnum.CANCELLED, label: "Cancelled" },
  { value: WorkOrderStatusEnum.ON_HOLD, label: "On Hold" },
];

const WorkOrderDetailsForm: React.FC<WorkOrderDetailsFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
  showStatus = true,
  statusEditable = true,
}) => {
  const timeOptions = getTimeOptions();

  // Vehicle dropdown
  const [vehicleSearch, setVehicleSearch] = useState("");
  const { vehicles, isPending: isLoadingVehicles } = useVehicles();
  const vehicleOptions = useMemo(
    () =>
      vehicles.map((v: VehicleWithLabels) => ({ value: v.id, label: v.name })),
    [vehicles],
  );

  const filteredVehicles = useMemo(() => {
    if (!vehicleSearch) return vehicleOptions;
    const searchLower = vehicleSearch.toLowerCase();
    return vehicleOptions.filter(v =>
      v.label.toLowerCase().includes(searchLower),
    );
  }, [vehicleSearch, vehicleOptions]);

  const selectedVehicle =
    vehicleOptions.find(v => v.value === value.vehicleID) || null;

  // Technician dropdown
  const { technicians, isPending: isLoadingTechnicians } = useTechnicians({
    PageNumber: 1,
    PageSize: 100,
    Search: "",
  });
  const technicianOptions = useMemo(
    () =>
      technicians.map(
        (t: { id: string; firstName: string; lastName: string }) => ({
          value: t.id,
          label: `${t.firstName} ${t.lastName}`,
        }),
      ),
    [technicians],
  );

  const [technicianSearch, setTechnicianSearch] = useState("");
  const filteredTechnicians = useMemo(() => {
    if (!technicianSearch) return technicianOptions;
    const searchLower = technicianSearch.toLowerCase();
    return technicianOptions.filter(t =>
      t.label.toLowerCase().includes(searchLower),
    );
  }, [technicianSearch, technicianOptions]);

  const selectedTechnician =
    technicianOptions.find(t => t.value === value.assignedToUserID) || null;

  // Local state for time selection
  const [scheduledStartTime, setScheduledStartTime] = useState<string>("");
  const [actualStartTime, setActualStartTime] = useState<string>("");
  const [scheduledCompletionTime, setScheduledCompletionTime] =
    useState<string>("");
  const [actualCompletionTime, setActualCompletionTime] = useState<string>("");

  // Prefill times when dates change
  useEffect(() => {
    setScheduledStartTime(extractTimeFromISO(value.scheduledStartDate));
  }, [value.scheduledStartDate]);

  useEffect(() => {
    setActualStartTime(extractTimeFromISO(value.actualStartDate));
  }, [value.actualStartDate]);

  useEffect(() => {
    setScheduledCompletionTime(
      extractTimeFromISO(value.scheduledCompletionDate),
    );
  }, [value.scheduledCompletionDate]);

  useEffect(() => {
    setActualCompletionTime(extractTimeFromISO(value.actualCompletionDate));
  }, [value.actualCompletionDate]);

  return (
    <FormContainer title="Details" className="mt-6 max-w-4xl mx-auto w-full">
      {/* Title */}
      <FormField label="Title" required error={errors.title}>
        <input
          type="text"
          value={value.title}
          onChange={e => onChange("title", e.target.value)}
          placeholder="Enter work order title"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white h-[40px]"
          disabled={disabled}
        />
      </FormField>

      {/* Vehicle */}
      <FormField label="Vehicle" required error={errors.vehicleID}>
        <Combobox
          value={selectedVehicle}
          onChange={v => v && onChange("vehicleID", v.value)}
          disabled={disabled || isLoadingVehicles}
        >
          <div className="relative">
            <ComboboxInput
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              displayValue={(
                vehicle: { value: number; label: string } | null,
              ) => vehicle?.label || ""}
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
                ))
              )}
            </ComboboxOptions>
          </div>
        </Combobox>
      </FormField>

      {/* Work Order Type */}
      <FormField label="Work Order Type" required error={errors.workOrderType}>
        <Combobox
          value={
            workTypeOptions.find(opt => opt.value === value.workOrderType) ||
            null
          }
          onChange={opt => opt && onChange("workOrderType", opt.value)}
          disabled={disabled}
        >
          <div className="relative">
            <ComboboxInput
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              displayValue={(opt: { value: number; label: string } | null) =>
                opt?.label || ""
              }
              placeholder="Select work order type..."
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
              {workTypeOptions.map(opt => (
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

      {/* Priority Level */}
      <FormField label="Priority Level" required error={errors.priorityLevel}>
        <Combobox
          value={
            priorityLevelOptions.find(
              opt => opt.value === value.priorityLevel,
            ) || null
          }
          onChange={opt => opt && onChange("priorityLevel", opt.value)}
          disabled={disabled}
        >
          <div className="relative">
            <ComboboxInput
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              displayValue={(opt: { value: number; label: string } | null) =>
                opt?.label || ""
              }
              placeholder="Select priority level..."
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
              {priorityLevelOptions.map(opt => (
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

      {/* Status */}
      {showStatus && (
        <FormField label="Status" required error={errors.status}>
          <Combobox
            value={
              statusOptions.find(opt => opt.value === value.status) || null
            }
            onChange={opt => opt && onChange("status", opt.value)}
            disabled={disabled || !statusEditable}
          >
            <div className="relative">
              <ComboboxInput
                className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                displayValue={(opt: { value: number; label: string } | null) =>
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
                {statusOptions.map(opt => (
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

      {/* Assigned To */}
      <FormField label="Assigned To" required error={errors.assignedToUserID}>
        <Combobox
          value={selectedTechnician}
          onChange={t => t && onChange("assignedToUserID", t.value)}
          disabled={disabled || isLoadingTechnicians}
        >
          <div className="relative">
            <ComboboxInput
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              displayValue={(
                technician: { value: string; label: string } | null,
              ) => technician?.label || ""}
              onChange={e => setTechnicianSearch(e.target.value)}
              placeholder="Search technicians..."
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
              {filteredTechnicians.length === 0 && (
                <div className="px-4 py-2 text-gray-500">
                  No technicians found.
                </div>
              )}
              {filteredTechnicians.map(opt => (
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

      {/* Scheduled Start Date and Actual Start Date */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <FormField
          label="Scheduled Start Date"
          error={errors.scheduledStartDate}
        >
          <div className="flex">
            <div className="w-1/2 mr-2">
              <LocalizationProvider dateAdapter={AdapterDateFns}>
                <DatePicker
                  value={
                    value.scheduledStartDate &&
                    !isNaN(new Date(value.scheduledStartDate).getTime())
                      ? new Date(value.scheduledStartDate)
                      : null
                  }
                  onChange={date => {
                    let newTime = scheduledStartTime;
                    if (!newTime) {
                      newTime = timeOptions[0];
                      setScheduledStartTime(newTime);
                    }
                    const iso = combineDateAndTime(
                      date ? date.toISOString() : "",
                      newTime,
                    );
                    onChange("scheduledStartDate", iso);
                  }}
                  slotProps={{ textField: { size: "small" } }}
                  disabled={disabled}
                />
              </LocalizationProvider>
            </div>
            <div className="w-1/2">
              <Autocomplete
                options={timeOptions}
                value={scheduledStartTime}
                onChange={(_e, newValue) => {
                  setScheduledStartTime(newValue || "");
                  if (value.scheduledStartDate && newValue) {
                    const iso = combineDateAndTime(
                      value.scheduledStartDate,
                      newValue,
                    );
                    onChange("scheduledStartDate", iso);
                  }
                }}
                renderInput={params => (
                  <TextField
                    {...params}
                    placeholder="Select time"
                    size="small"
                  />
                )}
                disabled={disabled}
                ListboxProps={{ style: { maxHeight: 200, overflowY: "auto" } }}
              />
            </div>
          </div>
        </FormField>

        <FormField label="Actual Start Date" error={errors.actualStartDate}>
          <div className="flex">
            <div className="w-1/2 mr-2">
              <LocalizationProvider dateAdapter={AdapterDateFns}>
                <DatePicker
                  value={
                    value.actualStartDate &&
                    !isNaN(new Date(value.actualStartDate).getTime())
                      ? new Date(value.actualStartDate)
                      : null
                  }
                  onChange={date => {
                    let newTime = actualStartTime;
                    if (!newTime) {
                      newTime = timeOptions[0];
                      setActualStartTime(newTime);
                    }
                    const iso = combineDateAndTime(
                      date ? date.toISOString() : "",
                      newTime,
                    );
                    onChange("actualStartDate", iso);
                  }}
                  slotProps={{ textField: { size: "small" } }}
                  disabled={disabled}
                />
              </LocalizationProvider>
            </div>
            <div className="w-1/2">
              <Autocomplete
                options={timeOptions}
                value={actualStartTime}
                onChange={(_e, newValue) => {
                  setActualStartTime(newValue || "");
                  if (value.actualStartDate && newValue) {
                    const iso = combineDateAndTime(
                      value.actualStartDate,
                      newValue,
                    );
                    onChange("actualStartDate", iso);
                  }
                }}
                renderInput={params => (
                  <TextField
                    {...params}
                    placeholder="Select time"
                    size="small"
                  />
                )}
                disabled={disabled}
                ListboxProps={{ style: { maxHeight: 200, overflowY: "auto" } }}
              />
            </div>
          </div>
        </FormField>
      </div>

      {/* Expected Completion Date and Actual Completion Date */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <FormField
          label="Expected Completion Date"
          error={errors.scheduledCompletionDate}
        >
          <div className="flex">
            <div className="w-1/2 mr-2">
              <LocalizationProvider dateAdapter={AdapterDateFns}>
                <DatePicker
                  value={
                    value.scheduledCompletionDate &&
                    !isNaN(new Date(value.scheduledCompletionDate).getTime())
                      ? new Date(value.scheduledCompletionDate)
                      : null
                  }
                  onChange={date => {
                    let newTime = scheduledCompletionTime;
                    if (!newTime) {
                      newTime = timeOptions[0];
                      setScheduledCompletionTime(newTime);
                    }
                    const iso = combineDateAndTime(
                      date ? date.toISOString() : "",
                      newTime,
                    );
                    onChange("scheduledCompletionDate", iso);
                  }}
                  slotProps={{ textField: { size: "small" } }}
                  disabled={disabled}
                />
              </LocalizationProvider>
            </div>
            <div className="w-1/2">
              <Autocomplete
                options={timeOptions}
                value={scheduledCompletionTime}
                onChange={(_e, newValue) => {
                  setScheduledCompletionTime(newValue || "");
                  if (value.scheduledCompletionDate && newValue) {
                    const iso = combineDateAndTime(
                      value.scheduledCompletionDate,
                      newValue,
                    );
                    onChange("scheduledCompletionDate", iso);
                  }
                }}
                renderInput={params => (
                  <TextField
                    {...params}
                    placeholder="Select time"
                    size="small"
                  />
                )}
                disabled={disabled}
                ListboxProps={{ style: { maxHeight: 200, overflowY: "auto" } }}
              />
            </div>
          </div>
        </FormField>

        <FormField
          label="Actual Completion Date"
          error={errors.actualCompletionDate}
        >
          <div className="flex">
            <div className="w-1/2 mr-2">
              <LocalizationProvider dateAdapter={AdapterDateFns}>
                <DatePicker
                  value={
                    value.actualCompletionDate &&
                    !isNaN(new Date(value.actualCompletionDate).getTime())
                      ? new Date(value.actualCompletionDate)
                      : null
                  }
                  onChange={date => {
                    let newTime = actualCompletionTime;
                    if (!newTime) {
                      newTime = timeOptions[0];
                      setActualCompletionTime(newTime);
                    }
                    const iso = combineDateAndTime(
                      date ? date.toISOString() : "",
                      newTime,
                    );
                    onChange("actualCompletionDate", iso);
                  }}
                  slotProps={{ textField: { size: "small" } }}
                  disabled={disabled}
                />
              </LocalizationProvider>
            </div>
            <div className="w-1/2">
              <Autocomplete
                options={timeOptions}
                value={actualCompletionTime}
                onChange={(_e, newValue) => {
                  setActualCompletionTime(newValue || "");
                  if (value.actualCompletionDate && newValue) {
                    const iso = combineDateAndTime(
                      value.actualCompletionDate,
                      newValue,
                    );
                    onChange("actualCompletionDate", iso);
                  }
                }}
                renderInput={params => (
                  <TextField
                    {...params}
                    placeholder="Select time"
                    size="small"
                  />
                )}
                disabled={disabled}
                ListboxProps={{ style: { maxHeight: 200, overflowY: "auto" } }}
              />
            </div>
          </div>
        </FormField>
      </div>

      {/* Start Odometer */}
      <FormField label="Start Odometer" required error={errors.startOdometer}>
        <input
          type="number"
          min={0}
          step={0.1}
          value={value.startOdometer || ""}
          onChange={e =>
            onChange(
              "startOdometer",
              e.target.value ? Number(e.target.value) : null,
            )
          }
          placeholder="Enter start odometer reading"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
          disabled={disabled}
        />
      </FormField>

      {/* End Odometer */}
      <FormField label="End Odometer" error={errors.endOdometer}>
        <input
          type="number"
          min={0}
          step={0.1}
          value={value.endOdometer || ""}
          onChange={e =>
            onChange(
              "endOdometer",
              e.target.value ? Number(e.target.value) : null,
            )
          }
          placeholder="Enter end odometer reading (optional)"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
          disabled={disabled}
        />
      </FormField>

      {/* Description */}
      <FormField label="Description" error={errors.description}>
        <textarea
          value={value.description || ""}
          onChange={e => onChange("description", e.target.value)}
          placeholder="Describe the work order in detail..."
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white min-h-[100px] resize-y"
          disabled={disabled}
        />
      </FormField>
    </FormContainer>
  );
};

export default WorkOrderDetailsForm;
