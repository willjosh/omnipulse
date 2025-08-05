import React from "react";
import FormContainer from "@/components/ui/Form/FormContainer";
import FormField from "@/components/ui/Form/FormField";

export interface InspectionDetailsFormValues {
  vehicleID: number;
  technicianID: string;
}

interface InspectionDetailsFormProps {
  value: InspectionDetailsFormValues;
  errors: Partial<Record<keyof InspectionDetailsFormValues, string>>;
  onChange: (field: keyof InspectionDetailsFormValues, value: any) => void;
  disabled?: boolean;
}

const InspectionDetailsForm: React.FC<InspectionDetailsFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
}) => {
  return (
    <FormContainer title="Inspection Details">
      <FormField label="Vehicle" required error={errors.vehicleID}>
        <div className="relative">
          <input
            type="text"
            value={value.vehicleID ? `${value.vehicleID} [Vehicle Name]` : ""}
            onChange={e => onChange("vehicleID", e.target.value)}
            placeholder="Select vehicle"
            className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white pr-10"
            disabled={disabled}
          />
          <div className="absolute inset-y-0 right-0 flex items-center pr-3">
            <button
              type="button"
              className="text-gray-400 hover:text-gray-600"
              onClick={() => onChange("vehicleID", "")}
            >
              <svg
                className="w-4 h-4"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth="2"
                  d="M6 18L18 6M6 6l12 12"
                />
              </svg>
            </button>
          </div>
          <div className="absolute inset-y-0 right-8 flex items-center">
            <button type="button" className="text-gray-400 hover:text-gray-600">
              <svg
                className="w-4 h-4"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth="2"
                  d="M19 9l-7 7-7-7"
                />
              </svg>
            </button>
          </div>
        </div>
      </FormField>

      <FormField label="Technician" required error={errors.technicianID}>
        <div className="relative">
          <input
            type="text"
            value={
              value.technicianID
                ? `${value.technicianID} [Technician Name]`
                : ""
            }
            onChange={e => onChange("technicianID", e.target.value)}
            placeholder="Select technician"
            className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white pr-10"
            disabled={disabled}
          />
          <div className="absolute inset-y-0 right-0 flex items-center pr-3">
            <button
              type="button"
              className="text-gray-400 hover:text-gray-600"
              onClick={() => onChange("technicianID", "")}
            >
              <svg
                className="w-4 h-4"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth="2"
                  d="M6 18L18 6M6 6l12 12"
                />
              </svg>
            </button>
          </div>
          <div className="absolute inset-y-0 right-8 flex items-center">
            <button type="button" className="text-gray-400 hover:text-gray-600">
              <svg
                className="w-4 h-4"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth="2"
                  d="M19 9l-7 7-7-7"
                />
              </svg>
            </button>
          </div>
        </div>
      </FormField>
    </FormContainer>
  );
};

export default InspectionDetailsForm;
