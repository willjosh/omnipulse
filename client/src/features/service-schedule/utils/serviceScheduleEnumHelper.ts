import { TimeUnitEnum } from "@/features/service-schedule/types/serviceScheduleEnum";

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
