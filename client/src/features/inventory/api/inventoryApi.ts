import { agent } from "@/lib/axios/agent";
import {
  Inventory,
  InventoryFilter,
  UpdateInventoryCommand,
} from "../types/inventoryType";
import { convertInventoryData } from "../utils/inventoryHelper";

export { convertInventoryData };

export const inventoryApi = {
  getInventories: async (filter: InventoryFilter = {}) => {
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
      items: Inventory[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>(`/api/Inventories${queryString ? `?${queryString}` : ""}`);
    return data;
  },

  getInventory: async (id: number) => {
    const { data } = await agent.get<Inventory>(`/api/Inventories/${id}`);
    return data;
  },

  updateInventory: async (command: UpdateInventoryCommand) => {
    const { data } = await agent.put(
      `/api/Inventories/${command.inventoryID}`,
      command,
    );
    return data;
  },

  deleteInventory: async (id: number) => {
    const { data } = await agent.delete(`/api/Inventories/${id}`);
    return data;
  },
};
