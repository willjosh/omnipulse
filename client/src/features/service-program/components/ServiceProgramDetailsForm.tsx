import React from "react";
import FormContainer from "@/components/ui/Form/FormContainer";
import FormField from "@/components/ui/Form/FormField";

export interface ServiceProgramDetailsFormValues {
  name: string;
  description?: string;
  isActive?: boolean; // Optional for backward compatibility
}

interface ServiceProgramDetailsFormProps {
  value: ServiceProgramDetailsFormValues;
  errors: Partial<Record<keyof ServiceProgramDetailsFormValues, string>>;
  onChange: (field: keyof ServiceProgramDetailsFormValues, value: any) => void;
  disabled?: boolean;
  showIsActive?: boolean; // New prop to control whether to show the isActive field
}

const ServiceProgramDetailsForm: React.FC<ServiceProgramDetailsFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
  showIsActive = false,
}) => {
  return (
    <FormContainer title="Details">
      <FormField label="Name" required error={errors.name}>
        <input
          type="text"
          value={value.name}
          onChange={e => onChange("name", e.target.value)}
          placeholder="Enter service program name"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
          disabled={disabled}
        />
      </FormField>
      <FormField label="Description" error={errors.description}>
        <textarea
          value={value.description || ""}
          onChange={e => onChange("description", e.target.value)}
          placeholder="Optional notes or description"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white min-h-[80px] resize-y"
          disabled={disabled}
        />
      </FormField>
      {showIsActive && (
        <FormField label="Status" error={errors.isActive}>
          <div className="flex items-center">
            <input
              type="checkbox"
              id="isActive"
              checked={value.isActive ?? true}
              onChange={e => onChange("isActive", e.target.checked)}
              className="size-4 text-blue-600 rounded border-gray-300 focus:ring-blue-500"
              disabled={disabled}
            />
            <label htmlFor="isActive" className="ml-2 text-sm text-gray-700">
              Active
            </label>
          </div>
        </FormField>
      )}
    </FormContainer>
  );
};

export default ServiceProgramDetailsForm;
