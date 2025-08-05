import React from "react";
import FormContainer from "@/components/ui/Form/FormContainer";
import FormField from "@/components/ui/Form/FormField";
import { VehicleConditionEnum } from "@/features/inspection/types/inspectionEnum";
import { getVehicleConditionLabel } from "@/features/inspection/utils/inspectionEnumHelper";
import {
  Combobox,
  ComboboxInput,
  ComboboxButton,
  ComboboxOptions,
  ComboboxOption,
} from "@headlessui/react";

export interface InspectionSignOffFormValues {
  vehicleCondition: VehicleConditionEnum;
  notes?: string | null;
}

interface InspectionSignOffFormProps {
  value: InspectionSignOffFormValues;
  errors: Partial<Record<keyof InspectionSignOffFormValues, string>>;
  onChange: (field: keyof InspectionSignOffFormValues, value: any) => void;
  disabled?: boolean;
}

const vehicleConditionOptions = [
  {
    value: VehicleConditionEnum.Excellent,
    label: getVehicleConditionLabel(VehicleConditionEnum.Excellent),
  },
  {
    value: VehicleConditionEnum.HasIssuesButSafeToOperate,
    label: getVehicleConditionLabel(
      VehicleConditionEnum.HasIssuesButSafeToOperate,
    ),
  },
  {
    value: VehicleConditionEnum.NotSafeToOperate,
    label: getVehicleConditionLabel(VehicleConditionEnum.NotSafeToOperate),
  },
];

const InspectionSignOffForm: React.FC<InspectionSignOffFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
}) => {
  const [conditionSearch, setConditionSearch] = React.useState("");
  const filteredConditions = React.useMemo(() => {
    if (!conditionSearch) return vehicleConditionOptions;
    const searchLower = conditionSearch.toLowerCase();
    return vehicleConditionOptions.filter(opt =>
      opt.label.toLowerCase().includes(searchLower),
    );
  }, [conditionSearch]);
  const selectedCondition =
    vehicleConditionOptions.find(opt => opt.value === value.vehicleCondition) ||
    null;

  return (
    <FormContainer title="Vehicle Condition">
      <FormField label="Condition" required error={errors.vehicleCondition}>
        <Combobox
          value={selectedCondition}
          onChange={condition => {
            if (condition) onChange("vehicleCondition", condition.value);
          }}
          disabled={disabled}
        >
          <div className="relative">
            <ComboboxInput
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              displayValue={(
                condition: { value: number; label: string } | null,
              ) => condition?.label || ""}
              onChange={e => setConditionSearch(e.target.value)}
              placeholder="Select or search condition..."
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
              {filteredConditions.length === 0 ? (
                <div className="px-4 py-2 text-gray-500">
                  No conditions found.
                </div>
              ) : (
                filteredConditions.map(opt => (
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
                ))
              )}
            </ComboboxOptions>
          </div>
        </Combobox>
      </FormField>

      <FormField label="Notes" error={errors.notes}>
        <textarea
          value={value.notes || ""}
          onChange={e => onChange("notes", e.target.value)}
          placeholder="Optional notes about the vehicle condition..."
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white min-h-[80px] resize-y"
          disabled={disabled}
        />
      </FormField>
    </FormContainer>
  );
};

export default InspectionSignOffForm;
