import { agent } from "@/lib/axios/agent";
import {
  ServiceProgram,
  CreateServiceProgramCommand,
  UpdateServiceProgramCommand,
  ServiceProgramFilter,
} from "../types/serviceProgramType";

export const serviceProgramApi = {
  getServicePrograms: async (filter: ServiceProgramFilter = {}) => {
    const queryParams = new URLSearchParams();
    if (filter.PageNumber)
      queryParams.append("PageNumber", filter.PageNumber.toString());
    if (filter.PageSize)
      queryParams.append("PageSize", filter.PageSize.toString());
    if (filter.SortBy) queryParams.append("SortBy", filter.SortBy);
    if (filter.SortDescending !== undefined)
      queryParams.append("SortDescending", filter.SortDescending.toString());
    if (filter.Search) queryParams.append("Search", filter.Search);
    const queryString = queryParams.toString();
    const { data } = await agent.get<{
      items: ServiceProgram[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>(`/api/ServicePrograms${queryString ? `?${queryString}` : ""}`);
    return data;
  },

  getServiceProgram: async (id: number) => {
    const { data } = await agent.get<ServiceProgram>(
      `/api/ServicePrograms/${id}`,
    );
    return data;
  },

  createServiceProgram: async (command: CreateServiceProgramCommand) => {
    const { data } = await agent.post("/api/ServicePrograms", command);
    return data;
  },

  updateServiceProgram: async (command: UpdateServiceProgramCommand) => {
    const { data } = await agent.put(
      `/api/ServicePrograms/${command.serviceProgramID}`,
      command,
    );
    return data;
  },

  deleteServiceProgram: async (id: number) => {
    const { data } = await agent.delete(`/api/ServicePrograms/${id}`);
    return data;
  },
};
