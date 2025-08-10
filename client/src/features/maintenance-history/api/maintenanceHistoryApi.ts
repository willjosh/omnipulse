import { agent } from "@/lib/axios/agent";
import {
  MaintenanceHistory,
  MaintenanceHistoryWithFormattedDates,
  MaintenanceHistoryFilter,
  CreateMaintenanceHistoryCommand,
} from "../types/maintenanceHistoryType";

function formatDate(date?: string | null): string | null {
  if (!date) return null;
  const d = new Date(date);
  return isNaN(d.getTime()) ? null : d.toLocaleString();
}

export const convertMaintenanceHistoryData = (
  maintenanceHistory: MaintenanceHistory,
): MaintenanceHistoryWithFormattedDates => ({
  ...maintenanceHistory,
  serviceDateFormatted: formatDate(maintenanceHistory.serviceDate),
});

export const maintenanceHistoryApi = {
  getMaintenanceHistories: async (filter: MaintenanceHistoryFilter = {}) => {
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
      items: MaintenanceHistory[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>(`/api/maintenancehistory${queryString ? `?${queryString}` : ""}`);
    return data;
  },

  createMaintenanceHistory: async (
    command: CreateMaintenanceHistoryCommand,
  ) => {
    const { data } = await agent.post("/api/maintenancehistory", command);
    return data;
  },
};
