export interface Vehicle {
  ID: number;
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

export enum VehicleTypeEnum {
  TRUCK = 1,
  VAN = 2,
  CAR = 3,
  MOTORCYCLE = 4,
  BUS = 5,
  HEAVY_VEHICLE = 6,
  TRAILER = 7,
  OTHER = 8,
}

export enum FuelTypeEnum {
  PETROL = 1,
  DIESEL = 2,
  ELECTRIC = 3,
  HYBRID = 4,
  GAS = 5,
  LPG = 6,
  CNG = 7,
  BIO_DIESEL = 8,
  OTHER = 9,
}

export enum VehicleStatusEnum {
  ACTIVE = 1,
  MAINTENANCE = 2,
  OUT_OF_SERVICE = 3,
  INACTIVE = 4,
}
