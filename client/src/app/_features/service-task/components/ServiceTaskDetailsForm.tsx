import React from "react";
import FormContainer from "@/app/_features/shared/form/FormContainer";
import FormField from "@/app/_features/shared/form/FormField";
import { ServiceTaskCategoryEnum } from "@/app/_hooks/service-task/serviceTaskEnum";
import {
  Combobox,
  ComboboxInput,
  ComboboxButton,
  ComboboxOptions,
  ComboboxOption,
} from "@headlessui/react";

export interface ServiceTaskDetailsFormValues {
  name: string;
  description?: string;
  estimatedLabourHours: number | string;
  estimatedCost: number | string;
  category: ServiceTaskCategoryEnum | "";
  isActive: boolean;
}

interface ServiceTaskDetailsFormProps {
  value: ServiceTaskDetailsFormValues;
  errors: Partial<Record<keyof ServiceTaskDetailsFormValues, string>>;
  onChange: (field: keyof ServiceTaskDetailsFormValues, value: any) => void;
  disabled?: boolean;
  showIsActive?: boolean;
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
  showIsActive = false,
}) => {
  const [categorySearch, setCategorySearch] = React.useState("");
  const filteredCategories = React.useMemo(() => {
    if (!categorySearch) return categoryOptions;
    const searchLower = categorySearch.toLowerCase();
    return categoryOptions.filter(opt =>
      opt.label.toLowerCase().includes(searchLower),
    );
  }, [categorySearch]);
  const selectedCategory =
    categoryOptions.find(opt => opt.value === value.category) || null;

  // For IsActive Combobox
  const isActiveOptions = [
    { value: true, label: "Active" },
    { value: false, label: "Inactive" },
  ];
  const selectedIsActive =
    isActiveOptions.find(opt => opt.value === value.isActive) ||
    isActiveOptions[0];

  return (
    <FormContainer title="Details">
      <FormField label="Name" required error={errors.name}>
        <input
          type="text"
          value={value.name}
          onChange={e => onChange("name", e.target.value)}
          placeholder="Enter service task name"
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
      <FormField
        label="Estimated Labour Hours"
        required
        error={errors.estimatedLabourHours}
      >
        <input
          type="number"
          min={0}
          step={0.1}
          value={value.estimatedLabourHours}
          onChange={e => onChange("estimatedLabourHours", e.target.value)}
          placeholder="e.g. 2.5"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
          disabled={disabled}
        />
      </FormField>
      <FormField label="Estimated Cost" required error={errors.estimatedCost}>
        <input
          type="number"
          min={0}
          step={0.01}
          value={value.estimatedCost}
          onChange={e => onChange("estimatedCost", e.target.value)}
          placeholder="e.g. 100.00"
          className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
          disabled={disabled}
        />
      </FormField>
      <FormField label="Category" required error={errors.category}>
        <Combobox
          value={selectedCategory}
          onChange={opt => {
            if (opt) onChange("category", opt.value);
          }}
          disabled={disabled}
        >
          <div className="relative">
            <ComboboxInput
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              displayValue={(opt: { value: number; label: string } | null) =>
                opt?.label || ""
              }
              onChange={e => setCategorySearch(e.target.value)}
              placeholder="Select or search category..."
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
              {filteredCategories.length === 0 ? (
                <div className="px-4 py-2 text-gray-500">
                  No categories found.
                </div>
              ) : (
                filteredCategories.map(opt => (
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
      {showIsActive && (
        <FormField label="Active">
          <Combobox
            value={selectedIsActive}
            onChange={opt => {
              if (opt) onChange("isActive", opt.value);
            }}
            disabled={disabled}
          >
            <div className="relative">
              <ComboboxInput
                className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                displayValue={(opt: { value: boolean; label: string } | null) =>
                  opt?.label || ""
                }
                readOnly
                placeholder="Select status..."
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
                {isActiveOptions.map(opt => (
                  <ComboboxOption
                    key={String(opt.value)}
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
                ))}
              </ComboboxOptions>
            </div>
          </Combobox>
        </FormField>
      )}
    </FormContainer>
  );
};

export default ServiceTaskDetailsForm;
