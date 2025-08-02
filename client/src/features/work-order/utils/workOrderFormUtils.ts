import {
  CreateWorkOrderCommand,
  WorkOrderLineItem,
} from "../types/workOrderType";
import { WorkOrderDetailsFormValues } from "../components/WorkOrderDetailsForm";
import { WorkOrderIssuesFormValues } from "../components/WorkOrderIssuesForm";
import { WorkOrderLineItemsFormValues } from "../components/WorkOrderLineItemsForm";
import {
  WorkTypeEnum,
  PriorityLevelEnum,
  WorkOrderStatusEnum,
} from "../types/workOrderEnum";

export interface WorkOrderFormState
  extends WorkOrderDetailsFormValues,
    WorkOrderIssuesFormValues,
    WorkOrderLineItemsFormValues {}

export const emptyWorkOrderFormState: WorkOrderFormState = {
  title: "",
  description: null,
  vehicleID: 0,
  workOrderType: WorkTypeEnum.SCHEDULED,
  priorityLevel: PriorityLevelEnum.MEDIUM,
  status: WorkOrderStatusEnum.CREATED,
  assignedToUserID: "",
  scheduledStartDate: null,
  actualStartDate: null,
  startOdometer: 0,
  endOdometer: null,
  issueIdList: [],
  workOrderLineItems: [],
};

export const validateWorkOrderForm = (
  form: WorkOrderFormState,
): { [key: string]: string } => {
  const errors: { [key: string]: string } = {};

  if (!form.title?.trim()) {
    errors.title = "Title is required";
  }

  if (!form.vehicleID) {
    errors.vehicleID = "Vehicle is required";
  }

  if (!form.assignedToUserID) {
    errors.assignedToUserID = "Assigned To is required";
  }

  if (form.startOdometer < 0) {
    errors.startOdometer = "Start odometer must be a positive number";
  }

  if (
    form.endOdometer !== null &&
    form.endOdometer !== undefined &&
    form.endOdometer < form.startOdometer
  ) {
    errors.endOdometer = "End odometer cannot be less than start odometer";
  }

  return errors;
};

export const mapFormToCreateWorkOrderCommand = (
  form: WorkOrderFormState,
): CreateWorkOrderCommand => {
  return {
    vehicleID: form.vehicleID,
    assignedToUserID: form.assignedToUserID,
    title: form.title,
    description: form.description,
    workOrderType: form.workOrderType,
    priorityLevel: form.priorityLevel,
    status: form.status,
    scheduledStartDate: form.scheduledStartDate,
    actualStartDate: form.actualStartDate,
    startOdometer: form.startOdometer,
    endOdometer: form.endOdometer,
    issueIdList: form.issueIdList.length > 0 ? form.issueIdList : null,
    workOrderLineItems:
      form.workOrderLineItems.length > 0 ? form.workOrderLineItems : null,
  };
};
