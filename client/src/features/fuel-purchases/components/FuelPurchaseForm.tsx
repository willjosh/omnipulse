"use client";
import React, { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { FormField } from "@/components/ui/Form";
import { PrimaryButton, SecondaryButton } from "@/components/ui/Button";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import {
  useCreateFuelPurchase,
  useUpdateFuelPurchase,
  useFuelPurchase,
} from "../hooks/useFuelPurchases";
import {
  useVehicles,
  useTechnicians,
} from "@/features/vehicle/hooks/useVehicles";
import {
  CreateFuelPurchaseCommand,
  UpdateFuelPurchaseCommand,
} from "../types/fuelPurchaseType";
import { getErrorMessage, getErrorFields } from "@/utils/fieldErrorUtils";

interface FuelPurchaseFormProps {
  mode: "create" | "edit";
  fuelPurchaseId?: string;
}

const FuelPurchaseForm: React.FC<FuelPurchaseFormProps> = ({
  mode,
  fuelPurchaseId,
}) => {
  const router = useRouter();
  const notify = useNotification();
  const createFuelPurchaseMutation = useCreateFuelPurchase();
  const updateFuelPurchaseMutation = useUpdateFuelPurchase();

  const {
    fuelPurchase: existingFuelPurchase,
    isPending: isLoadingFuelPurchase,
  } = useFuelPurchase(fuelPurchaseId || "");

  // Get reference data for dropdowns
  const { vehicles, isPending: isLoadingVehicles } = useVehicles();
  const { technicians, isPending: isLoadingTechnicians } = useTechnicians();

  const [formData, setFormData] = useState<CreateFuelPurchaseCommand>({
    vehicleId: 0,
    purchasedByUserId: "",
    purchaseDate: new Date().toISOString().split("T")[0],
    odometerReading: 0,
    volume: 0,
    pricePerUnit: 0,
    totalCost: 0,
    fuelStation: "",
    receiptNumber: "",
    notes: "",
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  // Calculate total cost when volume or price changes
  useEffect(() => {
    const calculatedTotal = formData.volume * formData.pricePerUnit;
    setFormData(prev => ({ ...prev, totalCost: calculatedTotal }));
  }, [formData.volume, formData.pricePerUnit]);

  useEffect(() => {
    setErrors({});
    if (mode === "edit" && existingFuelPurchase) {
      setFormData({
        vehicleId: existingFuelPurchase.vehicleId,
        purchasedByUserId: existingFuelPurchase.purchasedByUserId,
        purchaseDate: existingFuelPurchase.purchaseDate.split("T")[0],
        odometerReading: existingFuelPurchase.odometerReading,
        volume: existingFuelPurchase.volume,
        pricePerUnit: existingFuelPurchase.pricePerUnit,
        totalCost: existingFuelPurchase.totalCost,
        fuelStation: existingFuelPurchase.fuelStation,
        receiptNumber: existingFuelPurchase.receiptNumber,
        notes: existingFuelPurchase.notes || "",
      });
    }
  }, [mode, existingFuelPurchase]);

  const handleInputChange = (
    field: keyof CreateFuelPurchaseCommand,
    value: string | number,
  ) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: "" }));
    }
  };

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    if (!formData.vehicleId || formData.vehicleId === 0) {
      newErrors.vehicleId = "Vehicle is required";
    }
    if (!formData.purchasedByUserId.trim()) {
      newErrors.purchasedByUserId = "Purchased by user is required";
    }
    if (!formData.purchaseDate) {
      newErrors.purchaseDate = "Purchase date is required";
    }
    if (!formData.odometerReading || formData.odometerReading <= 0) {
      newErrors.odometerReading = "Valid odometer reading is required";
    }
    if (!formData.volume || formData.volume <= 0) {
      newErrors.volume = "Volume must be greater than 0";
    }
    if (!formData.pricePerUnit || formData.pricePerUnit <= 0) {
      newErrors.pricePerUnit = "Price per unit must be greater than 0";
    }
    if (!formData.fuelStation.trim()) {
      newErrors.fuelStation = "Fuel station is required";
    }
    if (!formData.receiptNumber.trim()) {
      newErrors.receiptNumber = "Receipt number is required";
    }

    // Validate date is not in the future
    const today = new Date();
    const purchaseDate = new Date(formData.purchaseDate);
    if (purchaseDate > today) {
      newErrors.purchaseDate = "Purchase date cannot be in the future";
    }

    return newErrors;
  };

  const handleSubmit = async () => {
    try {
      const validationErrors = validateForm();
      if (Object.keys(validationErrors).length > 0) {
        setErrors(validationErrors);
        const errorMessages = Object.values(validationErrors);
        notify(
          `Please fix the following errors:\n• ${errorMessages.join("\n• ")}`,
          "error",
        );
        return;
      }

      setErrors({});

      if (mode === "create") {
        await createFuelPurchaseMutation.mutateAsync(formData);
        notify("Fuel purchase created successfully!", "success");
      } else if (mode === "edit" && fuelPurchaseId) {
        const updateCommand: UpdateFuelPurchaseCommand = {
          fuelPurchaseId: parseInt(fuelPurchaseId),
          ...formData,
        };
        await updateFuelPurchaseMutation.mutateAsync(updateCommand);
        notify("Fuel purchase updated successfully!", "success");
      }

      router.push("/fuel-purchases");
    } catch (error: any) {
      const errorMessage = getErrorMessage(
        error,
        "Failed to save fuel purchase. Please check your input and try again.",
      );

      const fieldErrors = getErrorFields(error, [
        "vehicleId",
        "purchasedByUserId",
        "purchaseDate",
        "odometerReading",
        "volume",
        "pricePerUnit",
        "fuelStation",
        "receiptNumber",
        "notes",
      ]);

      setErrors(fieldErrors);
      notify(errorMessage, "error");
    }
  };

  const handleCancel = () => {
    router.push("/fuel-purchases");
  };

  const isSaving =
    createFuelPurchaseMutation.isPending ||
    updateFuelPurchaseMutation.isPending;
  const isLoading =
    isLoadingFuelPurchase || isLoadingVehicles || isLoadingTechnicians;

  if (isLoading) {
    return (
      <div className="max-w-2xl mx-auto py-8 px-4">
        <div className="bg-white rounded-lg shadow-lg p-6">
          <div className="animate-pulse">
            <div className="h-4 bg-gray-200 rounded w-1/4 mb-6"></div>
            <div className="space-y-4">
              <div className="h-4 bg-gray-200 rounded"></div>
              <div className="h-4 bg-gray-200 rounded w-3/4"></div>
              <div className="h-4 bg-gray-200 rounded w-1/2"></div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-2xl mx-auto py-8 px-4">
      <div className="bg-white rounded-lg shadow-lg p-6">
        <div className="mb-6">
          <h1 className="text-2xl font-bold text-gray-900">
            {mode === "create" ? "Add New Fuel Purchase" : "Edit Fuel Purchase"}
          </h1>
          <p className="text-gray-600 mt-1">
            {mode === "create"
              ? "Record a new fuel purchase"
              : "Update fuel purchase information"}
          </p>
        </div>

        <form onSubmit={e => e.preventDefault()} className="space-y-6">
          {/* Vehicle Selection */}
          <FormField
            label="Vehicle"
            required
            error={errors.vehicleId}
            htmlFor="vehicleId"
          >
            <select
              id="vehicleId"
              value={formData.vehicleId}
              onChange={e =>
                handleInputChange("vehicleId", parseInt(e.target.value))
              }
              className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                errors.vehicleId ? "border-red-300" : "border-gray-300"
              }`}
            >
              <option value={0}>Select a vehicle</option>
              {vehicles.map(vehicle => (
                <option key={vehicle.id} value={vehicle.id}>
                  {vehicle.name} ({vehicle.licensePlate})
                </option>
              ))}
            </select>
          </FormField>

          {/* Purchased By */}
          <FormField
            label="Purchased By"
            required
            error={errors.purchasedByUserId}
            htmlFor="purchasedByUserId"
          >
            <select
              id="purchasedByUserId"
              value={formData.purchasedByUserId}
              onChange={e =>
                handleInputChange("purchasedByUserId", e.target.value)
              }
              className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                errors.purchasedByUserId ? "border-red-300" : "border-gray-300"
              }`}
            >
              <option value="">Select technician</option>
              {technicians.map(technician => (
                <option key={technician.id} value={technician.id}>
                  {technician.firstName} {technician.lastName}
                </option>
              ))}
            </select>
          </FormField>

          {/* Purchase Date */}
          <FormField
            label="Purchase Date"
            required
            error={errors.purchaseDate}
            htmlFor="purchaseDate"
          >
            <input
              type="date"
              id="purchaseDate"
              value={formData.purchaseDate}
              onChange={e => handleInputChange("purchaseDate", e.target.value)}
              className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                errors.purchaseDate ? "border-red-300" : "border-gray-300"
              }`}
            />
          </FormField>

          {/* Odometer Reading */}
          <FormField
            label="Odometer Reading (km)"
            required
            error={errors.odometerReading}
            htmlFor="odometerReading"
          >
            <input
              type="number"
              id="odometerReading"
              value={formData.odometerReading || ""}
              onChange={e =>
                handleInputChange(
                  "odometerReading",
                  parseFloat(e.target.value) || 0,
                )
              }
              min="0"
              step="0.1"
              className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                errors.odometerReading ? "border-red-300" : "border-gray-300"
              }`}
              placeholder="Enter odometer reading"
            />
          </FormField>

          {/* Volume and Price Per Unit - Side by side */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <FormField
              label="Volume (L)"
              required
              error={errors.volume}
              htmlFor="volume"
            >
              <input
                type="number"
                id="volume"
                value={formData.volume || ""}
                onChange={e =>
                  handleInputChange("volume", parseFloat(e.target.value) || 0)
                }
                min="0"
                step="0.01"
                className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                  errors.volume ? "border-red-300" : "border-gray-300"
                }`}
                placeholder="Enter volume in litres"
              />
            </FormField>

            <FormField
              label="Price Per Unit ($)"
              required
              error={errors.pricePerUnit}
              htmlFor="pricePerUnit"
            >
              <input
                type="number"
                id="pricePerUnit"
                value={formData.pricePerUnit || ""}
                onChange={e =>
                  handleInputChange(
                    "pricePerUnit",
                    parseFloat(e.target.value) || 0,
                  )
                }
                min="0"
                step="0.01"
                className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                  errors.pricePerUnit ? "border-red-300" : "border-gray-300"
                }`}
                placeholder="Enter price per unit"
              />
            </FormField>
          </div>

          {/* Total Cost - Display only */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Total Cost
            </label>
            <div className="w-full px-3 py-2 bg-gray-50 border border-gray-300 rounded-lg text-gray-700 font-medium">
              ${formData.totalCost.toFixed(2)}
            </div>
          </div>

          {/* Fuel Station */}
          <FormField
            label="Fuel Station"
            required
            error={errors.fuelStation}
            htmlFor="fuelStation"
          >
            <input
              type="text"
              id="fuelStation"
              value={formData.fuelStation}
              onChange={e => handleInputChange("fuelStation", e.target.value)}
              className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                errors.fuelStation ? "border-red-300" : "border-gray-300"
              }`}
              placeholder="Enter fuel station name"
            />
          </FormField>

          {/* Receipt Number */}
          <FormField
            label="Receipt Number"
            required
            error={errors.receiptNumber}
            htmlFor="receiptNumber"
          >
            <input
              type="text"
              id="receiptNumber"
              value={formData.receiptNumber}
              onChange={e => handleInputChange("receiptNumber", e.target.value)}
              className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                errors.receiptNumber ? "border-red-300" : "border-gray-300"
              }`}
              placeholder="Enter receipt number"
            />
          </FormField>

          {/* Notes */}
          <FormField label="Notes" error={errors.notes} htmlFor="notes">
            <textarea
              id="notes"
              value={formData.notes || ""}
              onChange={e => handleInputChange("notes", e.target.value)}
              rows={3}
              className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                errors.notes ? "border-red-300" : "border-gray-300"
              }`}
              placeholder="Enter any additional notes (optional)"
            />
          </FormField>

          {/* Form Actions */}
          <div className="flex justify-between items-center pt-6 border-t border-gray-200">
            <SecondaryButton onClick={handleCancel} disabled={isSaving}>
              Cancel
            </SecondaryButton>

            <PrimaryButton onClick={handleSubmit} disabled={isSaving}>
              {isSaving
                ? "Saving..."
                : mode === "create"
                  ? "Create Fuel Purchase"
                  : "Update Fuel Purchase"}
            </PrimaryButton>
          </div>
        </form>
      </div>
    </div>
  );
};

export default FuelPurchaseForm;
