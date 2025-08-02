export enum LineItemTypeEnum {
  LABOR = 1,
  ITEM = 2,
  BOTH = 3,
}

export enum WorkTypeEnum {
  SCHEDULED = 1,
  UNSCHEDULED = 2,
}

export enum PriorityLevelEnum {
  LOW = 1,
  MEDIUM = 2,
  HIGH = 3,
  CRITICAL = 4,
}

export enum WorkOrderStatusEnum {
  CREATED = 1,
  ASSIGNED = 2,
  IN_PROGRESS = 3,
  WAITING_PARTS = 4,
  COMPLETED = 5,
  CANCELLED = 6,
  ON_HOLD = 7,
}
