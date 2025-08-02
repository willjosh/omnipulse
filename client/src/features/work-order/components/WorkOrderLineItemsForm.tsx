import React, { useState } from "react";
import FormContainer from "@/components/ui/Form/FormContainer";
import { Button } from "@mui/material";
import { Add as AddIcon } from "@mui/icons-material";
import { LineItemTypeEnum } from "../types/workOrderEnum";
import { WorkOrderLineItem } from "../types/workOrderType";
import WorkOrderLineItemForm from "./WorkOrderLineItemForm";

export interface WorkOrderLineItemsFormValues {
  workOrderLineItems: WorkOrderLineItem[];
}

interface WorkOrderLineItemsFormProps {
  value: WorkOrderLineItemsFormValues;
  errors: Partial<Record<keyof WorkOrderLineItemsFormValues, string>>;
  onChange: (
    field: keyof WorkOrderLineItemsFormValues,
    value: WorkOrderLineItem[],
  ) => void;
  disabled?: boolean;
}

const WorkOrderLineItemsForm: React.FC<WorkOrderLineItemsFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
}) => {
  // Add a unique instance ID to help with React reconciliation
  const instanceId = React.useMemo(
    () => `form-${Date.now()}-${Math.random()}`,
    [],
  );

  // Generate stable keys for each line item
  const lineItemKeys = React.useMemo(() => {
    return (value.workOrderLineItems || []).map(
      (_, index) => `${instanceId}-line-${index}`,
    );
  }, [instanceId, value.workOrderLineItems?.length]);
  const addLineItem = () => {
    const newLineItem: WorkOrderLineItem = {
      id: Date.now() + Math.random(), // Simple unique ID
      workOrderID: 0, // Will be set when work order is created
      itemType: LineItemTypeEnum.LABOR,
      quantity: 1,
      description: "",
      inventoryItemID: null,
      inventoryItemName: "",
      assignedToUserID: "",
      assignedToUserName: "",
      subTotal: 0,
      laborCost: 0,
      itemCost: 0,
      serviceTaskID: 0,
      serviceTaskName: "",
    };

    const updatedItems = [...(value.workOrderLineItems || []), newLineItem];
    onChange("workOrderLineItems", updatedItems);
  };

  const removeLineItem = (index: number) => {
    const updatedItems =
      value.workOrderLineItems?.filter((_, i) => i !== index) || [];
    onChange("workOrderLineItems", updatedItems);
  };

  const updateLineItem = (
    index: number,
    updatedLineItem: WorkOrderLineItem,
  ) => {
    const updatedItems = [...(value.workOrderLineItems || [])];
    updatedItems[index] = updatedLineItem;
    onChange("workOrderLineItems", updatedItems);
  };

  return (
    <FormContainer title="Line Items" className="mt-6 max-w-4xl mx-auto w-full">
      <div className="space-y-4">
        {/* Header with Add Line Item Button */}
        <div className="flex items-center justify-between border-b border-gray-200 pb-2">
          <p className="text-xs text-gray-500">
            Add line items for labor, parts, or services
          </p>
          <Button
            variant="outlined"
            startIcon={<AddIcon />}
            onClick={addLineItem}
            disabled={disabled}
            size="small"
          >
            Add Line Item
          </Button>
        </div>

        {/* Line Items List */}
        {(value.workOrderLineItems || [])
          .filter(lineItem => lineItem.id && lineItem.id > 0)
          .map((lineItem, index) => (
            <WorkOrderLineItemForm
              key={lineItemKeys[index]}
              lineItem={lineItem}
              index={index}
              onUpdate={updatedLineItem => {
                const originalIndex =
                  value.workOrderLineItems?.findIndex(
                    item => item.id === lineItem.id,
                  ) ?? -1;
                if (originalIndex !== -1) {
                  updateLineItem(originalIndex, updatedLineItem);
                }
              }}
              onRemove={() => {
                const originalIndex =
                  value.workOrderLineItems?.findIndex(
                    item => item.id === lineItem.id,
                  ) ?? -1;
                if (originalIndex !== -1) {
                  removeLineItem(originalIndex);
                }
              }}
              disabled={disabled}
            />
          ))}

        {/* Empty State */}
        {(!value.workOrderLineItems ||
          value.workOrderLineItems.length === 0) && (
          <div className="text-center py-8 text-gray-500">
            No line items added yet. Click "Add Line Item" to get started.
          </div>
        )}

        {/* Total Summary */}
        {value.workOrderLineItems && value.workOrderLineItems.length > 0 && (
          <div className="border-t pt-4">
            <div className="flex justify-between items-center text-lg font-semibold">
              <span>Total Cost:</span>
              <span className="text-blue-600">
                $
                {value.workOrderLineItems
                  .reduce((sum, item) => sum + item.subTotal, 0)
                  .toFixed(2)}
              </span>
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
