import { agent } from "@/lib/axios/agent";
import {
  Vehicle,
  VehicleWithLabels,
  VehicleFilter,
  CreateVehicleCommand,
  UpdateVehicleCommand,
} from "../types/vehicleType";
import {
  VehicleTypeEnum,
  VehicleStatusEnum,
  FuelTypeEnum,
} from "../types/vehicleEnum";
import {
  getVehicleTypeLabel,
  getStatusLabel,
  getFuelTypeLabel,
} from "@/features/vehicle/utils/vehicleEnumHelper";

export const convertVehicleData = (vehicle: Vehicle): VehicleWithLabels => ({
  ...vehicle,
  vehicleType: vehicle.vehicleType as number,
  vehicleTypeLabel: getVehicleTypeLabel(vehicle.vehicleType),
  vehicleTypeEnum: vehicle.vehicleType as VehicleTypeEnum,
  status: vehicle.status as number,
  statusLabel: getStatusLabel(vehicle.status),
  statusEnum: vehicle.status as VehicleStatusEnum,
  fuelType: vehicle.fuelType as number,
  fuelTypeLabel: getFuelTypeLabel(vehicle.fuelType),
  fuelTypeEnum: vehicle.fuelType as FuelTypeEnum,
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
};
