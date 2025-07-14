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
  Category: item.Category as number,
  CategoryLabel: getInventoryItemCategoryLabel(item.Category),
  CategoryEnum: item.Category as InventoryItemCategoryEnum,
  UnitCostMeasurementUnit: item.UnitCostMeasurementUnit as number,
  UnitCostMeasurementUnitLabel: getInventoryItemUnitCostMeasurementUnitLabel(
    item.UnitCostMeasurementUnit,
  ),
  UnitCostMeasurementUnitEnum:
    item.UnitCostMeasurementUnit as InventoryItemUnitCostMeasurementUnitEnum,
});

export function useInventoryItems(filter: InventoryItemFilter = {}) {
  const debouncedSearch = useDebounce(filter?.search || "", 300);
  const debouncedFilter = { ...filter, search: debouncedSearch };

  const queryParams = new URLSearchParams();
  if (debouncedFilter.page)
    queryParams.append("page", debouncedFilter.page.toString());
  if (debouncedFilter.pageSize)
    queryParams.append("pageSize", debouncedFilter.pageSize.toString());
  if (debouncedFilter.search)
    queryParams.append("search", debouncedFilter.search);
  if (debouncedFilter.sortBy)
    queryParams.append("sortBy", debouncedFilter.sortBy);
  if (debouncedFilter.sortOrder)
    queryParams.append("sortOrder", debouncedFilter.sortOrder);

  const queryString = queryParams.toString();

  const { data, isPending, isError, isSuccess, error } = useQuery<
    PagedResponse<InventoryItemWithLabels>
  >({
    queryKey: ["inventoryItems", debouncedFilter],
    queryFn: async () => {
      const { data } = await agent.get<PagedResponse<InventoryItem>>(
        `/inventoryItems${queryString ? `?${queryString}` : ""}`,
      );
      return { ...data, Items: data.Items.map(convertInventoryItemData) };
    },
  });

  return {
    inventoryItems: data?.Items ?? [],
    pagination: data
      ? {
          totalCount: data.TotalCount,
          pageNumber: data.PageNumber,
          pageSize: data.PageSize,
          totalPages: data.TotalPages,
          hasPreviousPage: data.HasPreviousPage,
          hasNextPage: data.HasNextPage,
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
      const { data } = await agent.get<InventoryItem>(`/inventoryItems/${id}`);
      return convertInventoryItemData(data);
    },
    enabled: !!id,
  });
}

export function useCreateInventoryItem() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: CreateInventoryItemCommand) => {
      const { data } = await agent.post("/inventoryItems", command);
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
        `/inventoryItems/${command.id}`,
        command,
      );
      return data;
    },
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["inventoryItems"] });
      await queryClient.invalidateQueries({
        queryKey: ["inventoryItem", variables.id.toString()],
      });
    },
  });
}

export function useDeactivateInventoryItem() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id: number) => {
      const { data } = await agent.post(`/inventoryItems/deactivate/${id}`);
      return data;
    },
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["inventoryItems"] });
      await queryClient.invalidateQueries({ queryKey: ["inventoryItem", id] });
    },
  });
}
