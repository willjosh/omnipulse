import { CreateWorkOrderCommand } from "../types/workOrderType";
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
  scheduledCompletionDate: null,
  actualCompletionDate: null,
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

  // Validate completion dates
  if (
    form.scheduledCompletionDate &&
    form.scheduledStartDate &&
    new Date(form.scheduledCompletionDate) <= new Date(form.scheduledStartDate)
  ) {
    errors.scheduledCompletionDate =
      "Expected completion date must be after scheduled start date";
  }

  if (
    form.actualCompletionDate &&
    form.actualStartDate &&
    new Date(form.actualCompletionDate) <= new Date(form.actualStartDate)
  ) {
    errors.actualCompletionDate =
      "Actual completion date must be after actual start date";
  }

  if (
    form.actualCompletionDate &&
    form.scheduledCompletionDate &&
    new Date(form.actualCompletionDate) < new Date(form.scheduledCompletionDate)
  ) {
    errors.actualCompletionDate =
      "Actual completion date cannot be before expected completion date";
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
    scheduledCompletionDate: form.scheduledCompletionDate,
    actualCompletionDate: form.actualCompletionDate,
    startOdometer: form.startOdometer,
    endOdometer: form.endOdometer,
    issueIdList: form.issueIdList.length > 0 ? form.issueIdList : null,
    workOrderLineItems:
      form.workOrderLineItems.length > 0 ? form.workOrderLineItems : null,
  };
};
