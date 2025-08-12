import React from "react";
import FormContainer from "@/components/ui/Form/FormContainer";
import FormField from "@/components/ui/Form/FormField";

export interface InspectionFormDetailsFormValues {
  title: string;
  description?: string;
  isActive: boolean;
}

interface InspectionFormDetailsFormProps {
  value: InspectionFormDetailsFormValues;
  errors: Partial<Record<keyof InspectionFormDetailsFormValues, string>>;
  onChange: (field: keyof InspectionFormDetailsFormValues, value: any) => void;
  disabled?: boolean;
}

const InspectionFormDetailsForm: React.FC<InspectionFormDetailsFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
}) => {
  return (
    <FormContainer title="Form Details">
      <FormField label="Title" required error={errors.title}>
        <input
          type="text"
          value={value.title}
          onChange={e => onChange("title", e.target.value)}
          placeholder="Enter inspection form title"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
          disabled={disabled}
        />
      </FormField>

      <FormField label="Description" error={errors.description}>
        <textarea
          value={value.description || ""}
          onChange={e => onChange("description", e.target.value)}
          placeholder="Optional description of the inspection form"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white min-h-[80px] resize-y"
          disabled={disabled}
        />
      </FormField>
    </FormContainer>
  );
};

export default InspectionFormDetailsForm;
