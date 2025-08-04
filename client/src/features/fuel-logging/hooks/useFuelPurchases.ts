import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  fuelPurchaseApi,
  convertFuelPurchaseData,
} from "../api/fuelPurchaseApi";
import {
  FuelPurchaseWithLabels,
  FuelPurchaseFilter,
} from "../types/fuelPurchaseType";
import { useDebounce } from "@/hooks/useDebounce";

export function useFuelPurchases(filter: FuelPurchaseFilter = {}) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const { data, isPending, isError, isSuccess, error } = useQuery({
    queryKey: ["fuelPurchases", debouncedFilter],
    queryFn: async () => {
      const data = await fuelPurchaseApi.getFuelPurchases(debouncedFilter);
      return { ...data, items: data.items.map(convertFuelPurchaseData) };
    },
  });

  return {
    fuelPurchases: data?.items ?? [],
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

export function useFuelPurchase(id: number) {
  const { data, isPending, isError, isSuccess, error } =
    useQuery<FuelPurchaseWithLabels>({
      queryKey: ["fuelPurchase", id],
      queryFn: async () => {
        const data = await fuelPurchaseApi.getFuelPurchase(id);
        return convertFuelPurchaseData(data);
      },
      enabled: !!id,
    });

  return { fuelPurchase: data, isPending, isError, isSuccess, error };
}

export function useFuelPurchasesByVehicle(
  vehicleId: number,
  filter: FuelPurchaseFilter = {},
) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = {
    ...filter,
    Search: debouncedSearch,
    VehicleId: vehicleId,
  };

  const { data, isPending, isError, isSuccess, error } = useQuery({
    queryKey: ["fuelPurchasesByVehicle", vehicleId, debouncedFilter],
    queryFn: async () => {
      const data = await fuelPurchaseApi.getFuelPurchasesByVehicle(
        vehicleId,
        debouncedFilter,
      );
      return { ...data, items: data.items.map(convertFuelPurchaseData) };
    },
    enabled: !!vehicleId,
  });

  return {
    fuelPurchases: data?.items ?? [],
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

export function useCreateFuelPurchase() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: fuelPurchaseApi.createFuelPurchase,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["fuelPurchases"] });
      await queryClient.invalidateQueries({
        queryKey: ["fuelPurchasesByVehicle"],
      });
    },
  });
}

export function useUpdateFuelPurchase() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: fuelPurchaseApi.updateFuelPurchase,
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["fuelPurchases"] });
      await queryClient.invalidateQueries({
        queryKey: ["fuelPurchasesByVehicle"],
      });
      await queryClient.invalidateQueries({
        queryKey: ["fuelPurchase", variables.fuelPurchaseId],
      });
    },
  });
}

export function useArchiveFuelPurchase() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: fuelPurchaseApi.archiveFuelPurchase,
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["fuelPurchases"] });
      await queryClient.invalidateQueries({
        queryKey: ["fuelPurchasesByVehicle"],
      });
      await queryClient.invalidateQueries({ queryKey: ["fuelPurchase", id] });
    },
  });
}

export function useDeleteFuelPurchase() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: fuelPurchaseApi.deleteFuelPurchase,
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["fuelPurchases"] });
      await queryClient.invalidateQueries({
        queryKey: ["fuelPurchasesByVehicle"],
      });
      await queryClient.invalidateQueries({ queryKey: ["fuelPurchase", id] });
    },
  });
}

export function useValidateOdometerReading() {
  return useMutation({
    mutationFn: ({
      vehicleId,
      odometerReading,
    }: {
      vehicleId: number;
      odometerReading: number;
    }) => fuelPurchaseApi.validateOdometerReading(vehicleId, odometerReading),
  });
}

export function useValidateReceiptNumber() {
  return useMutation({
    mutationFn: (receiptNumber: string) =>
      fuelPurchaseApi.validateReceiptNumber(receiptNumber),
  });
}
