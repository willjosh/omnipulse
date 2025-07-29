import {
  ServiceTask,
  ServiceTaskWithLabels,
} from "../../service-task/types/serviceTaskType";
import { TimeUnitEnum } from "./serviceScheduleEnum";

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
  firstServiceTimeValue?: number | null;
  firstServiceTimeUnit?: TimeUnitEnum | null;
  firstServiceMileage?: number | null;
  isActive: boolean;
}

export interface ServiceScheduleWithLabels
  extends Omit<
    ServiceSchedule,
    | "serviceTasks"
    | "timeIntervalUnit"
    | "timeBufferUnit"
    | "firstServiceTimeUnit"
  > {
  serviceTasks: ServiceTaskWithLabels[];
  timeIntervalUnit?: number | null;
  timeIntervalUnitLabel?: string | null;
  timeIntervalUnitEnum?: TimeUnitEnum | null;
  timeBufferUnit?: number | null;
  timeBufferUnitLabel?: string | null;
  timeBufferUnitEnum?: TimeUnitEnum | null;
  firstServiceTimeUnit?: number | null;
  firstServiceTimeUnitLabel?: string | null;
  firstServiceTimeUnitEnum?: TimeUnitEnum | null;
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
  firstServiceTimeValue?: number | null;
  firstServiceTimeUnit?: TimeUnitEnum | null;
  firstServiceMileage?: number | null;
  isActive: boolean;
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
