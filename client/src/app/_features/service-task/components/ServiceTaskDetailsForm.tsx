import React from "react";
import FormContainer from "@/app/_features/shared/form/FormContainer";
import FormField from "@/app/_features/shared/form/FormField";
import { ServiceTaskCategoryEnum } from "@/app/_hooks/service-task/serviceTaskEnum";

export interface ServiceTaskDetailsFormValues {
  Name: string;
  Description?: string;
  EstimatedLabourHours: number | string;
  EstimatedCost: number | string;
  Category: ServiceTaskCategoryEnum | "";
  IsActive: boolean;
}

interface ServiceTaskDetailsFormProps {
  value: ServiceTaskDetailsFormValues;
  errors: Partial<Record<keyof ServiceTaskDetailsFormValues, string>>;
  onChange: (field: keyof ServiceTaskDetailsFormValues, value: any) => void;
  disabled?: boolean;
}

const categoryOptions = [
  { value: ServiceTaskCategoryEnum.PREVENTIVE, label: "Preventive" },
  { value: ServiceTaskCategoryEnum.CORRECTIVE, label: "Corrective" },
  { value: ServiceTaskCategoryEnum.EMERGENCY, label: "Emergency" },
  { value: ServiceTaskCategoryEnum.INSPECTION, label: "Inspection" },
  { value: ServiceTaskCategoryEnum.WARRANTY, label: "Warranty" },
];

const ServiceTaskDetailsForm: React.FC<ServiceTaskDetailsFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
}) => {
  return (
    <FormContainer title="Details">
      <FormField label="Name" required error={errors.Name}>
        <input
          type="text"
          value={value.Name}
          onChange={e => onChange("Name", e.target.value)}
          placeholder="Enter service task name"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
          disabled={disabled}
        />
      </FormField>
      <FormField label="Description" error={errors.Description}>
        <textarea
          value={value.Description || ""}
          onChange={e => onChange("Description", e.target.value)}
          placeholder="Optional notes or description"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white min-h-[80px] resize-y"
          disabled={disabled}
        />
      </FormField>
      <FormField
        label="Estimated Labour Hours"
        required
        error={errors.EstimatedLabourHours}
      >
        <input
          type="number"
          min={0}
          step={0.1}
          value={value.EstimatedLabourHours}
          onChange={e => onChange("EstimatedLabourHours", e.target.value)}
          placeholder="e.g. 2.5"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
          disabled={disabled}
        />
      </FormField>
      <FormField label="Estimated Cost" required error={errors.EstimatedCost}>
        <input
          type="number"
          min={0}
          step={0.01}
          value={value.EstimatedCost}
          onChange={e => onChange("EstimatedCost", e.target.value)}
          placeholder="e.g. 100.00"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
          disabled={disabled}
        />
      </FormField>
      <FormField label="Category" required error={errors.Category}>
        <select
          value={value.Category}
          onChange={e => onChange("Category", e.target.value)}
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
          disabled={disabled}
        >
          <option value="">Select category</option>
          {categoryOptions.map(opt => (
            <option key={opt.value} value={opt.value}>
              {opt.label}
            </option>
          ))}
        </select>
      </FormField>
    </FormContainer>
  );
};

export default ServiceTaskDetailsForm;
