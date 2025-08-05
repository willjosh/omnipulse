import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { workOrderApi, convertWorkOrderData } from "../api/workOrderApi";
import {
  WorkOrderWithLabels,
  WorkOrderFilter,
  WorkOrderStatusData,
} from "../types/workOrderType";
import { useDebounce } from "@/hooks/useDebounce";
import { VehicleStatusData } from "@/features/vehicle/types/vehicleType";

export function useWorkOrders(filter: WorkOrderFilter = {}) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const { data, isPending, isError, isSuccess, error } = useQuery({
    queryKey: ["workOrders", debouncedFilter],
    queryFn: async () => {
      const data = await workOrderApi.getWorkOrders(debouncedFilter);
      return { ...data, items: data.items.map(convertWorkOrderData) };
    },
  });

  return {
    workOrders: data?.items ?? [],
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

export function useWorkOrder(id: number) {
  const { data, isPending, isError, isSuccess, error } =
    useQuery<WorkOrderWithLabels>({
      queryKey: ["workOrder", id],
      queryFn: async () => {
        const data = await workOrderApi.getWorkOrder(id);
        return convertWorkOrderData(data);
      },
      enabled: !!id,
    });

  return { workOrder: data, isPending, isError, isSuccess, error };
}

export function useCreateWorkOrder() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: workOrderApi.createWorkOrder,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["workOrders"] });
    },
  });
}

export function useUpdateWorkOrder() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: workOrderApi.updateWorkOrder,
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["workOrders"] });
      await queryClient.invalidateQueries({
        queryKey: ["workOrder", variables.workOrderID],
      });
    },
  });
}

export function useDeleteWorkOrder() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: workOrderApi.deleteWorkOrder,
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["workOrders"] });
      await queryClient.invalidateQueries({ queryKey: ["workOrder", id] });
    },
  });
}

export function useWorkOrderStatusData() {
  const { data, isPending, isError, isSuccess, error } =
    useQuery<WorkOrderStatusData>({
      queryKey: ["workOrderStatusData"],
      queryFn: async () => {
        const data = await workOrderApi.getWorkOrderStatusData();
        return data;
      },
    });

  return {
    createdCount: data?.createdCount ?? 0,
    inProgressCount: data?.inProgressCount ?? 0,
    isLoadingworkOrderStatusData: isPending,
    isError,
    isSuccess,
    error,
  };
}
