import { agent } from "@/lib/axios/agent";
import {
  ServiceSchedule,
  ServiceScheduleWithLabels,
  ServiceScheduleFilter,
  CreateServiceScheduleCommand,
  UpdateServiceScheduleCommand,
} from "../types/serviceScheduleType";
import { TimeUnitEnum } from "../types/serviceScheduleEnum";
import { getTimeUnitEnumLabel } from "../utils/serviceScheduleEnumHelper";
import { convertServiceTaskData } from "../../service-task/hooks/useServiceTasks";

function formatDate(date?: string | null): string | null {
  if (!date) return null;
  const d = new Date(date);
  return isNaN(d.getTime()) ? null : d.toLocaleString();
}

export const convertServiceScheduleData = (
  schedule: ServiceSchedule,
): ServiceScheduleWithLabels => ({
  ...schedule,
  firstServiceDate: formatDate(schedule.firstServiceDate),
  serviceTasks: schedule.serviceTasks.map(convertServiceTaskData),
  timeIntervalUnit: schedule.timeIntervalUnit as number,
  timeIntervalUnitLabel: getTimeUnitEnumLabel(schedule.timeIntervalUnit),
  timeIntervalUnitEnum: schedule.timeIntervalUnit as TimeUnitEnum,
  timeBufferUnit: schedule.timeBufferUnit as number,
  timeBufferUnitLabel: getTimeUnitEnumLabel(schedule.timeBufferUnit),
  timeBufferUnitEnum: schedule.timeBufferUnit as TimeUnitEnum,
});

export const serviceScheduleApi = {
  getServiceSchedules: async (filter: ServiceScheduleFilter = {}) => {
    const queryParams = new URLSearchParams();
    if (filter.PageNumber)
      queryParams.append("PageNumber", filter.PageNumber.toString());
    if (filter.PageSize)
      queryParams.append("PageSize", filter.PageSize.toString());
    if (filter.Search) queryParams.append("Search", filter.Search);
    if (filter.SortBy) queryParams.append("SortBy", filter.SortBy);
    if (filter.SortDescending !== undefined)
      queryParams.append("SortDescending", filter.SortDescending.toString());
    const queryString = queryParams.toString();
    const { data } = await agent.get<{
      items: ServiceSchedule[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>(`/api/ServiceSchedules${queryString ? `?${queryString}` : ""}`);
    return data;
  },

  getServiceSchedule: async (id: number) => {
    const { data } = await agent.get<ServiceSchedule>(
      `/api/ServiceSchedules/${id}`,
    );
    return data;
  },

  createServiceSchedule: async (command: CreateServiceScheduleCommand) => {
    const { data } = await agent.post("/api/ServiceSchedules", command);
    return data;
  },

  updateServiceSchedule: async (command: UpdateServiceScheduleCommand) => {
    const { data } = await agent.put(
      `/api/ServiceSchedules/${command.serviceScheduleID}`,
      command,
    );
    return data;
  },

  deleteServiceSchedule: async (id: number) => {
    const { data } = await agent.delete(`/api/ServiceSchedules/${id}`);
    return data;
  },
};
