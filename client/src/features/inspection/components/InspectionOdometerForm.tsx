import React from "react";
import FormContainer from "@/components/ui/Form/FormContainer";
import FormField from "@/components/ui/Form/FormField";
import { PrimaryButton } from "@/components/ui/Button";

export interface InspectionOdometerFormValues {
  odometerReading: number | null;
}

interface InspectionOdometerFormProps {
  value: InspectionOdometerFormValues;
  errors: Partial<Record<keyof InspectionOdometerFormValues, string>>;
  onChange: (field: keyof InspectionOdometerFormValues, value: any) => void;
  disabled?: boolean;
}

const InspectionOdometerForm: React.FC<InspectionOdometerFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
}) => {
  return (
    <FormContainer title="Odometer Reading">
      <FormField label="Odometer Reading" error={errors.odometerReading}>
        <input
          type="number"
          min={0}
          step={1}
          value={value.odometerReading === null ? "" : value.odometerReading}
          onChange={e => {
            const value = e.target.value;
            if (value === "") {
              onChange("odometerReading", null);
            } else {
              const parsedValue = parseInt(value);
              if (!isNaN(parsedValue)) {
                onChange("odometerReading", parsedValue);
              }
            }
          }}
          placeholder="e.g. 50000"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
          disabled={disabled}
        />
      </FormField>
    </FormContainer>
  );
};

export default InspectionOdometerForm;
