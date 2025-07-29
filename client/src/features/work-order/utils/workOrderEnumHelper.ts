import {
  LineItemTypeEnum,
  WorkTypeEnum,
  PriorityLevelEnum,
  WorkOrderStatusEnum,
} from "../types/workOrderEnum";

export const getLineItemTypeLabel = (type: LineItemTypeEnum) => {
  switch (type) {
    case LineItemTypeEnum.LABOR:
      return "Labor";
    case LineItemTypeEnum.ITEM:
      return "Item";
    case LineItemTypeEnum.BOTH:
      return "Both";
    default:
      return "Unknown";
  }
};

export const getWorkTypeLabel = (type: WorkTypeEnum) => {
  switch (type) {
    case WorkTypeEnum.SCHEDULED:
      return "Scheduled";
    case WorkTypeEnum.UNSCHEDULED:
      return "Unscheduled";
    default:
      return "Unknown";
  }
};

export const getPriorityLevelLabel = (priority: PriorityLevelEnum) => {
  switch (priority) {
    case PriorityLevelEnum.LOW:
      return "Low";
    case PriorityLevelEnum.MEDIUM:
      return "Medium";
    case PriorityLevelEnum.HIGH:
      return "High";
    case PriorityLevelEnum.CRITICAL:
      return "Critical";
    default:
      return "Unknown";
  }
};

export const getWorkOrderStatusLabel = (status: WorkOrderStatusEnum) => {
  switch (status) {
    case WorkOrderStatusEnum.CREATED:
      return "Created";
    case WorkOrderStatusEnum.ASSIGNED:
      return "Assigned";
    case WorkOrderStatusEnum.IN_PROGRESS:
      return "In Progress";
    case WorkOrderStatusEnum.WAITING_PARTS:
      return "Waiting for Parts";
    case WorkOrderStatusEnum.COMPLETED:
      return "Completed";
    case WorkOrderStatusEnum.CANCELLED:
      return "Cancelled";
    case WorkOrderStatusEnum.ON_HOLD:
      return "On Hold";
    default:
      return "Unknown";
  }
};
