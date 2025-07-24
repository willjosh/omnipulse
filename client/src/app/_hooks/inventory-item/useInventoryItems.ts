import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { agent } from "@/app/_lib/axios/agent";
import {
  InventoryItem,
  InventoryItemWithLabels,
  InventoryItemFilter,
  CreateInventoryItemCommand,
  UpdateInventoryItemCommand,
} from "./inventoryItemType";
import {
  InventoryItemCategoryEnum,
  InventoryItemUnitCostMeasurementUnitEnum,
} from "./inventoryItemEnum";
import { PagedResponse } from "@/app/_hooks/shared_types/pagedResponse";
import { useDebounce } from "@/app/_hooks/shared_types/useDebounce";
import {
  getInventoryItemCategoryLabel,
  getInventoryItemUnitCostMeasurementUnitLabel,
} from "@/app/_utils/inventoryItemEnumHelper";

const convertInventoryItemData = (
  item: InventoryItem,
): InventoryItemWithLabels => ({
  ...item,
  category: item.category as number,
  categoryLabel: getInventoryItemCategoryLabel(item.category),
  categoryEnum: item.category as InventoryItemCategoryEnum,
  unitCostMeasurementUnit: item.unitCostMeasurementUnit as number,
  unitCostMeasurementUnitLabel: getInventoryItemUnitCostMeasurementUnitLabel(
    item.unitCostMeasurementUnit,
  ),
  unitCostMeasurementUnitEnum:
    item.unitCostMeasurementUnit as InventoryItemUnitCostMeasurementUnitEnum,
});

export function useInventoryItems(filter: InventoryItemFilter = {}) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const queryParams = new URLSearchParams();
  if (debouncedFilter.PageNumber)
    queryParams.append("PageNumber", debouncedFilter.PageNumber.toString());
  if (debouncedFilter.PageSize)
    queryParams.append("PageSize", debouncedFilter.PageSize.toString());
  if (debouncedFilter.Search)
    queryParams.append("Search", debouncedFilter.Search);
  if (debouncedFilter.SortBy)
    queryParams.append("SortBy", debouncedFilter.SortBy);
  if (debouncedFilter.SortDescending !== undefined)
    queryParams.append(
      "SortDescending",
      debouncedFilter.SortDescending.toString(),
    );

  const queryString = queryParams.toString();

  const { data, isPending, isError, isSuccess, error } = useQuery<
    PagedResponse<InventoryItemWithLabels>
  >({
    queryKey: ["inventoryItems", debouncedFilter],
    queryFn: async () => {
      const { data } = await agent.get<PagedResponse<InventoryItem>>(
        `/api/InventoryItems${queryString ? `?${queryString}` : ""}`,
      );
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
  return useQuery<InventoryItemWithLabels>({
    queryKey: ["inventoryItem", id],
    queryFn: async () => {
      const { data } = await agent.get<InventoryItem>(
        `/api/InventoryItems/${id}`,
      );
      return convertInventoryItemData(data);
    },
    enabled: !!id,
  });
}

export function useCreateInventoryItem() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: CreateInventoryItemCommand) => {
      const { data } = await agent.post("/api/InventoryItems", command);
      return data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["inventoryItems"] });
    },
  });
}

export function useUpdateInventoryItem() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: UpdateInventoryItemCommand) => {
      const { data } = await agent.put(
        `/api/InventoryItems/${command.inventoryItemID}`,
        command,
      );
      return data;
    },
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
    mutationFn: async (id: number) => {
      const { data } = await agent.delete(`/api/InventoryItems/${id}`);
      return data;
    },
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["inventoryItems"] });
      await queryClient.invalidateQueries({ queryKey: ["inventoryItem", id] });
    },
  });
}
