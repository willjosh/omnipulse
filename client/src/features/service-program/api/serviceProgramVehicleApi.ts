import { agent } from "@/lib/axios/agent";
import {
  ServiceProgramVehicle,
  AddVehicleToServiceProgramCommand,
  RemoveVehicleFromServiceProgramCommand,
  RemoveVehicleFromServiceProgramResponse,
  ServiceProgramVehicleFilter,
} from "../types/serviceProgramVehicleType";
import { VehicleWithLabels } from "@/features/vehicle/types/vehicleType";
import {
  FuelTypeEnum,
  VehicleStatusEnum,
  VehicleTypeEnum,
} from "../../vehicle/types/vehicleEnum";

export interface ServiceProgramVehicleWithDetails
  extends ServiceProgramVehicle {
  vehicle: VehicleWithLabels;
}

export const serviceProgramVehicleApi = {
  getServiceProgramVehicles: async (
    serviceProgramId: number,
    filter: ServiceProgramVehicleFilter = {},
  ) => {
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
    // Get the service program vehicles
    const { data: serviceProgramVehicles } = await agent.get<{
      items: ServiceProgramVehicle[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>(
      `/api/ServicePrograms/${serviceProgramId}/vehicles${queryString ? `?${queryString}` : ""}`,
    );
    // Fetch full vehicle details for each service program vehicle
    const enhancedVehicles = await Promise.all(
      serviceProgramVehicles.items.map(async spVehicle => {
        try {
          const { data: vehicleDetails } = await agent.get<VehicleWithLabels>(
            `/api/Vehicles/${spVehicle.vehicleID}`,
          );
          return { ...spVehicle, vehicle: vehicleDetails };
        } catch (error) {
          // Return the basic service program vehicle data if vehicle details fetch fails
          return {
            ...spVehicle,
            vehicle: {
              id: spVehicle.vehicleID,
              name: spVehicle.vehicleName,
              make: "",
              model: "",
              year: 0,
              vin: "",
              licensePlate: "",
              licensePlateExpirationDate: "",
              vehicleType: VehicleTypeEnum.OTHER,
              vehicleTypeLabel: "Other",
              vehicleTypeEnum: VehicleTypeEnum.OTHER,
              vehicleGroupID: 0,
              vehicleGroupName: "",
              assignedTechnicianName: "",
              assignedTechnicianID: null,
              trim: "",
              mileage: 0,
              fuelCapacity: 0,
              fuelType: FuelTypeEnum.OTHER,
              fuelTypeLabel: "Other",
              fuelTypeEnum: FuelTypeEnum.OTHER,
              purchaseDate: "",
              purchasePrice: 0,
              status: VehicleStatusEnum.INACTIVE,
              statusLabel: "Inactive",
              statusEnum: VehicleStatusEnum.INACTIVE,
              location: "",
            } as VehicleWithLabels,
          };
        }
      }),
    );
    return { ...serviceProgramVehicles, items: enhancedVehicles };
  },

  addVehicleToServiceProgram: async (
    command: AddVehicleToServiceProgramCommand,
  ) => {
    const { data } = await agent.post(
      `/api/ServicePrograms/${command.serviceProgramID}/vehicles`,
      command,
    );
    return data;
  },

  removeVehicleFromServiceProgram: async (
    command: RemoveVehicleFromServiceProgramCommand,
  ) => {
    const { data } =
      await agent.delete<RemoveVehicleFromServiceProgramResponse>(
        `/api/ServicePrograms/${command.serviceProgramID}/vehicles/${command.vehicleID}`,
      );
    return data;
  },
};
