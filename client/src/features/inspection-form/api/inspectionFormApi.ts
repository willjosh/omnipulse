import { agent } from "@/lib/axios/agent";
import {
  InspectionForm,
  CreateInspectionFormCommand,
  UpdateInspectionFormCommand,
  InspectionFormFilter,
} from "../types/inspectionFormType";

export const inspectionFormApi = {
  getInspectionForms: async (filter: InspectionFormFilter = {}) => {
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
      items: InspectionForm[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>(`/api/InspectionForms${queryString ? `?${queryString}` : ""}`);
    return data;
  },

  getInspectionForm: async (id: number) => {
    const { data } = await agent.get<InspectionForm>(
      `/api/InspectionForms/${id}`,
    );
    return data;
  },

  createInspectionForm: async (command: CreateInspectionFormCommand) => {
    const { data } = await agent.post("/api/InspectionForms", command);
    return data;
  },

  updateInspectionForm: async (command: UpdateInspectionFormCommand) => {
    const { data } = await agent.put(
      `/api/InspectionForms/${command.inspectionFormID}`,
      command,
    );
    return data;
  },

  deactivateInspectionForm: async (id: number) => {
    const { data } = await agent.patch(`/api/InspectionForms/${id}/deactivate`);
    return data;
  },
};
