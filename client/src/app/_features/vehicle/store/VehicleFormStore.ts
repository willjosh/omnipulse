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

interface VehicleFormStore {
  formData: VehicleFormData;
  currentSection: FormSection;
  showValidation: boolean;

  updateDetails: (data: Partial<VehicleDetailsData>) => void;
  updateMaintenance: (data: Partial<VehicleMaintenanceData>) => void;
  updateLifecycle: (data: Partial<VehicleLifecycleData>) => void;
  updateFinancial: (data: Partial<VehicleFinancialData>) => void;
  updateSpecifications: (data: Partial<VehicleSpecificationsData>) => void;
  setCurrentSection: (section: FormSection) => void;
  setShowValidation: (show: boolean) => void;
  resetForm: () => void;

  isDetailsComplete: () => boolean;
}

export const useVehicleFormStore = create<VehicleFormStore>((set, get) => ({
  formData: createEmptyVehicleFormData(),
  currentSection: FormSection.DETAILS,
  showValidation: false,

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
