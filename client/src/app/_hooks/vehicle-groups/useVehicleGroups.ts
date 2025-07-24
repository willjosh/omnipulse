import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { agent } from "@/app/_lib/axios/agent";
import {
  VehicleGroup,
  CreateVehicleGroupCommand,
  UpdateVehicleGroupCommand,
  VehicleGroupFilter,
} from "./vehicleGroupType";
import { PagedResponse } from "@/app/_hooks/shared_types/pagedResponse";
import { useDebounce } from "@/app/_hooks/shared_types/useDebounce";

export function useVehicleGroups(filter: VehicleGroupFilter = {}) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const queryParams = new URLSearchParams();
  if (debouncedFilter.PageNumber)
    queryParams.append("PageNumber", debouncedFilter.PageNumber.toString());
  if (debouncedFilter.PageSize)
    queryParams.append("PageSize", debouncedFilter.PageSize.toString());
  if (debouncedFilter.Search)
    queryParams.append("Search", debouncedFilter.Search);
  if (debouncedFilter.SortBy)
    queryParams.append("SortBy", debouncedFilter.SortBy);
  if (debouncedFilter.SortDescending !== undefined)
    queryParams.append(
      "SortDescending",
      debouncedFilter.SortDescending.toString(),
    );

  const queryString = queryParams.toString();

  const { data, isPending, isError, isSuccess, error } = useQuery<
    PagedResponse<VehicleGroup>
  >({
    queryKey: ["vehicleGroups", debouncedFilter],
    queryFn: async () => {
      const { data } = await agent.get<PagedResponse<VehicleGroup>>(
        `/api/VehicleGroups${queryString ? `?${queryString}` : ""}`,
      );
      return data;
    },
  });

  return {
    vehicleGroups: data?.items ?? [],
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

export function useVehicleGroup(id: number) {
  return useQuery({
    queryKey: ["vehicleGroup", id],
    queryFn: async () => {
      const { data } = await agent.get<VehicleGroup>(
        `/api/VehicleGroups/${id}`,
      );
      return data;
    },
    enabled: !!id,
  });
}

export function useCreateVehicleGroup() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: CreateVehicleGroupCommand) => {
      const { data } = await agent.post("/api/VehicleGroups", command);
      return data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["vehicleGroups"] });
    },
  });
}

export function useUpdateVehicleGroup() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: UpdateVehicleGroupCommand) => {
      const { data } = await agent.put(
        `/api/VehicleGroups/${command.vehicleGroupId}`,
        command,
      );
      return data;
    },
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["vehicleGroups"] });
      await queryClient.invalidateQueries({
        queryKey: ["vehicleGroup", variables.vehicleGroupId],
      });
    },
  });
}

export function useDeleteVehicleGroup() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id: number) => {
      const { data } = await agent.delete(`/api/VehicleGroups/${id}`);
      return data;
    },
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["vehicleGroups"] });
      await queryClient.invalidateQueries({ queryKey: ["vehicleGroup", id] });
    },
  });
}
