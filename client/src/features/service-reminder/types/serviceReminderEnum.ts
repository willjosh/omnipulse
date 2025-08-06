export enum ServiceReminderStatusEnum {
  UPCOMING = 1,
  DUE_SOON = 2,
  OVERDUE = 3,
  COMPLETED = 4,
  CANCELLED = 5,
}

export enum PriorityLevelEnum {
  LOW = 1,
  MEDIUM = 2,
  HIGH = 3,
  CRITICAL = 4,
}

export enum TimeUnitEnum {
  Hours = 1,
  Days = 2,
  Weeks = 3,
}

export enum ServiceTaskCategoryEnum {
  PREVENTIVE = 1,
  CORRECTIVE = 2,
  EMERGENCY = 3,
  INSPECTION = 4,
  WARRANTY = 5,
}

export enum ServiceScheduleTypeEnum {
  TIME = 1,
  MILEAGE = 2,
}
