import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  CreateVehicleCommand,
  UpdateVehicleCommand,
  Vehicle,
  VehicleFilter,
  VehicleWithLabels,
} from "./vehicleType";
import { agent } from "@/app/_lib/axios/agent";
import { PagedResponse } from "../shared_types/pagedResponse";
import { useDebounce } from "../shared_types/useDebounce";
import {
  getVehicleTypeLabel,
  getStatusLabel,
  getFuelTypeLabel,
} from "@/app/_utils/vehicleEnumHelper";
import {
  VehicleTypeEnum,
  VehicleStatusEnum,
  FuelTypeEnum,
} from "@/app/_hooks/vehicle/vehicleEnum";

// Helper function to convert vehicle data
const convertVehicleData = (vehicle: Vehicle): VehicleWithLabels => ({
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

export const useVehicles = (filters?: VehicleFilter, id?: string) => {
  const queryClient = useQueryClient();
  const queryParams = new URLSearchParams();
  const debouncedSearch = useDebounce(filters?.Search || "", 300);

  const debouncedFilters = { ...filters, Search: debouncedSearch };

  // Build query params...
  if (debouncedFilters?.PageNumber)
    queryParams.append("PageNumber", debouncedFilters.PageNumber.toString());
  if (debouncedFilters?.PageSize)
    queryParams.append("PageSize", debouncedFilters.PageSize.toString());
  if (debouncedFilters?.SortBy)
    queryParams.append("SortBy", debouncedFilters.SortBy);
  if (debouncedFilters?.SortDescending !== undefined)
    queryParams.append(
      "SortDescending",
      debouncedFilters.SortDescending.toString(),
    );
  if (debouncedFilters?.Search)
    queryParams.append("Search", debouncedFilters.Search);

  // Fetch vehicles with conversion
  const { data: vehiclesResponse, isLoading: isLoadingVehicles } = useQuery<
    PagedResponse<VehicleWithLabels>
  >({
    queryKey: ["vehicles", debouncedFilters],
    queryFn: async () => {
      const url = queryParams.toString()
        ? `/api/Vehicles?${queryParams.toString()}`
        : "/api/Vehicles";
      const response = await agent.get<PagedResponse<Vehicle>>(url);

      // Convert the vehicles data
      const convertedItems = response.data.items.map(convertVehicleData);

      return { ...response.data, items: convertedItems };
    },
  });

  // Fetch single vehicle with conversion
  const { data: vehicle, isLoading: isLoadingVehicle } =
    useQuery<VehicleWithLabels>({
      queryKey: ["vehicle", id],
      queryFn: async () => {
        const response = await agent.get<Vehicle>(`/api/Vehicles/${id}`);
        return convertVehicleData(response.data);
      },
      enabled: !!id,
    });

  // Function to create vehicles.
  const createVehicleMutation = useMutation({
    mutationFn: async (createVehicleCommand: CreateVehicleCommand) => {
      const response = await agent.post("/api/Vehicles", createVehicleCommand);
      return response.data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["vehicles"] });
    },
  });

  // Function to update vehicles.
  const updateVehicleMutation = useMutation({
    mutationFn: async (updateVehicleCommand: UpdateVehicleCommand) => {
      const response = await agent.put(
        `/api/Vehicles/${updateVehicleCommand.vehicleID}`,
        updateVehicleCommand,
      );
      return response.data;
    },
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["vehicles"] });
      await queryClient.invalidateQueries({
        queryKey: ["vehicle", variables.vehicleID],
      });
    },
  });

  const deactivateVehicleMutation = useMutation({
    mutationFn: async (id: string) => {
      const response = await agent.patch(`/api/Vehicles/${id}/deactivate`);
      return response.data;
    },
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["vehicles"] });
      await queryClient.invalidateQueries({ queryKey: ["vehicle", id] });
    },
  });

  return {
    vehiclesResponse,
    vehicles: vehiclesResponse?.items || [],
    pagination: vehiclesResponse?.items
      ? {
          totalCount: vehiclesResponse.totalCount,
          pageNumber: vehiclesResponse.pageNumber,
          pageSize: vehiclesResponse.pageSize,
          totalPages: vehiclesResponse.totalPages,
          hasPreviousPage: vehiclesResponse.hasPreviousPage,
          hasNextPage: vehiclesResponse.hasNextPage,
        }
      : null,
    vehicle,
    isLoadingVehicles,
    isLoadingVehicle,
    createVehicleMutation,
    updateVehicleMutation,
    deactivateVehicleMutation,
  };
};
