import React from "react";
import FormContainer from "@/components/ui/Form/FormContainer";
import FormField from "@/components/ui/Form/FormField";
import { PrimaryButton } from "@/components/ui/Button";

export interface InspectionOdometerFormValues {
  odometerReading: number;
  voidOdometer: boolean;
  photoFile?: File | null;
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
  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0] || null;
    onChange("photoFile", file);
  };

  return (
    <FormContainer title="Odometer Reading">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <FormField
          label="Primary Meter"
          required
          error={errors.odometerReading}
        >
          <div className="flex items-center gap-2">
            <input
              type="number"
              value={value.odometerReading || ""}
              onChange={e =>
                onChange("odometerReading", parseInt(e.target.value) || 0)
              }
              placeholder="Enter odometer reading"
              className="flex-1 border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              disabled={disabled}
            />
            <span className="text-sm text-gray-600">mi</span>
            <label className="flex items-center gap-2 text-sm text-gray-600">
              <input
                type="checkbox"
                checked={value.voidOdometer}
                onChange={e => onChange("voidOdometer", e.target.checked)}
                disabled={disabled}
                className="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
              />
              Void
            </label>
          </div>
          <div className="text-xs text-gray-500 mt-1">
            Last updated: 20,811 mi (13 days ago)
          </div>
        </FormField>

        <FormField
          label="Meter Entry Photo Verification"
          error={errors.photoFile}
        >
          <div className="space-y-2">
            <PrimaryButton
              type="button"
              onClick={() => document.getElementById("photo-upload")?.click()}
              disabled={disabled}
              className="w-full"
            >
              Pick File
            </PrimaryButton>
            <input
              id="photo-upload"
              type="file"
              accept="image/*"
              onChange={handleFileChange}
              className="hidden"
              disabled={disabled}
            />
            <div className="text-sm text-gray-500">
              {value.photoFile ? value.photoFile.name : "No file selected"}
            </div>
          </div>
        </FormField>
      </div>
    </FormContainer>
  );
};

export default InspectionOdometerForm;
