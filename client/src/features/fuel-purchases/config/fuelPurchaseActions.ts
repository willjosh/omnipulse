export enum FuelPurchaseActionType {
  VIEW = "view",
  EDIT = "edit",
  DELETE = "delete",
}

export const FUEL_PURCHASE_ACTION_CONFIG = {
  [FuelPurchaseActionType.VIEW]: {
    label: "View Details",
    variant: "default" as const,
  },
  [FuelPurchaseActionType.EDIT]: { label: "Edit", variant: "default" as const },
  [FuelPurchaseActionType.DELETE]: {
    label: "Delete",
    variant: "danger" as const,
  },
} as const;

export const ACTION_SETS = {
  STANDARD: [
    FuelPurchaseActionType.VIEW,
    FuelPurchaseActionType.EDIT,
    FuelPurchaseActionType.DELETE,
  ],
  READ_ONLY: [FuelPurchaseActionType.VIEW],
} as const;
