import React from "react";
import FormContainer from "@/components/ui/Form/FormContainer";
import FormField from "@/components/ui/Form/FormField";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { Autocomplete, TextField } from "@mui/material";
import {
  TimeUnitEnum,
  ServiceScheduleTypeEnum,
} from "../types/serviceScheduleEnum";
import { ServiceTaskWithLabels } from "../../service-task/types/serviceTaskType";
import { Vehicle } from "../../vehicle/types/vehicleType";
import { ServiceProgram } from "../../service-program/types/serviceProgramType";
import {
  Combobox,
  ComboboxInput,
  ComboboxButton,
  ComboboxOptions,
  ComboboxOption,
} from "@headlessui/react";
import {
  getTimeOptions,
  combineDateAndTime,
  extractTimeFromISO,
} from "@/utils/dateTimeUtils";

export interface ServiceScheduleDetailsFormValues {
  name: string;
  timeIntervalValue?: number | string;
  timeIntervalUnit?: TimeUnitEnum | "";
  mileageInterval?: number | string;
  timeBufferValue?: number | string;
  timeBufferUnit?: TimeUnitEnum | "";
  mileageBuffer?: number | string;
  firstServiceDate?: string;
  firstServiceMileage?: number | string;
  serviceTaskIDs: number[];
  scheduleType: ServiceScheduleTypeEnum;
  serviceProgramID: number | string;
}

interface ServiceScheduleDetailsFormProps {
  value: ServiceScheduleDetailsFormValues;
  errors: Partial<Record<keyof ServiceScheduleDetailsFormValues, string>>;
  onChange: (field: keyof ServiceScheduleDetailsFormValues, value: any) => void;
  disabled?: boolean;
  showScheduleType?: boolean;
  showServiceProgram?: boolean;
  availableServiceTasks: ServiceTaskWithLabels[];
  availableVehicles: Vehicle[];
  availableServicePrograms: ServiceProgram[];
}

const timeUnitOptions = [
  { value: TimeUnitEnum.Hours, label: "Hours" },
  { value: TimeUnitEnum.Days, label: "Days" },
  { value: TimeUnitEnum.Weeks, label: "Weeks" },
];

const scheduleTypeOptions = [
  { value: ServiceScheduleTypeEnum.TIME, label: "Time-based" },
  { value: ServiceScheduleTypeEnum.MILEAGE, label: "Mileage-based" },
];

const ServiceScheduleDetailsForm: React.FC<ServiceScheduleDetailsFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
  showScheduleType = false,
  showServiceProgram = true,
  availableServiceTasks,
  availableServicePrograms,
}) => {
  const timeOptions = getTimeOptions();

  // Local state for time selection
  const [firstServiceTime, setFirstServiceTime] = React.useState<string>("");

  // Prefill time when date changes
  React.useEffect(() => {
    setFirstServiceTime(extractTimeFromISO(value.firstServiceDate));
  }, [value.firstServiceDate]);

  // Service Task Multi-Select Combobox
  const [taskSearch, setTaskSearch] = React.useState("");
  const filteredTasks = React.useMemo(() => {
    if (!taskSearch) return availableServiceTasks;
    const searchLower = taskSearch.toLowerCase();
    return availableServiceTasks.filter(task =>
      task.name.toLowerCase().includes(searchLower),
    );
  }, [taskSearch, availableServiceTasks]);
  const selectedTasks = availableServiceTasks.filter(task =>
    value.serviceTaskIDs.includes(task.id),
  );

  // Combobox search state for unit fields
  const [timeIntervalUnitSearch, setTimeIntervalUnitSearch] =
    React.useState("");
  const [timeBufferUnitSearch, setTimeBufferUnitSearch] = React.useState("");

  const filteredTimeIntervalUnitOptions = React.useMemo(() => {
    if (!timeIntervalUnitSearch) return timeUnitOptions;
    const searchLower = timeIntervalUnitSearch.toLowerCase();
    return timeUnitOptions.filter(opt =>
      opt.label.toLowerCase().includes(searchLower),
    );
  }, [timeIntervalUnitSearch]);
  const filteredTimeBufferUnitOptions = React.useMemo(() => {
    if (!timeBufferUnitSearch) return timeUnitOptions;
    const searchLower = timeBufferUnitSearch.toLowerCase();
    return timeUnitOptions.filter(opt =>
      opt.label.toLowerCase().includes(searchLower),
    );
  }, [timeBufferUnitSearch]);

  const selectedTimeIntervalUnit =
    timeUnitOptions.find(opt => opt.value === value.timeIntervalUnit) || null;
  const selectedTimeBufferUnit =
    timeUnitOptions.find(opt => opt.value === value.timeBufferUnit) || null;

  // Add search state for Schedule Type
  const [scheduleTypeSearch, setScheduleTypeSearch] = React.useState("");
  const filteredScheduleTypeOptions = React.useMemo(() => {
    if (!scheduleTypeSearch) return scheduleTypeOptions;
    const searchLower = scheduleTypeSearch.toLowerCase();
    return scheduleTypeOptions.filter(opt =>
      opt.label.toLowerCase().includes(searchLower),
    );
  }, [scheduleTypeSearch]);

  const selectedScheduleType =
    scheduleTypeOptions.find(opt => opt.value === value.scheduleType) || null;

  // Add search state for Service Program
  const [serviceProgramSearch, setServiceProgramSearch] = React.useState("");
  const filteredServiceProgramOptions = React.useMemo(() => {
    if (!serviceProgramSearch) return availableServicePrograms;
    const searchLower = serviceProgramSearch.toLowerCase();
    return availableServicePrograms.filter(opt =>
      opt.name.toLowerCase().includes(searchLower),
    );
  }, [serviceProgramSearch, availableServicePrograms]);

  return (
    <FormContainer title="Service Schedule Details">
      <FormField label="Name" required error={errors.name}>
        <input
          type="text"
          value={value.name}
          onChange={e => onChange("name", e.target.value)}
          placeholder="Enter schedule name"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
          disabled={disabled}
        />
      </FormField>

      {/* Schedule Type Selection - Only show when showScheduleType is true */}
      {showScheduleType && (
        <FormField
          label="Service Schedule Type"
          required
          error={errors.scheduleType}
        >
          <Combobox
            value={selectedScheduleType}
            onChange={opt => {
              if (opt) onChange("scheduleType", opt.value);
            }}
            disabled={disabled}
          >
            <div className="relative">
              <ComboboxInput
                className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                displayValue={(
                  opt: { value: ServiceScheduleTypeEnum; label: string } | null,
                ) => opt?.label || ""}
                onChange={e => setScheduleTypeSearch(e.target.value)}
                placeholder="Select or search schedule type..."
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
                {filteredScheduleTypeOptions.length === 0 ? (
                  <div className="px-4 py-2 text-gray-500">
                    No schedule types found.
                  </div>
                ) : (
                  filteredScheduleTypeOptions.map(opt => (
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
      )}

      {/* Time-based Fields - Only show when Time is selected */}
      {value.scheduleType === ServiceScheduleTypeEnum.TIME && (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <FormField label="Time Interval" error={errors.timeIntervalValue}>
              <div className="flex gap-2">
                <input
                  type="number"
                  min={0}
                  value={value.timeIntervalValue || ""}
                  onChange={e => onChange("timeIntervalValue", e.target.value)}
                  placeholder="e.g. 6"
                  className="w-24 border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                  disabled={disabled}
                />
                <Combobox
                  value={selectedTimeIntervalUnit}
                  onChange={opt =>
                    onChange("timeIntervalUnit", opt?.value || "")
                  }
                  disabled={disabled}
                >
                  <div className="relative">
                    <ComboboxInput
                      className="w-32 border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                      displayValue={(opt: { label?: string } | undefined) =>
                        opt?.label || ""
                      }
                      onChange={e => setTimeIntervalUnitSearch(e.target.value)}
                      placeholder="Select unit..."
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
                      {filteredTimeIntervalUnitOptions.length === 0 ? (
                        <div className="px-4 py-2 text-gray-500">
                          No units found.
                        </div>
                      ) : (
                        filteredTimeIntervalUnitOptions.map(opt => (
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
              </div>
            </FormField>
            <FormField label="Time Buffer" error={errors.timeBufferValue}>
              <div className="flex gap-2">
                <input
                  type="number"
                  min={0}
                  value={value.timeBufferValue || ""}
                  onChange={e => onChange("timeBufferValue", e.target.value)}
                  placeholder="e.g. 1"
                  className="w-24 border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                  disabled={disabled}
                />
                <Combobox
                  value={selectedTimeBufferUnit}
                  onChange={opt => onChange("timeBufferUnit", opt?.value || "")}
                  disabled={disabled}
                >
                  <div className="relative">
                    <ComboboxInput
                      className="w-32 border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                      displayValue={(opt: { label?: string } | undefined) =>
                        opt?.label || ""
                      }
                      onChange={e => setTimeBufferUnitSearch(e.target.value)}
                      placeholder="Select unit..."
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
                      {filteredTimeBufferUnitOptions.length === 0 ? (
                        <div className="px-4 py-2 text-gray-500">
                          No units found.
                        </div>
                      ) : (
                        filteredTimeBufferUnitOptions.map(opt => (
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
              </div>
            </FormField>
          </div>

          {/* First Service Date - Only show for Time-based schedules */}
          <div className="w-2/3">
            <FormField
              label="First Service Date"
              error={errors.firstServiceDate}
            >
              <div className="flex">
                <div className="w-1/2 mr-2">
                  <LocalizationProvider dateAdapter={AdapterDateFns}>
                    <DatePicker
                      value={
                        value.firstServiceDate &&
                        !isNaN(new Date(value.firstServiceDate).getTime())
                          ? new Date(value.firstServiceDate)
                          : null
                      }
                      onChange={date => {
                        if (!date) {
                          onChange("firstServiceDate", "");
                          return;
                        }

                        let newTime = firstServiceTime;
                        if (!newTime) {
                          newTime = timeOptions[0];
                          setFirstServiceTime(newTime);
                        }

                        // Create a new date object and set the time
                        // Use the date directly to avoid timezone issues
                        const newDate = new Date(date);

                        // Parse time more robustly
                        let hours = 0;
                        let minutes = 0;

                        if (newTime) {
                          // Handle different time formats: "12:00am", "12am", "12:30am", etc.
                          const timeMatch = newTime.match(
                            /(\d{1,2}):?(\d{0,2})(am|pm)/i,
                          );

                          if (timeMatch) {
                            hours = parseInt(timeMatch[1], 10);
                            minutes = timeMatch[2]
                              ? parseInt(timeMatch[2], 10)
                              : 0;
                            const meridian = timeMatch[3].toLowerCase();

                            // Convert to 24-hour format
                            if (meridian === "pm" && hours !== 12) hours += 12;
                            if (meridian === "am" && hours === 12) hours = 0;
                          }
                        }

                        // Set the time in local timezone
                        newDate.setHours(hours, minutes, 0, 0);

                        // Convert to ISO string but preserve local time
                        // This creates a date string that represents the local time
                        const year = newDate.getFullYear();
                        const month = String(newDate.getMonth() + 1).padStart(
                          2,
                          "0",
                        );
                        const day = String(newDate.getDate()).padStart(2, "0");
                        // Store without Z suffix to treat as local time, not UTC
                        const localISOString = `${year}-${month}-${day}T${String(hours).padStart(2, "0")}:${String(minutes).padStart(2, "0")}:00.000`;

                        onChange("firstServiceDate", localISOString);
                      }}
                      slotProps={{ textField: { size: "small" } }}
                      disabled={disabled}
                    />
                  </LocalizationProvider>
                </div>
                <div className="w-1/2">
                  <Autocomplete
                    options={timeOptions}
                    value={firstServiceTime}
                    onChange={(_e, newValue) => {
                      setFirstServiceTime(newValue || "");
                      if (value.firstServiceDate && newValue) {
                        // Create a new date object and set the time
                        const newDate = new Date(value.firstServiceDate);

                        // Parse time more robustly
                        let hours = 0;
                        let minutes = 0;

                        if (newValue) {
                          // Handle different time formats: "12:00am", "12am", "12:30am", etc.
                          const timeMatch = newValue.match(
                            /(\d{1,2}):?(\d{0,2})(am|pm)/i,
                          );

                          if (timeMatch) {
                            hours = parseInt(timeMatch[1], 10);
                            minutes = timeMatch[2]
                              ? parseInt(timeMatch[2], 10)
                              : 0;
                            const meridian = timeMatch[3].toLowerCase();

                            // Convert to 24-hour format
                            if (meridian === "pm" && hours !== 12) hours += 12;
                            if (meridian === "am" && hours === 12) hours = 0;
                          }
                        }

                        // Set the time in local timezone
                        newDate.setHours(hours, minutes, 0, 0);

                        // Convert to ISO string but preserve local time
                        const year = newDate.getFullYear();
                        const month = String(newDate.getMonth() + 1).padStart(
                          2,
                          "0",
                        );
                        const day = String(newDate.getDate()).padStart(2, "0");
                        // Store without Z suffix to treat as local time, not UTC
                        const localISOString = `${year}-${month}-${day}T${String(hours).padStart(2, "0")}:${String(minutes).padStart(2, "0")}:00.000`;

                        onChange("firstServiceDate", localISOString);
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
                    ListboxProps={{
                      style: { maxHeight: 200, overflowY: "auto" },
                    }}
                  />
                </div>
              </div>
            </FormField>
          </div>
        </>
      )}

      {/* Mileage-based Fields - Only show when Mileage is selected */}
      {value.scheduleType === ServiceScheduleTypeEnum.MILEAGE && (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <FormField
              label="Mileage Interval (km)"
              error={errors.mileageInterval}
            >
              <input
                type="number"
                min={0}
                value={value.mileageInterval || ""}
                onChange={e => onChange("mileageInterval", e.target.value)}
                placeholder="e.g. 10000"
                className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                disabled={disabled}
              />
            </FormField>
            <FormField label="Mileage Buffer (km)" error={errors.mileageBuffer}>
              <input
                type="number"
                min={0}
                value={value.mileageBuffer || ""}
                onChange={e => onChange("mileageBuffer", e.target.value)}
                placeholder="e.g. 500"
                className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                disabled={disabled}
              />
            </FormField>
          </div>

          {/* First Service Mileage - Only show for Mileage-based schedules */}
          <FormField
            label="First Service Mileage (km)"
            error={errors.firstServiceMileage}
          >
            <input
              type="number"
              min={0}
              value={value.firstServiceMileage || ""}
              onChange={e => onChange("firstServiceMileage", e.target.value)}
              placeholder="e.g. 5000"
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              disabled={disabled}
            />
          </FormField>
        </>
      )}

      {/* Service Tasks Selection */}
      <FormField label="Service Tasks" required error={errors.serviceTaskIDs}>
        <Combobox
          multiple
          value={selectedTasks}
          onChange={opts =>
            onChange(
              "serviceTaskIDs",
              opts.map((t: ServiceTaskWithLabels) => t.id),
            )
          }
          disabled={disabled}
        >
          <div className="relative">
            <ComboboxInput
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              displayValue={() => ""}
              onChange={e => setTaskSearch(e.target.value)}
              placeholder="Select or search service tasks..."
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
              {filteredTasks.length === 0 ? (
                <div className="px-4 py-2 text-gray-500">No tasks found.</div>
              ) : (
                filteredTasks.map(task => (
                  <ComboboxOption
                    key={task.id}
                    value={task}
                    className={({ active }) =>
                      `cursor-pointer select-none px-4 py-2 flex items-center ${active ? "bg-blue-100" : ""}`
                    }
                  >
                    <span className="flex-1">{task.name}</span>
                    {value.serviceTaskIDs.includes(task.id) && (
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
                  </ComboboxOption>
                ))
              )}
            </ComboboxOptions>
          </div>
          {/* Show selected as chips */}
          <div className="flex flex-wrap gap-2 mt-2">
            {selectedTasks.map(task => (
              <span
                key={task.id}
                className="inline-block bg-blue-100 text-blue-800 text-xs font-medium px-3 py-1 rounded-full"
              >
                {task.name}
              </span>
            ))}
          </div>
        </Combobox>
      </FormField>

      {showServiceProgram && (
        <FormField
          label="Service Program"
          required
          error={errors.serviceProgramID}
        >
          <Combobox
            value={
              availableServicePrograms.find(
                opt => opt.id === value.serviceProgramID,
              ) || null
            }
            onChange={opt => onChange("serviceProgramID", opt?.id || null)}
            disabled={disabled}
          >
            <div className="relative">
              <ComboboxInput
                className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                displayValue={(opt: ServiceProgram | undefined) =>
                  opt?.name || ""
                }
                onChange={e => setServiceProgramSearch(e.target.value)}
                placeholder="Select or search program..."
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
                {filteredServiceProgramOptions.map(opt => (
                  <ComboboxOption
                    key={opt.id}
                    value={opt}
                    className={({ active, selected }) =>
                      `cursor-pointer select-none px-4 py-2 flex items-center ${active ? "bg-blue-100" : ""}`
                    }
                  >
                    {({ selected }) => (
                      <>
                        <span className="flex-1">{opt.name}</span>
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
    </FormContainer>
  );
};

export default ServiceScheduleDetailsForm;
