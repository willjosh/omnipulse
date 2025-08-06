"use client";

import React, { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { PrimaryButton, SecondaryButton } from "@/components/ui/Button";
import { useUpdateInventory } from "../hooks/useInventory";
import { Inventory, UpdateInventoryCommand } from "../types/inventoryType";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import { getErrorFields, getErrorMessage } from "@/utils/fieldErrorUtils";

interface InventoryFormContainerProps {
  mode: "edit";
  inventoryData: Inventory;
}

const InventoryFormContainer: React.FC<InventoryFormContainerProps> = ({
  mode,
  inventoryData,
}) => {
  const router = useRouter();
  const notify = useNotification();
  const updateInventoryMutation = useUpdateInventory();

  const [formData, setFormData] = useState({
    quantityOnHand: inventoryData?.quantityOnHand || 0,
    unitCost: inventoryData?.unitCost || 0,
    minStockLevel: inventoryData?.minStockLevel || 0,
    maxStockLevel: inventoryData?.maxStockLevel || 0,
    isAdjustment: false,
  });

  const [errors, setErrors] = useState<{
    quantityOnHand?: string;
    unitCost?: string;
    minStockLevel?: string;
    maxStockLevel?: string;
  }>({});

  useEffect(() => {
    if (inventoryData) {
      setFormData({
        quantityOnHand: inventoryData.quantityOnHand,
        unitCost: inventoryData.unitCost,
        minStockLevel: inventoryData.minStockLevel,
        maxStockLevel: inventoryData.maxStockLevel,
        isAdjustment: false,
      });
    }
  }, [inventoryData]);

  const validate = () => {
    const newErrors: typeof errors = {};

    if (formData.quantityOnHand < 0) {
      newErrors.quantityOnHand = "Quantity on hand cannot be negative";
    }

    if (formData.unitCost < 0) {
      newErrors.unitCost = "Unit cost cannot be negative";
    }

    if (formData.minStockLevel < 0) {
      newErrors.minStockLevel = "Minimum stock level cannot be negative";
    }

    if (formData.maxStockLevel < 0) {
      newErrors.maxStockLevel = "Maximum stock level cannot be negative";
    }

    if (formData.maxStockLevel < formData.minStockLevel) {
      newErrors.maxStockLevel =
        "Maximum stock level must be greater than or equal to minimum stock level";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validate()) {
      // Create a summary of validation errors
      const validationErrors = Object.values(errors).filter(Boolean);
      if (validationErrors.length > 0) {
        const errorMessage = `Please fix the following issues:\n• ${validationErrors.join("\n• ")}`;
        notify(errorMessage, "error");
      }
      return;
    }

    try {
      const command: UpdateInventoryCommand = {
        inventoryID: inventoryData.id,
        quantityOnHand: formData.quantityOnHand,
        unitCost: formData.unitCost,
        minStockLevel: formData.minStockLevel,
        maxStockLevel: formData.maxStockLevel,
        isAdjustment: formData.isAdjustment,
        performedByUserID: "current-user",
      };

      await updateInventoryMutation.mutateAsync(command);
      notify("Inventory updated successfully!", "success");
      router.push("/inventory");
    } catch (error: any) {
      console.error("Failed to update inventory:", error);

      const errorMessage = getErrorMessage(
        error,
        "Failed to update inventory. Please try again.",
      );
      const fieldErrors = getErrorFields(error, [
        "quantityOnHand",
        "unitCost",
        "minStockLevel",
        "maxStockLevel",
      ]);

      setErrors(fieldErrors);
      notify(errorMessage, "error");
    }
  };

  const handleChange = (field: string, value: any) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    if (errors[field as keyof typeof errors]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  const handleCancel = () => {
    router.push("/inventory");
  };

  return (
    <div className="max-w-4xl mx-auto p-6">
      <div className="mb-6">
        <h1 className="text-2xl font-semibold text-gray-900">
          Edit Inventory - {inventoryData?.inventoryItemName}
        </h1>
        <p className="text-gray-600 mt-1">
          Update inventory levels and pricing information
        </p>
      </div>

      <div className="bg-white rounded-lg shadow">
        <form onSubmit={handleSubmit} className="p-6 space-y-6">
          {/* Item Information (Read-only) */}
          <div className="bg-gray-50 p-4 rounded-lg">
            <h3 className="text-lg font-medium text-gray-900 mb-4">
              Item Information
            </h3>
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Item Name
                </label>
                <div className="mt-1 text-sm text-gray-900">
                  {inventoryData?.inventoryItemName}
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Location
                </label>
                <div className="mt-1 text-sm text-gray-900">
                  {inventoryData?.locationName}
                </div>
              </div>
            </div>
          </div>

          {/* Editable Fields */}
          <div className="grid grid-cols-2 gap-6">
            <div>
              <label
                htmlFor="quantityOnHand"
                className="block text-sm font-medium text-gray-700"
              >
                Quantity on Hand *
              </label>
              <input
                type="number"
                id="quantityOnHand"
                min="0"
                step="1"
                value={formData.quantityOnHand || ""}
                onChange={e => {
                  const value =
                    e.target.value === "" ? null : parseInt(e.target.value);
                  handleChange("quantityOnHand", value || 0);
                }}
                className={`mt-1 block w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                  errors.quantityOnHand ? "border-red-300" : "border-gray-300"
                }`}
              />
              {errors.quantityOnHand && (
                <p className="mt-2 text-sm text-red-600">
                  {errors.quantityOnHand}
                </p>
              )}
            </div>

            <div>
              <label
                htmlFor="unitCost"
                className="block text-sm font-medium text-gray-700"
              >
                Unit Cost ($) *
              </label>
              <input
                type="number"
                id="unitCost"
                min="0"
                step="0.01"
                value={formData.unitCost}
                onChange={e =>
                  handleChange("unitCost", parseFloat(e.target.value) || 0)
                }
                className={`mt-1 block w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                  errors.unitCost ? "border-red-300" : "border-gray-300"
                }`}
              />
              {errors.unitCost && (
                <p className="mt-2 text-sm text-red-600">{errors.unitCost}</p>
              )}
            </div>

            <div>
              <label
                htmlFor="minStockLevel"
                className="block text-sm font-medium text-gray-700"
              >
                Minimum Stock Level *
              </label>
              <input
                type="number"
                id="minStockLevel"
                min="0"
                step="1"
                value={formData.minStockLevel || ""}
                onChange={e => {
                  const value =
                    e.target.value === "" ? null : parseInt(e.target.value);
                  handleChange("minStockLevel", value || 0);
                }}
                className={`mt-1 block w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                  errors.minStockLevel ? "border-red-300" : "border-gray-300"
                }`}
              />
              {errors.minStockLevel && (
                <p className="mt-2 text-sm text-red-600">
                  {errors.minStockLevel}
                </p>
              )}
            </div>

            <div>
              <label
                htmlFor="maxStockLevel"
                className="block text-sm font-medium text-gray-700"
              >
                Maximum Stock Level *
              </label>
              <input
                type="number"
                id="maxStockLevel"
                min="0"
                step="1"
                value={formData.maxStockLevel || ""}
                onChange={e => {
                  const value =
                    e.target.value === "" ? null : parseInt(e.target.value);
                  handleChange("maxStockLevel", value || 0);
                }}
                className={`mt-1 block w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                  errors.maxStockLevel ? "border-red-300" : "border-gray-300"
                }`}
              />
              {errors.maxStockLevel && (
                <p className="mt-2 text-sm text-red-600">
                  {errors.maxStockLevel}
                </p>
              )}
            </div>
          </div>

          <div className="flex items-center">
            <input
              id="isAdjustment"
              type="checkbox"
              checked={formData.isAdjustment}
              onChange={e => handleChange("isAdjustment", e.target.checked)}
              className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
            />
            <label
              htmlFor="isAdjustment"
              className="ml-2 block text-sm text-gray-900"
            >
              Mark as inventory adjustment
            </label>
          </div>

          {/* Action Buttons */}
          <div className="flex justify-end space-x-3 pt-6 border-t border-gray-200">
            <SecondaryButton
              type="button"
              onClick={handleCancel}
              disabled={updateInventoryMutation.isPending}
            >
              Cancel
            </SecondaryButton>
            <PrimaryButton
              type="submit"
              disabled={updateInventoryMutation.isPending}
            >
              {updateInventoryMutation.isPending
                ? "Updating..."
                : "Update Inventory"}
            </PrimaryButton>
          </div>
        </form>
      </div>
    </div>
  );
};

export default InventoryFormContainer;
