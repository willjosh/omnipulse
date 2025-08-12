import {
  TimeUnitEnum,
  ServiceScheduleTypeEnum,
} from "@/features/service-schedule/types/serviceScheduleEnum";

export function getTimeUnitEnumLabel(
  unit: TimeUnitEnum | null | undefined,
): string {
  if (unit === null || unit === undefined) return "N/A";
  switch (unit) {
    case TimeUnitEnum.Hours:
      return "Hours";
    case TimeUnitEnum.Days:
      return "Days";
    case TimeUnitEnum.Weeks:
      return "Weeks";
    default:
      return "Unknown";
  }
}

export function getServiceScheduleTypeEnumLabel(
  scheduleType: ServiceScheduleTypeEnum | null | undefined,
): string {
  if (scheduleType === null || scheduleType === undefined) return "N/A";
  switch (scheduleType) {
    case ServiceScheduleTypeEnum.TIME:
      return "Time-based";
    case ServiceScheduleTypeEnum.MILEAGE:
      return "Mileage-based";
    default:
      return "Unknown";
  }
}
