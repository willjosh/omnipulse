import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { inventoryApi, convertInventoryData } from "../api/inventoryApi";
import { Inventory, InventoryFilter } from "../types/inventoryType";
import { useDebounce } from "@/hooks/useDebounce";

export function useInventories(filter: InventoryFilter = {}) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const { data, isPending, isError, isSuccess, error } = useQuery({
    queryKey: ["inventories", debouncedFilter],
    queryFn: async () => {
      const data = await inventoryApi.getInventories(debouncedFilter);
      return { ...data, items: data.items.map(convertInventoryData) };
    },
  });

  return {
    inventories: data?.items ?? [],
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

export function useInventory(id: number) {
  const { data, isPending, isError, isSuccess, error } = useQuery<Inventory>({
    queryKey: ["inventory", id],
    queryFn: async () => {
      const data = await inventoryApi.getInventory(id);
      return data;
    },
    enabled: !!id,
  });

  return { inventory: data, isPending, isError, isSuccess, error };
}

export function useCreateInventory() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: inventoryApi.createInventory,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["inventories"] });
    },
  });
}

export function useUpdateInventory() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: inventoryApi.updateInventory,
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["inventories"] });
      await queryClient.invalidateQueries({
        queryKey: ["inventory", variables.inventoryID],
      });
    },
  });
}

export function useDeleteInventory() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: inventoryApi.deleteInventory,
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["inventories"] });
      await queryClient.invalidateQueries({ queryKey: ["inventory", id] });
    },
  });
}
