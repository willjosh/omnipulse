import {
  ServiceTask,
  ServiceTaskWithLabels,
} from "../../service-task/types/serviceTaskType";
import { TimeUnitEnum, ServiceScheduleTypeEnum } from "./serviceScheduleEnum";

export interface ServiceSchedule {
  serviceTasks: ServiceTask[];
  id: number;
  serviceProgramID: number;
  name: string;
  timeIntervalValue?: number | null;
  timeIntervalUnit?: TimeUnitEnum | null;
  timeBufferValue?: number | null;
  timeBufferUnit?: TimeUnitEnum | null;
  mileageInterval?: number | null;
  mileageBuffer?: number | null;
  firstServiceDate?: string | null;
  firstServiceMileage?: number | null;
  scheduleType: ServiceScheduleTypeEnum;
}

export interface ServiceScheduleWithLabels
  extends Omit<
    ServiceSchedule,
    "serviceTasks" | "timeIntervalUnit" | "timeBufferUnit" | "scheduleType"
  > {
  serviceTasks: ServiceTaskWithLabels[];
  timeIntervalUnit?: number | null;
  timeIntervalUnitLabel?: string | null;
  timeIntervalUnitEnum?: TimeUnitEnum | null;
  timeBufferUnit?: number | null;
  timeBufferUnitLabel?: string | null;
  timeBufferUnitEnum?: TimeUnitEnum | null;
  scheduleType: number;
  scheduleTypeLabel: string;
  scheduleTypeEnum: ServiceScheduleTypeEnum;
}

export interface CreateServiceScheduleCommand {
  serviceProgramID: number;
  name: string;
  serviceTaskIDs: number[];
  timeIntervalValue?: number | null;
  timeIntervalUnit?: TimeUnitEnum | null;
  timeBufferValue?: number | null;
  timeBufferUnit?: TimeUnitEnum | null;
  mileageInterval?: number | null;
  mileageBuffer?: number | null;
  firstServiceDate?: string | null;
  firstServiceMileage?: number | null;
}

export interface UpdateServiceScheduleCommand
  extends CreateServiceScheduleCommand {
  serviceScheduleID: number;
}

export interface ServiceScheduleFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
}
