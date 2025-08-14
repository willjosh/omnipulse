import { agent } from "@/lib/axios/agent";
import {
  FuelPurchase,
  FuelPurchaseWithLabels,
  FuelPurchaseFilter,
  CreateFuelPurchaseCommand,
  UpdateFuelPurchaseCommand,
} from "../types/fuelPurchaseType";

export const convertFuelPurchaseData = (
  fuelPurchase: FuelPurchase,
): FuelPurchaseWithLabels => ({ ...fuelPurchase });

export const fuelPurchaseApi = {
  getFuelPurchases: async (filter: FuelPurchaseFilter = {}) => {
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
      items: FuelPurchase[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>(`/api/fuelpurchases${queryString ? `?${queryString}` : ""}`);
    return data;
  },

  getFuelPurchase: async (id: number) => {
    const { data } = await agent.get<FuelPurchase>(`/api/fuelpurchases/${id}`);
    return data;
  },

  createFuelPurchase: async (command: CreateFuelPurchaseCommand) => {
    const { data } = await agent.post("/api/fuelpurchases", command);
    return data;
  },

  updateFuelPurchase: async (command: UpdateFuelPurchaseCommand) => {
    const { data } = await agent.put(
      `/api/fuelpurchases/${command.fuelPurchaseId}`,
      command,
    );
    return data;
  },

  deleteFuelPurchase: async (id: number) => {
    const { data } = await agent.delete(`/api/fuelpurchases/${id}`);
    return data;
  },
};
