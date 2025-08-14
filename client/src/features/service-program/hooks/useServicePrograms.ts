import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { serviceProgramApi } from "../api/serviceProgramApi";
import {
  ServiceProgramDetailsWithLabels,
  ServiceProgramFilter,
} from "../types/serviceProgramType";
import { useDebounce } from "@/hooks/useDebounce";
import { convertServiceProgramDetailsData } from "../api/serviceProgramApi";

export function useServicePrograms(filter: ServiceProgramFilter = {}) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const { data, isPending, isError, isSuccess, error } = useQuery({
    queryKey: ["servicePrograms", debouncedFilter],
    queryFn: async () => {
      const data = await serviceProgramApi.getServicePrograms(debouncedFilter);
      return data;
    },
  });

  return {
    servicePrograms: data?.items ?? [],
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

export function useServiceProgram(id: number) {
  const { data, isPending, isError, isSuccess, error } =
    useQuery<ServiceProgramDetailsWithLabels>({
      queryKey: ["serviceProgram", id],
      queryFn: async () => {
        const data = await serviceProgramApi.getServiceProgram(id);
        return convertServiceProgramDetailsData(data);
      },
      enabled: !!id,
    });

  return { serviceProgram: data, isPending, isError, isSuccess, error };
}

export function useCreateServiceProgram() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: serviceProgramApi.createServiceProgram,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["servicePrograms"] });
    },
  });
}

export function useUpdateServiceProgram() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: serviceProgramApi.updateServiceProgram,
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["servicePrograms"] });
      await queryClient.invalidateQueries({
        queryKey: ["serviceProgram", variables.serviceProgramID],
      });
    },
  });
}

export function useDeleteServiceProgram() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: serviceProgramApi.deleteServiceProgram,
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["servicePrograms"] });
      await queryClient.invalidateQueries({ queryKey: ["serviceProgram", id] });
    },
  });
}
