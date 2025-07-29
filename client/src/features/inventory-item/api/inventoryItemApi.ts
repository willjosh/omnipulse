import { agent } from "@/lib/axios/agent";
import {
  InventoryItem,
  InventoryItemWithLabels,
  InventoryItemFilter,
  CreateInventoryItemCommand,
  UpdateInventoryItemCommand,
} from "../types/inventoryItemType";
import {
  InventoryItemCategoryEnum,
  InventoryItemUnitCostMeasurementUnitEnum,
} from "../types/inventoryItemEnum";
import {
  getInventoryItemCategoryLabel,
  getInventoryItemUnitCostMeasurementUnitLabel,
} from "../utils/inventoryItemEnumHelper";

export const convertInventoryItemData = (
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

export const inventoryItemApi = {
  getInventoryItems: async (filter: InventoryItemFilter = {}) => {
    const queryParams = new URLSearchParams();
    if (filter.PageNumber)
      queryParams.append("PageNumber", filter.PageNumber.toString());
    if (filter.PageSize)
      queryParams.append("PageSize", filter.PageSize.toString());
    if (filter.Search) queryParams.append("Search", filter.Search);
    if (filter.SortBy) queryParams.append("SortBy", filter.SortBy);
    if (filter.SortDescending !== undefined)
      queryParams.append("SortDescending", filter.SortDescending.toString());
    const queryString = queryParams.toString();
    const { data } = await agent.get<{
      items: InventoryItem[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>(`/api/InventoryItems${queryString ? `?${queryString}` : ""}`);
    return data;
  },

  getInventoryItem: async (id: number) => {
    const { data } = await agent.get<InventoryItem>(
      `/api/InventoryItems/${id}`,
    );
    return data;
  },

  createInventoryItem: async (command: CreateInventoryItemCommand) => {
    const { data } = await agent.post("/api/InventoryItems", command);
    return data;
  },

  updateInventoryItem: async (command: UpdateInventoryItemCommand) => {
    const { data } = await agent.put(
      `/api/InventoryItems/${command.inventoryItemID}`,
      command,
    );
    return data;
  },

  deleteInventoryItem: async (id: number) => {
    const { data } = await agent.delete(`/api/InventoryItems/${id}`);
    return data;
  },
};
