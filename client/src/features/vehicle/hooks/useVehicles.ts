import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { vehicleApi, convertVehicleData } from "../api/vehicleApi";
import {
  VehicleWithLabels,
  VehicleFilter,
  VehicleAssignedData,
  VehicleStatusData,
} from "../types/vehicleType";
import { useDebounce } from "@/hooks/useDebounce";

export function useVehicles(filter: VehicleFilter = {}) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const { data, isPending, isError, isSuccess, error } = useQuery({
    queryKey: ["vehicles", debouncedFilter],
    queryFn: async () => {
      const data = await vehicleApi.getVehicles(debouncedFilter);
      return { ...data, items: data.items.map(convertVehicleData) };
    },
  });

  return {
    vehicles: data?.items ?? [],
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

export function useVehicle(id: string) {
  const { data, isPending, isError, isSuccess, error } =
    useQuery<VehicleWithLabels>({
      queryKey: ["vehicle", id],
      queryFn: async () => {
        const data = await vehicleApi.getVehicle(id);
        return convertVehicleData(data);
      },
      enabled: !!id,
    });

  return { vehicle: data, isPending, isError, isSuccess, error };
}

export function useCreateVehicle() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: vehicleApi.createVehicle,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["vehicles"] });
    },
  });
}

export function useVehicleAssignedData() {
  const { data, isPending, isError, isSuccess, error } =
    useQuery<VehicleAssignedData>({
      queryKey: ["vehicleAssignedData"],
      queryFn: async () => {
        const data = await vehicleApi.getVehicleAssignedData();
        return data;
      },
    });

  return {
    assignedVehicleCount: data?.assignedVehicleCount ?? 0,
    unassignedVehicleCount: data?.unassignedVehicleCount ?? 0,
    isAssignedVehicleDataLoading: isPending,
    isError,
    isSuccess,
    error,
  };
}

export function useVehicleStatusData() {
  const { data, isPending, isError, isSuccess, error } =
    useQuery<VehicleStatusData>({
      queryKey: ["vehicleStatusData"],
      queryFn: async () => {
        const data = await vehicleApi.getVehicleStatusData();
        return data;
      },
    });

  return {
    activeVehicleCount: data?.activeVehicleCount ?? 0,
    inactiveVehicleCount: data?.inactiveVehicleCount ?? 0,
    maintenanceVehicleCount: data?.maintenanceVehicleCount ?? 0,
    outOfServiceVehicleCount: data?.outOfServiceVehicleCount ?? 0,
    isVehicleStatusDataLoading: isPending,
    isError,
    isSuccess,
    error,
  };
}

export function useUpdateVehicle() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: vehicleApi.updateVehicle,
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["vehicles"] });
      await queryClient.invalidateQueries({
        queryKey: ["vehicle", variables.vehicleID],
      });
    },
  });
}

export function useDeactivateVehicle() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: vehicleApi.deactivateVehicle,
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["vehicles"] });
      await queryClient.invalidateQueries({ queryKey: ["vehicle", id] });
    },
  });
}

export function useVehicleGroups() {
  const { data, isPending, isError, error } = useQuery({
    queryKey: ["vehicleGroups"],
    queryFn: vehicleApi.getVehicleGroups,
  });

  return { vehicleGroups: data ?? [], isPending, isError, error };
}

export function useTechnicians() {
  const { data, isPending, isError, error } = useQuery({
    queryKey: ["technicians"],
    queryFn: async () => {
      const technicians = await vehicleApi.getTechnicians();
      return technicians.map(tech => ({
        id: tech.id,
        firstName: tech.firstName,
        lastName: tech.lastName,
        email: tech.email,
        isActive: tech.isActive,
        hireDate: tech.hireDate || "", // Ensure hireDate is included
      }));
    },
  });

  return { technicians: data ?? [], isPending, isError, error };
}

// TODO: Future Implementation - Vehicle Status API
