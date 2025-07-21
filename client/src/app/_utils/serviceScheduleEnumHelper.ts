import { TimeUnitEnum } from "../_hooks/service-schedule/serviceScheduleEnum";

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
