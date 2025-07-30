import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  inventoryItemApi,
  convertInventoryItemData,
} from "../api/inventoryItemApi";
import {
  InventoryItemWithLabels,
  InventoryItemFilter,
} from "../types/inventoryItemType";
import { useDebounce } from "@/hooks/useDebounce";

export function useInventoryItems(filter: InventoryItemFilter = {}) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const { data, isPending, isError, isSuccess, error } = useQuery({
    queryKey: ["inventoryItems", debouncedFilter],
    queryFn: async () => {
      const data = await inventoryItemApi.getInventoryItems(debouncedFilter);
      return { ...data, items: data.items.map(convertInventoryItemData) };
    },
  });

  return {
    inventoryItems: data?.items ?? [],
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

export function useInventoryItem(id: number) {
  const { data, isPending, isError, isSuccess, error } =
    useQuery<InventoryItemWithLabels>({
      queryKey: ["inventoryItem", id],
      queryFn: async () => {
        const data = await inventoryItemApi.getInventoryItem(id);
        return convertInventoryItemData(data);
      },
      enabled: !!id,
    });

  return { inventoryItem: data, isPending, isError, isSuccess, error };
}

export function useCreateInventoryItem() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: inventoryItemApi.createInventoryItem,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["inventoryItems"] });
    },
  });
}

export function useUpdateInventoryItem() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: inventoryItemApi.updateInventoryItem,
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["inventoryItems"] });
      await queryClient.invalidateQueries({
        queryKey: ["inventoryItem", variables.inventoryItemID],
      });
    },
  });
}

export function useDeleteInventoryItem() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: inventoryItemApi.deleteInventoryItem,
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["inventoryItems"] });
      await queryClient.invalidateQueries({ queryKey: ["inventoryItem", id] });
    },
  });
}
