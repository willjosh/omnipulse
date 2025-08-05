import {
  VehicleTypeEnum,
  FuelTypeEnum,
  VehicleStatusEnum,
} from "./vehicleEnum";

export interface Vehicle {
  id: number;
  name: string;
  make: string;
  model: string;
  year: number;
  vin: string;
  licensePlate: string;
  licensePlateExpirationDate: string;
  vehicleType: VehicleTypeEnum;
  vehicleGroupID: number;
  vehicleGroupName: string;
  assignedTechnicianName: string;
  assignedTechnicianID?: string | null;
  trim: string;
  mileage: number;
  engineHours: number;
  fuelCapacity: number;
  fuelType: FuelTypeEnum;
  purchaseDate: string;
  purchasePrice: number;
  status: VehicleStatusEnum;
  location: string;
}

export interface VehicleWithLabels
  extends Omit<Vehicle, "vehicleType" | "status" | "fuelType"> {
  vehicleType: number;
  vehicleTypeLabel: string;
  status: number;
  statusLabel: string;
  fuelType: number;
  fuelTypeLabel: string;
  vehicleTypeEnum: VehicleTypeEnum;
  statusEnum: VehicleStatusEnum;
  fuelTypeEnum: FuelTypeEnum;
}

export interface CreateVehicleCommand {
  name: string;
  make: string;
  model: string;
  year: number;
  vin: string;
  licensePlate: string;
  licensePlateExpirationDate: string;
  vehicleType: VehicleTypeEnum;
  vehicleGroupID: number;
  trim: string;
  mileage: number;
  engineHours: number;
  fuelCapacity: number;
  fuelType: FuelTypeEnum;
  purchaseDate: string;
  purchasePrice: number;
  status: VehicleStatusEnum;
  location: string;
  assignedTechnicianID?: string | null;
}

export interface UpdateVehicleCommand {
  vehicleID: number;
  name: string;
  make: string;
  model: string;
  year: number;
  vin: string;
  licensePlate: string;
  licensePlateExpirationDate: string;
  vehicleType: VehicleTypeEnum;
  vehicleGroupID: number;
  trim: string;
  mileage: number;
  engineHours: number;
  fuelCapacity: number;
  fuelType: FuelTypeEnum;
  purchaseDate: string;
  purchasePrice: number;
  status: VehicleStatusEnum;
  location: string;
  assignedTechnicianID?: string | null;
}

export interface VehicleFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
}

export interface VehicleAssignedData {
  assignedVehicleCount: number;
  unassignedVehicleCount: number;
}

export interface VehicleStatusData {
  activeVehicleCount: number;
  inactiveVehicleCount: number;
  maintenanceVehicleCount: number;
  outOfServiceVehicleCount: number;
}
