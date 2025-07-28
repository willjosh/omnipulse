import { agent } from "@/lib/axios/agent";
import {
  VehicleGroup,
  CreateVehicleGroupCommand,
  UpdateVehicleGroupCommand,
  VehicleGroupFilter,
} from "../types/vehicleGroupType";

export const vehicleGroupApi = {
  getVehicleGroups: async (filter: VehicleGroupFilter = {}) => {
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
      items: VehicleGroup[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>(`/api/VehicleGroups${queryString ? `?${queryString}` : ""}`);
    return data;
  },

  getVehicleGroup: async (id: number) => {
    const { data } = await agent.get<VehicleGroup>(`/api/VehicleGroups/${id}`);
    return data;
  },

  createVehicleGroup: async (command: CreateVehicleGroupCommand) => {
    const { data } = await agent.post("/api/VehicleGroups", command);
    return data;
  },

  updateVehicleGroup: async (command: UpdateVehicleGroupCommand) => {
    const { data } = await agent.put(
      `/api/VehicleGroups/${command.vehicleGroupId}`,
      command,
    );
    return data;
  },

  deleteVehicleGroup: async (id: number) => {
    const { data } = await agent.delete(`/api/VehicleGroups/${id}`);
    return data;
  },
};
