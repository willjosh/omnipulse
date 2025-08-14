"use client";
import React, { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { FormField } from "@/components/ui/Form";
import { PrimaryButton, SecondaryButton } from "@/components/ui/Button";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import {
  useVehicles,
  useCreateVehicle,
  useUpdateVehicle,
  useVehicle,
} from "@/features/vehicle/hooks/useVehicles";
import {
  CreateVehicleCommand,
  UpdateVehicleCommand,
  Vehicle,
} from "@/features/vehicle/types/vehicleType";
import {
  VehicleTypeEnum,
  FuelTypeEnum,
  VehicleStatusEnum,
} from "@/features/vehicle/types/vehicleEnum";
import { useVehicleGroups } from "@/features/vehicle-group/hooks/useVehicleGroups";
import { useTechnicians } from "@/features/technician/hooks/useTechnicians";
import { getErrorMessage, getErrorFields } from "@/utils/fieldErrorUtils";

interface VehicleFormProps {
  mode: "create" | "edit";
  vehicleData?: Vehicle;
}

interface VehicleFormData {
  // Basic vehicle information
  name: string;
  year: number | null;
  make: string;
  model: string;
  vin: string;
  vehicleType: VehicleTypeEnum | "";
  licensePlate: string;
  licensePlateExpirationDate: string;
  fuelType: FuelTypeEnum | "";
  trim: string;
  status: VehicleStatusEnum | "";

  // Vehicle group and assignment
  vehicleGroupID: number;
  assignedTechnicianID: string | null;

  // Operational data
  mileage: number | null;
  fuelCapacity: number | null;
  location: string;

  // Financial data
  purchaseDate: string;
  purchasePrice: number | null;
}

const getVehicleTypeOptions = () => [
  { value: VehicleTypeEnum.TRUCK, label: "Truck" },
  { value: VehicleTypeEnum.VAN, label: "Van" },
  { value: VehicleTypeEnum.CAR, label: "Car" },
  { value: VehicleTypeEnum.MOTORCYCLE, label: "Motorcycle" },
  { value: VehicleTypeEnum.BUS, label: "Bus" },
  { value: VehicleTypeEnum.HEAVY_VEHICLE, label: "Heavy Vehicle" },
  { value: VehicleTypeEnum.TRAILER, label: "Trailer" },
  { value: VehicleTypeEnum.OTHER, label: "Other" },
];

const getFuelTypeOptions = () => [
  { value: FuelTypeEnum.HYDROGEN, label: "Hydrogen" },
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

const getVehicleStatusOptions = () => [
  { value: VehicleStatusEnum.ACTIVE, label: "Active" },
  { value: VehicleStatusEnum.MAINTENANCE, label: "Maintenance" },
  { value: VehicleStatusEnum.OUT_OF_SERVICE, label: "Out of Service" },
  { value: VehicleStatusEnum.INACTIVE, label: "Inactive" },
];

const VehicleForm: React.FC<VehicleFormProps> = ({ mode, vehicleData }) => {
  const router = useRouter();
  const notify = useNotification();
  const createVehicleMutation = useCreateVehicle();
  const updateVehicleMutation = useUpdateVehicle();

  // Get reference data
  const { vehicleGroups, isPending: isLoadingVehicleGroups } =
    useVehicleGroups();
  const { technicians, isPending: isLoadingTechnicians } = useTechnicians();

  const [formData, setFormData] = useState<VehicleFormData>({
    name: "",
    year: null,
    make: "",
    model: "",
    vin: "",
    vehicleType: "",
    licensePlate: "",
    licensePlateExpirationDate: "",
    fuelType: "",
    trim: "",
    status: "",
    vehicleGroupID: 0,
    assignedTechnicianID: null,
    mileage: null,
    fuelCapacity: null,
    location: "",
    purchaseDate: "",
    purchasePrice: null,
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  // Initialize form data for edit mode
  useEffect(() => {
    setErrors({});
    if (mode === "edit" && vehicleData) {
      setFormData({
        name: vehicleData.name,
        year: vehicleData.year,
        make: vehicleData.make,
        model: vehicleData.model,
        vin: vehicleData.vin,
        vehicleType: vehicleData.vehicleType,
        licensePlate: vehicleData.licensePlate,
        licensePlateExpirationDate: vehicleData.licensePlateExpirationDate,
        fuelType: vehicleData.fuelType,
        trim: vehicleData.trim,
        status: vehicleData.status,
        vehicleGroupID: vehicleData.vehicleGroupID,
        assignedTechnicianID: vehicleData.assignedTechnicianID ?? null,
        mileage: vehicleData.mileage,
        fuelCapacity: vehicleData.fuelCapacity,
        location: vehicleData.location,
        purchaseDate: vehicleData.purchaseDate.split("T")[0],
        purchasePrice: vehicleData.purchasePrice,
      });
    }
  }, [mode, vehicleData]);

  const handleInputChange = (
    field: keyof VehicleFormData,
    value: string | number | boolean | null,
  ) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    if (errors[field]) {
      setErrors(prev => {
        const newErrors = { ...prev };
        delete newErrors[field];
        return newErrors;
      });
    }
  };

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    if (!formData.name.trim()) {
      newErrors.name = "Vehicle name is required";
    }

    if (!formData.vin.trim()) {
      newErrors.vin = "VIN is required";
    } else if (formData.vin.length !== 17) {
      newErrors.vin = "VIN must be 17 characters long";
    }

    if (!formData.licensePlate.trim()) {
      newErrors.licensePlate = "License plate is required";
    }

    if (!formData.licensePlateExpirationDate) {
      newErrors.licensePlateExpirationDate =
        "License plate expiration date is required";
    }

    if (
      !formData.year ||
      formData.year < 1900 ||
      formData.year > new Date().getFullYear() + 1
    ) {
      newErrors.year = "Valid year is required";
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

    if (!formData.vehicleType) {
      newErrors.vehicleType = "Vehicle type is required";
    }

    if (!formData.fuelType) {
      newErrors.fuelType = "Fuel type is required";
    }

    if (!formData.status) {
      newErrors.status = "Status is required";
    }

    if (formData.vehicleGroupID === 0) {
      newErrors.vehicleGroupID = "Vehicle group is required";
    }

    if (formData.mileage !== null && formData.mileage < 0) {
      newErrors.mileage = "Mileage cannot be negative";
    }

    if (formData.fuelCapacity !== null && formData.fuelCapacity <= 0) {
      newErrors.fuelCapacity = "Fuel capacity must be greater than 0";
    }

    if (!formData.location.trim()) {
      newErrors.location = "Location is required";
    }

    if (!formData.purchaseDate) {
      newErrors.purchaseDate = "Purchase date is required";
    }

    if (formData.purchasePrice !== null && formData.purchasePrice < 0) {
      newErrors.purchasePrice = "Purchase price cannot be negative";
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
        const createCommand: CreateVehicleCommand = {
          name: formData.name,
          make: formData.make,
          model: formData.model,
          year: formData.year ?? 0,
          vin: formData.vin,
          licensePlate: formData.licensePlate,
          licensePlateExpirationDate: formData.licensePlateExpirationDate,
          vehicleType: formData.vehicleType as VehicleTypeEnum,
          vehicleGroupID: formData.vehicleGroupID,
          trim: formData.trim,
          mileage: formData.mileage ?? 0,
          fuelCapacity: formData.fuelCapacity ?? 0,
          fuelType: formData.fuelType as FuelTypeEnum,
          purchaseDate: formData.purchaseDate,
          purchasePrice: formData.purchasePrice ?? 0,
          status: formData.status as VehicleStatusEnum,
          location: formData.location,
          assignedTechnicianID: formData.assignedTechnicianID,
        };

        await createVehicleMutation.mutateAsync(createCommand);
        notify("Vehicle created successfully!", "success");
      } else {
        const updateCommand: UpdateVehicleCommand = {
          vehicleID: vehicleData!.id,
          name: formData.name,
          make: formData.make,
          model: formData.model,
          year: formData.year ?? 0,
          vin: formData.vin,
          licensePlate: formData.licensePlate,
          licensePlateExpirationDate: formData.licensePlateExpirationDate,
          vehicleType: formData.vehicleType as VehicleTypeEnum,
          vehicleGroupID: formData.vehicleGroupID,
          trim: formData.trim,
          mileage: formData.mileage ?? 0,
          fuelCapacity: formData.fuelCapacity ?? 0,
          fuelType: formData.fuelType as FuelTypeEnum,
          purchaseDate: formData.purchaseDate,
          purchasePrice: formData.purchasePrice ?? 0,
          status: formData.status as VehicleStatusEnum,
          location: formData.location,
          assignedTechnicianID: formData.assignedTechnicianID,
        };

        await updateVehicleMutation.mutateAsync(updateCommand);
        notify("Vehicle updated successfully!", "success");
      }

      router.push("/vehicles");
    } catch (error: any) {
      let errorMessage =
        mode === "create"
          ? getErrorMessage(
              error,
              "Failed to create vehicle. Please check your input and try again.",
            )
          : getErrorMessage(
              error,
              "Failed to update vehicle. Please check your input and try again.",
            );

      const fieldErrors = getErrorFields(error, [
        "name",
        "make",
        "model",
        "year",
        "vin",
        "licensePlate",
        "licensePlateExpirationDate",
        "vehicleType",
        "fuelType",
        "status",
        "vehicleGroupID",
        "assignedTechnicianID",
        "trim",
        "location",
        "mileage",
        "fuelCapacity",
        "purchaseDate",
        "purchasePrice",
      ]);
      setErrors(fieldErrors);
      notify(errorMessage, "error");
    }
  };

  const handleCancel = () => {
    setErrors({});
    router.push("/vehicles");
  };

  const saveButtonText =
    mode === "create" ? "Create Vehicle" : "Update Vehicle";
  const isSaving =
    mode === "create"
      ? createVehicleMutation.isPending
      : updateVehicleMutation.isPending;

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <form onSubmit={handleSubmit} className="space-y-8">
          {/* Basic Information Section */}
          <div className="space-y-6">
            <h2 className="text-lg font-medium text-gray-900 border-b border-gray-200 pb-2">
              Basic Information
            </h2>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {/* Vehicle Name */}
              <FormField
                label="Vehicle Name"
                htmlFor="name"
                required
                error={errors.name}
              >
                <input
                  type="text"
                  id="name"
                  value={formData.name}
                  onChange={e => handleInputChange("name", e.target.value)}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.name ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="Enter vehicle name"
                />
              </FormField>

              {/* Year */}
              <FormField
                label="Year"
                htmlFor="year"
                required
                error={errors.year}
              >
                <input
                  type="number"
                  id="year"
                  value={formData.year ?? ""}
                  onChange={e => {
                    const value = e.target.value;
                    handleInputChange(
                      "year",
                      value === "" ? null : parseInt(value),
                    );
                  }}
                  min="1900"
                  max={new Date().getFullYear() + 1}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.year ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="Enter year"
                />
              </FormField>

              {/* Make */}
              <FormField
                label="Make"
                htmlFor="make"
                required
                error={errors.make}
              >
                <input
                  type="text"
                  id="make"
                  value={formData.make}
                  onChange={e => handleInputChange("make", e.target.value)}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.make ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="Enter make"
                />
              </FormField>

              {/* Model */}
              <FormField
                label="Model"
                htmlFor="model"
                required
                error={errors.model}
              >
                <input
                  type="text"
                  id="model"
                  value={formData.model}
                  onChange={e => handleInputChange("model", e.target.value)}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.model ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="Enter model"
                />
              </FormField>

              {/* VIN */}
              <FormField label="VIN" htmlFor="vin" required error={errors.vin}>
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
                  placeholder="Enter VIN (17 characters)"
                />
              </FormField>

              {/* Trim */}
              <FormField
                label="Trim"
                htmlFor="trim"
                required
                error={errors.trim}
              >
                <input
                  type="text"
                  id="trim"
                  value={formData.trim}
                  onChange={e => handleInputChange("trim", e.target.value)}
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.trim ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="Enter trim"
                />
              </FormField>

              {/* Vehicle Type */}
              <FormField
                label="Vehicle Type"
                htmlFor="vehicleType"
                required
                error={errors.vehicleType}
              >
                <select
                  id="vehicleType"
                  value={formData.vehicleType}
                  onChange={e =>
                    handleInputChange("vehicleType", parseInt(e.target.value))
                  }
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.vehicleType ? "border-red-300" : "border-gray-300"
                  }`}
                >
                  <option value="">Select vehicle type</option>
                  {getVehicleTypeOptions().map(option => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </select>
              </FormField>

              {/* Fuel Type */}
              <FormField
                label="Fuel Type"
                htmlFor="fuelType"
                required
                error={errors.fuelType}
              >
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
              </FormField>

              {/* Status */}
              <FormField
                label="Status"
                htmlFor="status"
                required
                error={errors.status}
              >
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
              </FormField>

              {/* License Plate */}
              <FormField
                label="License Plate"
                htmlFor="licensePlate"
                required
                error={errors.licensePlate}
              >
                <input
                  type="text"
                  id="licensePlate"
                  value={formData.licensePlate}
                  onChange={e =>
                    handleInputChange("licensePlate", e.target.value)
                  }
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.licensePlate ? "border-red-300" : "border-gray-300"
                  }`}
                  placeholder="Enter license plate"
                />
              </FormField>

              {/* License Plate Expiration Date */}
              <FormField
                label="License Plate Expiration Date"
                htmlFor="licensePlateExpirationDate"
                required
                error={errors.licensePlateExpirationDate}
              >
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
              </FormField>

              {/* Vehicle Group */}
              <FormField
                label="Vehicle Group"
                htmlFor="vehicleGroupID"
                required
                error={errors.vehicleGroupID}
              >
                <select
                  id="vehicleGroupID"
                  value={formData.vehicleGroupID}
                  onChange={e =>
                    handleInputChange(
                      "vehicleGroupID",
                      parseInt(e.target.value),
                    )
                  }
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.vehicleGroupID ? "border-red-300" : "border-gray-300"
                  }`}
                  disabled={isLoadingVehicleGroups}
                >
                  <option value={0}>Select vehicle group</option>
                  {vehicleGroups?.map(group => (
                    <option key={group.id} value={group.id}>
                      {group.name}
                    </option>
                  ))}
                </select>
              </FormField>

              {/* Assigned Technician */}
              <FormField
                label="Assigned Technician"
                htmlFor="assignedTechnicianID"
                error={errors.assignedTechnicianID}
              >
                <select
                  id="assignedTechnicianID"
                  value={formData.assignedTechnicianID || ""}
                  onChange={e =>
                    handleInputChange(
                      "assignedTechnicianID",
                      e.target.value || null,
                    )
                  }
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    errors.assignedTechnicianID
                      ? "border-red-300"
                      : "border-gray-300"
                  }`}
                  disabled={isLoadingTechnicians}
                >
                  <option value="">No technician assigned</option>
                  {technicians?.map(tech => (
                    <option key={tech.id} value={tech.id}>
                      {`${tech.firstName} ${tech.lastName}`}
                    </option>
                  ))}
                </select>
              </FormField>

              {/* Location */}
              <FormField
                label="Location"
                htmlFor="location"
                required
                error={errors.location}
              >
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
              </FormField>
            </div>
          </div>

          {/* Operational Data Section */}
          <div className="space-y-6">
            <h2 className="text-lg font-medium text-gray-900 border-b border-gray-200 pb-2">
              Operational Data
            </h2>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {/* Mileage */}
              <FormField
                label="Mileage (km)"
                htmlFor="mileage"
                error={errors.mileage}
              >
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
                  placeholder="Enter mileage in kilometers"
                />
              </FormField>

              {/* Fuel Capacity */}
              <FormField
                label="Fuel Capacity (L)"
                htmlFor="fuelCapacity"
                error={errors.fuelCapacity}
              >
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
                  placeholder="Enter fuel capacity in litres"
                />
              </FormField>
            </div>
          </div>

          {/* Financial Information Section */}
          <div className="space-y-6">
            <h2 className="text-lg font-medium text-gray-900 border-b border-gray-200 pb-2">
              Financial Information
            </h2>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {/* Purchase Date */}
              <FormField
                label="Purchase Date"
                htmlFor="purchaseDate"
                required
                error={errors.purchaseDate}
              >
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
              </FormField>

              {/* Purchase Price */}
              <FormField
                label="Purchase Price ($)"
                htmlFor="purchasePrice"
                error={errors.purchasePrice}
              >
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
              </FormField>
            </div>
          </div>

          {/* Form Actions */}
          <div className="flex justify-between items-center pt-6 border-t border-gray-200">
            <SecondaryButton onClick={handleCancel} disabled={isSaving}>
              Cancel
            </SecondaryButton>

            <PrimaryButton type="submit" disabled={isSaving}>
              {isSaving ? "Saving..." : saveButtonText}
            </PrimaryButton>
          </div>
        </form>
      </div>
    </div>
  );
};

export default VehicleForm;
