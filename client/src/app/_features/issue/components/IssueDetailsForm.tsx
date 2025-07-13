// NOTE: Most fields are commented out for user story separation. Only Title and ReportedByUserID are active.

import React from "react";
import FormContainer from "../../shared/form/FormContainer";
import FormField from "../../shared/form/FormField";
// import DropdownFilter from "../../shared/filter/DropdownFilter";
import SearchInput from "../../shared/filter/SearchInput";
// import {
//   IssueCategoryEnum,
//   PriorityLevelEnum,
//   IssueStatusEnum,
// } from "@/app/_hooks/issue/issueEnum";

// Dummy data for users (replace with real data/fetch in future)
const users = [
  { value: "5e6e0ab3-3a11-403e-adb9-7a25fe678936", label: "John Smith" },
  { value: "952188a4-dc48-4dad-9c7b-c75da50bb241", label: "Sarah Wilson" },
  { value: "f1e2d3c4-b5a6-9870-fedc-ba0987654321", label: "Emma Davis" },
];

interface IssueDetailsFormProps {
  value: {
    // VehicleID: string;
    // PriorityLevel: string;
    // ReportedDate: string;
    Title: string;
    // Description: string;
    // Category: string;
    // Status: string;
    ReportedByUserID: string;
  };
  errors: { [key: string]: string };
  onChange: (field: string, value: string) => void;
  disabled?: boolean;
}

const IssueDetailsForm: React.FC<IssueDetailsFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
}) => {
  return (
    <FormContainer title="Details" className="mt-6 max-w-2xl mx-auto w-full">
      {/* <FormField label="Asset" required error={errors.VehicleID}>
        <DropdownFilter
          value={value.VehicleID}
          onChange={v => onChange("VehicleID", v)}
          options={vehicles}
          placeholder="Select vehicle"
          className="w-full"
        />
      </FormField> */}
      {/* <FormField label="Priority" required error={errors.PriorityLevel}>
        <DropdownFilter
          value={value.PriorityLevel}
          onChange={v => onChange("PriorityLevel", v)}
          options={priorityOptions}
          placeholder="Select priority"
          className="w-full"
        />
      </FormField> */}
      {/* <FormField label="Reported Date" required error={errors.ReportedDate}>
        <input
          type="datetime-local"
          className="px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 w-full"
          value={value.ReportedDate}
          onChange={e => onChange("ReportedDate", e.target.value)}
          disabled={disabled}
        />
      </FormField> */}
      <FormField label="Summary" required error={errors.Title}>
        <SearchInput
          value={value.Title}
          onChange={v => onChange("Title", v)}
          placeholder="Enter summary"
        />
      </FormField>
      {/* <FormField label="Description">
        <textarea
          className="px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 w-full min-h-[80px]"
          value={value.Description}
          onChange={e => onChange("Description", e.target.value)}
          placeholder="Enter description (optional)"
          disabled={disabled}
        />
      </FormField> */}
      {/* <FormField label="Category" required error={errors.Category}>
        <DropdownFilter
          value={value.Category}
          onChange={v => onChange("Category", v)}
          options={categoryOptions}
          placeholder="Select category"
          className="w-full"
        />
      </FormField> */}
      {/* <FormField label="Status" required error={errors.Status}>
        <DropdownFilter
          value={value.Status}
          onChange={v => onChange("Status", v)}
          options={statusOptions}
          placeholder="Select status"
          className="w-full"
        />
      </FormField> */}
      <FormField label="Reported By" required error={errors.ReportedByUserID}>
        <select
          value={value.ReportedByUserID}
          onChange={e => onChange("ReportedByUserID", e.target.value)}
          className="px-3 py-2 border border-gray-300 rounded-3xl text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white w-full"
          disabled={disabled}
        >
          <option value="">Select user</option>
          {users.map(opt => (
            <option key={opt.value} value={opt.value}>
              {opt.label}
            </option>
          ))}
        </select>
      </FormField>
    </FormContainer>
  );
};

export default IssueDetailsForm;
