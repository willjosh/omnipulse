import React, { useState, useMemo } from "react";
import FormField from "@/components/ui/Form/FormField";
import {
  Combobox,
  ComboboxInput,
  ComboboxButton,
  ComboboxOptions,
  ComboboxOption,
} from "@headlessui/react";
import { IconButton } from "@mui/material";
import {
  Delete as DeleteIcon,
  ExpandMore as ExpandMoreIcon,
  ExpandLess as ExpandLessIcon,
} from "@mui/icons-material";
import { useServiceTasks } from "@/features/service-task/hooks/useServiceTasks";
import { useInventoryItems } from "@/features/inventory-item/hooks/useInventoryItems";
import { useTechnicians } from "@/features/technician/hooks/useTechnicians";
import { ServiceTaskWithLabels } from "@/features/service-task/types/serviceTaskType";
import { InventoryItemWithLabels } from "@/features/inventory-item/types/inventoryItemType";
import { LineItemTypeEnum } from "../types/workOrderEnum";
import { WorkOrderLineItem } from "../types/workOrderType";

const lineItemTypeOptions = [
  { value: LineItemTypeEnum.LABOR, label: "Labor" },
  { value: LineItemTypeEnum.ITEM, label: "Item" },
  { value: LineItemTypeEnum.BOTH, label: "Both" },
];

interface WorkOrderLineItemFormProps {
  lineItem: WorkOrderLineItem;
  index: number;
  onUpdate: (updatedLineItem: WorkOrderLineItem) => void;
  onRemove: () => void;
  disabled?: boolean;
}

const WorkOrderLineItemForm: React.FC<WorkOrderLineItemFormProps> = ({
  lineItem,
  index,
  onUpdate,
  onRemove,
  disabled = false,
}) => {
  // Fetch data for dropdowns
  const { serviceTasks, isPending: isLoadingServiceTasks } = useServiceTasks({
    PageNumber: 1,
    PageSize: 100,
    Search: "",
  });

  const { inventoryItems, isPending: isLoadingInventoryItems } =
    useInventoryItems({ PageNumber: 1, PageSize: 100, Search: "" });

  const { technicians, isPending: isLoadingTechnicians } = useTechnicians();

  // Local state for search
  const [serviceTaskSearch, setServiceTaskSearch] = useState("");
  const [inventoryItemSearch, setInventoryItemSearch] = useState("");
  const [technicianSearch, setTechnicianSearch] = useState("");
  const [isExpanded, setIsExpanded] = useState(false);

  // Create option structures following WorkOrderDetailsForm pattern
  const serviceTaskOptions = useMemo(
    () =>
      serviceTasks.map((task: ServiceTaskWithLabels) => ({
        value: task.id,
        label: task.name,
      })),
    [serviceTasks],
  );

  const inventoryItemOptions = useMemo(
    () =>
      inventoryItems.map((item: InventoryItemWithLabels) => ({
        value: item.id,
        label: item.itemName,
      })),
    [inventoryItems],
  );

  const technicianOptions = useMemo(
    () =>
      technicians.map(
        (tech: { id: string; firstName: string; lastName: string }) => ({
          value: tech.id,
          label: `${tech.firstName} ${tech.lastName}`,
        }),
      ),
    [technicians],
  );

  // Filtered options following WorkOrderDetailsForm pattern
  const filteredServiceTasks = useMemo(() => {
    if (!serviceTaskSearch) return serviceTaskOptions;
    const searchLower = serviceTaskSearch.toLowerCase();
    return serviceTaskOptions.filter(opt =>
      opt.label.toLowerCase().includes(searchLower),
    );
  }, [serviceTaskSearch, serviceTaskOptions]);

  const filteredInventoryItems = useMemo(() => {
    if (!inventoryItemSearch) return inventoryItemOptions;
    const searchLower = inventoryItemSearch.toLowerCase();
    return inventoryItemOptions.filter(opt =>
      opt.label.toLowerCase().includes(searchLower),
    );
  }, [inventoryItemSearch, inventoryItemOptions]);

  const filteredTechnicians = useMemo(() => {
    if (!technicianSearch) return technicianOptions;
    const searchLower = technicianSearch.toLowerCase();
    return technicianOptions.filter(opt =>
      opt.label.toLowerCase().includes(searchLower),
    );
  }, [technicianSearch, technicianOptions]);

  const updateLineItem = (field: keyof WorkOrderLineItem, value: any) => {
    const updatedLineItem = { ...lineItem, [field]: value };

    // Clear search states when items are selected
    if (field === "serviceTaskID") {
      setServiceTaskSearch("");
    }
    if (field === "inventoryItemID") {
      setInventoryItemSearch("");
    }
    if (field === "assignedToUserID") {
      setTechnicianSearch("");
    }

    // Auto-calculate costs based on type and quantity
    if (
      field === "quantity" ||
      field === "itemType" ||
      field === "inventoryItemID" ||
      field === "serviceTaskID"
    ) {
      // Recalculate costs
      let laborCost = 0;
      let itemCost = 0;

      if (updatedLineItem.serviceTaskID) {
        const serviceTask = serviceTasks.find(
          (st: ServiceTaskWithLabels) =>
            st.id === updatedLineItem.serviceTaskID,
        );
        if (serviceTask) {
          laborCost =
            serviceTask.estimatedLabourHours * updatedLineItem.quantity;
          updatedLineItem.serviceTaskName = serviceTask.name;
        }
      }

      if (updatedLineItem.inventoryItemID) {
        const inventoryItem = inventoryItems.find(
          (ii: InventoryItemWithLabels) =>
            ii.id === updatedLineItem.inventoryItemID,
        );
        if (inventoryItem && inventoryItem.unitCost) {
          itemCost = inventoryItem.unitCost * updatedLineItem.quantity;
          updatedLineItem.inventoryItemName = inventoryItem.itemName;
        }
      }

      updatedLineItem.laborCost = laborCost;
      updatedLineItem.itemCost = itemCost;
      updatedLineItem.subTotal = laborCost + itemCost;
    }

    onUpdate(updatedLineItem);
  };

  const toggleExpanded = () => {
    setIsExpanded(!isExpanded);
  };

  const getItemTypeLabel = (itemType: LineItemTypeEnum) => {
    return (
      lineItemTypeOptions.find(opt => opt.value === itemType)?.label ||
      "Unknown"
    );
  };

  // Create selected values following WorkOrderDetailsForm pattern
  const selectedServiceTask =
    serviceTaskOptions.find(opt => opt.value === lineItem.serviceTaskID) ||
    null;

  const selectedInventoryItem =
    inventoryItemOptions.find(opt => opt.value === lineItem.inventoryItemID) ||
    null;

  const selectedTechnician =
    technicianOptions.find(opt => opt.value === lineItem.assignedToUserID) ||
    null;

  return (
    <div className="border border-gray-200 rounded-lg overflow-hidden">
      {/* Collapsed Header */}
      <div
        className="flex items-center justify-between p-4 bg-gray-50 cursor-pointer hover:bg-gray-100 transition-colors"
        onClick={toggleExpanded}
      >
        <div className="flex items-center space-x-4 flex-1">
          <div className="flex items-center space-x-2">
            {isExpanded ? (
              <ExpandLessIcon className="text-gray-500" />
            ) : (
              <ExpandMoreIcon className="text-gray-500" />
            )}
            <span className="text-sm font-medium text-gray-900">
              Line Item #{index + 1}
            </span>
          </div>

          <div className="flex items-center space-x-6 text-sm text-gray-600">
            <span className="bg-blue-100 text-blue-800 px-2 py-1 rounded-full text-xs">
              {getItemTypeLabel(lineItem.itemType)}
            </span>
            <span>Qty: {lineItem.quantity}</span>
            {lineItem.serviceTaskName && (
              <span className="max-w-xs truncate">
                {lineItem.serviceTaskName}
              </span>
            )}
            {lineItem.inventoryItemName && (
              <span className="max-w-xs truncate">
                {lineItem.inventoryItemName}
              </span>
            )}
          </div>
        </div>

        <div className="flex items-center space-x-4">
          <div className="text-right">
            <div className="text-sm font-semibold text-blue-600">
              ${lineItem.subTotal.toFixed(2)}
            </div>
            <div className="text-xs text-gray-500">
              Labor: ${lineItem.laborCost.toFixed(2)} | Items: $
              {lineItem.itemCost.toFixed(2)}
            </div>
          </div>

          <IconButton
            size="small"
            onClick={e => {
              e.stopPropagation();
              onRemove();
            }}
            disabled={disabled}
            className="text-red-500 hover:text-red-700"
          >
            <DeleteIcon />
          </IconButton>
        </div>
      </div>

      {/* Expanded Content */}
      {isExpanded && (
        <div className="p-4 space-y-4 bg-white">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            {/* Item Type */}
            <FormField label="Item Type" required>
              <Combobox
                value={
                  lineItemTypeOptions.find(
                    opt => opt.value === lineItem.itemType,
                  ) || null
                }
                onChange={opt => opt && updateLineItem("itemType", opt.value)}
                disabled={disabled}
              >
                <div className="relative">
                  <ComboboxInput
                    className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                    displayValue={(
                      opt: { value: number; label: string } | null,
                    ) => opt?.label || ""}
                    placeholder="Select item type..."
                    disabled={disabled}
                    readOnly
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
                    {lineItemTypeOptions.map(opt => (
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
                    ))}
                  </ComboboxOptions>
                </div>
              </Combobox>
            </FormField>

            {/* Quantity */}
            <FormField label="Quantity" required>
              <input
                type="number"
                min={1}
                value={lineItem.quantity || 1}
                onChange={e =>
                  updateLineItem("quantity", Number(e.target.value))
                }
                className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                disabled={disabled}
              />
            </FormField>

            {/* Service Task */}
            <FormField label="Service Task" required>
              <Combobox
                value={selectedServiceTask}
                onChange={(task: { value: number; label: string } | null) =>
                  task && updateLineItem("serviceTaskID", task.value)
                }
                disabled={disabled || isLoadingServiceTasks}
              >
                <div className="relative">
                  <ComboboxInput
                    className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                    displayValue={(
                      task: { value: number; label: string } | null,
                    ) => task?.label || ""}
                    onChange={e => setServiceTaskSearch(e.target.value)}
                    placeholder="Search service tasks..."
                    disabled={disabled || isLoadingServiceTasks}
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
                    {filteredServiceTasks.map(opt => (
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
                    ))}
                  </ComboboxOptions>
                </div>
              </Combobox>
            </FormField>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {/* Inventory Item */}
            <FormField label="Inventory Item">
              <Combobox
                value={selectedInventoryItem}
                onChange={(item: { value: number; label: string } | null) =>
                  item && updateLineItem("inventoryItemID", item.value)
                }
                disabled={disabled || isLoadingInventoryItems}
              >
                <div className="relative">
                  <ComboboxInput
                    className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                    displayValue={(
                      item: { value: number; label: string } | null,
                    ) => item?.label || ""}
                    onChange={e => setInventoryItemSearch(e.target.value)}
                    placeholder="Search inventory items..."
                    disabled={disabled || isLoadingInventoryItems}
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
                    {filteredInventoryItems.map(opt => (
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
                    ))}
                  </ComboboxOptions>
                </div>
              </Combobox>
            </FormField>

            {/* Assigned Technician */}
            <FormField label="Assigned Technician">
              <Combobox
                value={selectedTechnician}
                onChange={(tech: { value: string; label: string } | null) =>
                  tech && updateLineItem("assignedToUserID", tech.value)
                }
                disabled={disabled || isLoadingTechnicians}
              >
                <div className="relative">
                  <ComboboxInput
                    className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                    displayValue={(
                      tech: { value: string; label: string } | null,
                    ) => tech?.label || ""}
                    onChange={e => setTechnicianSearch(e.target.value)}
                    placeholder="Search technicians..."
                    disabled={disabled || isLoadingTechnicians}
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
                    {filteredTechnicians.map(opt => (
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
                    ))}
                  </ComboboxOptions>
                </div>
              </Combobox>
            </FormField>
          </div>

          {/* Description */}
          <FormField label="Description">
            <textarea
              value={lineItem.description || ""}
              onChange={e => updateLineItem("description", e.target.value)}
              placeholder="Additional description for this line item..."
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white min-h-[80px] resize-y"
              disabled={disabled}
            />
          </FormField>

          {/* Cost Summary */}
          <div className="grid grid-cols-3 gap-4 text-sm">
            <div className="text-center p-2 bg-gray-50 rounded">
              <div className="font-medium">Labor Cost</div>
              <div className="text-gray-600">
                ${lineItem.laborCost.toFixed(2)}
              </div>
            </div>
            <div className="text-center p-2 bg-gray-50 rounded">
              <div className="font-medium">Item Cost</div>
              <div className="text-gray-600">
                ${lineItem.itemCost.toFixed(2)}
              </div>
            </div>
            <div className="text-center p-2 bg-blue-50 rounded">
              <div className="font-medium">Subtotal</div>
              <div className="text-blue-600 font-semibold">
                ${lineItem.subTotal.toFixed(2)}
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default WorkOrderLineItemForm;
