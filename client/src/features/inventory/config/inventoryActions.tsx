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
  [InventoryActionType.EDIT]: { label: "Edit", variant: "default" as const },
  [InventoryActionType.ARCHIVE]: {
    label: "Archive",
    variant: "danger" as const,
  },
};
