import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { agent } from "@/app/_lib/axios/agent";
import {
  ServiceProgramVehicle,
  AddVehicleToServiceProgramCommand,
  RemoveVehicleFromServiceProgramCommand,
  RemoveVehicleFromServiceProgramResponse,
  ServiceProgramVehicleFilter,
} from "./serviceProgramVehicleType";
import { PagedResponse } from "@/app/_hooks/shared_types/pagedResponse";
import { useDebounce } from "@/app/_hooks/shared_types/useDebounce";
import { VehicleWithLabels } from "@/app/_hooks/vehicle/vehicleType";
import {
  FuelTypeEnum,
  VehicleStatusEnum,
  VehicleTypeEnum,
} from "../vehicle/vehicleEnum";

// Enhanced type that combines service program vehicle data with full vehicle details
export interface ServiceProgramVehicleWithDetails
  extends ServiceProgramVehicle {
  vehicle: VehicleWithLabels;
}

export function useServiceProgramVehicles(
  serviceProgramId: number,
  filter: ServiceProgramVehicleFilter = {},
) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const queryParams = new URLSearchParams();
  if (debouncedFilter.PageNumber)
    queryParams.append("PageNumber", debouncedFilter.PageNumber.toString());
  if (debouncedFilter.PageSize)
    queryParams.append("PageSize", debouncedFilter.PageSize.toString());
  if (debouncedFilter.SortBy)
    queryParams.append("SortBy", debouncedFilter.SortBy);
  if (debouncedFilter.SortDescending !== undefined)
    queryParams.append(
      "SortDescending",
      debouncedFilter.SortDescending.toString(),
    );
  if (debouncedFilter.Search)
    queryParams.append("Search", debouncedFilter.Search);

  const queryString = queryParams.toString();

  const { data, isPending, isError, isSuccess, error } = useQuery<
    PagedResponse<ServiceProgramVehicleWithDetails>
  >({
    queryKey: ["serviceProgramVehicles", serviceProgramId, debouncedFilter],
    queryFn: async () => {
      // First, get the service program vehicles
      const { data: serviceProgramVehicles } = await agent.get<
        PagedResponse<ServiceProgramVehicle>
      >(
        `/api/ServicePrograms/${serviceProgramId}/vehicles${queryString ? `?${queryString}` : ""}`,
      );

      // Then, fetch full vehicle details for each service program vehicle
      const enhancedVehicles = await Promise.all(
        serviceProgramVehicles.items.map(async spVehicle => {
          try {
            const { data: vehicleDetails } = await agent.get<VehicleWithLabels>(
              `/api/Vehicles/${spVehicle.vehicleID}`,
            );

            return { ...spVehicle, vehicle: vehicleDetails };
          } catch (error) {
            console.error(
              `Failed to fetch vehicle details for ID ${spVehicle.vehicleID}:`,
              error,
            );
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
                engineHours: 0,
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
    enabled: !!serviceProgramId,
  });

  return {
    serviceProgramVehicles: data?.items ?? [],
    pagination: data
      ? {
          totalCount: data.totalCount,
          pageNumber: data.pageNumber,
          pageSize: data.pageSize,
          totalPages: data.totalPages,
          hasPreviousPage: data.hasPreviousPage,
          hasNextPage: data.hasNextPage,
        }
      : null,
    isPending,
    isError,
    isSuccess,
    error,
  };
}

export function useAddVehicleToServiceProgram() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: AddVehicleToServiceProgramCommand) => {
      const { data } = await agent.post(
        `/api/ServicePrograms/${command.serviceProgramID}/vehicles`,
        command,
      );
      return data;
    },
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({
        queryKey: ["serviceProgramVehicles", variables.serviceProgramID],
      });
      await queryClient.invalidateQueries({
        queryKey: ["serviceProgram", variables.serviceProgramID],
      });
    },
  });
}

export function useRemoveVehicleFromServiceProgram() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: RemoveVehicleFromServiceProgramCommand) => {
      const { data } =
        await agent.delete<RemoveVehicleFromServiceProgramResponse>(
          `/api/ServicePrograms/${command.serviceProgramID}/vehicles/${command.vehicleID}`,
        );
      return data;
    },
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({
        queryKey: ["serviceProgramVehicles", variables.serviceProgramID],
      });
      await queryClient.invalidateQueries({
        queryKey: ["serviceProgram", variables.serviceProgramID],
      });
    },
  });
}
