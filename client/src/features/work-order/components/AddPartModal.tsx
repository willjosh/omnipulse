import React, { useState, useMemo } from "react";
import ModalPortal from "@/components/ui/Modal/ModalPortal";
import FormField from "@/components/ui/Form/FormField";
import {
  Combobox,
  ComboboxInput,
  ComboboxButton,
  ComboboxOptions,
  ComboboxOption,
} from "@headlessui/react";
import { useInventoryItems } from "@/features/inventory-item/hooks/useInventoryItem";
import { InventoryItemWithLabels } from "@/features/inventory-item/types/inventoryItemType";

interface AddPartModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (part: {
    inventoryItemID: number;
    quantity: number;
    unitPrice: number;
  }) => void;
  initialValues?: {
    inventoryItemID?: number;
    quantity?: number;
    unitPrice?: number;
  };
}

const AddPartModal: React.FC<AddPartModalProps> = ({
  isOpen,
  onClose,
  onSave,
  initialValues = {},
}) => {
  const [inventoryItemID, setInventoryItemID] = useState(
    initialValues.inventoryItemID || 0,
  );
  const [quantity, setQuantity] = useState(initialValues.quantity || "");
  const [unitPrice, setUnitPrice] = useState(initialValues.unitPrice || "");
  const [inventoryItemSearch, setInventoryItemSearch] = useState("");

  const { inventoryItems, isPending: isLoadingInventoryItems } =
    useInventoryItems({ PageNumber: 1, PageSize: 100, Search: "" });

  const inventoryItemOptions = useMemo(
    () =>
      inventoryItems.map((item: InventoryItemWithLabels) => ({
        value: item.id,
        label: item.itemName,
        description: item.description,
        unitCost: item.unitCost,
      })),
    [inventoryItems],
  );

  const filteredInventoryItems = useMemo(() => {
    if (!inventoryItemSearch) return inventoryItemOptions;
    const searchLower = inventoryItemSearch.toLowerCase();
    return inventoryItemOptions.filter(
      opt =>
        opt.label.toLowerCase().includes(searchLower) ||
        (opt.description &&
          opt.description.toLowerCase().includes(searchLower)),
    );
  }, [inventoryItemSearch, inventoryItemOptions]);

  const selectedInventoryItem =
    inventoryItemOptions.find(item => item.value === inventoryItemID) || null;

  const handleSave = () => {
    const quantityNum = typeof quantity === "number" ? quantity : 0;
    const unitPriceNum = typeof unitPrice === "number" ? unitPrice : 0;

    if (!inventoryItemID || quantityNum <= 0 || unitPriceNum <= 0) {
      return;
    }

    onSave({ inventoryItemID, quantity: quantityNum, unitPrice: unitPriceNum });
    onClose();
  };

  const handleClose = () => {
    setInventoryItemID(initialValues.inventoryItemID || 0);
    setQuantity(initialValues.quantity || "");
    setUnitPrice(initialValues.unitPrice || "");
    setInventoryItemSearch("");
    onClose();
  };

  const handleInventoryItemSelect = (item: InventoryItemWithLabels) => {
    setInventoryItemID(item.id);
    setUnitPrice(item.unitCost || 0);
  };

  return (
    <ModalPortal isOpen={isOpen}>
      <div className="fixed inset-0 backdrop-brightness-50 bg-opacity-50 flex items-center justify-center z-[100]">
        <div className="bg-white rounded-lg shadow-xl max-w-md w-full mx-4">
          {/* Header */}
          <div className="px-6 py-4 border-b border-gray-200">
            <div className="flex items-center justify-between">
              <h2 className="text-lg font-semibold text-gray-900">Add Part</h2>
              <button
                onClick={handleClose}
                className="text-gray-400 hover:text-gray-600"
              >
                <svg
                  className="w-6 h-6"
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
          </div>

          {/* Content */}
          <div className="px-6 py-4 space-y-4">
            {/* Inventory Item */}
            <FormField label="Part (Inventory Item)" required>
              <Combobox
                value={selectedInventoryItem}
                onChange={(item: any) => {
                  if (item) {
                    const foundItem = inventoryItems.find(
                      ii => ii.id === item.value,
                    );
                    if (foundItem) {
                      handleInventoryItemSelect(foundItem);
                    }
                  }
                }}
                disabled={isLoadingInventoryItems}
              >
                <div className="relative">
                  <ComboboxInput
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                    displayValue={(
                      item: { value: number; label: string } | null,
                    ) => item?.label || ""}
                    onChange={e => setInventoryItemSearch(e.target.value)}
                    placeholder="Search inventory items..."
                    disabled={isLoadingInventoryItems}
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
                  <ComboboxOptions className="absolute z-10 mt-1 w-full bg-white border border-gray-200 rounded-lg shadow-lg max-h-60 overflow-auto">
                    {isLoadingInventoryItems ? (
                      <div className="px-4 py-2 text-gray-500">Loading...</div>
                    ) : filteredInventoryItems.length === 0 ? (
                      <div className="px-4 py-2 text-gray-500">
                        No inventory items found.
                      </div>
                    ) : (
                      filteredInventoryItems.map(opt => (
                        <ComboboxOption
                          key={opt.value}
                          value={opt}
                          className={({ active, selected }) =>
                            `cursor-pointer select-none px-4 py-3 ${active ? "bg-blue-50" : ""}`
                          }
                        >
                          <div className="flex flex-col">
                            <div className="font-medium text-gray-900">
                              {opt.label}
                            </div>
                            {opt.description && (
                              <div className="text-sm text-gray-500 mt-1">
                                {opt.description}
                              </div>
                            )}
                            <div className="text-xs text-gray-400 mt-1">
                              ${opt.unitCost || 0}
                            </div>
                          </div>
                        </ComboboxOption>
                      ))
                    )}
                  </ComboboxOptions>
                </div>
              </Combobox>
            </FormField>

            {/* Quantity */}
            <FormField label="Quantity" required>
              <input
                type="number"
                min={1}
                step={1}
                value={quantity || ""}
                onChange={e =>
                  setQuantity(e.target.value ? Number(e.target.value) : "")
                }
                placeholder="Enter quantity"
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              />
            </FormField>

            {/* Unit Cost */}
            <FormField label="Unit Cost" required>
              <input
                type="number"
                min={0}
                step={0.01}
                value={unitPrice || ""}
                onChange={e =>
                  setUnitPrice(e.target.value ? Number(e.target.value) : "")
                }
                placeholder="Enter unit cost"
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
              />
            </FormField>

            {/* Total Cost Preview */}
            {typeof quantity === "number" &&
              typeof unitPrice === "number" &&
              quantity > 0 &&
              unitPrice > 0 && (
                <div className="bg-blue-50 p-3 rounded-lg">
                  <div className="text-sm text-blue-800">
                    <span className="font-medium">Total Part Cost:</span> $
                    {(quantity * unitPrice).toFixed(2)}
                  </div>
                </div>
              )}
          </div>

          {/* Footer */}
          <div className="px-6 py-4 border-t border-gray-200 bg-gray-50">
            <div className="flex justify-end space-x-3">
              <button
                onClick={handleClose}
                className="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50"
              >
                Cancel
              </button>
              <button
                onClick={handleSave}
                disabled={
                  !inventoryItemID ||
                  typeof quantity !== "number" ||
                  quantity <= 0 ||
                  typeof unitPrice !== "number" ||
                  unitPrice <= 0
                }
                className="px-4 py-2 text-sm font-medium text-white bg-blue-600 border border-transparent rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Add Part
              </button>
            </div>
          </div>
        </div>
      </div>
    </ModalPortal>
  );
};

export default AddPartModal;
