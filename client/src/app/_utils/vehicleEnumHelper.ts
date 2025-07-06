import {
  FuelTypeEnum,
  VehicleStatusEnum,
  VehicleTypeEnum,
} from "../hooks/Vehicle/vehicleType";

export const getVehicleTypeLabel = (type: VehicleTypeEnum) => {
  switch (type) {
    case VehicleTypeEnum.TRUCK:
      return "Truck";
    case VehicleTypeEnum.VAN:
      return "Van";
    case VehicleTypeEnum.CAR:
      return "Car";
    case VehicleTypeEnum.MOTORCYCLE:
      return "Motorcycle";
    case VehicleTypeEnum.BUS:
      return "Bus";
    case VehicleTypeEnum.HEAVY_VEHICLE:
      return "Heavy Vehicle";
    case VehicleTypeEnum.TRAILER:
      return "Trailer";
    case VehicleTypeEnum.OTHER:
      return "Other";
    default:
      return "Unknown";
  }
};

export const getStatusLabel = (status: VehicleStatusEnum) => {
  switch (status) {
    case VehicleStatusEnum.ACTIVE:
      return "Active";
    case VehicleStatusEnum.INACTIVE:
      return "Inactive";
    case VehicleStatusEnum.MAINTENANCE:
      return "In Shop";
    case VehicleStatusEnum.OUT_OF_SERVICE:
      return "Out of Service";
    default:
      return "Unknown";
  }
};

export const getVehicleIcon = (type: VehicleTypeEnum) => {
  switch (type) {
    case VehicleTypeEnum.BUS:
      return "🚌";
    case VehicleTypeEnum.CAR:
      return "🚗";
    case VehicleTypeEnum.TRUCK:
      return "🚛";
    case VehicleTypeEnum.VAN:
      return "🚐";
    case VehicleTypeEnum.MOTORCYCLE:
      return "🏍️";
    case VehicleTypeEnum.TRAILER:
      return "🚚";
    default:
      return "🚗";
  }
};

export const getFuelTypeLabel = (fuelType: FuelTypeEnum) => {
  switch (fuelType) {
    case FuelTypeEnum.PETROL:
      return "Petrol";
    case FuelTypeEnum.DIESEL:
      return "Diesel";
    case FuelTypeEnum.ELECTRIC:
      return "Electric";
    case FuelTypeEnum.HYBRID:
      return "Hybrid";
    case FuelTypeEnum.GAS:
      return "Gas";
    case FuelTypeEnum.LPG:
      return "LPG";
    case FuelTypeEnum.CNG:
      return "CNG";
    case FuelTypeEnum.BIO_DIESEL:
      return "Bio Diesel";
    default:
      return "Unknown";
  }
};

export const getStatusDot = (status: VehicleStatusEnum) => {
  switch (status) {
    case VehicleStatusEnum.ACTIVE:
      return "bg-green-500";
    case VehicleStatusEnum.INACTIVE:
      return "bg-blue-500";
    case VehicleStatusEnum.MAINTENANCE:
      return "bg-orange-500";
    case VehicleStatusEnum.OUT_OF_SERVICE:
      return "bg-red-500";
    default:
      return "bg-gray-500";
  }
};
