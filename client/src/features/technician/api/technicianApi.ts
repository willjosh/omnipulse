import { agent } from "@/lib/axios/agent";
import {
  Technician,
  TechnicianFilter,
  CreateTechnicianCommand,
  UpdateTechnicianCommand,
} from "../types/technicianType";

export const technicianApi = {
  getTechnicians: async (filter: TechnicianFilter = {}) => {
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
      items: Technician[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>(`/api/Technicians${queryString ? `?${queryString}` : ""}`);
    return data;
  },

  getTechnician: async (id: string) => {
    const { data } = await agent.get<Technician>(`/api/Technicians/${id}`);
    return data;
  },

  createTechnician: async (command: CreateTechnicianCommand) => {
    const { data } = await agent.post("/api/Technicians", command);
    return data;
  },

  updateTechnician: async (command: UpdateTechnicianCommand) => {
    const { data } = await agent.put(`/api/Technicians/${command.id}`, command);
    return data;
  },

  deactivateTechnician: async (id: string) => {
    const { data } = await agent.patch(`/api/Technicians/${id}/deactivate`);
    return data;
  },
};
