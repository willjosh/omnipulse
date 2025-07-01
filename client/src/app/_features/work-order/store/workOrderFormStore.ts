import { create } from "zustand";
import {
  WorkOrderFormData,
  WorkOrderFormSection,
  WorkOrderDetailsData,
  WorkOrderSchedulingData,
  WorkOrderOdometerData,
  initialFormData,
} from "../types/workOrderFormTypes";

interface WorkOrderFormStore {
  formData: WorkOrderFormData;
  currentSection: WorkOrderFormSection;
  showValidation: boolean;

  updateDetails: (data: Partial<WorkOrderDetailsData>) => void;
  updateScheduling: (data: Partial<WorkOrderSchedulingData>) => void;
  updateOdometer: (data: Partial<WorkOrderOdometerData>) => void;
  setCurrentSection: (section: WorkOrderFormSection) => void;
  setShowValidation: (show: boolean) => void;
  resetForm: () => void;
}

export const useWorkOrderFormStore = create<WorkOrderFormStore>(set => ({
  formData: initialFormData,
  currentSection: WorkOrderFormSection.DETAILS,
  showValidation: false,

  updateDetails: data =>
    set(state => ({
      formData: {
        ...state.formData,
        details: { ...state.formData.details, ...data },
      },
    })),

  updateScheduling: data =>
    set(state => ({
      formData: {
        ...state.formData,
        scheduling: { ...state.formData.scheduling, ...data },
      },
    })),

  updateOdometer: data =>
    set(state => ({
      formData: {
        ...state.formData,
        odometer: { ...state.formData.odometer, ...data },
      },
    })),

  setCurrentSection: section => set({ currentSection: section }),
  setShowValidation: show => set({ showValidation: show }),

  resetForm: () =>
    set({
      formData: initialFormData,
      currentSection: WorkOrderFormSection.DETAILS,
      showValidation: false,
    }),
}));
