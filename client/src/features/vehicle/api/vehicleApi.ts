import { agent } from "@/lib/axios/agent";
import {
  Vehicle,
  VehicleWithLabels,
  VehicleFilter,
  CreateVehicleCommand,
  UpdateVehicleCommand,
  VehicleAssignedData,
  VehicleStatusData,
} from "../types/vehicleType";
import {
  getVehicleTypeLabel,
  getStatusLabel,
  getFuelTypeLabel,
} from "../utils/vehicleEnumHelper";
import { VehicleGroup } from "@/features/vehicle-group/types/vehicleGroupType";
import { Technician } from "@/features/technician/types/technicianType";

export const convertVehicleData = (vehicle: Vehicle): VehicleWithLabels => ({
  ...vehicle,
  vehicleType: vehicle.vehicleType as number,
  vehicleTypeLabel: getVehicleTypeLabel(vehicle.vehicleType),
  status: vehicle.status as number,
  statusLabel: getStatusLabel(vehicle.status),
  fuelType: vehicle.fuelType as number,
  fuelTypeLabel: getFuelTypeLabel(vehicle.fuelType),
  vehicleTypeEnum: vehicle.vehicleType,
  statusEnum: vehicle.status,
  fuelTypeEnum: vehicle.fuelType,
});

export const vehicleApi = {
  getVehicles: async (filter: VehicleFilter = {}) => {
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
      items: Vehicle[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>(`/api/Vehicles${queryString ? `?${queryString}` : ""}`);
    return data;
  },

  getVehicle: async (id: string) => {
    const { data } = await agent.get<Vehicle>(`/api/Vehicles/${id}`);
    return data;
  },

  getVehicleAssignedData: async () => {
    const { data } = await agent.get<VehicleAssignedData>(
      `/api/Vehicles/assigned-data`,
    );
    return data;
  },

  getVehicleStatusData: async (): Promise<VehicleStatusData> => {
    const { data } = await agent.get<VehicleStatusData>(
      `/api/Vehicles/status-data`,
    );
    return data;
  },

  createVehicle: async (command: CreateVehicleCommand) => {
    const { data } = await agent.post("/api/Vehicles", command);
    return data;
  },

  updateVehicle: async (command: UpdateVehicleCommand) => {
    const { data } = await agent.put(
      `/api/Vehicles/${command.vehicleID}`,
      command,
    );
    return data;
  },

  deactivateVehicle: async (id: string) => {
    const { data } = await agent.patch(`/api/Vehicles/${id}/deactivate`);
    return data;
  },

  getVehicleGroups: async () => {
    const { data } = await agent.get<{
      items: VehicleGroup[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>("/api/vehiclegroups?PageSize=100");
    return data.items.filter(group => group.isActive);
  },

  getTechnicians: async () => {
    const { data } = await agent.get<{
      items: Technician[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>("/api/technicians?PageSize=100");
    return data.items.filter(tech => tech.isActive);
  },
};
