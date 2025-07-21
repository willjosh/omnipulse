import {
  ServiceTask,
  ServiceTaskWithLabels,
} from "../service-task/serviceTaskType";
import { TimeUnitEnum } from "./serviceScheduleEnum";

export interface ServiceSchedule {
  id: number;
  ServiceTasks: ServiceTask[];
  ServiceProgramID: number;
  Name: string;
  TimeIntervalValue?: number | null;
  TimeIntervalUnit?: TimeUnitEnum | null;
  TimeBufferValue?: number | null;
  TimeBufferUnit?: TimeUnitEnum | null;
  MileageInterval?: number | null;
  MileageBuffer?: number | null;
  FirstServiceTimeValue?: number | null;
  FirstServiceTimeUnit?: TimeUnitEnum | null;
  FirstServiceMileage?: number | null;
  IsActive: boolean;
}

export interface ServiceScheduleWithLabels
  extends Omit<
    ServiceSchedule,
    | "ServiceTasks"
    | "TimeIntervalUnit"
    | "TimeBufferUnit"
    | "FirstServiceTimeUnit"
  > {
  ServiceTasks: ServiceTaskWithLabels[];
  TimeIntervalUnit?: number | null;
  TimeIntervalUnitLabel?: string;
  TimeIntervalUnitEnum?: TimeUnitEnum | null;
  TimeBufferUnit?: number | null;
  TimeBufferUnitLabel?: string;
  TimeBufferUnitEnum?: TimeUnitEnum | null;
  FirstServiceTimeUnit?: number | null;
  FirstServiceTimeUnitLabel?: string;
  FirstServiceTimeUnitEnum?: TimeUnitEnum | null;
}

export interface CreateServiceScheduleCommand {
  ServiceProgramID: number;
  Name: string;
  ServiceTaskIDs: number[];
  TimeIntervalValue?: number | null;
  TimeIntervalUnit?: TimeUnitEnum | null;
  TimeBufferValue?: number | null;
  TimeBufferUnit?: TimeUnitEnum | null;
  MileageInterval?: number | null;
  MileageBuffer?: number | null;
  FirstServiceTimeValue?: number | null;
  FirstServiceTimeUnit?: TimeUnitEnum | null;
  FirstServiceMileage?: number | null;
  IsActive: boolean;
}

export interface UpdateServiceScheduleCommand
  extends CreateServiceScheduleCommand {
  ServiceScheduleID: number;
}

export interface ServiceScheduleFilter {
  page?: number;
  pageSize?: number;
  search?: string;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
}
