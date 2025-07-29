import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { technicianApi } from "../api/technicianApi";
import { Technician, TechnicianFilter } from "../types/technicianType";
import { useDebounce } from "@/hooks/useDebounce";

export function useTechnicians(filter: TechnicianFilter = {}) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const { data, isPending, isError, isSuccess, error } = useQuery({
    queryKey: ["technicians", debouncedFilter],
    queryFn: async () => {
      const data = await technicianApi.getTechnicians(debouncedFilter);
      return data;
    },
  });

  return {
    technicians: data?.items ?? [],
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

export function useTechnician(id: string) {
  const { data, isPending, isError, isSuccess, error } = useQuery<Technician>({
    queryKey: ["technician", id],
    queryFn: async () => {
      const data = await technicianApi.getTechnician(id);
      return data;
    },
    enabled: !!id,
  });

  return { technician: data, isPending, isError, isSuccess, error };
}

export function useCreateTechnician() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: technicianApi.createTechnician,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["technicians"] });
    },
  });
}

export function useUpdateTechnician() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: technicianApi.updateTechnician,
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["technicians"] });
      await queryClient.invalidateQueries({
        queryKey: ["technician", variables.id],
      });
    },
  });
}

export function useDeactivateTechnician() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: technicianApi.deactivateTechnician,
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["technicians"] });
      await queryClient.invalidateQueries({ queryKey: ["technician", id] });
    },
  });
}
