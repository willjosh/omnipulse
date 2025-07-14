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

// Helper function to convert vehicle data
const convertVehicleData = (vehicle: Vehicle): VehicleWithLabels => ({
  ...vehicle,
  VehicleTypeLabel: getVehicleTypeLabel(vehicle.VehicleType),
  StatusLabel: getStatusLabel(vehicle.Status),
  FuelTypeLabel: getFuelTypeLabel(vehicle.FuelType),
  VehicleTypeEnum: vehicle.VehicleType,
  StatusEnum: vehicle.Status,
  FuelTypeEnum: vehicle.FuelType,
});

export const useVehicles = (filters?: VehicleFilter, id?: string) => {
  const queryClient = useQueryClient();
  const queryParams = new URLSearchParams();
  const debouncedSearch = useDebounce(filters?.search || "", 300);

  const debouncedFilters = { ...filters, search: debouncedSearch };

  // Build query params...
  if (debouncedFilters?.page)
    queryParams.append("page", debouncedFilters.page.toString());
  if (debouncedFilters?.pageSize)
    queryParams.append("pageSize", debouncedFilters.pageSize.toString());
  if (debouncedFilters?.sortBy)
    queryParams.append("sortBy", debouncedFilters.sortBy);
  if (debouncedFilters?.sortOrder)
    queryParams.append("sortOrder", debouncedFilters.sortOrder);
  if (debouncedFilters?.search)
    queryParams.append("search", debouncedFilters.search);

  // Fetch vehicles with conversion
  const { data: vehiclesResponse, isLoading: isLoadingVehicles } = useQuery<
    PagedResponse<VehicleWithLabels>
  >({
    queryKey: ["vehicles", debouncedFilters],
    queryFn: async () => {
      const url = queryParams.toString()
        ? `vehicles?${queryParams.toString()}`
        : "vehicles";
      const response = await agent.get<PagedResponse<Vehicle>>(url);

      // Convert the vehicles data
      const convertedItems = response.data.Items.map(convertVehicleData);

      return { ...response.data, Items: convertedItems };
    },
  });

  // Fetch single vehicle with conversion
  const { data: vehicle, isLoading: isLoadingVehicle } =
    useQuery<VehicleWithLabels>({
      queryKey: ["vehicle", id],
      queryFn: async () => {
        const response = await agent.get<Vehicle>(`vehicles/${id}`);
        return convertVehicleData(response.data);
      },
      enabled: !!id, // Only run query if id is provided
    });

  // This hook provides functions to create vehicles.
  const createVehicleMutation = useMutation({
    mutationFn: async (createVehicleCommand: CreateVehicleCommand) => {
      const response = await agent.post("vehicles", createVehicleCommand);
      return response.data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["vehicles"] });
    },
  });

  // this hooks provides functions to update vehicles.
  const updateVehicleMutation = useMutation({
    mutationFn: async (updateVehicleCommand: UpdateVehicleCommand) => {
      const response = await agent.put(
        `vehicles/${updateVehicleCommand.id}`,
        updateVehicleCommand,
      );
      return response.data;
    },
    onSuccess: async (data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["vehicles"] });
      await queryClient.invalidateQueries({
        queryKey: ["vehicle", variables.id],
      });
    },
  });

  const deactivateVehicleMutation = useMutation({
    mutationFn: async (id: string) => {
      const response = await agent.post(`vehicles/deactivate/${id}`);
      return response.data;
    },
    onSuccess: async (data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["vehicles"] });
      await queryClient.invalidateQueries({ queryKey: ["vehicle", variables] });
    },
  });

  return {
    vehiclesResponse,
    vehicles: vehiclesResponse?.Items || [],
    pagination: vehiclesResponse?.Items
      ? {
          totalCount: vehiclesResponse.TotalCount,
          pageNumber: vehiclesResponse.PageNumber,
          pageSize: vehiclesResponse.PageSize,
          totalPages: vehiclesResponse.TotalPages,
          hasPreviousPage: vehiclesResponse.HasPreviousPage,
          hasNextPage: vehiclesResponse.HasNextPage,
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
