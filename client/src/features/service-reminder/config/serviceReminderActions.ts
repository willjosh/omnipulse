export enum ServiceReminderActionType {
  ADD_WORK_ORDER = "addWorkOrder",
}

export const SERVICE_REMINDER_ACTION_CONFIG = {
  [ServiceReminderActionType.ADD_WORK_ORDER]: {
    label: "Add Work Order",
    variant: "default" as const,
  },
} as const;

export const ACTION_SETS = {
  STANDARD: [ServiceReminderActionType.ADD_WORK_ORDER],
} as const;
