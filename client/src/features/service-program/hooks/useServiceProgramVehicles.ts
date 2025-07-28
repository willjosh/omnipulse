import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { serviceProgramVehicleApi } from "../api/serviceProgramVehicleApi";
import { ServiceProgramVehicleFilter } from "../types/serviceProgramVehicleType";
import { useDebounce } from "@/hooks/useDebounce";

export function useServiceProgramVehicles(
  serviceProgramId: number,
  filter: ServiceProgramVehicleFilter = {},
) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const { data, isPending, isError, isSuccess, error } = useQuery({
    queryKey: ["serviceProgramVehicles", serviceProgramId, debouncedFilter],
    queryFn: async () => {
      const data = await serviceProgramVehicleApi.getServiceProgramVehicles(
        serviceProgramId,
        debouncedFilter,
      );
      return data;
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
    mutationFn: serviceProgramVehicleApi.addVehicleToServiceProgram,
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({
        predicate: query => {
          const queryKey = query.queryKey;
          return (
            Array.isArray(queryKey) &&
            queryKey[0] === "serviceProgramVehicles" &&
            queryKey[1] === variables.serviceProgramID
          );
        },
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
    mutationFn: serviceProgramVehicleApi.removeVehicleFromServiceProgram,
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({
        predicate: query => {
          const queryKey = query.queryKey;
          return (
            Array.isArray(queryKey) &&
            queryKey[0] === "serviceProgramVehicles" &&
            queryKey[1] === variables.serviceProgramID
          );
        },
      });
      await queryClient.invalidateQueries({
        queryKey: ["serviceProgram", variables.serviceProgramID],
      });
    },
  });
}
