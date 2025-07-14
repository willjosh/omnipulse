import { ServiceTaskCategoryEnum } from "@/app/_hooks/service-task/serviceTaskEnum";

export function getServiceTaskCategoryLabel(
  category: ServiceTaskCategoryEnum,
): string {
  switch (category) {
    case ServiceTaskCategoryEnum.PREVENTIVE:
      return "Preventive";
    case ServiceTaskCategoryEnum.CORRECTIVE:
      return "Corrective";
    case ServiceTaskCategoryEnum.EMERGENCY:
      return "Emergency";
    case ServiceTaskCategoryEnum.INSPECTION:
      return "Inspection";
    case ServiceTaskCategoryEnum.WARRANTY:
      return "Warranty";
    default:
      return "Unknown";
  }
}
