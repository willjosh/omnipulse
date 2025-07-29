import { agent } from "@/lib/axios/agent";
import {
  ServiceSchedule,
  ServiceScheduleFilter,
  CreateServiceScheduleCommand,
  UpdateServiceScheduleCommand,
} from "../types/serviceScheduleType";

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
