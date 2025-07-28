export type VehicleStatus =
  | "Active"
  | "Inactive"
  | "In Shop"
  | "Out of Service";
export type MeterUnit = "mi" | "km" | "hr";
export type LoanLeaseType = "none" | "loan" | "lease";

export interface VehicleDetailsData {
  vehicleName: string;
  year: number | null;
  make: string;
  model: string;
  vin: string;
  type: string;
  telematicsDevice: string;
  licensePlate: string;
  fuelType: string;
  trim: string;
  registrationState: string;
  labels: string;

  status: string;
  group: string;
  ownership: string;
}

export interface VehicleMaintenanceData {
  serviceProgram: string;
}

export interface VehicleLifecycleData {
  inServiceDate: string;
  inServiceOdometer: number | null;
  estimatedServiceLifeMonths: number | null;
  estimatedServiceLifeMeter: number | null;
  estimatedResaleValue: number | null;
  outOfServiceDate: string;
  outOfServiceOdometer: number | null;
}

export interface VehicleFinancialData {
  purchaseVendor: string;
  purchaseDate: string;
  purchasePrice: number | null;
  odometer: number | null;
  notes: string;
  loanLeaseType: "none" | "loan" | "lease";
}

export interface VehicleSpecificationsData {
  width: number | null;
  height: number | null;
  length: number | null;
  interiorVolume: number | null;
  passengerVolume: number | null;
  cargoVolume: number | null;
  groundClearance: number | null;
  bedLength: number | null;

  curbWeight: number | null;
  grossVehicleWeightRating: number | null;

  towingCapacity: number | null;
  maxPayload: number | null;

  epaCity: string;
  epaHighway: string;
  epaCombined: string;

  engineSummary: string;
  engineBrand: string;
  aspiration: string;
  blockType: string;
  bore: number | null;
  camType: string;
  compression: string;
  cylinders: number | null;
  displacement: string;
  fuelInduction: string;
  maxHp: number | null;
  maxTorque: string;
  redlineRpm: number | null;
  stroke: string;
  valves: string;

  transmissionSummary: string;
  transmissionBrand: string;
  transmissionType: string;
  transmissionGears: string;

  driveType: string;
  brakeSystem: string;
  frontTrackWidth: string;
  rearTrackWidth: string;
  wheelbase: number | null;
  frontWheelDiameter: string;
  rearWheelDiameter: string;
  rearAxle: string;
  frontTireType: string;
  frontTirePsi: string;
  rearTireType: string;
  rearTirePsi: string;

  fuelQuality: string;
  fuelTank1Capacity: number | null;
  fuelTank2Capacity: number | null;

  oilCapacity: string;
}

export interface VehicleFormData {
  details: VehicleDetailsData;
  maintenance: VehicleMaintenanceData;
  lifecycle: VehicleLifecycleData;
  financial: VehicleFinancialData;
  specifications: VehicleSpecificationsData;
}

export enum FormSection {
  DETAILS = "details",
  MAINTENANCE = "maintenance",
  LIFECYCLE = "lifecycle",
  FINANCIAL = "financial",
  SPECIFICATIONS = "specifications",
}

export interface VehicleFormState {
  currentSection: FormSection;
  completedSections: FormSection[];
  isDetailsComplete: boolean;
  hasUnsavedChanges: boolean;
}

export interface NavigationItem {
  section: FormSection;
  label: string;
  icon: string;
  isRequired: boolean;
  isCompleted: boolean;
  isAccessible: boolean;
}

export const createEmptyVehicleDetailsData = (): VehicleDetailsData => ({
  vehicleName: "",
  telematicsDevice: "",
  vin: "",
  licensePlate: "",
  type: "",
  fuelType: "",
  year: null,
  make: "",
  model: "",
  trim: "",
  registrationState: "",
  labels: "",
  status: "",
  group: "",
  ownership: "",
});

export const createEmptyVehicleMaintenanceData =
  (): VehicleMaintenanceData => ({ serviceProgram: "" });

export const createEmptyVehicleLifecycleData = (): VehicleLifecycleData => ({
  inServiceDate: "",
  inServiceOdometer: null,
  estimatedServiceLifeMonths: null,
  estimatedServiceLifeMeter: null,
  estimatedResaleValue: null,
  outOfServiceDate: "",
  outOfServiceOdometer: null,
});

export const createEmptyVehicleFinancialData = (): VehicleFinancialData => ({
  purchaseVendor: "",
  purchaseDate: "",
  purchasePrice: null,
  odometer: null,
  notes: "",
  loanLeaseType: "none",
});

export const createEmptyVehicleSpecificationsData =
  (): VehicleSpecificationsData => ({
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
  });

export const createEmptyVehicleFormData = (): VehicleFormData => ({
  details: createEmptyVehicleDetailsData(),
  maintenance: createEmptyVehicleMaintenanceData(),
  lifecycle: createEmptyVehicleLifecycleData(),
  financial: createEmptyVehicleFinancialData(),
  specifications: createEmptyVehicleSpecificationsData(),
});
