import { VehicleStatusEnum } from "../../vehicle/types/vehicleEnum";
export { VehicleStatusEnum } from "../../vehicle/types/vehicleEnum";

export const getVehicleStatusLabel = (status: VehicleStatusEnum): string => {
  switch (status) {
    case VehicleStatusEnum.ACTIVE:
      return "Active";
    case VehicleStatusEnum.MAINTENANCE:
      return "Maintenance";
    case VehicleStatusEnum.OUT_OF_SERVICE:
      return "Out of Service";
    case VehicleStatusEnum.INACTIVE:
      return "Inactive";
    default:
      return "Unknown";
  }
};

export const getVehicleStatusColor = (status: VehicleStatusEnum): string => {
  switch (status) {
    case VehicleStatusEnum.ACTIVE:
      return "green";
    case VehicleStatusEnum.MAINTENANCE:
      return "orange";
    case VehicleStatusEnum.OUT_OF_SERVICE:
      return "red";
    case VehicleStatusEnum.INACTIVE:
      return "red";
    default:
      return "gray";
  }
};

export const getAllVehicleStatusOptions = () => {
  return Object.values(VehicleStatusEnum)
    .filter(value => typeof value === "number")
    .map(statusValue => {
      const statusEnum = statusValue as VehicleStatusEnum;
      return {
        value: statusEnum,
        label: getVehicleStatusLabel(statusEnum),
        color: getVehicleStatusColor(statusEnum),
      };
    });
};
