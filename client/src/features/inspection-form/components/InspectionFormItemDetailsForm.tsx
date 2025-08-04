import React from "react";
import FormContainer from "@/components/ui/Form/FormContainer";
import FormField from "@/components/ui/Form/FormField";
import { InspectionFormItemTypeEnum } from "../types/inspectionFormEnum";
import { getInspectionFormItemTypeLabel } from "../utils/inspectionFormEnumHelper";
import {
  Combobox,
  ComboboxInput,
  ComboboxButton,
  ComboboxOptions,
  ComboboxOption,
} from "@headlessui/react";

export interface InspectionFormItemDetailsFormValues {
  itemLabel: string;
  itemDescription?: string;
  itemInstructions?: string;
  inspectionFormItemTypeEnum: InspectionFormItemTypeEnum;
  isRequired: boolean;
}

interface InspectionFormItemDetailsFormProps {
  value: InspectionFormItemDetailsFormValues;
  errors: Partial<Record<keyof InspectionFormItemDetailsFormValues, string>>;
  onChange: (
    field: keyof InspectionFormItemDetailsFormValues,
    value: any,
  ) => void;
  disabled?: boolean;
}

const itemTypeOptions = [
  {
    value: InspectionFormItemTypeEnum.PassFail,
    label: getInspectionFormItemTypeLabel(InspectionFormItemTypeEnum.PassFail),
  },
];

const InspectionFormItemDetailsForm: React.FC<
  InspectionFormItemDetailsFormProps
> = ({ value, errors, onChange, disabled = false }) => {
  const [itemTypeSearch, setItemTypeSearch] = React.useState("");
  const filteredItemTypes = React.useMemo(() => {
    if (!itemTypeSearch) return itemTypeOptions;
    const searchLower = itemTypeSearch.toLowerCase();
    return itemTypeOptions.filter(opt =>
      opt.label.toLowerCase().includes(searchLower),
    );
  }, [itemTypeSearch]);
  const selectedItemType =
    itemTypeOptions.find(
      opt => opt.value === value.inspectionFormItemTypeEnum,
    ) || null;

  return (
    <FormContainer title="Item Details">
      <FormField label="Item Label" required error={errors.itemLabel}>
        <input
          type="text"
          value={value.itemLabel}
          onChange={e => onChange("itemLabel", e.target.value)}
          placeholder="Enter item label"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
          disabled={disabled}
        />
      </FormField>

      <FormField label="Description" error={errors.itemDescription}>
        <textarea
          value={value.itemDescription || ""}
          onChange={e => onChange("itemDescription", e.target.value)}
          placeholder="Enter item description (optional)"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white min-h-[80px] resize-y"
          disabled={disabled}
        />
      </FormField>

      <FormField label="Instructions" error={errors.itemInstructions}>
        <textarea
          value={value.itemInstructions || ""}
          onChange={e => onChange("itemInstructions", e.target.value)}
          placeholder="Enter inspection instructions (optional)"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white min-h-[80px] resize-y"
          disabled={disabled}
        />
      </FormField>

      <FormField label="Item Type" error={errors.inspectionFormItemTypeEnum}>
        <Combobox
          value={selectedItemType}
          onChange={opt => {
            if (opt) onChange("inspectionFormItemTypeEnum", opt.value);
          }}
          disabled={disabled}
        >
          <div className="relative">
            <ComboboxInput
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              displayValue={(opt: { value: number; label: string } | null) =>
                opt?.label || ""
              }
              onChange={e => setItemTypeSearch(e.target.value)}
              placeholder="Select or search item type..."
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
              {filteredItemTypes.length === 0 ? (
                <div className="px-4 py-2 text-gray-500">
                  No item types found.
                </div>
              ) : (
                filteredItemTypes.map(opt => (
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

      <FormField label="Required">
        <div className="flex items-center">
          <input
            type="checkbox"
            id="isRequired"
            checked={value.isRequired}
            onChange={e => onChange("isRequired", e.target.checked)}
            className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
            disabled={disabled}
          />
          <label htmlFor="isRequired" className="ml-2 text-sm text-gray-700">
            This item is required
          </label>
        </div>
        <p className="mt-1 text-sm text-gray-500">
          Required items must be completed during inspections
        </p>
      </FormField>
    </FormContainer>
  );
};

export default InspectionFormItemDetailsForm;
