import { agent } from "@/lib/axios/agent";
import {
  ServiceTask,
  ServiceTaskFilter,
  CreateServiceTaskCommand,
  UpdateServiceTaskCommand,
} from "../types/serviceTaskType";

export const serviceTaskApi = {
  getServiceTasks: async (filter: ServiceTaskFilter = {}) => {
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
      items: ServiceTask[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>(`/api/ServiceTasks${queryString ? `?${queryString}` : ""}`);
    return data;
  },

  getServiceTask: async (id: number) => {
    const { data } = await agent.get<ServiceTask>(`/api/ServiceTasks/${id}`);
    return data;
  },

  createServiceTask: async (command: CreateServiceTaskCommand) => {
    const { data } = await agent.post("/api/ServiceTasks", command);
    return data;
  },

  updateServiceTask: async (command: UpdateServiceTaskCommand) => {
    const { data } = await agent.put(
      `/api/ServiceTasks/${command.ServiceTaskID}`,
      command,
    );
    return data;
  },

  deleteServiceTask: async (id: number) => {
    const { data } = await agent.delete(`/api/ServiceTasks/${id}`);
    return data;
  },
};
