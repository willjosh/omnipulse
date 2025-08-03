import React, { useState, useMemo } from "react";
import FormField from "@/components/ui/Form/FormField";
import {
  Combobox,
  ComboboxInput,
  ComboboxButton,
  ComboboxOptions,
  ComboboxOption,
} from "@headlessui/react";
import { IconButton, Button } from "@mui/material";
import {
  Delete as DeleteIcon,
  ExpandMore as ExpandMoreIcon,
  ExpandLess as ExpandLessIcon,
  Add as AddIcon,
} from "@mui/icons-material";
import { useServiceTasks } from "@/features/service-task/hooks/useServiceTasks";
import { useInventoryItems } from "@/features/inventory-item/hooks/useInventoryItems";
import { useTechnicians } from "@/features/technician/hooks/useTechnicians";
import { ServiceTaskWithLabels } from "@/features/service-task/types/serviceTaskType";
import { InventoryItemWithLabels } from "@/features/inventory-item/types/inventoryItemType";
import { LineItemTypeEnum } from "../types/workOrderEnum";
import { CreateWorkOrderLineItem } from "../types/workOrderType";
import AddLaborModal from "./AddLaborModal";
import AddPartModal from "./AddPartModal";
import SecondaryButton from "@/components/ui/Button/SecondaryButton";
import { Labor, Parts } from "@/components/ui/Icons";

// Helper function to determine item type based on selections
const determineItemType = (
  hasLabor: boolean,
  hasParts: boolean,
): LineItemTypeEnum => {
  if (hasLabor && hasParts) return LineItemTypeEnum.BOTH;
  if (hasLabor) return LineItemTypeEnum.LABOR;
  if (hasParts) return LineItemTypeEnum.ITEM;
  return LineItemTypeEnum.LABOR; // Default to labor if nothing is selected
};

interface WorkOrderLineItemFormProps {
  lineItem: CreateWorkOrderLineItem;
  index: number;
  onUpdate: (updatedLineItem: CreateWorkOrderLineItem) => void;
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
  const [isAddLaborModalOpen, setIsAddLaborModalOpen] = useState(false);
  const [isAddPartModalOpen, setIsAddPartModalOpen] = useState(false);
  const [isLaborTableExpanded, setIsLaborTableExpanded] = useState(true);
  const [isPartsTableExpanded, setIsPartsTableExpanded] = useState(true);
  const [hasLaborAdded, setHasLaborAdded] = useState(false);
  const [hasPartsAdded, setHasPartsAdded] = useState(false);

  // Fetch data for dropdowns
  const { serviceTasks, isPending: isLoadingServiceTasks } = useServiceTasks({
    PageNumber: 1,
    PageSize: 100,
    Search: "",
  });

  const { inventoryItems, isPending: isLoadingInventoryItems } =
    useInventoryItems({ PageNumber: 1, PageSize: 100, Search: "" });

  const { technicians, isPending: isLoadingTechnicians } = useTechnicians();

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

  const updateLineItem = (field: keyof CreateWorkOrderLineItem, value: any) => {
    const updatedLineItem = { ...lineItem, [field]: value };

    // Auto-populate rates and hours based on selected items
    if (field === "serviceTaskID" && updatedLineItem.serviceTaskID) {
      const serviceTask = serviceTasks.find(
        (st: ServiceTaskWithLabels) => st.id === updatedLineItem.serviceTaskID,
      );
      if (serviceTask) {
        // Use estimated cost as hourly rate if available, otherwise keep existing
        updatedLineItem.hourlyRate =
          serviceTask.estimatedCost > 0
            ? serviceTask.estimatedCost
            : updatedLineItem.hourlyRate;
        updatedLineItem.laborHours = serviceTask.estimatedLabourHours || null;
      }
    }

    if (field === "inventoryItemID" && updatedLineItem.inventoryItemID) {
      const inventoryItem = inventoryItems.find(
        (ii: InventoryItemWithLabels) =>
          ii.id === updatedLineItem.inventoryItemID,
      );
      if (inventoryItem) {
        updatedLineItem.unitPrice = inventoryItem.unitCost || null;
      }
    }

    // Auto-determine item type based on selections
    const hasLabor = !!(
      updatedLineItem.serviceTaskID &&
      (updatedLineItem.hourlyRate || updatedLineItem.laborHours)
    );
    const hasParts = !!(
      updatedLineItem.inventoryItemID && updatedLineItem.unitPrice
    );
    updatedLineItem.itemType = determineItemType(hasLabor, hasParts);

    onUpdate(updatedLineItem);
  };

  const toggleExpanded = () => {
    setIsExpanded(!isExpanded);
  };

  const getItemTypeLabel = (itemType: LineItemTypeEnum) => {
    // Check if any labor or parts are actually added
    const hasLabor = !!(lineItem.hourlyRate && lineItem.laborHours);
    const hasParts = !!(lineItem.inventoryItemID && lineItem.unitPrice);

    if (!hasLabor && !hasParts) {
      return "Unassigned";
    }

    switch (itemType) {
      case LineItemTypeEnum.LABOR:
        return "Labor";
      case LineItemTypeEnum.ITEM:
        return "Item";
      case LineItemTypeEnum.BOTH:
        return "Both";
      default:
        return "Unknown";
    }
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

  const handleAddLabor = (labor: {
    assignedToUserID: string;
    laborHours: number;
    hourlyRate: number;
  }) => {
    const updatedLineItem = { ...lineItem };
    updatedLineItem.assignedToUserID = labor.assignedToUserID;
    updatedLineItem.laborHours = labor.laborHours;
    updatedLineItem.hourlyRate = labor.hourlyRate;

    // Auto-determine item type
    const hasLabor = !!(
      updatedLineItem.serviceTaskID &&
      (updatedLineItem.hourlyRate || updatedLineItem.laborHours)
    );
    const hasParts = !!(
      updatedLineItem.inventoryItemID && updatedLineItem.unitPrice
    );
    updatedLineItem.itemType = determineItemType(hasLabor, hasParts);

    setHasLaborAdded(true);
    onUpdate(updatedLineItem);
  };

  const handleAddPart = (part: {
    inventoryItemID: number;
    quantity: number;
    unitPrice: number;
  }) => {
    const updatedLineItem = { ...lineItem };
    updatedLineItem.inventoryItemID = part.inventoryItemID;
    updatedLineItem.quantity = part.quantity;
    updatedLineItem.unitPrice = part.unitPrice;

    // Auto-determine item type
    const hasLabor = !!(
      updatedLineItem.serviceTaskID &&
      (updatedLineItem.hourlyRate || updatedLineItem.laborHours)
    );
    const hasParts = !!(
      updatedLineItem.inventoryItemID && updatedLineItem.unitPrice
    );
    updatedLineItem.itemType = determineItemType(hasLabor, hasParts);

    setHasPartsAdded(true);
    onUpdate(updatedLineItem);
  };

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
              {selectedServiceTask
                ? selectedServiceTask.label
                : `Service Task #${index + 1}`}
            </span>
          </div>

          <div className="flex items-center space-x-6 text-sm text-gray-600">
            <span className="bg-blue-100 text-blue-800 px-2 py-1 rounded-full text-xs">
              {getItemTypeLabel(lineItem.itemType)}
            </span>
          </div>
        </div>

        <div className="flex items-center space-x-6">
          {/* Labor Cost */}
          <div className="text-center min-w-[80px]">
            <div className="text-xs text-gray-500 mb-1">Labor</div>
            <div className="text-sm font-medium text-gray-900 bg-white border border-gray-200 rounded px-2 py-1">
              $
              {(
                (lineItem.hourlyRate || 0) * (lineItem.laborHours || 0)
              ).toFixed(2)}
            </div>
          </div>

          {/* Parts Cost */}
          <div className="text-center min-w-[80px]">
            <div className="text-xs text-gray-500 mb-1">Parts</div>
            <div className="text-sm font-medium text-gray-900 bg-white border border-gray-200 rounded px-2 py-1">
              ${((lineItem.unitPrice || 0) * lineItem.quantity).toFixed(2)}
            </div>
          </div>

          {/* Subtotal */}
          <div className="text-center min-w-[80px]">
            <div className="text-xs text-gray-500 mb-1">Subtotal</div>
            <div className="text-sm font-semibold text-blue-600 bg-white border border-gray-200 rounded px-2 py-1">
              $
              {(
                (lineItem.hourlyRate || 0) * (lineItem.laborHours || 0) +
                (lineItem.unitPrice || 0) * lineItem.quantity
              ).toFixed(2)}
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
          {/* Add Labor and Add Part Buttons */}
          <div className="flex space-x-3">
            <SecondaryButton
              onClick={() => setIsAddLaborModalOpen(true)}
              disabled={disabled}
              className="flex items-center space-x-1"
            >
              <Labor />
              <span>Add Labor</span>
            </SecondaryButton>
            <SecondaryButton
              onClick={() => setIsAddPartModalOpen(true)}
              disabled={disabled}
              className="flex items-center space-x-1"
            >
              <Parts />
              <span>Add Part</span>
            </SecondaryButton>
          </div>

          {/* Description */}
          <FormField label="">
            <textarea
              value={lineItem.description || ""}
              onChange={e => updateLineItem("description", e.target.value)}
              placeholder="Additional description for this line item..."
              className="w-full border border-gray-300 rounded-3xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white min-h-[80px] resize-y"
              disabled={disabled}
            />
          </FormField>

          {/* Labor Table */}
          {hasLaborAdded && (
            <div className="border border-gray-200 rounded-lg overflow-hidden">
              <div
                className="bg-blue-50 px-4 py-3 border-b border-gray-200 cursor-pointer hover:bg-blue-100"
                onClick={() => setIsLaborTableExpanded(!isLaborTableExpanded)}
              >
                <div className="flex items-center justify-between">
                  <h3 className="text-sm font-medium text-blue-900">
                    Labor Details
                  </h3>
                  <svg
                    className={`w-4 h-4 text-blue-600 transition-transform ${
                      isLaborTableExpanded ? "rotate-180" : ""
                    }`}
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth="2"
                      d="M19 9l-7 7-7-7"
                    />
                  </svg>
                </div>
              </div>

              {isLaborTableExpanded && (
                <div className="overflow-x-auto">
                  <table className="min-w-full divide-y divide-gray-200">
                    <thead className="bg-gray-50">
                      <tr>
                        <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Technician
                        </th>
                        <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Hours
                        </th>
                        <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Rate
                        </th>
                        <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Amount
                        </th>
                      </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                      <tr>
                        <td className="px-4 py-3 whitespace-nowrap text-sm text-gray-900">
                          {selectedTechnician
                            ? selectedTechnician.label
                            : "Not assigned"}
                        </td>
                        <td className="px-4 py-3 whitespace-nowrap text-sm text-gray-900">
                          {lineItem.laborHours || 0} hrs
                        </td>
                        <td className="px-4 py-3 whitespace-nowrap text-sm text-gray-900">
                          ${lineItem.hourlyRate || 0}/hr
                        </td>
                        <td className="px-4 py-3 whitespace-nowrap text-sm font-medium text-gray-900 relative">
                          $
                          {(
                            (lineItem.hourlyRate || 0) *
                            (lineItem.laborHours || 0)
                          ).toFixed(2)}
                          <button
                            type="button"
                            onClick={e => {
                              e.preventDefault();
                              e.stopPropagation();
                              setIsAddLaborModalOpen(true);
                            }}
                            className="absolute right-2 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600 p-1"
                            disabled={disabled}
                          >
                            <svg
                              className="w-4 h-4"
                              fill="none"
                              stroke="currentColor"
                              viewBox="0 0 24 24"
                            >
                              <path
                                strokeLinecap="round"
                                strokeLinejoin="round"
                                strokeWidth="2"
                                d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"
                              />
                            </svg>
                          </button>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          )}

          {/* Parts Table */}
          {hasPartsAdded && (
            <div className="border border-gray-200 rounded-lg overflow-hidden">
              <div
                className="bg-green-50 px-4 py-3 border-b border-gray-200 cursor-pointer hover:bg-green-100"
                onClick={() => setIsPartsTableExpanded(!isPartsTableExpanded)}
              >
                <div className="flex items-center justify-between">
                  <h3 className="text-sm font-medium text-green-900">
                    Parts Details
                  </h3>
                  <svg
                    className={`w-4 h-4 text-green-600 transition-transform ${
                      isPartsTableExpanded ? "rotate-180" : ""
                    }`}
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth="2"
                      d="M19 9l-7 7-7-7"
                    />
                  </svg>
                </div>
              </div>

              {isPartsTableExpanded && (
                <div className="overflow-x-auto">
                  <table className="min-w-full divide-y divide-gray-200">
                    <thead className="bg-gray-50">
                      <tr>
                        <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Part
                        </th>
                        <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Quantity
                        </th>
                        <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Unit Cost
                        </th>
                        <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Amount
                        </th>
                      </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                      <tr>
                        <td className="px-4 py-3 whitespace-nowrap text-sm text-gray-900">
                          {selectedInventoryItem
                            ? selectedInventoryItem.label
                            : "No part selected"}
                        </td>
                        <td className="px-4 py-3 whitespace-nowrap text-sm text-gray-900">
                          {lineItem.quantity || 0}
                        </td>
                        <td className="px-4 py-3 whitespace-nowrap text-sm text-gray-900">
                          ${lineItem.unitPrice || 0}
                        </td>
                        <td className="px-4 py-3 whitespace-nowrap text-sm font-medium text-gray-900 relative">
                          $
                          {(
                            (lineItem.unitPrice || 0) * lineItem.quantity
                          ).toFixed(2)}
                          <button
                            type="button"
                            onClick={e => {
                              e.preventDefault();
                              e.stopPropagation();
                              setIsAddPartModalOpen(true);
                            }}
                            className="absolute right-2 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600 p-1"
                            disabled={disabled}
                          >
                            <svg
                              className="w-4 h-4"
                              fill="none"
                              stroke="currentColor"
                              viewBox="0 0 24 24"
                            >
                              <path
                                strokeLinecap="round"
                                strokeLinejoin="round"
                                strokeWidth="2"
                                d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"
                              />
                            </svg>
                          </button>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          )}
        </div>
      )}

      {/* Add Labor Modal */}
      <AddLaborModal
        isOpen={isAddLaborModalOpen}
        onClose={() => setIsAddLaborModalOpen(false)}
        onSave={handleAddLabor}
        initialValues={{
          assignedToUserID: lineItem.assignedToUserID || "",
          laborHours: lineItem.laborHours || 1,
          hourlyRate: lineItem.hourlyRate || 0,
        }}
      />

      {/* Add Part Modal */}
      <AddPartModal
        isOpen={isAddPartModalOpen}
        onClose={() => setIsAddPartModalOpen(false)}
        onSave={handleAddPart}
        initialValues={{
          inventoryItemID: lineItem.inventoryItemID || 0,
          quantity: lineItem.quantity || 1,
          unitPrice: lineItem.unitPrice || 0,
        }}
      />
    </div>
  );
};

export default WorkOrderLineItemForm;
