import React, { useState, useMemo } from "react";
import FormContainer from "@/components/ui/Form/FormContainer";
import { LineItemTypeEnum } from "../types/workOrderEnum";
import { CreateWorkOrderLineItem } from "../types/workOrderType";
import { ServiceTaskWithLabels } from "@/features/service-task/types/serviceTaskType";
import WorkOrderLineItemForm from "./WorkOrderLineItemForm";
import { useServiceTasks } from "@/features/service-task/hooks/useServiceTasks";
import {
  Combobox,
  ComboboxInput,
  ComboboxButton,
  ComboboxOptions,
  ComboboxOption,
} from "@headlessui/react";
import TabNavigation from "@/components/ui/Tabs/TabNavigation";

export interface WorkOrderLineItemsFormValues {
  workOrderLineItems: CreateWorkOrderLineItem[];
}

interface WorkOrderLineItemsFormProps {
  value: WorkOrderLineItemsFormValues;
  errors: Partial<Record<keyof WorkOrderLineItemsFormValues, string>>;
  onChange: (
    field: keyof WorkOrderLineItemsFormValues,
    value: CreateWorkOrderLineItem[],
  ) => void;
  disabled?: boolean;
}

const WorkOrderLineItemsForm: React.FC<WorkOrderLineItemsFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
}) => {
  const [activeTab, setActiveTab] = useState("service-tasks");
  const [serviceTaskSearch, setServiceTaskSearch] = useState("");

  // Generate stable keys for line items using index and service task ID
  const lineItemKeys = React.useMemo(() => {
    return (value.workOrderLineItems || []).map(
      (item, index) => `line-${index}-${item.serviceTaskID}`,
    );
  }, [value.workOrderLineItems]);

  // Fetch service tasks
  const { serviceTasks, isPending: isLoadingServiceTasks } = useServiceTasks({
    PageNumber: 1,
    PageSize: 100,
    Search: "",
  });

  // Create service task options and filter
  const { filteredServiceTasks } = useMemo(() => {
    const options = serviceTasks.map((task: ServiceTaskWithLabels) => ({
      value: task.id,
      label: task.name,
      description: task.description,
      estimatedLabourHours: task.estimatedLabourHours,
      estimatedCost: task.estimatedCost,
    }));

    let filtered = options;
    if (serviceTaskSearch) {
      const searchLower = serviceTaskSearch.toLowerCase();
      filtered = options.filter(
        opt =>
          opt.label.toLowerCase().includes(searchLower) ||
          (opt.description &&
            opt.description.toLowerCase().includes(searchLower)),
      );
    }

    return { filteredServiceTasks: filtered };
  }, [serviceTasks, serviceTaskSearch]);

  const tabs = [
    {
      key: "service-tasks",
      label: "Service Task",
      count: value.workOrderLineItems?.length || 0,
    },
    { key: "labor", label: "Labor", count: 0 },
    { key: "parts", label: "Parts", count: 0 },
  ];

  const handleServiceTaskSelect = (serviceTask: ServiceTaskWithLabels) => {
    const newLineItem: CreateWorkOrderLineItem = {
      itemType: LineItemTypeEnum.LABOR,
      quantity: 1,
      description: "",
      inventoryItemID: null,
      assignedToUserID: null,
      serviceTaskID: serviceTask.id,
      unitPrice: null,
      hourlyRate: null,
      laborHours: null,
    };

    const updatedItems = [...(value.workOrderLineItems || []), newLineItem];
    onChange("workOrderLineItems", updatedItems);
    setServiceTaskSearch("");
  };

  const removeLineItem = (index: number) => {
    const updatedItems =
      value.workOrderLineItems?.filter((_, i) => i !== index) || [];
    onChange("workOrderLineItems", updatedItems);
  };

  const updateLineItem = (
    index: number,
    updatedLineItem: CreateWorkOrderLineItem,
  ) => {
    const updatedItems = [...(value.workOrderLineItems || [])];
    updatedItems[index] = updatedLineItem;
    onChange("workOrderLineItems", updatedItems);
  };

  return (
    <FormContainer title="Line Items" className="mt-6 max-w-4xl mx-auto w-full">
      <div className="space-y-4">
        {/* Tab Navigation */}
        <TabNavigation
          tabs={tabs}
          activeTab={activeTab}
          onTabChange={setActiveTab}
        />

        {activeTab === "service-tasks" && (
          <div className="space-y-2">
            <div className="relative">
              <Combobox
                value={null}
                onChange={(task: any) => {
                  if (task) {
                    const selectedTask = serviceTasks.find(
                      st => st.id === task.value,
                    );
                    if (selectedTask) {
                      handleServiceTaskSelect(selectedTask);
                    }
                  }
                }}
                disabled={disabled || isLoadingServiceTasks}
              >
                <div className="relative">
                  <ComboboxInput
                    className="w-full border border-gray-300 rounded-lg px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                    displayValue={() => serviceTaskSearch}
                    onChange={e => setServiceTaskSearch(e.target.value)}
                    placeholder="Search service tasks..."
                    disabled={disabled || isLoadingServiceTasks}
                  />
                  <ComboboxButton className="absolute inset-y-0 right-0 flex items-center pr-3">
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
                </div>

                <ComboboxOptions className="absolute z-10 mt-1 w-full bg-white border border-gray-200 rounded-lg shadow-lg max-h-60 overflow-auto">
                  {isLoadingServiceTasks ? (
                    <div className="px-4 py-2 text-gray-500">Loading...</div>
                  ) : filteredServiceTasks.length === 0 ? (
                    <div className="px-4 py-2 text-gray-500">
                      No service tasks found.
                    </div>
                  ) : (
                    filteredServiceTasks.map(opt => (
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
                            Est. {opt.estimatedLabourHours}h • $
                            {opt.estimatedCost}
                          </div>
                        </div>
                      </ComboboxOption>
                    ))
                  )}
                </ComboboxOptions>
              </Combobox>
            </div>
            <p className="text-xs text-gray-500">
              Search and select service tasks to add as line items
            </p>
          </div>
        )}

        {activeTab === "labor" && (
          <div className="space-y-2">
            <label className="block text-sm font-medium text-gray-700">
              Add Labor
            </label>
            <div className="p-4 border border-gray-200 rounded-lg bg-gray-50">
              <p className="text-sm text-gray-600">
                Labor management features coming soon. This tab will allow you
                to add labor items directly.
              </p>
            </div>
          </div>
        )}

        {activeTab === "parts" && (
          <div className="space-y-2">
            <label className="block text-sm font-medium text-gray-700">
              Add Parts
            </label>
            <div className="p-4 border border-gray-200 rounded-lg bg-gray-50">
              <p className="text-sm text-gray-600">
                Parts management features coming soon. This tab will allow you
                to add parts directly.
              </p>
            </div>
          </div>
        )}

        {(value.workOrderLineItems || []).map((lineItem, index) => (
          <WorkOrderLineItemForm
            key={lineItemKeys[index]}
            lineItem={lineItem}
            index={index}
            onUpdate={updatedLineItem => {
              updateLineItem(index, updatedLineItem);
            }}
            onRemove={() => {
              removeLineItem(index);
            }}
            disabled={disabled}
          />
        ))}

        {activeTab === "service-tasks" &&
          (!value.workOrderLineItems ||
            value.workOrderLineItems.length === 0) && (
            <div className="border-2 border-dashed border-gray-300 rounded-lg bg-gray-50 p-8 text-center">
              <div className="text-gray-500 mb-4">
                No Service Task line items added
              </div>
              <button
                type="button"
                onClick={() => {
                  // Focus on the service task search input
                  const searchInput = document.querySelector(
                    'input[placeholder="Search service tasks..."]',
                  ) as HTMLInputElement;
                  if (searchInput) {
                    searchInput.focus();
                  }
                }}
                className="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors"
              >
                Add Service Task
              </button>
            </div>
          )}

        {value.workOrderLineItems && value.workOrderLineItems.length > 0 && (
          <div>
            <div className="p-4">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">
                Cost Summary
              </h3>

              <div className="space-y-3">
                <div className="flex justify-between items-center">
                  <span className="text-sm font-medium text-gray-700">
                    Labor
                  </span>
                  <span className="text-sm font-semibold text-gray-900">
                    $
                    {value.workOrderLineItems
                      .reduce((sum, item) => {
                        const laborCost =
                          (item.hourlyRate || 0) * (item.laborHours || 0);
                        return sum + laborCost;
                      }, 0)
                      .toFixed(2)}
                  </span>
                </div>

                <div className="flex justify-between items-center">
                  <span className="text-sm font-medium text-gray-700">
                    Parts
                  </span>
                  <span className="text-sm font-semibold text-gray-900">
                    $
                    {value.workOrderLineItems
                      .reduce((sum, item) => {
                        const itemCost = (item.unitPrice || 0) * item.quantity;
                        return sum + itemCost;
                      }, 0)
                      .toFixed(2)}
                  </span>
                </div>

                <div className="border-t border-gray-200 pt-3">
                  <div className="flex justify-between items-center">
                    <span className="text-base font-semibold text-gray-900">
                      Subtotal
                    </span>
                    <span className="text-lg font-bold text-blue-600">
                      $
                      {value.workOrderLineItems
                        .reduce((sum, item) => {
                          const laborCost =
                            (item.hourlyRate || 0) * (item.laborHours || 0);
                          const itemCost =
                            (item.unitPrice || 0) * item.quantity;
                          return sum + laborCost + itemCost;
                        }, 0)
                        .toFixed(2)}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
      {errors.workOrderLineItems && (
        <span className="text-sm text-red-500 mt-1">
          {errors.workOrderLineItems}
        </span>
      )}
    </FormContainer>
  );
};

export default WorkOrderLineItemsForm;
