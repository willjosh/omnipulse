import { create } from "zustand";
import { subscribeWithSelector } from "zustand/middleware";
import {
  Vehicle,
  CreateVehicleCommand,
  UpdateVehicleCommand,
} from "@/app/_hooks/Vehicle/vehicleType";
import {
  VehicleTypeEnum,
  FuelTypeEnum,
  VehicleStatusEnum,
} from "@/app/_hooks/Vehicle/vehicleEnum";

interface VehicleFormData {
  // Basic vehicle information
  vehicleName: string;
  year: number;
  make: string;
  model: string;
  vin: string;
  type: VehicleTypeEnum;
  licensePlate: string;
  licensePlateExpirationDate: string;
  fuelType: FuelTypeEnum;
  trim: string;
  status: VehicleStatusEnum;

  // Vehicle group and assignment
  vehicleGroupID: number;
  vehicleGroupName: string;
  assignedTechnicianID: string | null;
  assignedTechnicianName: string;

  // Operational data - changed to nullable
  mileage: number | null;
  engineHours: number | null;
  fuelCapacity: number | null;
  location: string;

  // Financial data
  purchaseDate: string;
  purchasePrice: number;
}

interface VehicleGroup {
  id: number;
  name: string;
}

interface Technician {
  id: string;
  name: string;
}

interface VehicleFormStore {
  // State
  formData: VehicleFormData;
  showValidation: boolean;
  mode: "create" | "edit";
  vehicleId: number | null;
  isLoading: boolean;
  isDirty: boolean;
  validationErrors: Record<string, string>;

  // Reference data
  vehicleGroups: VehicleGroup[];
  technicians: Technician[];

  // Actions
  updateFormData: (data: Partial<VehicleFormData>) => void;
  setShowValidation: (show: boolean) => void;
  setLoading: (loading: boolean) => void;
  setValidationErrors: (errors: Record<string, string>) => void;
  setVehicleGroups: (groups: VehicleGroup[]) => void;
  setTechnicians: (technicians: Technician[]) => void;
  resetForm: () => void;
  initializeForEdit: (vehicleId: number, vehicleData: Vehicle) => void;
  initializeForCreate: () => void;

  // Validation
  isFormComplete: () => boolean;
  isFormValid: () => boolean;

  // Data transformation
  toCreateCommand: () => CreateVehicleCommand;
  toUpdateCommand: () => UpdateVehicleCommand;
}

// Create empty form data - changed to use null for numeric fields
const createEmptyFormData = (): VehicleFormData => ({
  vehicleName: "",
  year: new Date().getFullYear(),
  make: "",
  model: "",
  vin: "",
  type: VehicleTypeEnum.CAR,
  licensePlate: "",
  licensePlateExpirationDate: "",
  fuelType: FuelTypeEnum.PETROL,
  trim: "",
  status: VehicleStatusEnum.ACTIVE,
  vehicleGroupID: 0,
  vehicleGroupName: "",
  assignedTechnicianID: null,
  assignedTechnicianName: "",
  mileage: null,
  engineHours: null,
  fuelCapacity: null,
  location: "",
  purchaseDate: "",
  purchasePrice: 0,
});

// Mapping function from API Vehicle to form data
const mapVehicleToFormData = (vehicle: Vehicle): VehicleFormData => {
  return {
    vehicleName: vehicle.Name,
    year: vehicle.Year,
    make: vehicle.Make,
    model: vehicle.Model,
    vin: vehicle.VIN,
    type: vehicle.VehicleType,
    licensePlate: vehicle.LicensePlate,
    licensePlateExpirationDate: vehicle.LicensePlateExpirationDate,
    fuelType: vehicle.FuelType,
    trim: vehicle.Trim,
    status: vehicle.Status,
    vehicleGroupID: vehicle.VehicleGroupID,
    vehicleGroupName: vehicle.VehicleGroupName,
    assignedTechnicianID: vehicle.AssignedTechnicianID ?? null,
    assignedTechnicianName: vehicle.AssignedTechnicianName,
    mileage: vehicle.Mileage,
    engineHours: vehicle.EngineHours,
    fuelCapacity: vehicle.FuelCapacity,
    location: vehicle.Location,
    purchaseDate: vehicle.PurchaseDate,
    purchasePrice: vehicle.PurchasePrice,
  };
};

// Basic validation for required fields - updated to handle nullable numbers
const validateFormData = (
  formData: VehicleFormData,
): Record<string, string> => {
  const errors: Record<string, string> = {};

  if (!formData.vehicleName.trim()) {
    errors.vehicleName = "Vehicle name is required";
  }

  if (!formData.vin.trim()) {
    errors.vin = "VIN is required";
  } else if (formData.vin.length !== 17) {
    errors.vin = "VIN must be 17 characters long";
  }

  if (!formData.licensePlate.trim()) {
    errors.licensePlate = "License plate is required";
  }

  if (!formData.licensePlateExpirationDate) {
    errors.licensePlateExpirationDate =
      "License plate expiration date is required";
  }

  if (
    !formData.year ||
    formData.year < 1900 ||
    formData.year > new Date().getFullYear() + 1
  ) {
    errors.year = "Valid year is required";
  }

  if (!formData.make.trim()) {
    errors.make = "Make is required";
  }

  if (!formData.model.trim()) {
    errors.model = "Model is required";
  }

  if (!formData.trim.trim()) {
    errors.trim = "Trim is required";
  }

  if (formData.vehicleGroupID === 0) {
    errors.vehicleGroupID = "Vehicle group is required";
  }

  // Updated validation for nullable numeric fields
  if (formData.mileage !== null && formData.mileage < 0) {
    errors.mileage = "Mileage cannot be negative";
  }

  if (formData.engineHours !== null && formData.engineHours < 0) {
    errors.engineHours = "Engine hours cannot be negative";
  }

  if (formData.fuelCapacity !== null && formData.fuelCapacity <= 0) {
    errors.fuelCapacity = "Fuel capacity must be greater than 0";
  }

  if (!formData.location.trim()) {
    errors.location = "Location is required";
  }

  if (!formData.purchaseDate) {
    errors.purchaseDate = "Purchase date is required";
  }

  if (formData.purchasePrice < 0) {
    errors.purchasePrice = "Purchase price cannot be negative";
  }

  return errors;
};

export const useVehicleFormStore = create<VehicleFormStore>()(
  subscribeWithSelector((set, get) => ({
    // Initial state
    formData: createEmptyFormData(),
    showValidation: false,
    mode: "create",
    vehicleId: null,
    isLoading: false,
    isDirty: false,
    validationErrors: {},
    vehicleGroups: [],
    technicians: [],

    // Update actions with dirty tracking
    updateFormData: data =>
      set(state => ({
        formData: { ...state.formData, ...data },
        isDirty: true,
        validationErrors: {}, // Clear errors on update
      })),

    setShowValidation: show => set({ showValidation: show }),

    setLoading: loading => set({ isLoading: loading }),

    setValidationErrors: errors => set({ validationErrors: errors }),

    setVehicleGroups: groups => set({ vehicleGroups: groups }),

    setTechnicians: technicians => set({ technicians }),

    resetForm: () =>
      set({
        formData: createEmptyFormData(),
        showValidation: false,
        mode: "create",
        vehicleId: null,
        isDirty: false,
        validationErrors: {},
      }),

    initializeForEdit: (vehicleId, vehicleData) =>
      set({
        mode: "edit",
        vehicleId,
        formData: mapVehicleToFormData(vehicleData),
        showValidation: false,
        isDirty: false,
        validationErrors: {},
      }),

    initializeForCreate: () =>
      set({
        mode: "create",
        vehicleId: null,
        formData: createEmptyFormData(),
        showValidation: false,
        isDirty: false,
        validationErrors: {},
      }),

    // Validation methods
    isFormComplete: () => {
      const errors = validateFormData(get().formData);
      return Object.keys(errors).length === 0;
    },

    isFormValid: () => {
      const errors = validateFormData(get().formData);
      return Object.keys(errors).length === 0;
    },

    // Data transformation methods - updated to handle nullable values
    toCreateCommand: (): CreateVehicleCommand => {
      const { formData } = get();
      return {
        Name: formData.vehicleName,
        Make: formData.make,
        Model: formData.model,
        Year: formData.year,
        VIN: formData.vin,
        LicensePlate: formData.licensePlate,
        LicensePlateExpirationDate: formData.licensePlateExpirationDate,
        VehicleType: formData.type,
        VehicleGroupID: formData.vehicleGroupID,
        Trim: formData.trim,
        Mileage: formData.mileage ?? 0,
        EngineHours: formData.engineHours ?? 0,
        FuelCapacity: formData.fuelCapacity ?? 0,
        FuelType: formData.fuelType,
        PurchaseDate: formData.purchaseDate,
        PurchasePrice: formData.purchasePrice,
        VehicleStatus: formData.status,
        Location: formData.location,
        AssignedTechnicianID: formData.assignedTechnicianID,
      };
    },

    toUpdateCommand: (): UpdateVehicleCommand => {
      const { formData, vehicleId } = get();
      if (!vehicleId) {
        throw new Error("Vehicle ID is required for update");
      }

      return {
        id: vehicleId,
        Name: formData.vehicleName,
        Make: formData.make,
        Model: formData.model,
        Year: formData.year,
        VIN: formData.vin,
        LicensePlate: formData.licensePlate,
        LicensePlateExpirationDate: formData.licensePlateExpirationDate,
        VehicleType: formData.type,
        VehicleGroupID: formData.vehicleGroupID,
        Trim: formData.trim,
        Mileage: formData.mileage ?? 0,
        EngineHours: formData.engineHours ?? 0,
        FuelCapacity: formData.fuelCapacity ?? 0,
        FuelType: formData.fuelType,
        PurchaseDate: formData.purchaseDate,
        PurchasePrice: formData.purchasePrice,
        VehicleStatus: formData.status,
        Location: formData.location,
        AssignedTechnicianID: formData.assignedTechnicianID,
      };
    },
  })),
);

// Selector hooks for better performance
export const useVehicleFormData = () =>
  useVehicleFormStore(state => state.formData);
export const useVehicleFormMode = () =>
  useVehicleFormStore(state => state.mode);
export const useVehicleFormValidation = () => {
  const showValidation = useVehicleFormStore(state => state.showValidation);
  const validationErrors = useVehicleFormStore(state => state.validationErrors);
  const isFormValid = useVehicleFormStore(state => state.isFormValid());

  return { showValidation, validationErrors, isFormValid };
};
export const useVehicleFormStatus = () => {
  const isLoading = useVehicleFormStore(state => state.isLoading);
  const isDirty = useVehicleFormStore(state => state.isDirty);
  const vehicleId = useVehicleFormStore(state => state.vehicleId);

  return { isLoading, isDirty, vehicleId };
};
export const useVehicleFormReferenceData = () => {
  const vehicleGroups = useVehicleFormStore(state => state.vehicleGroups);
  const technicians = useVehicleFormStore(state => state.technicians);

  return { vehicleGroups, technicians };
};
