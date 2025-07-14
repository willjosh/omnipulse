import {
  VehicleTypeEnum,
  FuelTypeEnum,
  VehicleStatusEnum,
} from "./vehicleEnum";

export interface Vehicle {
  id: number;
  Name: string;
  Make: string;
  Model: string;
  Year: number;
  VIN: string;
  LicensePlate: string;
  LicensePlateExpirationDate: string;
  VehicleType: VehicleTypeEnum;
  VehicleGroupID: number;
  VehicleGroupName: string;
  AssignedTechnicianName: string;
  AssignedTechnicianID?: string | null;
  Trim: string;
  Mileage: number;
  EngineHours: number;
  FuelCapacity: number;
  FuelType: FuelTypeEnum;
  PurchaseDate: string;
  PurchasePrice: number;
  Status: VehicleStatusEnum;
  Location: string;
}

export interface VehicleWithLabels
  extends Omit<Vehicle, "VehicleType" | "Status" | "FuelType"> {
  VehicleType: number;
  VehicleTypeLabel: string;
  Status: number;
  StatusLabel: string;
  FuelType: number;
  FuelTypeLabel: string;
  VehicleTypeEnum: VehicleTypeEnum;
  StatusEnum: VehicleStatusEnum;
  FuelTypeEnum: FuelTypeEnum;
}

export interface CreateVehicleCommand {
  Name: string;
  Make: string;
  Model: string;
  Year: number;
  VIN: string;
  LicensePlate: string;
  LicensePlateExpirationDate: string;
  VehicleType: VehicleTypeEnum;
  VehicleGroupID: number;
  Trim: string;
  Mileage: number;
  EngineHours: number;
  FuelCapacity: number;
  FuelType: FuelTypeEnum;
  PurchaseDate: string;
  PurchasePrice: number;
  VehicleStatus: VehicleStatusEnum;
  Location: string;
  AssignedTechnicianID?: string | null;
}

export interface UpdateVehicleCommand {
  id: number;
  Name: string;
  Make: string;
  Model: string;
  Year: number;
  VIN: string;
  LicensePlate: string;
  LicensePlateExpirationDate: string;
  VehicleType: VehicleTypeEnum;
  VehicleGroupID: number;
  Trim: string;
  Mileage: number;
  EngineHours: number;
  FuelCapacity: number;
  FuelType: FuelTypeEnum;
  PurchaseDate: string;
  PurchasePrice: number;
  VehicleStatus: VehicleStatusEnum;
  Location: string;
  AssignedTechnicianID?: string | null;
}

export interface VehicleFilter {
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
  search?: string;
}
