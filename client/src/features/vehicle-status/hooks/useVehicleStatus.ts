import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { vehicleStatusApi } from "../api/vehicleStatusApi";
import { VehicleStatus } from "../types/vehicleStatusType";

export function useVehicleStatuses() {
  const { data, isPending, isError, isSuccess, error } = useQuery<
    VehicleStatus[]
  >({
    queryKey: ["vehicleStatuses"],
    queryFn: async () => {
      const data = await vehicleStatusApi.getVehicleStatuses();
      return data;
    },
  });

  return { vehicleStatuses: data ?? [], isPending, isError, isSuccess, error };
}

export function useVehicleStatus(id: number) {
  const { data, isPending, isError, isSuccess, error } =
    useQuery<VehicleStatus>({
      queryKey: ["vehicleStatus", id],
      queryFn: async () => {
        const data = await vehicleStatusApi.getVehicleStatus(id);
        return data;
      },
      enabled: !!id,
    });

  return { vehicleStatus: data, isPending, isError, isSuccess, error };
}

export function useCreateVehicleStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: vehicleStatusApi.createVehicleStatus,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["vehicleStatuses"] });
    },
  });
}

export function useUpdateVehicleStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: vehicleStatusApi.updateVehicleStatus,
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["vehicleStatuses"] });
      await queryClient.invalidateQueries({
        queryKey: ["vehicleStatus", variables.id],
      });
    },
  });
}

export function useDeleteVehicleStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: vehicleStatusApi.deleteVehicleStatus,
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["vehicleStatuses"] });
      await queryClient.invalidateQueries({ queryKey: ["vehicleStatus", id] });
    },
  });
}
