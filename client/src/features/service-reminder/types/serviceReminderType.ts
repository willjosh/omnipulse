import {
  ServiceReminderStatusEnum,
  PriorityLevelEnum,
  TimeUnitEnum,
  ServiceTaskCategoryEnum,
  ServiceScheduleTypeEnum,
} from "./serviceReminderEnum";

export interface ServiceTaskInfo {
  serviceTaskID: number;
  serviceTaskName: string;
  serviceTaskCategory: ServiceTaskCategoryEnum;
  estimatedLabourHours: number;
  estimatedCost: number;
  description?: string | null;
  isRequired: boolean;
}

export interface ServiceTaskInfoWithLabels
  extends Omit<ServiceTaskInfo, "serviceTaskCategory"> {
  serviceTaskCategory: number;
  serviceTaskCategoryLabel: string;
  serviceTaskCategoryEnum: ServiceTaskCategoryEnum;
}

export interface ServiceReminder {
  id: number;
  workOrderID?: number | null;
  vehicleID: number;
  vehicleName: string;
  serviceProgramID?: number | null;
  serviceProgramName?: string | null;
  serviceScheduleID: number;
  serviceScheduleName: string;
  serviceTasks: ServiceTaskInfo[];
  totalEstimatedLabourHours: number;
  totalEstimatedCost: number;
  taskCount: number;
  dueDate?: string | null;
  dueMileage?: number | null;
  status: ServiceReminderStatusEnum;
  priorityLevel: PriorityLevelEnum;
  timeIntervalValue?: number | null;
  timeIntervalUnit?: TimeUnitEnum | null;
  mileageInterval?: number | null;
  timeBufferValue?: number | null;
  timeBufferUnit?: TimeUnitEnum | null;
  mileageBuffer?: number | null;
  currentMileage: number;
  mileageVariance?: number | null;
  daysUntilDue?: number | null;
  occurrenceNumber: number;
  scheduleType: ServiceScheduleTypeEnum;
}

export interface ServiceReminderWithLabels
  extends Omit<
    ServiceReminder,
    | "serviceTasks"
    | "status"
    | "priorityLevel"
    | "timeIntervalUnit"
    | "timeBufferUnit"
    | "scheduleType"
  > {
  serviceTasks: ServiceTaskInfoWithLabels[];
  status: number;
  statusLabel: string;
  statusEnum: ServiceReminderStatusEnum;
  priorityLevel: number;
  priorityLevelLabel: string;
  priorityLevelEnum: PriorityLevelEnum;
  timeIntervalUnit: number;
  timeIntervalUnitLabel: string;
  timeIntervalUnitEnum: TimeUnitEnum;
  timeBufferUnit: number;
  timeBufferUnitLabel: string;
  timeBufferUnitEnum: TimeUnitEnum;
  scheduleType: number;
  scheduleTypeLabel: string;
  scheduleTypeEnum: ServiceScheduleTypeEnum;
}

export interface ServiceReminderFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
}
