export enum VehicleActionType {
  VIEW = "view",
  EDIT = "edit",
  ARCHIVE = "archive",
  DUPLICATE = "duplicate",
  EXPORT = "export",
}

export const VEHICLE_ACTION_CONFIG = {
  [VehicleActionType.VIEW]: {
    label: "View Details",
    variant: "default" as const,
  },
  [VehicleActionType.EDIT]: {
    label: "Edit Vehicle",
    variant: "default" as const,
  },
  [VehicleActionType.ARCHIVE]: { label: "Archive", variant: "danger" as const },
  [VehicleActionType.DUPLICATE]: {
    label: "Duplicate",
    variant: "default" as const,
  },
  [VehicleActionType.EXPORT]: { label: "Export", variant: "default" as const },
} as const;

export const ACTION_SETS = {
  STANDARD: [
    VehicleActionType.VIEW,
    VehicleActionType.EDIT,
    VehicleActionType.ARCHIVE,
  ],
  ADMIN: [
    VehicleActionType.VIEW,
    VehicleActionType.EDIT,
    VehicleActionType.ARCHIVE,
    VehicleActionType.DUPLICATE,
  ],
  READ_ONLY: [VehicleActionType.VIEW],
} as const;
