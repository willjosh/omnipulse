import React from "react";
import FormContainer from "@/components/ui/Form/FormContainer";
import { ChevronDown } from "lucide-react";

export interface InspectionItemChecklistFormValues {
  items: {
    id: number;
    itemLabel: string;
    itemDescription?: string;
    itemInstructions?: string;
    isRequired: boolean;
    passed: boolean | null;
    comment?: string;
    showRemarks: boolean;
  }[];
}

interface InspectionItemChecklistFormProps {
  value: InspectionItemChecklistFormValues;
  errors: Partial<Record<string, string>>;
  onChange: (field: string, value: any) => void;
  disabled?: boolean;
}

const InspectionItemChecklistForm: React.FC<
  InspectionItemChecklistFormProps
> = ({ value, errors, onChange, disabled = false }) => {
  const handleItemChange = (itemId: number, field: string, fieldValue: any) => {
    const updatedItems = value.items.map(item =>
      item.id === itemId ? { ...item, [field]: fieldValue } : item,
    );
    onChange("items", updatedItems);
  };

  const toggleRemarks = (itemId: number) => {
    const updatedItems = value.items.map(item =>
      item.id === itemId ? { ...item, showRemarks: !item.showRemarks } : item,
    );
    onChange("items", updatedItems);
  };

  return (
    <FormContainer title="Item Checklist">
      <div className="space-y-6">
        {value.items.map((item, index) => (
          <div
            key={item.id}
            className="border-b border-gray-200 pb-6 last:border-b-0"
          >
            <div className="flex items-center mb-3">
              <div className="w-1/2">
                <h3 className="font-medium text-gray-900">
                  {item.itemLabel}
                  {item.isRequired && (
                    <span className="text-red-500 ml-1">*</span>
                  )}
                  {item.itemDescription && (
                    <span className="text-sm text-gray-500 ml-2">
                      ({item.itemDescription})
                    </span>
                  )}
                </h3>
                {errors[`item-${item.id}`] && (
                  <p className="text-red-500 text-sm mt-1">
                    {errors[`item-${item.id}`]}
                  </p>
                )}
              </div>

              <div className="w-1/2 flex items-center gap-4">
                <label className="flex items-center gap-2">
                  <input
                    type="radio"
                    name={`item-${item.id}`}
                    checked={item.passed === true}
                    onChange={() => handleItemChange(item.id, "passed", true)}
                    disabled={disabled}
                    className="text-blue-600 focus:ring-blue-500"
                  />
                  <span className="text-sm font-medium text-gray-700">
                    Pass
                  </span>
                </label>
                <label className="flex items-center gap-2">
                  <input
                    type="radio"
                    name={`item-${item.id}`}
                    checked={item.passed === false}
                    onChange={() => handleItemChange(item.id, "passed", false)}
                    disabled={disabled}
                    className="text-blue-600 focus:ring-blue-500"
                  />
                  <span className="text-sm font-medium text-gray-700">
                    Fail
                  </span>
                </label>
              </div>
            </div>

            <div className="flex items-start">
              <div className="w-1/2">
                {item.itemInstructions && (
                  <p className="text-sm text-gray-600">
                    {item.itemInstructions}
                  </p>
                )}
              </div>

              <div className="w-1/2 flex justify-end">
                <button
                  type="button"
                  onClick={() => toggleRemarks(item.id)}
                  disabled={disabled}
                  className="flex items-center gap-1 text-green-600 hover:text-green-700 font-medium text-sm"
                >
                  + Add Remark
                  <ChevronDown
                    size={16}
                    className={`transition-transform ${item.showRemarks ? "rotate-180" : ""}`}
                  />
                </button>
              </div>
            </div>

            {item.showRemarks && (
              <div className="mt-3 flex">
                <div className="w-1/2"></div>
                <div className="w-1/2">
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Comment
                  </label>
                  <textarea
                    value={item.comment || ""}
                    onChange={e =>
                      handleItemChange(item.id, "comment", e.target.value)
                    }
                    placeholder="Add your remarks here..."
                    className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white resize-none"
                    rows={3}
                    disabled={disabled}
                  />
                </div>
              </div>
            )}
          </div>
        ))}
      </div>
    </FormContainer>
  );
};

export default InspectionItemChecklistForm;
