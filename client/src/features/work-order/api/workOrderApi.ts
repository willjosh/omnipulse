import { agent } from "@/lib/axios/agent";
import {
  WorkOrder,
  WorkOrderWithLabels,
  WorkOrderLineItem,
  WorkOrderLineItemWithLabels,
  CreateWorkOrderCommand,
  UpdateWorkOrderCommand,
  WorkOrderFilter,
  WorkOrderStatusData,
} from "../types/workOrderType";
import {
  getLineItemTypeLabel,
  getWorkTypeLabel,
  getPriorityLevelLabel,
  getWorkOrderStatusLabel,
} from "../utils/workOrderEnumHelper";
import {
  PriorityLevelEnum,
  WorkOrderStatusEnum,
  WorkTypeEnum,
} from "../types/workOrderEnum";

function formatDate(date?: string | null): string | null {
  if (!date) return null;
  const d = new Date(date);
  return isNaN(d.getTime()) ? null : d.toLocaleString();
}

export const convertWorkOrderLineItemData = (
  item: WorkOrderLineItem,
): WorkOrderLineItemWithLabels => ({
  ...item,
  itemType: item.itemType as number,
  itemTypeLabel: getLineItemTypeLabel(item.itemType),
  itemTypeEnum: item.itemType,
});

export const convertWorkOrderData = (
  workOrder: WorkOrder,
): WorkOrderWithLabels => ({
  ...workOrder,
  workOrderType: workOrder.workOrderType as number,
  workOrderTypeLabel: getWorkTypeLabel(workOrder.workOrderType),
  workOrderTypeEnum: workOrder.workOrderType as WorkTypeEnum,
  priorityLevel: workOrder.priorityLevel as number,
  priorityLevelLabel: getPriorityLevelLabel(workOrder.priorityLevel),
  priorityLevelEnum: workOrder.priorityLevel as PriorityLevelEnum,
  status: workOrder.status as number,
  statusLabel: getWorkOrderStatusLabel(workOrder.status),
  statusEnum: workOrder.status as WorkOrderStatusEnum,
  // Preserve original dates for editing, don't format for display
  scheduledStartDate: workOrder.scheduledStartDate,
  actualStartDate: workOrder.actualStartDate,
  scheduledCompletionDate: workOrder.scheduledCompletionDate,
  actualCompletionDate: workOrder.actualCompletionDate,
  workOrderLineItems: workOrder.workOrderLineItems.map(
    convertWorkOrderLineItemData,
  ),
});

// Function to format work order data for display purposes
export const convertWorkOrderDataForDisplay = (
  workOrder: WorkOrder,
): WorkOrderWithLabels => ({
  ...workOrder,
  workOrderType: workOrder.workOrderType as number,
  workOrderTypeLabel: getWorkTypeLabel(workOrder.workOrderType),
  workOrderTypeEnum: workOrder.workOrderType as WorkTypeEnum,
  priorityLevel: workOrder.priorityLevel as number,
  priorityLevelLabel: getPriorityLevelLabel(workOrder.priorityLevel),
  priorityLevelEnum: workOrder.priorityLevel as PriorityLevelEnum,
  status: workOrder.status as number,
  statusLabel: getWorkOrderStatusLabel(workOrder.status),
  statusEnum: workOrder.status as WorkOrderStatusEnum,
  scheduledStartDate: formatDate(workOrder.scheduledStartDate),
  actualStartDate: formatDate(workOrder.actualStartDate),
  scheduledCompletionDate: formatDate(workOrder.scheduledCompletionDate),
  actualCompletionDate: formatDate(workOrder.actualCompletionDate),
  workOrderLineItems: workOrder.workOrderLineItems.map(
    convertWorkOrderLineItemData,
  ),
});

export const workOrderApi = {
  getWorkOrders: async (filter: WorkOrderFilter = {}) => {
    const queryParams = new URLSearchParams();
    if (filter.PageNumber)
      queryParams.append("PageNumber", filter.PageNumber.toString());
    if (filter.PageSize)
      queryParams.append("PageSize", filter.PageSize.toString());
    if (filter.SortBy) queryParams.append("SortBy", filter.SortBy);
    if (filter.SortDescending !== undefined)
      queryParams.append("SortDescending", filter.SortDescending.toString());
    if (filter.Search) queryParams.append("Search", filter.Search);
    const queryString = queryParams.toString();
    const { data } = await agent.get<{
      items: WorkOrder[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>(`/api/WorkOrders${queryString ? `?${queryString}` : ""}`);
    return data;
  },

  getWorkOrderStatusData: async () => {
    const { data } = await agent.get<WorkOrderStatusData>(
      "/api/WorkOrders/status-data",
    );
    return data;
  },

  getWorkOrder: async (id: number) => {
    const { data } = await agent.get<WorkOrder>(`/api/WorkOrders/${id}`);
    return data;
  },

  createWorkOrder: async (command: CreateWorkOrderCommand) => {
    const { data } = await agent.post("/api/WorkOrders", command);
    return data;
  },

  updateWorkOrder: async (command: UpdateWorkOrderCommand) => {
    const { data } = await agent.put(
      `/api/WorkOrders/${command.workOrderID}`,
      command,
    );
    return data;
  },

  deleteWorkOrder: async (id: number) => {
    const { data } = await agent.delete(`/api/WorkOrders/${id}`);
    return data;
  },

  completeWorkOrder: async (id: number) => {
    const { data } = await agent.patch(`/api/WorkOrders/${id}/complete`);
    return data;
  },

  getWorkOrderInvoicePdf: async (id: number) => {
    const response = await agent.get(`/api/WorkOrders/${id}/invoice.pdf`, {
      responseType: "blob",
    });
    return response.data;
  },
};
