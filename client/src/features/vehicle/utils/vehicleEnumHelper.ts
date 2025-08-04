import {
  FuelTypeEnum,
  VehicleStatusEnum,
  VehicleTypeEnum,
} from "@/features/vehicle/types/vehicleEnum";
import {
  getVehicleStatusLabel,
  getVehicleStatusColor,
  getAllVehicleStatusOptions,
} from "@/features/vehicle-status/types/vehicleStatusEnum";
export const getVehicleStatusOptions = () => getAllVehicleStatusOptions();

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

export const getStatusLabel = (status: VehicleStatusEnum) =>
  getVehicleStatusLabel(status);

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
  const color = getVehicleStatusColor(status);
  return `bg-${color}-500`;
};

export const getStatusColor = (status: VehicleStatusEnum) => {
  const color = getVehicleStatusColor(status);
  switch (color) {
    case "green":
      return "text-green-600 bg-green-50";
    case "orange":
      return "text-orange-600 bg-orange-50";
    case "red":
      return "text-red-600 bg-red-50";
    case "blue":
      return "text-blue-600 bg-blue-50";
    default:
      return "text-gray-600 bg-gray-50";
  }
};
