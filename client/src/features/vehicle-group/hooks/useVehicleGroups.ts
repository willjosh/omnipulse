import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { vehicleGroupApi } from "../api/vehicleGroupApi";
import { VehicleGroup, VehicleGroupFilter } from "../types/vehicleGroupType";
import { useDebounce } from "@/hooks/useDebounce";

export function useVehicleGroups(filter: VehicleGroupFilter = {}) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const { data, isPending, isError, isSuccess, error } = useQuery({
    queryKey: ["vehicleGroups", debouncedFilter],
    queryFn: async () => {
      const data = await vehicleGroupApi.getVehicleGroups(debouncedFilter);
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
  const { data, isPending, isError, isSuccess, error } = useQuery<VehicleGroup>(
    {
      queryKey: ["vehicleGroup", id],
      queryFn: async () => {
        const data = await vehicleGroupApi.getVehicleGroup(id);
        return data;
      },
      enabled: !!id,
    },
  );

  return { vehicleGroup: data, isPending, isError, isSuccess, error };
}

export function useCreateVehicleGroup() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: vehicleGroupApi.createVehicleGroup,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["vehicleGroups"] });
    },
  });
}

export function useUpdateVehicleGroup() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: vehicleGroupApi.updateVehicleGroup,
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
    mutationFn: vehicleGroupApi.deleteVehicleGroup,
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["vehicleGroups"] });
      await queryClient.invalidateQueries({ queryKey: ["vehicleGroup", id] });
    },
  });
}
