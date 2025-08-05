import React from "react";
import FormContainer from "@/components/ui/Form/FormContainer";
import FormField from "@/components/ui/Form/FormField";
import { ChevronDown } from "lucide-react";

export interface InspectionSignOffFormValues {
  vehicleConditionOK: boolean;
  vehicleConditionRemarks?: string;
  showVehicleConditionRemarks: boolean;
  driverSignature: string;
  signatureRemarks?: string;
  showSignatureRemarks: boolean;
}

interface InspectionSignOffFormProps {
  value: InspectionSignOffFormValues;
  errors: Partial<Record<keyof InspectionSignOffFormValues, string>>;
  onChange: (field: keyof InspectionSignOffFormValues, value: any) => void;
  disabled?: boolean;
}

const InspectionSignOffForm: React.FC<InspectionSignOffFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
}) => {
  const toggleVehicleConditionRemarks = () => {
    onChange("showVehicleConditionRemarks", !value.showVehicleConditionRemarks);
  };

  const toggleSignatureRemarks = () => {
    onChange("showSignatureRemarks", !value.showSignatureRemarks);
  };

  return (
    <FormContainer title="Sign-Off">
      <div className="space-y-6">
        {/* Vehicle Condition OK */}
        <FormField
          label="Vehicle Condition OK"
          required
          error={errors.vehicleConditionOK}
        >
          <div className="space-y-2">
            <p className="text-sm text-gray-600">
              This must be checked if there are no defects.
            </p>
            <div className="flex items-center gap-2">
              <input
                type="checkbox"
                checked={value.vehicleConditionOK}
                onChange={e => onChange("vehicleConditionOK", e.target.checked)}
                disabled={disabled}
                className="rounded border-gray-300 text-blue-600 focus:ring-blue-500 h-4 w-4"
              />
              <span className="text-sm text-gray-700">
                Vehicle condition is acceptable
              </span>
            </div>

            {/* Vehicle Condition Remarks */}
            <div className="flex justify-end">
              <button
                type="button"
                onClick={toggleVehicleConditionRemarks}
                disabled={disabled}
                className="flex items-center gap-1 text-green-600 hover:text-green-700 font-medium text-sm"
              >
                + Add Remark
                <ChevronDown
                  size={16}
                  className={`transition-transform ${value.showVehicleConditionRemarks ? "rotate-180" : ""}`}
                />
              </button>
            </div>

            {value.showVehicleConditionRemarks && (
              <textarea
                value={value.vehicleConditionRemarks || ""}
                onChange={e =>
                  onChange("vehicleConditionRemarks", e.target.value)
                }
                placeholder="Add remarks about vehicle condition..."
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white resize-none"
                rows={3}
                disabled={disabled}
              />
            )}
          </div>
        </FormField>

        <hr className="border-gray-200" />

        {/* Driver Signature */}
        <FormField
          label="Reviewing Driver's Signature"
          required
          error={errors.driverSignature}
        >
          <div className="space-y-2">
            <div className="border-2 border-dashed border-gray-300 rounded-lg p-4 min-h-[100px] flex items-center justify-center">
              {value.driverSignature ? (
                <div className="text-center">
                  <p className="text-sm text-gray-600 mb-2">Signed by:</p>
                  <p className="font-medium text-gray-900">
                    {value.driverSignature}
                  </p>
                </div>
              ) : (
                <div className="text-center">
                  <button
                    type="button"
                    onClick={() => {
                      const signature = prompt("Type your name to sign:");
                      if (signature) {
                        onChange("driverSignature", signature);
                      }
                    }}
                    disabled={disabled}
                    className="inline-flex items-center gap-2 bg-yellow-100 text-yellow-800 px-3 py-1 rounded-md text-sm font-medium hover:bg-yellow-200"
                  >
                    Sign →
                  </button>
                  <p className="text-sm text-gray-500 mt-2 italic">
                    Type your name to sign
                  </p>
                </div>
              )}
            </div>

            {/* Signature Remarks */}
            <div className="flex justify-end">
              <button
                type="button"
                onClick={toggleSignatureRemarks}
                disabled={disabled}
                className="flex items-center gap-1 text-green-600 hover:text-green-700 font-medium text-sm"
              >
                + Add Remark
                <ChevronDown
                  size={16}
                  className={`transition-transform ${value.showSignatureRemarks ? "rotate-180" : ""}`}
                />
              </button>
            </div>

            {value.showSignatureRemarks && (
              <textarea
                value={value.signatureRemarks || ""}
                onChange={e => onChange("signatureRemarks", e.target.value)}
                placeholder="Add remarks about the signature..."
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white resize-none"
                rows={3}
                disabled={disabled}
              />
            )}
          </div>
        </FormField>
      </div>
    </FormContainer>
  );
};

export default InspectionSignOffForm;
