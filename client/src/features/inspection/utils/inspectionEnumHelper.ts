import { VehicleConditionEnum } from "../types/inspectionEnum";

export const getVehicleConditionLabel = (
  condition: VehicleConditionEnum | number,
): string => {
  switch (condition) {
    case VehicleConditionEnum.Excellent:
      return "Excellent";
    case VehicleConditionEnum.HasIssuesButSafeToOperate:
      return "Has Issues But Safe To Operate";
    case VehicleConditionEnum.NotSafeToOperate:
      return "Not Safe To Operate";
    default:
      return "Unknown";
  }
};
