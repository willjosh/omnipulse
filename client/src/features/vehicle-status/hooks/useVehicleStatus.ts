import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { VehicleStatus } from "../types/vehicleStatusType";
import { getAllVehicleStatusOptions } from "../types/vehicleStatusEnum";

// Mock vehicle count data as backend is not implemented yet
const getMockVehicleCount = (statusId: number) => {
  const counts: Record<number, number> = {
    1: 15, // Active vehicles
    2: 3, // Maintenance vehicles
    3: 1, // Out of service vehicles
    4: 5, // Inactive vehicles
  };
  return counts[statusId] || 0;
};

export function useVehicleStatuses() {
  const { data, isPending, isError, isSuccess, error } = useQuery<
    VehicleStatus[]
  >({
    queryKey: ["vehicleStatuses"],
    queryFn: async () => {
      const enumOptions = getAllVehicleStatusOptions();
      return enumOptions.map(option => ({
        id: option.value,
        name: option.label,
        color: option.color,
        isActive: true,
        vehicleCount: getMockVehicleCount(option.value),
      }));
    },
  });

  return { vehicleStatuses: data ?? [], isPending, isError, isSuccess, error };
}

export function useVehicleStatus(id: number) {
  const { data, isPending, isError, isSuccess, error } =
    useQuery<VehicleStatus>({
      queryKey: ["vehicleStatus", id],
      queryFn: async () => {
        // Use enum data instead of API call
        const enumOptions = getAllVehicleStatusOptions();
        const option = enumOptions.find(opt => opt.value === id);
        if (!option) {
          throw new Error(`Vehicle status with id ${id} not found`);
        }
        return {
          id: option.value,
          name: option.label,
          color: option.color,
          isActive: true,
          vehicleCount: getMockVehicleCount(option.value),
        };
      },
      enabled: !!id,
    });

  return { vehicleStatus: data, isPending, isError, isSuccess, error };
}

export function useCreateVehicleStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async () => {
      // Mock create as statuses are currently implemented as enums, so new statuses can't be created
      console.log(
        "Create vehicle status - mock implementation (enums are fixed)",
      );
      return Promise.resolve();
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["vehicleStatuses"] });
    },
  });
}

export function useUpdateVehicleStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async () => {
      // Mock create as statuses are currently implemented as enums, so new statuses can't be updated
      console.log(
        "Update vehicle status - mock implementation (enums are fixed)",
      );
      return Promise.resolve();
    },
    onSuccess: async (_data, variables: any) => {
      await queryClient.invalidateQueries({ queryKey: ["vehicleStatuses"] });
      await queryClient.invalidateQueries({
        queryKey: ["vehicleStatus", variables?.id],
      });
    },
  });
}

export function useDeleteVehicleStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async () => {
      // Mock create as statuses are currently implemented as enums, so new statuses can't be deleted
      console.log(
        "Delete vehicle status - mock implementation (enums are fixed)",
      );
      return Promise.resolve();
    },
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["vehicleStatuses"] });
      await queryClient.invalidateQueries({ queryKey: ["vehicleStatus", id] });
    },
  });
}
