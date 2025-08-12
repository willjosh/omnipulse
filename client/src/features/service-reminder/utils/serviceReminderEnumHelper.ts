import {
  ServiceReminderStatusEnum,
  TimeUnitEnum,
  ServiceTaskCategoryEnum,
  ServiceScheduleTypeEnum,
} from "../types/serviceReminderEnum";

export const getServiceReminderStatusLabel = (
  status: ServiceReminderStatusEnum | number,
): string => {
  switch (status) {
    case ServiceReminderStatusEnum.UPCOMING:
      return "Upcoming";
    case ServiceReminderStatusEnum.DUE_SOON:
      return "Due Soon";
    case ServiceReminderStatusEnum.OVERDUE:
      return "Overdue";
    case ServiceReminderStatusEnum.COMPLETED:
      return "Completed";
    case ServiceReminderStatusEnum.CANCELLED:
      return "Cancelled";
    default:
      return "Unknown";
  }
};

export const getTimeUnitLabel = (
  timeUnit: TimeUnitEnum | null | undefined,
): string => {
  if (!timeUnit) return "Unknown";
  switch (timeUnit) {
    case TimeUnitEnum.Hours:
      return "Hours";
    case TimeUnitEnum.Days:
      return "Days";
    case TimeUnitEnum.Weeks:
      return "Weeks";
    default:
      return "Unknown";
  }
};

export const getServiceTaskCategoryLabel = (
  category: ServiceTaskCategoryEnum | number,
): string => {
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
};

export const getServiceScheduleTypeLabel = (
  scheduleType: ServiceScheduleTypeEnum | number,
): string => {
  switch (scheduleType) {
    case ServiceScheduleTypeEnum.TIME:
      return "Time";
    case ServiceScheduleTypeEnum.MILEAGE:
      return "Mileage";
    default:
      return "Unknown";
  }
};
