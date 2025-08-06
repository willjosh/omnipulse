"use client";
import React, { useEffect, useState } from "react";
import { useRouter, useParams } from "next/navigation";
import {
  useVehicleFormStore,
  useVehicleFormData,
  useVehicleFormMode,
  useVehicleFormValidation,
  useVehicleFormStatus,
  useVehicleFormReferenceData,
} from "@/features/vehicle/stores/vehicleFormStore";
import {
  useVehicles,
  useCreateVehicle,
  useUpdateVehicle,
  useVehicleGroups,
  useTechnicians,
} from "@/features/vehicle/hooks/useVehicles";
import { Vehicle } from "@/features/vehicle/types/vehicleType";
import {
  VehicleTypeEnum,
  FuelTypeEnum,
} from "@/features/vehicle/types/vehicleEnum";
import { getVehicleStatusOptions } from "@/features/vehicle/utils/vehicleEnumHelper";
import PrimaryButton from "@/components/ui/Button/PrimaryButton";
import SecondaryButton from "@/components/ui/Button/SecondaryButton";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import { getErrorMessage, getErrorFields } from "@/utils/fieldErrorUtils";

interface VehicleFormProps {
  mode: "create" | "edit";
  onSave?: (vehicleData: any) => Promise<void>;
  onCancel?: () => void;
  vehicleData?: Vehicle;
}

// Helper function to get enum options
const getVehicleTypeOptions = () => [
  { value: VehicleTypeEnum.CAR, label: "Car" },
  { value: VehicleTypeEnum.TRUCK, label: "Truck" },
  { value: VehicleTypeEnum.VAN, label: "Van" },
  { value: VehicleTypeEnum.MOTORCYCLE, label: "Motorcycle" },
  { value: VehicleTypeEnum.BUS, label: "Bus" },
  { value: VehicleTypeEnum.HEAVY_VEHICLE, label: "Heavy Vehicle" },
  { value: VehicleTypeEnum.TRAILER, label: "Trailer" },
  { value: VehicleTypeEnum.OTHER, label: "Other" },
];

const getFuelTypeOptions = () => [
  { value: FuelTypeEnum.PETROL, label: "Petrol" },
  { value: FuelTypeEnum.DIESEL, label: "Diesel" },
  { value: FuelTypeEnum.ELECTRIC, label: "Electric" },
  { value: FuelTypeEnum.HYBRID, label: "Hybrid" },
  { value: FuelTypeEnum.GAS, label: "Gas" },
  { value: FuelTypeEnum.LPG, label: "LPG" },
  { value: FuelTypeEnum.CNG, label: "CNG" },
  { value: FuelTypeEnum.BIO_DIESEL, label: "Bio Diesel" },
  { value: FuelTypeEnum.OTHER, label: "Other" },
];

// Note: Vehicle status options are currently hardcoded in vehicleEnumHelper.ts

const VehicleForm: React.FC<VehicleFormProps> = ({
  mode,
  onSave,
  onCancel,
  vehicleData,
}) => {
  const router = useRouter();
  const params = useParams();
  const notify = useNotification();
  const [errors, setErrors] = useState<Record<string, string>>({});

  const formData = useVehicleFormData();
  const formMode = useVehicleFormMode();
  const { showValidation, validationErrors } = useVehicleFormValidation();
  const { isLoading, vehicleId } = useVehicleFormStatus();

  const { mutateAsync: createVehicle, isPending: isCreating } =
    useCreateVehicle();
  const { mutateAsync: updateVehicle, isPending: isUpdating } =
    useUpdateVehicle();

  const { vehicleGroups: apiVehicleGroups, isPending: isLoadingGroups } =
    useVehicleGroups();
  const { technicians: apiTechnicians, isPending: isLoadingTechnicians } =
    useTechnicians();

  const {
    updateFormData,
    setShowValidation,
    setValidationErrors,
    setLoading,
    resetForm,
    initializeForEdit,
    initializeForCreate,
    isFormComplete,
    toCreateCommand,
    toUpdateCommand,
  } = useVehicleFormStore();

  useEffect(() => {
    setErrors({});
    if (mode === "create") {
      initializeForCreate();
    } else if (mode === "edit") {
      if (vehicleData) {
        initializeForEdit(vehicleData.id, vehicleData);
      } else if (params.id) {
        console.warn("Edit mode requires vehicleData prop or API call");
        router.push("/vehicles");
      }
    }
  }, [
    mode,
    vehicleData,
    params.id,
    initializeForEdit,
    initializeForCreate,
    router,
  ]);

  const handleInputChange = (field: string, value: any) => {
    updateFormData({ [field]: value });
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: "" }));
    }
  };

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    // Required text fields
    if (!formData.vehicleName.trim()) {
      newErrors.vehicleName = "Vehicle name is required";
    }
    if (!formData.make.trim()) {
      newErrors.make = "Make is required";
    }
    if (!formData.model.trim()) {
      newErrors.model = "Model is required";
    }
    if (!formData.trim.trim()) {
      newErrors.trim = "Trim is required";
    }
    if (!formData.vin.trim()) {
      newErrors.vin = "VIN is required";
    }
    if (!formData.licensePlate.trim()) {
      newErrors.licensePlate = "License plate is required";
    }
    if (!formData.licensePlateExpirationDate) {
      newErrors.licensePlateExpirationDate =
        "License plate expiration date is required";
    }
    if (!formData.location.trim()) {
      newErrors.location = "Location is required";
    }
    if (!formData.purchaseDate) {
      newErrors.purchaseDate = "Purchase date is required";
    }

    // Required number fields
    if (!formData.year || formData.year <= 0) {
      newErrors.year = "Valid year is required";
    }
    if (formData.mileage === null || formData.mileage < 0) {
      newErrors.mileage = "Valid mileage is required";
    }
    if (formData.engineHours === null || formData.engineHours < 0) {
      newErrors.engineHours = "Valid engine hours is required";
    }
    if (formData.fuelCapacity === null || formData.fuelCapacity <= 0) {
      newErrors.fuelCapacity = "Valid fuel capacity is required";
    }
    if (formData.purchasePrice === null || formData.purchasePrice < 0) {
      newErrors.purchasePrice = "Valid purchase price is required";
    }

    // Required dropdown fields
    if (formData.type === "") {
      newErrors.type = "Vehicle type is required";
    }
    if (formData.fuelType === "") {
      newErrors.fuelType = "Fuel type is required";
    }
    if (formData.status === "") {
      newErrors.status = "Status is required";
    }
    if (!formData.vehicleGroupID || formData.vehicleGroupID === 0) {
      newErrors.vehicleGroupID = "Vehicle group is required";
    }
    if (!formData.assignedTechnicianID) {
      newErrors.assignedTechnicianID = "Assigned technician is required";
    }

    return newErrors;
  };

  const handleSaveVehicle = async () => {
    try {
      setLoading(true);

      // Validate form before attempting to save
      const validationErrors = validateForm();
      if (Object.keys(validationErrors).length > 0) {
        setErrors(validationErrors);

        // Create a summary of missing fields
        const missingFields = Object.values(validationErrors);
        const errorMessage = `Please fill in the following required fields:\n• ${missingFields.join("\n• ")}`;

        notify(errorMessage, "error");
        return;
      }

      // Clear any existing errors
      setErrors({});

      if (onSave) {
        const commandData =
          formMode === "create" ? toCreateCommand() : toUpdateCommand();
        await onSave(commandData);
      } else {
        if (formMode === "create") {
          const commandData = toCreateCommand();
          await createVehicle(commandData);
          notify("Vehicle created successfully!", "success");
        } else {
          const commandData = toUpdateCommand();
          await updateVehicle(commandData);
          notify("Vehicle updated successfully!", "success");
        }
      }

      resetForm();
      router.push("/vehicles");
    } catch (error: any) {
      const errorMessage = getErrorMessage(
        error,
        "Failed to save vehicle. Please check your input and try again.",
      );

      const fieldErrors = getErrorFields(error, [
        "vehicleName",
        "year",
        "make",
        "model",
        "trim",
        "vin",
        "type",
        "fuelType",
        "status",
        "licensePlate",
        "licensePlateExpirationDate",
        "vehicleGroupID",
        "location",
        "mileage",
        "engineHours",
        "fuelCapacity",
        "purchaseDate",
        "purchasePrice",
      ]);

      setErrors(fieldErrors);
      notify(errorMessage, "error");
    } finally {
      setLoading(false);
    }
  };

  const isSaving = isLoading || isCreating || isUpdating;

  const handleCancel = () => {
    resetForm();
    setErrors({});

    if (onCancel) {
      onCancel();
    } else {
      router.push("/vehicles");
    }
  };

  const originalVehicleName = vehicleData?.name || vehicleId;
  const pageTitle =
    formMode === "create"
      ? "Add New Vehicle"
      : `Edit Vehicle${originalVehicleName ? ` - ${originalVehicleName}` : ""}`;

  const saveButtonText =
    formMode === "create" ? "Save Vehicle" : "Update Vehicle";

  return (
    <div className="max-w-4xl mx-auto my-16">
      {/* Page Title */}
      <div className="mb-6">
        <h1 className="text-2xl font-semibold text-gray-900">{pageTitle}</h1>
      </div>

      {/* Form */}
      <div className="bg-white rounded-2xl p-6 shadow-sm border border-gray-200">
        <form onSubmit={e => e.preventDefault()} className="space-y-8">
          {/* Basic Information Section */}
          <div className="space-y-6">
            <h2 className="text-lg font-medium text-gray-900 border-b border-gray-200 pb-2">
              Basic Information
            </h2>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {/* Vehicle Name */}
              <div>
                <label
                  htmlFor="vehicleName"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Vehicle Name <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  id="vehicleName"
                  value={formData.vehicleName}
                  onChange={e =>
                    handleInputChange("vehicleName", e.target.value)
                  }
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.vehicleName ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="Enter vehicle name"
                />
              </div>

              {/* Year */}
              <div>
                <label
                  htmlFor="year"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Year <span className="text-red-500">*</span>
                </label>
                <input
                  type="number"
                  id="year"
                  value={formData.year > 0 ? formData.year : ""}
                  onChange={e => {
                    const value = e.target.value;
                    const yearValue = value === "" ? 0 : parseInt(value);
                    handleInputChange("year", yearValue);
                  }}
                  min="1900"
                  max={new Date().getFullYear() + 1}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.year ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="Enter year"
                />
              </div>

              {/* Make */}
              <div>
                <label
                  htmlFor="make"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Make <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  id="make"
                  value={formData.make}
                  onChange={e => handleInputChange("make", e.target.value)}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.make ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="e.g., Toyota, Ford, BMW"
                />
              </div>

              {/* Model */}
              <div>
                <label
                  htmlFor="model"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Model <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  id="model"
                  value={formData.model}
                  onChange={e => handleInputChange("model", e.target.value)}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.model ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="e.g., Camry, F-150, 3 Series"
                />
              </div>

              {/* Trim */}
              <div>
                <label
                  htmlFor="trim"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Trim <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  id="trim"
                  value={formData.trim}
                  onChange={e => handleInputChange("trim", e.target.value)}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.trim ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="e.g., LE, XLT, 320i"
                />
              </div>

              {/* VIN */}
              <div>
                <label
                  htmlFor="vin"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  VIN <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  id="vin"
                  value={formData.vin}
                  onChange={e =>
                    handleInputChange("vin", e.target.value.toUpperCase())
                  }
                  maxLength={17}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.vin ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="17-character VIN"
                />
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              {/* Vehicle Type */}
              <div>
                <label
                  htmlFor="type"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Vehicle Type <span className="text-red-500">*</span>
                </label>
                <select
                  id="type"
                  value={formData.type}
                  onChange={e =>
                    handleInputChange("type", parseInt(e.target.value))
                  }
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.type ? "border-red-300" : "border-gray-300"
                  }`}
                >
                  <option value="">Select vehicle type</option>
                  {getVehicleTypeOptions().map(option => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </select>
              </div>

              {/* Fuel Type */}
              <div>
                <label
                  htmlFor="fuelType"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Fuel Type <span className="text-red-500">*</span>
                </label>
                <select
                  id="fuelType"
                  value={formData.fuelType}
                  onChange={e =>
                    handleInputChange("fuelType", parseInt(e.target.value))
                  }
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.fuelType ? "border-red-300" : "border-gray-300"
                  }`}
                >
                  <option value="">Select fuel type</option>
                  {getFuelTypeOptions().map(option => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </select>
              </div>

              {/* Status */}
              <div>
                <label
                  htmlFor="status"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Status <span className="text-red-500">*</span>
                </label>
                <select
                  id="status"
                  value={formData.status}
                  onChange={e =>
                    handleInputChange("status", parseInt(e.target.value))
                  }
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.status ? "border-red-300" : "border-gray-300"
                  }`}
                >
                  <option value="">Select status</option>
                  {getVehicleStatusOptions().map(option => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </select>
              </div>
            </div>
          </div>

          {/* Registration & Assignment Section */}
          <div className="space-y-6">
            <h2 className="text-lg font-medium text-gray-900 border-b border-gray-200 pb-2">
              Registration & Assignment
            </h2>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {/* License Plate */}
              <div>
                <label
                  htmlFor="licensePlate"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  License Plate <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  id="licensePlate"
                  value={formData.licensePlate}
                  onChange={e =>
                    handleInputChange(
                      "licensePlate",
                      e.target.value.toUpperCase(),
                    )
                  }
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.licensePlate ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="Enter license plate"
                />
              </div>

              {/* License Plate Expiration Date */}
              <div>
                <label
                  htmlFor="licensePlateExpirationDate"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  License Plate Expiration Date{" "}
                  <span className="text-red-500">*</span>
                </label>
                <input
                  type="date"
                  id="licensePlateExpirationDate"
                  value={
                    formData.licensePlateExpirationDate
                      ? formData.licensePlateExpirationDate.split("T")[0]
                      : ""
                  }
                  onChange={e =>
                    handleInputChange(
                      "licensePlateExpirationDate",
                      e.target.value,
                    )
                  }
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.licensePlateExpirationDate
                      ? "border-red-300"
                      : "border-gray-300"
                  }`}
                />
              </div>

              {/* Vehicle Group */}
              <div>
                <label
                  htmlFor="vehicleGroupID"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Vehicle Group <span className="text-red-500">*</span>
                </label>
                <select
                  id="vehicleGroupID"
                  value={formData.vehicleGroupID}
                  onChange={e => {
                    const groupId = parseInt(e.target.value);
                    const group = (apiVehicleGroups || []).find(
                      g => g.id === groupId,
                    );
                    handleInputChange("vehicleGroupID", groupId);
                    handleInputChange("vehicleGroupName", group?.name || "");
                  }}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.vehicleGroupID ? "border-red-300" : "border-gray-300"
                  }`}
                >
                  <option value={0}>Select vehicle group</option>
                  {(apiVehicleGroups || []).map(group => (
                    <option key={group.id} value={group.id}>
                      {group.name}
                    </option>
                  ))}
                </select>
              </div>

              {/* Assigned Technician */}
              <div>
                <label
                  htmlFor="assignedTechnicianID"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Assigned Technician <span className="text-red-500">*</span>
                </label>
                <select
                  id="assignedTechnicianID"
                  value={formData.assignedTechnicianID || ""}
                  onChange={e => {
                    const techId = e.target.value || null;
                    const tech = (apiTechnicians || []).find(
                      t => t.id === techId,
                    );
                    handleInputChange("assignedTechnicianID", techId);
                    handleInputChange(
                      "assignedTechnicianName",
                      tech ? `${tech.firstName} ${tech.lastName}` : "",
                    );
                  }}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.assignedTechnicianID
                      ? "border-red-300"
                      : "border-gray-300"
                  }`}
                >
                  <option value="">Select technician</option>
                  {(apiTechnicians || []).map(tech => (
                    <option key={tech.id} value={tech.id}>
                      {`${tech.firstName} ${tech.lastName}`}
                    </option>
                  ))}
                </select>
              </div>

              {/* Location */}
              <div className="md:col-span-2">
                <label
                  htmlFor="location"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Location <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  id="location"
                  value={formData.location}
                  onChange={e => handleInputChange("location", e.target.value)}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.location ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="Enter vehicle location"
                />
              </div>
            </div>
          </div>

          {/* Operational Data Section */}
          <div className="space-y-6">
            <h2 className="text-lg font-medium text-gray-900 border-b border-gray-200 pb-2">
              Operational Data
            </h2>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              {/* Mileage */}
              <div>
                <label
                  htmlFor="mileage"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Mileage <span className="text-red-500">*</span>
                </label>
                <input
                  type="number"
                  id="mileage"
                  value={formData.mileage ?? ""}
                  onChange={e => {
                    const value = e.target.value;
                    handleInputChange(
                      "mileage",
                      value === "" ? null : parseInt(value),
                    );
                  }}
                  min="0"
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.mileage ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="Enter mileage"
                />
              </div>

              {/* Engine Hours */}
              <div>
                <label
                  htmlFor="engineHours"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Engine Hours <span className="text-red-500">*</span>
                </label>
                <input
                  type="number"
                  id="engineHours"
                  value={formData.engineHours ?? ""}
                  onChange={e => {
                    const value = e.target.value;
                    handleInputChange(
                      "engineHours",
                      value === "" ? null : parseFloat(value),
                    );
                  }}
                  min="0"
                  step="0.1"
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.engineHours ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="Enter engine hours"
                />
              </div>

              {/* Fuel Capacity */}
              <div>
                <label
                  htmlFor="fuelCapacity"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Fuel Capacity (Liters) <span className="text-red-500">*</span>
                </label>
                <input
                  type="number"
                  id="fuelCapacity"
                  value={formData.fuelCapacity ?? ""}
                  onChange={e => {
                    const value = e.target.value;
                    handleInputChange(
                      "fuelCapacity",
                      value === "" ? null : parseFloat(value),
                    );
                  }}
                  min="0"
                  step="0.1"
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.fuelCapacity ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="Enter fuel capacity"
                />
              </div>
            </div>
          </div>

          {/* Financial Information Section */}
          <div className="space-y-6">
            <h2 className="text-lg font-medium text-gray-900 border-b border-gray-200 pb-2">
              Financial Information
            </h2>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {/* Purchase Date */}
              <div>
                <label
                  htmlFor="purchaseDate"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Purchase Date <span className="text-red-500">*</span>
                </label>
                <input
                  type="date"
                  id="purchaseDate"
                  value={
                    formData.purchaseDate
                      ? formData.purchaseDate.split("T")[0]
                      : ""
                  }
                  onChange={e =>
                    handleInputChange("purchaseDate", e.target.value)
                  }
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.purchaseDate ? "border-red-300" : "border-gray-300"
                  }`}
                />
              </div>

              {/* Purchase Price */}
              <div>
                <label
                  htmlFor="purchasePrice"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Purchase Price ($) <span className="text-red-500">*</span>
                </label>
                <input
                  type="number"
                  id="purchasePrice"
                  value={formData.purchasePrice ?? ""}
                  onChange={e => {
                    const value = e.target.value;
                    handleInputChange(
                      "purchasePrice",
                      value === "" ? null : parseFloat(value),
                    );
                  }}
                  min="0"
                  step="0.01"
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.purchasePrice ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="Enter purchase price"
                />
              </div>
            </div>
          </div>

          {/* Form Actions */}
          <div className="flex justify-between items-center pt-6 border-t border-gray-200">
            <SecondaryButton onClick={handleCancel} disabled={isSaving}>
              Cancel
            </SecondaryButton>

            <PrimaryButton onClick={handleSaveVehicle} disabled={isSaving}>
              {isSaving ? "Saving..." : saveButtonText}
            </PrimaryButton>
          </div>
        </form>
      </div>
    </div>
  );
};

export default VehicleForm;
