import React, { useState, useMemo } from "react";
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
  IssueCategoryEnum,
  PriorityLevelEnum,
} from "../../../_hooks/issue/issueEnum";

// Dummy data for vehicles and users (replace with real data/fetch in future)
const users = [
  { value: "5e6e0ab3-3a11-403e-adb9-7a25fe678936", label: "John Smith" },
  { value: "952188a4-dc48-4dad-9c7b-c75da50bb241", label: "Sarah Wilson" },
  { value: "f1e2d3c4-b5a6-9870-fedc-ba0987654321", label: "Emma Davis" },
];

const categoryOptions = [
  { value: IssueCategoryEnum.ENGINE.toString(), label: "Engine" },
  { value: IssueCategoryEnum.TRANSMISSION.toString(), label: "Transmission" },
  { value: IssueCategoryEnum.BRAKES.toString(), label: "Brakes" },
  { value: IssueCategoryEnum.ELECTRICAL.toString(), label: "Electrical" },
  { value: IssueCategoryEnum.BODY.toString(), label: "Body" },
  { value: IssueCategoryEnum.TIRES.toString(), label: "Tires" },
  { value: IssueCategoryEnum.HVAC.toString(), label: "HVAC" },
  { value: IssueCategoryEnum.OTHER.toString(), label: "Other" },
];

const priorityOptions = [
  { value: PriorityLevelEnum.LOW.toString(), label: "Low" },
  { value: PriorityLevelEnum.MEDIUM.toString(), label: "Medium" },
  { value: PriorityLevelEnum.HIGH.toString(), label: "High" },
  { value: PriorityLevelEnum.CRITICAL.toString(), label: "Critical" },
];

interface IssueDetailsFormProps {
  value: {
    VehicleID: string;
    PriorityLevel: string;
    // ReportedDate: string;
    Title: string;
    Description: string;
    // Category: string;
    // Status: string;
    ReportedByUserID: string;
    Category: string;
  };
  errors: { [key: string]: string };
  onChange: (field: string, value: string) => void;
  disabled?: boolean;
  vehicles: { value: string; label: string }[];
}

const IssueDetailsForm: React.FC<IssueDetailsFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
  vehicles,
}) => {
  type VehicleOption = { value: string; label: string };
  // Search/filter state for vehicle dropdown
  const [vehicleSearch, setVehicleSearch] = useState("");
  const filteredVehicles = useMemo(() => {
    if (!vehicleSearch) return vehicles;
    const searchLower = vehicleSearch.toLowerCase();
    return vehicles.filter(v => v.label.toLowerCase().includes(searchLower));
  }, [vehicleSearch, vehicles]);
  const selectedVehicle: VehicleOption | null =
    vehicles.find(v => v.value === value.VehicleID) || null;

  const usersList: { value: string; label: string }[] = users;
  const [userSearch, setUserSearch] = useState("");
  const filteredUsers = useMemo(() => {
    if (!userSearch) return usersList;
    const searchLower = userSearch.toLowerCase();
    return usersList.filter(u => u.label.toLowerCase().includes(searchLower));
  }, [userSearch, usersList]);
  const selectedUser =
    usersList.find(u => u.value === value.ReportedByUserID) || null;

  return (
    <FormContainer title="Details" className="mt-6 max-w-2xl mx-auto w-full">
      {/* Asset (Vehicle) Dropdown with search */}
      <FormField label="Asset" required error={errors.VehicleID}>
        <Combobox
          value={selectedVehicle}
          onChange={v => v && onChange("VehicleID", v.value)}
          disabled={disabled}
        >
          <div className="relative">
            <ComboboxInput
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              displayValue={(vehicle: VehicleOption | null) =>
                vehicle?.label || ""
              }
              onChange={e => setVehicleSearch(e.target.value)}
              placeholder="Search vehicles..."
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
              {filteredVehicles.length === 0 && (
                <div className="px-4 py-2 text-gray-500">
                  No vehicles found.
                </div>
              )}
              {filteredVehicles.map(opt => (
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
      <FormField label="Priority Level" required error={errors.PriorityLevel}>
        <Combobox
          value={
            priorityOptions.find(opt => opt.value === value.PriorityLevel) ||
            null
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
              {priorityOptions.map(opt => (
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
            categoryOptions.find(opt => opt.value === value.Category) || null
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
              {categoryOptions.map(opt => (
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
          disabled={disabled}
        >
          <div className="relative">
            <ComboboxInput
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              displayValue={(user: { value: string; label: string } | null) =>
                user?.label || ""
              }
              onChange={e => setUserSearch(e.target.value)}
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
