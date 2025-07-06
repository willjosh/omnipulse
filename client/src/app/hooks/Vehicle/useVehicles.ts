import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  CreateVehicleCommand,
  UpdateVehicleCommand,
  Vehicle,
  VehicleFilter,
} from "./vehicleType";
import { agent } from "@/app/_lib/axios/agent";
import { PagedResponse } from "../shared_types/pagedResponse";
import { useDebounce } from "../shared_types/useDebounce";

export const useVehicles = (filters?: VehicleFilter, id?: string) => {
  const queryClient = useQueryClient();
  const queryParams = new URLSearchParams();
  const debouncedSearch = useDebounce(filters?.search || "", 300);

  const debouncedFilters = { ...filters, search: debouncedSearch };

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

  // This hook fetches vehicles data from the API and returns it along with a loading state.
  const { data: vehiclesResponse, isLoading: isLoadingVehicles } = useQuery<
    PagedResponse<Vehicle>
  >({
    queryKey: ["vehicles", debouncedFilters],
    queryFn: async () => {
      const url = queryParams.toString()
        ? `vehicles?${queryParams.toString()}`
        : "vehicles";
      const response = await agent.get(url);
      console.log("Vehicles Response:", response.data);
      return response.data;
    },
  });

  // This hook fetches a single vehicle by ID from the API and returns it along with a loading state.
  const { data: vehicle, isLoading: isLoadingVehicle } = useQuery<Vehicle>({
    queryKey: ["vehicle", id],
    queryFn: async () => {
      const response = await agent.get(`vehicles/${id}`);
      return response.data;
    },
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
    onSuccess: async variables => {
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
    onSuccess: async variables => {
      await queryClient.invalidateQueries({ queryKey: ["vehicles"] });
      await queryClient.invalidateQueries({
        queryKey: ["vehicle", variables.id],
      });
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
