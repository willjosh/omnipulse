import {
  LineItemTypeEnum,
  WorkTypeEnum,
  PriorityLevelEnum,
  WorkOrderStatusEnum,
} from "./workOrderEnum";

export interface WorkOrderLineItem {
  id: number;
  workOrderID: number;
  itemType: LineItemTypeEnum;
  quantity: number;
  description?: string | null;
  inventoryItemID?: number | null;
  inventoryItemName: string;
  assignedToUserID: string;
  assignedToUserName: string;
  subTotal: number;
  laborCost: number;
  itemCost: number;
  serviceTaskID: number;
  serviceTaskName: string;
}

export interface WorkOrderLineItemWithLabels
  extends Omit<WorkOrderLineItem, "itemType"> {
  itemType: number;
  itemTypeLabel: string;
  itemTypeEnum: LineItemTypeEnum;
}

export interface CreateWorkOrderLineItem {
  inventoryItemID?: number | null;
  serviceTaskID: number;
  assignedToUserID?: string | null;
  itemType: LineItemTypeEnum;
  description?: string | null;
  quantity: number;
  unitPrice?: number | null;
  hourlyRate?: number | null;
  laborHours?: number | null;
}

export interface WorkOrder {
  id: number;
  title: string;
  description?: string | null;
  workOrderType: WorkTypeEnum;
  priorityLevel: PriorityLevelEnum;
  status: WorkOrderStatusEnum;
  scheduledStartDate?: string | null;
  actualStartDate?: string | null;
  startOdometer: number;
  endOdometer?: number | null;
  totalCost?: number | null;
  totalLaborCost?: number | null;
  totalItemCost?: number | null;
  vehicleID: number;
  vehicleName: string;
  assignedToUserID: string;
  assignedToUserName: string;
  workOrderLineItems: WorkOrderLineItem[];
}

export interface WorkOrderWithLabels
  extends Omit<
    WorkOrder,
    "workOrderType" | "priorityLevel" | "status" | "workOrderLineItems"
  > {
  workOrderType: number;
  workOrderTypeLabel: string;
  workOrderTypeEnum: WorkTypeEnum;
  priorityLevel: number;
  priorityLevelLabel: string;
  priorityLevelEnum: PriorityLevelEnum;
  status: number;
  statusLabel: string;
  statusEnum: WorkOrderStatusEnum;
  workOrderLineItems: WorkOrderLineItemWithLabels[];
}

export interface CreateWorkOrderCommand {
  vehicleID: number;
  assignedToUserID: string;
  title: string;
  description?: string | null;
  workOrderType: WorkTypeEnum;
  priorityLevel: PriorityLevelEnum;
  status: WorkOrderStatusEnum;
  scheduledStartDate?: string | null;
  actualStartDate?: string | null;
  scheduledCompletionDate?: string | null;
  actualCompletionDate?: string | null;
  startOdometer: number;
  endOdometer?: number | null;
  issueIdList?: number[] | null;
  workOrderLineItems?: CreateWorkOrderLineItem[] | null;
}

export interface UpdateWorkOrderCommand extends CreateWorkOrderCommand {
  workOrderID: number;
}

export interface WorkOrderFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
}
