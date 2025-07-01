export interface WorkOrderDetailsData {
  vehicleId: string;
  status: "Open" | "Pending" | "Completed";
  description: string;
  assignedTo: string;
}

export interface WorkOrderSchedulingData {
  scheduledStartDate: string;
  scheduledStartTime: string;
  actualStartDate: string;
  actualStartTime: string;
  expectedCompletionDate: string;
  expectedCompletionTime: string;
}

export interface WorkOrderOdometerData {
  startOdometer: number | null;
}

export interface WorkOrderFormData {
  details: WorkOrderDetailsData;
  scheduling: WorkOrderSchedulingData;
  odometer: WorkOrderOdometerData;
}

export enum WorkOrderFormSection {
  DETAILS = "details",
  SCHEDULING = "scheduling",
  ODOMETER = "odometer",
}

export const createEmptyWorkOrderDetailsData = (): WorkOrderDetailsData => ({
  vehicleId: "",
  status: "Open",
  description: "",
  assignedTo: "",
});

export const createEmptyWorkOrderSchedulingData =
  (): WorkOrderSchedulingData => ({
    scheduledStartDate: "",
    scheduledStartTime: "",
    actualStartDate: "",
    actualStartTime: "",
    expectedCompletionDate: "",
    expectedCompletionTime: "",
  });

export const createEmptyWorkOrderOdometerData = (): WorkOrderOdometerData => ({
  startOdometer: null,
});

export const createEmptyWorkOrderFormData = (): WorkOrderFormData => ({
  details: createEmptyWorkOrderDetailsData(),
  scheduling: createEmptyWorkOrderSchedulingData(),
  odometer: createEmptyWorkOrderOdometerData(),
});
