import React from "react";
import FormContainer from "@/app/_features/shared/form/FormContainer";
import FormField from "@/app/_features/shared/form/FormField";
import { TimeUnitEnum } from "@/app/_hooks/service-schedule/serviceScheduleEnum";
import { ServiceTaskWithLabels } from "@/app/_hooks/service-task/serviceTaskType";
import { Vehicle } from "@/app/_hooks/vehicle/vehicleType";
import {
  Combobox,
  ComboboxInput,
  ComboboxButton,
  ComboboxOptions,
  ComboboxOption,
} from "@headlessui/react";

export interface ServiceScheduleDetailsFormValues {
  Name: string;
  TimeIntervalValue?: number | string;
  TimeIntervalUnit?: TimeUnitEnum | "";
  MileageInterval?: number | string;
  TimeBufferValue?: number | string;
  TimeBufferUnit?: TimeUnitEnum | "";
  MileageBuffer?: number | string;
  FirstServiceTimeValue?: number | string;
  FirstServiceTimeUnit?: TimeUnitEnum | "";
  FirstServiceMileage?: number | string;
  ServiceTaskIDs: number[];
  IsActive: boolean;
  ServiceProgramID: number | string;
}

interface ServiceScheduleDetailsFormProps {
  value: ServiceScheduleDetailsFormValues;
  errors: Partial<Record<keyof ServiceScheduleDetailsFormValues, string>>;
  onChange: (field: keyof ServiceScheduleDetailsFormValues, value: any) => void;
  disabled?: boolean;
  showIsActive?: boolean;
  availableServiceTasks: ServiceTaskWithLabels[];
  availableVehicles: Vehicle[];
}

const timeUnitOptions = [
  { value: TimeUnitEnum.Hours, label: "Hours" },
  { value: TimeUnitEnum.Days, label: "Days" },
  { value: TimeUnitEnum.Weeks, label: "Weeks" },
];

const serviceProgramOptions = [
  { value: 1, label: "Standard Maintenance" },
  { value: 2, label: "Heavy Duty Program" },
  { value: 3, label: "Seasonal Service" },
  { value: 4, label: "Warranty Program" },
];

const ServiceScheduleDetailsForm: React.FC<ServiceScheduleDetailsFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
  showIsActive = false,
  availableServiceTasks,
}) => {
  // Service Task Multi-Select Combobox
  const [taskSearch, setTaskSearch] = React.useState("");
  const filteredTasks = React.useMemo(() => {
    if (!taskSearch) return availableServiceTasks;
    const searchLower = taskSearch.toLowerCase();
    return availableServiceTasks.filter(task =>
      task.Name.toLowerCase().includes(searchLower),
    );
  }, [taskSearch, availableServiceTasks]);
  const selectedTasks = availableServiceTasks.filter(task =>
    value.ServiceTaskIDs.includes(task.id),
  );

  // Combobox search state for unit fields
  const [timeIntervalUnitSearch, setTimeIntervalUnitSearch] =
    React.useState("");
  const [timeBufferUnitSearch, setTimeBufferUnitSearch] = React.useState("");
  const [firstServiceTimeUnitSearch, setFirstServiceTimeUnitSearch] =
    React.useState("");

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
  const filteredFirstServiceTimeUnitOptions = React.useMemo(() => {
    if (!firstServiceTimeUnitSearch) return timeUnitOptions;
    const searchLower = firstServiceTimeUnitSearch.toLowerCase();
    return timeUnitOptions.filter(opt =>
      opt.label.toLowerCase().includes(searchLower),
    );
  }, [firstServiceTimeUnitSearch]);

  const selectedTimeIntervalUnit =
    timeUnitOptions.find(opt => opt.value === value.TimeIntervalUnit) || null;
  const selectedTimeBufferUnit =
    timeUnitOptions.find(opt => opt.value === value.TimeBufferUnit) || null;
  const selectedFirstServiceTimeUnit =
    timeUnitOptions.find(opt => opt.value === value.FirstServiceTimeUnit) ||
    null;

  // Add search state for Service Program
  const [serviceProgramSearch, setServiceProgramSearch] = React.useState("");
  const filteredServiceProgramOptions = React.useMemo(() => {
    if (!serviceProgramSearch) return serviceProgramOptions;
    const searchLower = serviceProgramSearch.toLowerCase();
    return serviceProgramOptions.filter(opt =>
      opt.label.toLowerCase().includes(searchLower),
    );
  }, [serviceProgramSearch]);

  return (
    <FormContainer title="Service Schedule Details">
      <FormField label="Name" required error={errors.Name}>
        <input
          type="text"
          value={value.Name}
          onChange={e => onChange("Name", e.target.value)}
          placeholder="Enter schedule name"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
          disabled={disabled}
        />
      </FormField>
      {/* Frequency Section */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <FormField label="Time Interval" error={errors.TimeIntervalValue}>
          <div className="flex gap-2">
            <input
              type="number"
              min={0}
              value={value.TimeIntervalValue || ""}
              onChange={e => onChange("TimeIntervalValue", e.target.value)}
              placeholder="e.g. 6"
              className="w-24 border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              disabled={disabled}
            />
            <Combobox
              value={selectedTimeIntervalUnit}
              onChange={opt => onChange("TimeIntervalUnit", opt?.value || "")}
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
        <FormField label="Mileage Interval (km)" error={errors.MileageInterval}>
          <input
            type="number"
            min={0}
            value={value.MileageInterval || ""}
            onChange={e => onChange("MileageInterval", e.target.value)}
            placeholder="e.g. 10000"
            className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
            disabled={disabled}
          />
        </FormField>
      </div>
      {/* Buffer Section */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <FormField label="Time Buffer" error={errors.TimeBufferValue}>
          <div className="flex gap-2">
            <input
              type="number"
              min={0}
              value={value.TimeBufferValue || ""}
              onChange={e => onChange("TimeBufferValue", e.target.value)}
              placeholder="e.g. 1"
              className="w-24 border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              disabled={disabled}
            />
            <Combobox
              value={selectedTimeBufferUnit}
              onChange={opt => onChange("TimeBufferUnit", opt?.value || "")}
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
        <FormField label="Mileage Buffer (km)" error={errors.MileageBuffer}>
          <input
            type="number"
            min={0}
            value={value.MileageBuffer || ""}
            onChange={e => onChange("MileageBuffer", e.target.value)}
            placeholder="e.g. 500"
            className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
            disabled={disabled}
          />
        </FormField>
      </div>
      {/* First Service Section */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <FormField
          label="First Service Time"
          error={errors.FirstServiceTimeValue}
        >
          <div className="flex gap-2">
            <input
              type="number"
              min={0}
              value={value.FirstServiceTimeValue || ""}
              onChange={e => onChange("FirstServiceTimeValue", e.target.value)}
              placeholder="e.g. 12"
              className="w-24 border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              disabled={disabled}
            />
            <Combobox
              value={selectedFirstServiceTimeUnit}
              onChange={opt =>
                onChange("FirstServiceTimeUnit", opt?.value || "")
              }
              disabled={disabled}
            >
              <div className="relative">
                <ComboboxInput
                  className="w-32 border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                  displayValue={(opt: { label?: string } | undefined) =>
                    opt?.label || ""
                  }
                  onChange={e => setFirstServiceTimeUnitSearch(e.target.value)}
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
                  {filteredFirstServiceTimeUnitOptions.length === 0 ? (
                    <div className="px-4 py-2 text-gray-500">
                      No units found.
                    </div>
                  ) : (
                    filteredFirstServiceTimeUnitOptions.map(opt => (
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
        <FormField
          label="First Service Mileage (km)"
          error={errors.FirstServiceMileage}
        >
          <input
            type="number"
            min={0}
            value={value.FirstServiceMileage || ""}
            onChange={e => onChange("FirstServiceMileage", e.target.value)}
            placeholder="e.g. 5000"
            className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
            disabled={disabled}
          />
        </FormField>
      </div>
      {/* Service Tasks Selection */}
      <FormField label="Service Tasks" required error={errors.ServiceTaskIDs}>
        <Combobox
          multiple
          value={selectedTasks}
          onChange={opts =>
            onChange(
              "ServiceTaskIDs",
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
                    <span className="flex-1">{task.Name}</span>
                    {value.ServiceTaskIDs.includes(task.id) && (
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
                {task.Name}
              </span>
            ))}
          </div>
        </Combobox>
      </FormField>
      {showIsActive && (
        <FormField label="Active">
          <select
            value={value.IsActive ? "true" : "false"}
            onChange={e => onChange("IsActive", e.target.value === "true")}
            className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
            disabled={disabled}
          >
            <option value="true">Active</option>
            <option value="false">Inactive</option>
          </select>
        </FormField>
      )}
      <FormField
        label="Service Program"
        required
        error={errors.ServiceProgramID}
      >
        <Combobox
          value={
            serviceProgramOptions.find(
              opt => opt.value === value.ServiceProgramID,
            ) || null
          }
          onChange={opt => onChange("ServiceProgramID", opt?.value || null)}
          disabled={disabled}
        >
          <div className="relative">
            <ComboboxInput
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              displayValue={(opt: { label?: string } | undefined) =>
                opt?.label || ""
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
    </FormContainer>
  );
};

export default ServiceScheduleDetailsForm;
