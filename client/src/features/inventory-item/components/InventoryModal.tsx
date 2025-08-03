"use client";

import React, { useState, useEffect } from "react";
import {
  useCreateInventoryItem,
  useUpdateInventoryItem,
} from "../hooks/useInventoryItems";
import {
  CreateInventoryItemCommand,
  UpdateInventoryItemCommand,
  InventoryItemWithLabels,
} from "../types/inventoryItemType";
import {
  InventoryItemCategoryEnum,
  InventoryItemUnitCostMeasurementUnitEnum,
} from "../types/inventoryItemEnum";
import { X } from "lucide-react";
import { PrimaryButton, SecondaryButton } from "@/components/ui/Button";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import ModalPortal from "@/components/ui/Modal/ModalPortal";

interface InventoryModalProps {
  isOpen: boolean;
  onClose: () => void;
  mode: "create" | "edit";
  item?: InventoryItemWithLabels | null;
}

const InventoryModal: React.FC<InventoryModalProps> = ({
  isOpen,
  onClose,
  mode,
  item,
}) => {
  const notify = useNotification();
  const [formData, setFormData] = useState({
    itemNumber: "",
    itemName: "",
    description: "",
    category: InventoryItemCategoryEnum.ENGINE,
    manufacturer: "",
    manufacturerPartNumber: "",
    universalProductCode: "",
    unitCost: 0,
    unitCostMeasurementUnit: InventoryItemUnitCostMeasurementUnitEnum.Unit,
    supplier: "",
    weightKG: 0,
    isActive: true,
  });

  const createInventoryMutation = useCreateInventoryItem();
  const updateInventoryMutation = useUpdateInventoryItem();

  useEffect(() => {
    if (mode === "edit" && item) {
      setFormData({
        itemNumber: item.itemNumber || "",
        itemName: item.itemName || "",
        description: item.description || "",
        category: item.category ?? InventoryItemCategoryEnum.ENGINE,
        manufacturer: item.manufacturer || "",
        manufacturerPartNumber: item.manufacturerPartNumber || "",
        universalProductCode: item.universalProductCode || "",
        unitCost: item.unitCost ?? 0,
        unitCostMeasurementUnit:
          item.unitCostMeasurementUnit ??
          InventoryItemUnitCostMeasurementUnitEnum.Unit,
        supplier: item.supplier || "",
        weightKG: item.weightKG ?? 0,
        isActive: item.isActive ?? true,
      });
    } else if (mode === "create") {
      setFormData({
        itemNumber: "",
        itemName: "",
        description: "",
        category: InventoryItemCategoryEnum.ENGINE,
        manufacturer: "",
        manufacturerPartNumber: "",
        universalProductCode: "",
        unitCost: 0,
        unitCostMeasurementUnit: InventoryItemUnitCostMeasurementUnitEnum.Unit,
        supplier: "",
        weightKG: 0,
        isActive: true,
      });
    }
  }, [mode, item, isOpen]);

  const handleInputChange = (field: string, value: any) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formData.itemNumber.trim() || !formData.itemName.trim()) {
      notify("Item Number and Item Name are required", "error");
      return;
    }

    try {
      if (mode === "create") {
        const createCommand: CreateInventoryItemCommand = formData;
        await createInventoryMutation.mutateAsync(createCommand);
        notify("Inventory item created successfully!", "success");
      } else {
        const updateCommand: UpdateInventoryItemCommand = {
          inventoryItemID: item!.id,
          ...formData,
        };
        await updateInventoryMutation.mutateAsync(updateCommand);
        notify("Inventory item updated successfully!", "success");
      }

      onClose();
    } catch (error: any) {
      console.error(
        `Error ${mode === "create" ? "creating" : "updating"} inventory item:`,
        error,
      );

      let errorMessage =
        mode === "create"
          ? "Failed to create inventory item. Please check your input and try again."
          : "Failed to update inventory item. Please check your input and try again.";

      if (error?.response?.data) {
        const errorData = error.response.data;

        if (errorData.errors && typeof errorData.errors === "object") {
          const validationMessages = [];
          for (const [field, messages] of Object.entries(errorData.errors)) {
            if (Array.isArray(messages)) {
              validationMessages.push(...messages);
            } else if (typeof messages === "string") {
              validationMessages.push(messages);
            }
          }
          if (validationMessages.length > 0) {
            errorMessage = validationMessages.join(" ");
          }
        } else if (errorData.message) {
          errorMessage = errorData.message;
        } else if (errorData.title) {
          errorMessage = errorData.detail || errorData.title;
        }
      }

      notify(errorMessage, "error");
    }
  };

  const handleClose = () => {
    if (mode === "create") {
      setFormData({
        itemNumber: "",
        itemName: "",
        description: "",
        category: InventoryItemCategoryEnum.ENGINE,
        manufacturer: "",
        manufacturerPartNumber: "",
        universalProductCode: "",
        unitCost: 0,
        unitCostMeasurementUnit: InventoryItemUnitCostMeasurementUnitEnum.Unit,
        supplier: "",
        weightKG: 0,
        isActive: true,
      });
    }

    onClose();
  };

  if (!isOpen) return null;

  const isLoading =
    mode === "create"
      ? createInventoryMutation.isPending
      : updateInventoryMutation.isPending;

  const categoryOptions = [
    { value: InventoryItemCategoryEnum.ENGINE, label: "Engine" },
    { value: InventoryItemCategoryEnum.TRANSMISSION, label: "Transmission" },
    { value: InventoryItemCategoryEnum.BRAKES, label: "Brakes" },
    { value: InventoryItemCategoryEnum.TIRES, label: "Tires" },
    { value: InventoryItemCategoryEnum.ELECTRICAL, label: "Electrical" },
    { value: InventoryItemCategoryEnum.BODY, label: "Body" },
    { value: InventoryItemCategoryEnum.INTERIOR, label: "Interior" },
    { value: InventoryItemCategoryEnum.FLUIDS, label: "Fluids" },
    { value: InventoryItemCategoryEnum.FILTERS, label: "Filters" },
  ];

  const unitOptions = [
    { value: InventoryItemUnitCostMeasurementUnitEnum.Unit, label: "Unit" },
    { value: InventoryItemUnitCostMeasurementUnitEnum.Litre, label: "Litre" },
    { value: InventoryItemUnitCostMeasurementUnitEnum.Gram, label: "Gram" },
    {
      value: InventoryItemUnitCostMeasurementUnitEnum.Kilogram,
      label: "Kilogram",
    },
    { value: InventoryItemUnitCostMeasurementUnitEnum.Metre, label: "Metre" },
    {
      value: InventoryItemUnitCostMeasurementUnitEnum.SquareMetre,
      label: "Square Metre",
    },
    {
      value: InventoryItemUnitCostMeasurementUnitEnum.CubicMetre,
      label: "Cubic Metre",
    },
    { value: InventoryItemUnitCostMeasurementUnitEnum.Box, label: "Box" },
  ];

  const modalTitle =
    mode === "create" ? "Add New Item" : `Edit Item - ${item?.itemName}`;

  const submitButtonText =
    mode === "create"
      ? isLoading
        ? "Creating..."
        : "Create Item"
      : isLoading
        ? "Updating..."
        : "Update Item";

  return (
    <ModalPortal isOpen={isOpen}>
      <div className="fixed inset-0 backdrop-brightness-50 bg-opacity-50 flex items-center justify-center z-[100]">
        <div className="bg-white rounded-lg shadow-xl w-full max-w-2xl max-h-[90vh] overflow-y-auto">
          <div className="flex items-center justify-between p-6 border-b border-gray-200">
            <h2 className="text-xl font-semibold text-gray-900">
              {modalTitle}
            </h2>
            <button
              onClick={handleClose}
              className="text-gray-400 hover:text-gray-600"
            >
              <X className="h-6 w-6" />
            </button>
          </div>

          <form onSubmit={handleSubmit} className="p-6 space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Item Number
                </label>
                <input
                  type="text"
                  value={formData.itemNumber || ""}
                  onChange={e =>
                    handleInputChange("itemNumber", e.target.value)
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Item Name
                </label>
                <input
                  type="text"
                  value={formData.itemName || ""}
                  onChange={e => handleInputChange("itemName", e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  required
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Description
              </label>
              <textarea
                value={formData.description || ""}
                onChange={e => handleInputChange("description", e.target.value)}
                rows={3}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Category
                </label>
                <select
                  value={formData.category ?? InventoryItemCategoryEnum.ENGINE}
                  onChange={e =>
                    handleInputChange("category", parseInt(e.target.value))
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  {categoryOptions.map(option => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Manufacturer
                </label>
                <input
                  type="text"
                  value={formData.manufacturer || ""}
                  onChange={e =>
                    handleInputChange("manufacturer", e.target.value)
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Manufacturer Part Number
                </label>
                <input
                  type="text"
                  value={formData.manufacturerPartNumber || ""}
                  onChange={e =>
                    handleInputChange("manufacturerPartNumber", e.target.value)
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Universal Product Code
                </label>
                <input
                  type="text"
                  value={formData.universalProductCode || ""}
                  onChange={e =>
                    handleInputChange("universalProductCode", e.target.value)
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Unit Cost
                </label>
                <input
                  type="number"
                  step="0.01"
                  min="0"
                  value={formData.unitCost ?? 0}
                  onChange={e =>
                    handleInputChange(
                      "unitCost",
                      parseFloat(e.target.value) || 0,
                    )
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Unit Cost Measurement
                </label>
                <select
                  value={
                    formData.unitCostMeasurementUnit ??
                    InventoryItemUnitCostMeasurementUnitEnum.Unit
                  }
                  onChange={e =>
                    handleInputChange(
                      "unitCostMeasurementUnit",
                      parseInt(e.target.value),
                    )
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  {unitOptions.map(option => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </select>
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Supplier
                </label>
                <input
                  type="text"
                  value={formData.supplier || ""}
                  onChange={e => handleInputChange("supplier", e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Weight (KG)
                </label>
                <input
                  type="number"
                  step="0.01"
                  min="0"
                  value={formData.weightKG ?? 0}
                  onChange={e =>
                    handleInputChange(
                      "weightKG",
                      parseFloat(e.target.value) || 0,
                    )
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>
            </div>

            <div className="flex items-center">
              <input
                type="checkbox"
                id="isActive"
                checked={formData.isActive}
                onChange={e => handleInputChange("isActive", e.target.checked)}
                className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
              />
              <label htmlFor="isActive" className="ml-2 text-sm text-gray-700">
                Active
              </label>
            </div>

            <div className="flex justify-end gap-3 pt-6 border-t border-gray-200">
              <SecondaryButton onClick={handleClose}>Cancel</SecondaryButton>
              <PrimaryButton type="submit" disabled={isLoading}>
                {submitButtonText}
              </PrimaryButton>
            </div>
          </form>
        </div>
      </div>
    </ModalPortal>
  );
};

export default InventoryModal;
