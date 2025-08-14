import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  fuelPurchaseApi,
  convertFuelPurchaseData,
} from "../api/fuelPurchaseApi";
import {
  FuelPurchaseWithLabels,
  FuelPurchaseFilter,
  CreateFuelPurchaseCommand,
  UpdateFuelPurchaseCommand,
} from "../types/fuelPurchaseType";
import { useDebounce } from "@/hooks/useDebounce";

export function useFuelPurchases(filter: FuelPurchaseFilter = {}) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);

  const debouncedFilter = { ...filter };
  if (debouncedSearch && debouncedSearch.trim() !== "") {
    debouncedFilter.Search = debouncedSearch;
  }

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

export function useFuelPurchase(id: string) {
  const isValidId = Boolean(id && id.trim() !== "");

  const { data, isPending, isError, isSuccess, error } =
    useQuery<FuelPurchaseWithLabels>({
      queryKey: ["fuelPurchase", id],
      queryFn: async () => {
        const data = await fuelPurchaseApi.getFuelPurchase(parseInt(id));
        return convertFuelPurchaseData(data);
      },
      enabled: isValidId,
    });

  return {
    fuelPurchase: data,
    isPending: isValidId ? isPending : false,
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
        queryKey: ["fuelPurchase", variables.fuelPurchaseId.toString()],
      });
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
        queryKey: ["fuelPurchase", id.toString()],
      });
    },
  });
}
