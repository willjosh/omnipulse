import { create } from "zustand";
import {
  VehicleFormData,
  FormSection,
  createEmptyVehicleFormData,
  VehicleDetailsData,
  VehicleMaintenanceData,
  VehicleLifecycleData,
  VehicleFinancialData,
  VehicleSpecificationsData,
} from "../types/VehicleFormTypes";
import { VehicleListItem } from "../types/VehicleListTypes";

interface VehicleFormStore {
  formData: VehicleFormData;
  currentSection: FormSection;
  showValidation: boolean;
  mode: "create" | "edit";
  vehicleId: string | null;

  updateDetails: (data: Partial<VehicleDetailsData>) => void;
  updateMaintenance: (data: Partial<VehicleMaintenanceData>) => void;
  updateLifecycle: (data: Partial<VehicleLifecycleData>) => void;
  updateFinancial: (data: Partial<VehicleFinancialData>) => void;
  updateSpecifications: (data: Partial<VehicleSpecificationsData>) => void;
  setCurrentSection: (section: FormSection) => void;
  setShowValidation: (show: boolean) => void;
  resetForm: () => void;
  initializeForEdit: (vehicleId: string, vehicleData: VehicleListItem) => void;
  initializeForCreate: () => void;

  isDetailsComplete: () => boolean;
}

// Helper function for current front end implementation
const mapVehicleListItemToFormData = (
  vehicle: VehicleListItem,
): VehicleFormData => {
  return {
    details: {
      vehicleName: vehicle.name,
      year: vehicle.year,
      make: vehicle.make,
      model: vehicle.model,
      vin: vehicle.vin,
      type: vehicle.type,
      telematicsDevice: "",
      licensePlate: vehicle.licensePlate,
      fuelType: vehicle.fuelType,
      trim: "",
      registrationState: "",
      labels: "",
      status: vehicle.status,
      group: vehicle.group,
      ownership: "",
    },
    maintenance: { serviceProgram: "" },
    lifecycle: {
      inServiceDate: "",
      inServiceOdometer: vehicle.currentMeter,
      estimatedServiceLifeMonths: null,
      estimatedServiceLifeMeter: null,
      estimatedResaleValue: null,
      outOfServiceDate: "",
      outOfServiceOdometer: null,
    },
    financial: {
      purchaseVendor: "",
      purchaseDate: "",
      purchasePrice: null,
      odometer: vehicle.currentMeter, // Map current meter
      notes: "",
      loanLeaseType: "none",
    },
    specifications: {
      // All specifications start empty since not available in list data
      width: null,
      height: null,
      length: null,
      interiorVolume: null,
      passengerVolume: null,
      cargoVolume: null,
      groundClearance: null,
      bedLength: null,
      curbWeight: null,
      grossVehicleWeightRating: null,
      towingCapacity: null,
      maxPayload: null,
      epaCity: "",
      epaHighway: "",
      epaCombined: "",
      engineSummary: "",
      engineBrand: "",
      aspiration: "",
      blockType: "",
      bore: null,
      camType: "",
      compression: "",
      cylinders: null,
      displacement: "",
      fuelInduction: "",
      maxHp: null,
      maxTorque: "",
      redlineRpm: null,
      stroke: "",
      valves: "",
      transmissionSummary: "",
      transmissionBrand: "",
      transmissionType: "",
      transmissionGears: "",
      driveType: "",
      brakeSystem: "",
      frontTrackWidth: "",
      rearTrackWidth: "",
      wheelbase: null,
      frontWheelDiameter: "",
      rearWheelDiameter: "",
      rearAxle: "",
      frontTireType: "",
      frontTirePsi: "",
      rearTireType: "",
      rearTirePsi: "",
      fuelQuality: "",
      fuelTank1Capacity: null,
      fuelTank2Capacity: null,
      oilCapacity: "",
    },
  };
};

export const useVehicleFormStore = create<VehicleFormStore>((set, get) => ({
  formData: createEmptyVehicleFormData(),
  currentSection: FormSection.DETAILS,
  showValidation: false,
  mode: "create",
  vehicleId: null,

  updateDetails: data =>
    set(state => ({
      formData: {
        ...state.formData,
        details: { ...state.formData.details, ...data },
      },
    })),

  updateMaintenance: data =>
    set(state => ({
      formData: {
        ...state.formData,
        maintenance: { ...state.formData.maintenance, ...data },
      },
    })),

  updateLifecycle: data =>
    set(state => ({
      formData: {
        ...state.formData,
        lifecycle: { ...state.formData.lifecycle, ...data },
      },
    })),

  updateFinancial: data =>
    set(state => ({
      formData: {
        ...state.formData,
        financial: { ...state.formData.financial, ...data },
      },
    })),

  updateSpecifications: data =>
    set(state => ({
      formData: {
        ...state.formData,
        specifications: { ...state.formData.specifications, ...data },
      },
    })),

  setCurrentSection: section => set({ currentSection: section }),

  setShowValidation: show => set({ showValidation: show }),

  resetForm: () =>
    set({
      formData: createEmptyVehicleFormData(),
      currentSection: FormSection.DETAILS,
      showValidation: false,
      mode: "create",
      vehicleId: null,
    }),

  initializeForEdit: (vehicleId: string, vehicleData: VehicleListItem) =>
    set({
      mode: "edit",
      vehicleId,
      formData: mapVehicleListItemToFormData(vehicleData),
      currentSection: FormSection.DETAILS,
      showValidation: false,
    }),

  initializeForCreate: () =>
    set({
      mode: "create",
      vehicleId: null,
      formData: createEmptyVehicleFormData(),
      currentSection: FormSection.DETAILS,
      showValidation: false,
    }),

  isDetailsComplete: () => {
    const details = get().formData.details;
    const requiredFields = [
      "vehicleName",
      "vin",
      "licensePlate",
      "type",
      "fuelType",
      "year",
      "make",
      "model",
      "trim",
      "registrationState",
    ];

    return requiredFields.every(field => {
      const value = details[field as keyof VehicleDetailsData];
      return value !== "" && value !== null && value !== undefined;
    });
  },
}));
