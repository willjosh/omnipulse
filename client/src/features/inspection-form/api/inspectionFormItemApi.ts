import { agent } from "@/lib/axios/agent";
import {
  InspectionFormItem,
  InspectionFormItemWithLabels,
  CreateInspectionFormItemCommand,
  UpdateInspectionFormItemCommand,
  InspectionFormItemFilter,
} from "../types/inspectionFormItemType";
import { InspectionFormItemTypeEnum } from "../types/inspectionFormEnum";
import { getInspectionFormItemTypeLabel } from "../utils/inspectionFormEnumHelper";

export const convertInspectionFormItemData = (
  item: InspectionFormItem,
): InspectionFormItemWithLabels => ({
  ...item,
  inspectionFormItemType: item.inspectionFormItemTypeEnum as number,
  inspectionFormItemTypeLabel: getInspectionFormItemTypeLabel(
    item.inspectionFormItemTypeEnum,
  ),
  inspectionFormItemTypeEnum:
    item.inspectionFormItemTypeEnum as InspectionFormItemTypeEnum,
});

export const inspectionFormItemApi = {
  getInspectionFormItems: async (
    inspectionFormId: number,
    filter: InspectionFormItemFilter = {},
  ) => {
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
      items: InspectionFormItem[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>(
      `/api/InspectionForms/${inspectionFormId}/items${queryString ? `?${queryString}` : ""}`,
    );
    return data;
  },

  getInspectionFormItem: async (inspectionFormId: number, itemId: number) => {
    const { data } = await agent.get<InspectionFormItem>(
      `/api/InspectionForms/${inspectionFormId}/items/${itemId}`,
    );
    return data;
  },

  createInspectionFormItem: async (
    command: CreateInspectionFormItemCommand,
  ) => {
    const { data } = await agent.post(
      `/api/InspectionForms/${command.inspectionFormID}/items`,
      command,
    );
    return data;
  },

  updateInspectionFormItem: async (
    inspectionFormId: number,
    command: UpdateInspectionFormItemCommand,
  ) => {
    const { data } = await agent.put(
      `/api/InspectionForms/${inspectionFormId}/items/${command.inspectionFormItemID}`,
      command,
    );
    return data;
  },

  deactivateInspectionFormItem: async (
    inspectionFormId: number,
    itemId: number,
  ) => {
    const { data } = await agent.patch(
      `/api/InspectionForms/${inspectionFormId}/items/${itemId}/deactivate`,
    );
    return data;
  },
};
