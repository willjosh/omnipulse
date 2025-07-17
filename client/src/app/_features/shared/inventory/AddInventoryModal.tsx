"use client";

import React, { useState } from "react";
import { useCreateInventoryItem } from "@/app/_hooks/inventory-item/useInventoryItem";
import { CreateInventoryItemCommand } from "@/app/_hooks/inventory-item/inventoryItemType";
import {
  InventoryItemCategoryEnum,
  InventoryItemUnitCostMeasurementUnitEnum,
} from "@/app/_hooks/inventory-item/inventoryItemEnum";
import { X } from "lucide-react";
import { PrimaryButton, SecondaryButton } from "@/app/_features/shared/button";

interface AddInventoryModalProps {
  isOpen: boolean;
  onClose: () => void;
}

const AddInventoryModal: React.FC<AddInventoryModalProps> = ({
  isOpen,
  onClose,
}) => {
  const [formData, setFormData] = useState<CreateInventoryItemCommand>({
    ItemNumber: "",
    ItemName: "",
    Description: "",
    Category: InventoryItemCategoryEnum.ENGINE,
    Manufacturer: "",
    ManufacturerPartNumber: "",
    UniversalProductCode: "",
    UnitCost: 0,
    UnitCostMeasurementUnit: InventoryItemUnitCostMeasurementUnitEnum.Unit,
    Supplier: "",
    WeightKG: 0,
    IsActive: true,
  });

  const createInventoryMutation = useCreateInventoryItem();

  const handleInputChange = (
    field: keyof CreateInventoryItemCommand,
    value: any,
  ) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formData.ItemNumber.trim() || !formData.ItemName.trim()) {
      alert("Item Number and Item Name are required");
      return;
    }

    try {
      await createInventoryMutation.mutateAsync(formData);

      // Reset form and close modal
      setFormData({
        ItemNumber: "",
        ItemName: "",
        Description: "",
        Category: InventoryItemCategoryEnum.ENGINE,
        Manufacturer: "",
        ManufacturerPartNumber: "",
        UniversalProductCode: "",
        UnitCost: 0,
        UnitCostMeasurementUnit: InventoryItemUnitCostMeasurementUnitEnum.Unit,
        Supplier: "",
        WeightKG: 0,
        IsActive: true,
      });
      onClose();
    } catch (error) {
      console.error("Error creating inventory item:", error);
      alert("Failed to create inventory item. Please try again.");
    }
  };

  const handleClose = () => {
    // Reset form when closing
    setFormData({
      ItemNumber: "",
      ItemName: "",
      Description: "",
      Category: InventoryItemCategoryEnum.ENGINE,
      Manufacturer: "",
      ManufacturerPartNumber: "",
      UniversalProductCode: "",
      UnitCost: 0,
      UnitCostMeasurementUnit: InventoryItemUnitCostMeasurementUnitEnum.Unit,
      Supplier: "",
      WeightKG: 0,
      IsActive: true,
    });
    onClose();
  };

  if (!isOpen) return null;

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

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg shadow-xl w-full max-w-2xl max-h-[90vh] overflow-y-auto">
        <div className="flex items-center justify-between p-6 border-b border-gray-200">
          <h2 className="text-xl font-semibold text-gray-900">Add New Part</h2>
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
                Item Number *
              </label>
              <input
                type="text"
                value={formData.ItemNumber || ""}
                onChange={e => handleInputChange("ItemNumber", e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Item Name *
              </label>
              <input
                type="text"
                value={formData.ItemName}
                onChange={e => handleInputChange("ItemName", e.target.value)}
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
              value={formData.Description || ""}
              onChange={e => handleInputChange("Description", e.target.value)}
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
                value={formData.Category ?? InventoryItemCategoryEnum.ENGINE}
                onChange={e =>
                  handleInputChange("Category", parseInt(e.target.value))
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
                value={formData.Manufacturer || ""}
                onChange={e =>
                  handleInputChange("Manufacturer", e.target.value)
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
                value={formData.ManufacturerPartNumber || ""}
                onChange={e =>
                  handleInputChange("ManufacturerPartNumber", e.target.value)
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
                value={formData.UniversalProductCode || ""}
                onChange={e =>
                  handleInputChange("UniversalProductCode", e.target.value)
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
                value={formData.UnitCost ?? 0}
                onChange={e =>
                  handleInputChange("UnitCost", parseFloat(e.target.value) || 0)
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
                  formData.UnitCostMeasurementUnit ??
                  InventoryItemUnitCostMeasurementUnitEnum.Unit
                }
                onChange={e =>
                  handleInputChange(
                    "UnitCostMeasurementUnit",
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
                value={formData.Supplier || ""}
                onChange={e => handleInputChange("Supplier", e.target.value)}
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
                value={formData.WeightKG ?? 0}
                onChange={e =>
                  handleInputChange("WeightKG", parseFloat(e.target.value) || 0)
                }
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>

          <div className="flex items-center">
            <input
              type="checkbox"
              id="isActive"
              checked={formData.IsActive}
              onChange={e => handleInputChange("IsActive", e.target.checked)}
              className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
            />
            <label htmlFor="isActive" className="ml-2 text-sm text-gray-700">
              Active
            </label>
          </div>

          <div className="flex justify-end gap-3 pt-6 border-t border-gray-200">
            <SecondaryButton onClick={handleClose}>Cancel</SecondaryButton>
            <PrimaryButton
              type="submit"
              disabled={createInventoryMutation.isPending}
            >
              {createInventoryMutation.isPending
                ? "Creating..."
                : "Create Part"}
            </PrimaryButton>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddInventoryModal;
