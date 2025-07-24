"use client";
import React, { useEffect } from "react";
import { useRouter, useParams } from "next/navigation";
import {
  useVehicleFormStore,
  useVehicleFormData,
  useVehicleFormMode,
  useVehicleFormValidation,
  useVehicleFormStatus,
  useVehicleFormReferenceData,
} from "../../store/VehicleFormStore";
import { useVehicles } from "@/app/_hooks/vehicle/useVehicles";
import { Vehicle } from "@/app/_hooks/vehicle/vehicleType";
import {
  VehicleTypeEnum,
  FuelTypeEnum,
  VehicleStatusEnum,
} from "@/app/_hooks/vehicle/vehicleEnum";
import PrimaryButton from "@/app/_features/shared/button/PrimaryButton";
import SecondaryButton from "@/app/_features/shared/button/SecondaryButton";

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

const getStatusOptions = () => [
  { value: VehicleStatusEnum.ACTIVE, label: "Active" },
  { value: VehicleStatusEnum.MAINTENANCE, label: "Maintenance" },
  { value: VehicleStatusEnum.OUT_OF_SERVICE, label: "Out of Service" },
  { value: VehicleStatusEnum.INACTIVE, label: "Inactive" },
];

const VehicleForm: React.FC<VehicleFormProps> = ({
  mode,
  onSave,
  onCancel,
  vehicleData,
}) => {
  const router = useRouter();
  const params = useParams();

  // Use selector hooks for better performance
  const formData = useVehicleFormData();
  const formMode = useVehicleFormMode();
  const { showValidation, validationErrors, isFormValid } =
    useVehicleFormValidation();
  const { isLoading, isDirty, vehicleId } = useVehicleFormStatus();
  const { vehicleGroups, technicians } = useVehicleFormReferenceData();

  // Add the useVehicles hook to get mutation functions
  const { createVehicleMutation, updateVehicleMutation } = useVehicles();

  // Store actions
  const {
    updateFormData,
    setShowValidation,
    setValidationErrors,
    setLoading,
    setVehicleGroups,
    setTechnicians,
    resetForm,
    initializeForEdit,
    initializeForCreate,
    isFormComplete,
    toCreateCommand,
    toUpdateCommand,
  } = useVehicleFormStore();

  // form initialization based on mode
  useEffect(() => {
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

  useEffect(() => {
    // Mock data
    const mockGroups = [
      { id: 1, name: "Fleet A - Operations" },
      { id: 2, name: "Fleet B - Maintenance" },
      { id: 3, name: "Fleet C - Executive" },
      { id: 4, name: "Fleet D - Emergency" },
    ];

    const mockTechnicians = [
      { id: "tech-1", name: "John Smith" },
      { id: "tech-2", name: "Sarah Johnson" },
      { id: "tech-3", name: "Mike Davis" },
      { id: "tech-4", name: "Emily Brown" },
    ];

    setVehicleGroups(mockGroups);
    setTechnicians(mockTechnicians);
  }, [setVehicleGroups, setTechnicians]);

  const handleInputChange = (field: string, value: any) => {
    updateFormData({ [field]: value });
  };

  const handleSaveVehicle = async () => {
    if (!isFormValid) {
      setShowValidation(true);
      return;
    }

    setShowValidation(false);
    setValidationErrors({});

    try {
      setLoading(true);

      if (onSave) {
        // If there is an onSave callback provided, use it
        const commandData =
          formMode === "create" ? toCreateCommand() : toUpdateCommand();
        await onSave(commandData);
      } else {
        if (formMode === "create") {
          const commandData = toCreateCommand();
          await createVehicleMutation.mutateAsync(commandData);
          console.log("Vehicle created successfully:", commandData);
        } else {
          const commandData = toUpdateCommand();
          await updateVehicleMutation.mutateAsync(commandData);
          console.log("Vehicle updated successfully:", commandData);
        }
      }

      resetForm();
      router.push("/vehicles");
    } catch (error) {
      console.error("Error saving vehicle:", error);
      // Show error message to the user
      setValidationErrors({
        general: "Failed to save vehicle. Please try again.",
      });
    } finally {
      setLoading(false);
    }
  };

  // Update loading state to include mutation loading states
  const isSaving =
    isLoading ||
    createVehicleMutation.isPending ||
    updateVehicleMutation.isPending;

  const handleCancel = () => {
    if (isDirty) {
      const confirmed = window.confirm(
        "You have unsaved changes. Are you sure you want to cancel?",
      );
      if (!confirmed) return;
    }

    resetForm();

    if (onCancel) {
      onCancel();
    } else {
      router.push("/vehicles");
    }
  };

  const getFieldError = (fieldName: string) => {
    return showValidation ? validationErrors[fieldName] : "";
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
        {isDirty && (
          <p className="text-sm text-amber-600 mt-1">
            You have unsaved changes
          </p>
        )}
        {/* Show error message if there's an error */}
        {validationErrors.general && (
          <p className="text-sm text-red-600 mt-1">
            {validationErrors.general}
          </p>
        )}
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
                  Vehicle Name *
                </label>
                <input
                  type="text"
                  id="vehicleName"
                  value={formData.vehicleName}
                  onChange={e =>
                    handleInputChange("vehicleName", e.target.value)
                  }
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    getFieldError("vehicleName")
                      ? "border-red-500"
                      : "border-gray-300"
                  }`}
                  placeholder="Enter vehicle name"
                />
                {getFieldError("vehicleName") && (
                  <p className="mt-1 text-sm text-red-600">
                    {getFieldError("vehicleName")}
                  </p>
                )}
              </div>

              {/* Year */}
              <div>
                <label
                  htmlFor="year"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Year *
                </label>
                <input
                  type="number"
                  id="year"
                  value={formData.year}
                  onChange={e =>
                    handleInputChange("year", parseInt(e.target.value) || 0)
                  }
                  min="1900"
                  max={new Date().getFullYear() + 1}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    getFieldError("year") ? "border-red-500" : "border-gray-300"
                  }`}
                />
                {getFieldError("year") && (
                  <p className="mt-1 text-sm text-red-600">
                    {getFieldError("year")}
                  </p>
                )}
              </div>

              {/* Make */}
              <div>
                <label
                  htmlFor="make"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Make *
                </label>
                <input
                  type="text"
                  id="make"
                  value={formData.make}
                  onChange={e => handleInputChange("make", e.target.value)}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    getFieldError("make") ? "border-red-500" : "border-gray-300"
                  }`}
                  placeholder="e.g., Toyota, Ford, BMW"
                />
                {getFieldError("make") && (
                  <p className="mt-1 text-sm text-red-600">
                    {getFieldError("make")}
                  </p>
                )}
              </div>

              {/* Model */}
              <div>
                <label
                  htmlFor="model"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Model *
                </label>
                <input
                  type="text"
                  id="model"
                  value={formData.model}
                  onChange={e => handleInputChange("model", e.target.value)}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    getFieldError("model")
                      ? "border-red-500"
                      : "border-gray-300"
                  }`}
                  placeholder="e.g., Camry, F-150, 3 Series"
                />
                {getFieldError("model") && (
                  <p className="mt-1 text-sm text-red-600">
                    {getFieldError("model")}
                  </p>
                )}
              </div>

              {/* Trim */}
              <div>
                <label
                  htmlFor="trim"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Trim *
                </label>
                <input
                  type="text"
                  id="trim"
                  value={formData.trim}
                  onChange={e => handleInputChange("trim", e.target.value)}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    getFieldError("trim") ? "border-red-500" : "border-gray-300"
                  }`}
                  placeholder="e.g., LE, XLT, 320i"
                />
                {getFieldError("trim") && (
                  <p className="mt-1 text-sm text-red-600">
                    {getFieldError("trim")}
                  </p>
                )}
              </div>

              {/* VIN */}
              <div>
                <label
                  htmlFor="vin"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  VIN *
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
                    getFieldError("vin") ? "border-red-500" : "border-gray-300"
                  }`}
                  placeholder="17-character VIN"
                />
                {getFieldError("vin") && (
                  <p className="mt-1 text-sm text-red-600">
                    {getFieldError("vin")}
                  </p>
                )}
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              {/* Vehicle Type */}
              <div>
                <label
                  htmlFor="type"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Vehicle Type *
                </label>
                <select
                  id="type"
                  value={formData.type}
                  onChange={e =>
                    handleInputChange("type", parseInt(e.target.value))
                  }
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    getFieldError("type") ? "border-red-500" : "border-gray-300"
                  }`}
                >
                  <option value="">Select vehicle type</option>
                  {getVehicleTypeOptions().map(option => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </select>
                {getFieldError("type") && (
                  <p className="mt-1 text-sm text-red-600">
                    {getFieldError("type")}
                  </p>
                )}
              </div>

              {/* Fuel Type */}
              <div>
                <label
                  htmlFor="fuelType"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Fuel Type *
                </label>
                <select
                  id="fuelType"
                  value={formData.fuelType}
                  onChange={e =>
                    handleInputChange("fuelType", parseInt(e.target.value))
                  }
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    getFieldError("fuelType")
                      ? "border-red-500"
                      : "border-gray-300"
                  }`}
                >
                  <option value="">Select fuel type</option>
                  {getFuelTypeOptions().map(option => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </select>
                {getFieldError("fuelType") && (
                  <p className="mt-1 text-sm text-red-600">
                    {getFieldError("fuelType")}
                  </p>
                )}
              </div>

              {/* Status */}
              <div>
                <label
                  htmlFor="status"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Status *
                </label>
                <select
                  id="status"
                  value={formData.status}
                  onChange={e =>
                    handleInputChange("status", parseInt(e.target.value))
                  }
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    getFieldError("status")
                      ? "border-red-500"
                      : "border-gray-300"
                  }`}
                >
                  {getStatusOptions().map(option => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </select>
                {getFieldError("status") && (
                  <p className="mt-1 text-sm text-red-600">
                    {getFieldError("status")}
                  </p>
                )}
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
                  License Plate *
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
                    getFieldError("licensePlate")
                      ? "border-red-500"
                      : "border-gray-300"
                  }`}
                  placeholder="Enter license plate"
                />
                {getFieldError("licensePlate") && (
                  <p className="mt-1 text-sm text-red-600">
                    {getFieldError("licensePlate")}
                  </p>
                )}
              </div>

              {/* License Plate Expiration Date */}
              <div>
                <label
                  htmlFor="licensePlateExpirationDate"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  License Plate Expiration Date *
                </label>
                <input
                  type="date"
                  id="licensePlateExpirationDate"
                  value={formData.licensePlateExpirationDate}
                  onChange={e =>
                    handleInputChange(
                      "licensePlateExpirationDate",
                      e.target.value,
                    )
                  }
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    getFieldError("licensePlateExpirationDate")
                      ? "border-red-500"
                      : "border-gray-300"
                  }`}
                />
                {getFieldError("licensePlateExpirationDate") && (
                  <p className="mt-1 text-sm text-red-600">
                    {getFieldError("licensePlateExpirationDate")}
                  </p>
                )}
              </div>

              {/* Vehicle Group */}
              <div>
                <label
                  htmlFor="vehicleGroupID"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Vehicle Group *
                </label>
                <select
                  id="vehicleGroupID"
                  value={formData.vehicleGroupID}
                  onChange={e => {
                    const groupId = parseInt(e.target.value);
                    const group = vehicleGroups.find(g => g.id === groupId);
                    handleInputChange("vehicleGroupID", groupId);
                    handleInputChange("vehicleGroupName", group?.name || "");
                  }}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    getFieldError("vehicleGroupID")
                      ? "border-red-500"
                      : "border-gray-300"
                  }`}
                >
                  <option value={0}>Select vehicle group</option>
                  {vehicleGroups.map(group => (
                    <option key={group.id} value={group.id}>
                      {group.name}
                    </option>
                  ))}
                </select>
                {getFieldError("vehicleGroupID") && (
                  <p className="mt-1 text-sm text-red-600">
                    {getFieldError("vehicleGroupID")}
                  </p>
                )}
              </div>

              {/* Assigned Technician */}
              <div>
                <label
                  htmlFor="assignedTechnicianID"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Assigned Technician
                </label>
                <select
                  id="assignedTechnicianID"
                  value={formData.assignedTechnicianID || ""}
                  onChange={e => {
                    const techId = e.target.value || null;
                    const tech = technicians.find(t => t.id === techId);
                    handleInputChange("assignedTechnicianID", techId);
                    handleInputChange(
                      "assignedTechnicianName",
                      tech?.name || "",
                    );
                  }}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                >
                  <option value="">No technician assigned</option>
                  {technicians.map(tech => (
                    <option key={tech.id} value={tech.id}>
                      {tech.name}
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
                  Location *
                </label>
                <input
                  type="text"
                  id="location"
                  value={formData.location}
                  onChange={e => handleInputChange("location", e.target.value)}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    getFieldError("location")
                      ? "border-red-500"
                      : "border-gray-300"
                  }`}
                  placeholder="Enter vehicle location"
                />
                {getFieldError("location") && (
                  <p className="mt-1 text-sm text-red-600">
                    {getFieldError("location")}
                  </p>
                )}
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
                  Mileage *
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
                    getFieldError("mileage")
                      ? "border-red-500"
                      : "border-gray-300"
                  }`}
                  placeholder="Enter mileage"
                />
                {getFieldError("mileage") && (
                  <p className="mt-1 text-sm text-red-600">
                    {getFieldError("mileage")}
                  </p>
                )}
              </div>

              {/* Engine Hours */}
              <div>
                <label
                  htmlFor="engineHours"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Engine Hours *
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
                    getFieldError("engineHours")
                      ? "border-red-500"
                      : "border-gray-300"
                  }`}
                  placeholder="Enter engine hours"
                />
                {getFieldError("engineHours") && (
                  <p className="mt-1 text-sm text-red-600">
                    {getFieldError("engineHours")}
                  </p>
                )}
              </div>

              {/* Fuel Capacity */}
              <div>
                <label
                  htmlFor="fuelCapacity"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Fuel Capacity (Liters) *
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
                    getFieldError("fuelCapacity")
                      ? "border-red-500"
                      : "border-gray-300"
                  }`}
                  placeholder="Enter fuel capacity"
                />
                {getFieldError("fuelCapacity") && (
                  <p className="mt-1 text-sm text-red-600">
                    {getFieldError("fuelCapacity")}
                  </p>
                )}
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
                  Purchase Date *
                </label>
                <input
                  type="date"
                  id="purchaseDate"
                  value={formData.purchaseDate}
                  onChange={e =>
                    handleInputChange("purchaseDate", e.target.value)
                  }
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    getFieldError("purchaseDate")
                      ? "border-red-500"
                      : "border-gray-300"
                  }`}
                />
                {getFieldError("purchaseDate") && (
                  <p className="mt-1 text-sm text-red-600">
                    {getFieldError("purchaseDate")}
                  </p>
                )}
              </div>

              {/* Purchase Price */}
              <div>
                <label
                  htmlFor="purchasePrice"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Purchase Price ($) *
                </label>
                <input
                  type="number"
                  id="purchasePrice"
                  value={formData.purchasePrice}
                  onChange={e =>
                    handleInputChange(
                      "purchasePrice",
                      parseFloat(e.target.value) || 0,
                    )
                  }
                  min="0"
                  step="0.01"
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    getFieldError("purchasePrice")
                      ? "border-red-500"
                      : "border-gray-300"
                  }`}
                  placeholder="0.00"
                />
                {getFieldError("purchasePrice") && (
                  <p className="mt-1 text-sm text-red-600">
                    {getFieldError("purchasePrice")}
                  </p>
                )}
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
