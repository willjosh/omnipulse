import { InspectionFormItemTypeEnum } from "../types/inspectionFormEnum";

export const getInspectionFormItemTypeLabel = (
  type: InspectionFormItemTypeEnum | number,
): string => {
  switch (type) {
    case InspectionFormItemTypeEnum.PassFail:
      return "Pass/Fail";
    default:
      return "Unknown";
  }
};
