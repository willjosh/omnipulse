export enum InventoryActionType {
  VIEW = "view",
  EDIT = "edit",
  ARCHIVE = "archive",
}

export const INVENTORY_ACTION_CONFIG = {
  [InventoryActionType.VIEW]: {
    label: "View Details",
    variant: "default" as const,
  },
  [InventoryActionType.EDIT]: {
    label: "Edit Item",
    variant: "default" as const,
  },
  [InventoryActionType.ARCHIVE]: {
    label: "Archive",
    variant: "danger" as const,
  },
} as const;

export const INVENTORY_ACTION_SETS = {
  STANDARD: [
    InventoryActionType.VIEW,
    InventoryActionType.EDIT,
    InventoryActionType.ARCHIVE,
  ],
  READ_ONLY: [InventoryActionType.VIEW],
} as const;
