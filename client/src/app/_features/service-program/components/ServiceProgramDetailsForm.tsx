import React from "react";
import FormContainer from "@/app/_features/shared/form/FormContainer";
import FormField from "@/app/_features/shared/form/FormField";

export interface ServiceProgramDetailsFormValues {
  name: string;
  description?: string;
}

interface ServiceProgramDetailsFormProps {
  value: ServiceProgramDetailsFormValues;
  errors: Partial<Record<keyof ServiceProgramDetailsFormValues, string>>;
  onChange: (field: keyof ServiceProgramDetailsFormValues, value: any) => void;
  disabled?: boolean;
}

const ServiceProgramDetailsForm: React.FC<ServiceProgramDetailsFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
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
    </FormContainer>
  );
};

export default ServiceProgramDetailsForm;
