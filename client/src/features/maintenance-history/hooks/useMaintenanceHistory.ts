import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  maintenanceHistoryApi,
  convertMaintenanceHistoryData,
} from "../api/maintenanceHistoryApi";
import { MaintenanceHistoryFilter } from "../types/maintenanceHistoryType";
import { useDebounce } from "@/hooks/useDebounce";

export function useMaintenanceHistories(filter: MaintenanceHistoryFilter = {}) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const { data, isPending, isError, isSuccess, error } = useQuery({
    queryKey: ["maintenanceHistories", debouncedFilter],
    queryFn: async () => {
      const data =
        await maintenanceHistoryApi.getMaintenanceHistories(debouncedFilter);
      return { ...data, items: data.items.map(convertMaintenanceHistoryData) };
    },
  });

  return {
    maintenanceHistories: data?.items ?? [],
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

export function useCreateMaintenanceHistory() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: maintenanceHistoryApi.createMaintenanceHistory,
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["maintenanceHistories"],
      });
    },
  });
}
