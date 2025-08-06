"use client";

import React, { useState, useEffect } from "react";
import {
  useCreateInventoryItem,
  useUpdateInventoryItem,
} from "../hooks/useInventoryItem";
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
import { getErrorMessage, getErrorFields } from "@/utils/fieldErrorUtils";

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
  const [errors, setErrors] = useState<Record<string, string>>({});
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
    setErrors({});

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
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: "" }));
    }
  };

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    // Required text fields
    if (!formData.itemNumber.trim()) {
      newErrors.itemNumber = "Item Number is required";
    }
    if (!formData.itemName.trim()) {
      newErrors.itemName = "Item Name is required";
    }
    if (!formData.description.trim()) {
      newErrors.description = "Description is required";
    }
    if (!formData.manufacturer.trim()) {
      newErrors.manufacturer = "Manufacturer is required";
    }
    if (!formData.manufacturerPartNumber.trim()) {
      newErrors.manufacturerPartNumber = "Manufacturer Part Number is required";
    }
    if (!formData.universalProductCode.trim()) {
      newErrors.universalProductCode = "Universal Product Code is required";
    }
    if (!formData.supplier.trim()) {
      newErrors.supplier = "Supplier is required";
    }

    // Required number fields
    if (formData.unitCost <= 0) {
      newErrors.unitCost = "Valid unit cost is required";
    }
    if (formData.weightKG <= 0) {
      newErrors.weightKG = "Valid weight is required";
    }

    // Required dropdown fields
    if (formData.category === null || formData.category === undefined) {
      newErrors.category = "Category is required";
    }
    if (
      formData.unitCostMeasurementUnit === null ||
      formData.unitCostMeasurementUnit === undefined
    ) {
      newErrors.unitCostMeasurementUnit = "Unit Cost Measurement is required";
    }

    return newErrors;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const validationErrors = validateForm();
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);

      const missingFields = Object.values(validationErrors);
      const errorMessage = `Please fill in the following required fields:\n• ${missingFields.join("\n• ")}`;

      notify(errorMessage, "error");
      return;
    }

    setErrors({});

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
      let errorMessage =
        mode === "create"
          ? getErrorMessage(
              error,
              "Failed to create inventory item. Please check your input and try again.",
            )
          : getErrorMessage(
              error,
              "Failed to update inventory item. Please check your input and try again.",
            );

      const fieldErrors = getErrorFields(error, [
        "itemNumber",
        "itemName",
        "description",
        "category",
        "manufacturer",
        "manufacturerPartNumber",
        "universalProductCode",
        "unitCost",
        "unitCostMeasurementUnit",
        "supplier",
        "weightKG",
      ]);

      setErrors(fieldErrors);
      notify(errorMessage, "error");
    }
  };

  const handleClose = () => {
    setErrors({});

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
                  Item Number <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  value={formData.itemNumber || ""}
                  onChange={e =>
                    handleInputChange("itemNumber", e.target.value)
                  }
                  className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                    errors.itemNumber ? "border-red-300" : "border-gray-300"
                  }`}
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Item Name <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  value={formData.itemName || ""}
                  onChange={e => handleInputChange("itemName", e.target.value)}
                  className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                    errors.itemName ? "border-red-300" : "border-gray-300"
                  }`}
                  required
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Description <span className="text-red-500">*</span>
              </label>
              <textarea
                value={formData.description || ""}
                onChange={e => handleInputChange("description", e.target.value)}
                rows={3}
                className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                  errors.description ? "border-red-300" : "border-gray-300"
                }`}
              />
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Category <span className="text-red-500">*</span>
                </label>
                <select
                  value={formData.category ?? InventoryItemCategoryEnum.ENGINE}
                  onChange={e =>
                    handleInputChange("category", parseInt(e.target.value))
                  }
                  className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                    errors.category ? "border-red-300" : "border-gray-300"
                  }`}
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
                  Manufacturer <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  value={formData.manufacturer || ""}
                  onChange={e =>
                    handleInputChange("manufacturer", e.target.value)
                  }
                  className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                    errors.manufacturer ? "border-red-300" : "border-gray-300"
                  }`}
                />
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Manufacturer Part Number{" "}
                  <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  value={formData.manufacturerPartNumber || ""}
                  onChange={e =>
                    handleInputChange("manufacturerPartNumber", e.target.value)
                  }
                  className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                    errors.manufacturerPartNumber
                      ? "border-red-300"
                      : "border-gray-300"
                  }`}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Universal Product Code <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  value={formData.universalProductCode || ""}
                  onChange={e =>
                    handleInputChange("universalProductCode", e.target.value)
                  }
                  className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                    errors.universalProductCode
                      ? "border-red-300"
                      : "border-gray-300"
                  }`}
                />
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Unit Cost <span className="text-red-500">*</span>
                </label>
                <input
                  type="number"
                  step="0.01"
                  min="0"
                  value={formData.unitCost ? Number(formData.unitCost) : ""}
                  onChange={e =>
                    handleInputChange(
                      "unitCost",
                      parseFloat(e.target.value) || 0,
                    )
                  }
                  className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                    errors.unitCost ? "border-red-300" : "border-gray-300"
                  }`}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Unit Cost Measurement <span className="text-red-500">*</span>
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
                  className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                    errors.unitCostMeasurementUnit
                      ? "border-red-300"
                      : "border-gray-300"
                  }`}
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
                  Supplier <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  value={formData.supplier || ""}
                  onChange={e => handleInputChange("supplier", e.target.value)}
                  className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                    errors.supplier ? "border-red-300" : "border-gray-300"
                  }`}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Weight (KG) <span className="text-red-500">*</span>
                </label>
                <input
                  type="number"
                  step="0.01"
                  min="0"
                  value={formData.weightKG ? Number(formData.weightKG) : ""}
                  onChange={e =>
                    handleInputChange(
                      "weightKG",
                      parseFloat(e.target.value) || 0,
                    )
                  }
                  className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                    errors.weightKG ? "border-red-300" : "border-gray-300"
                  }`}
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
