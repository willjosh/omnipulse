export interface WorkOrderDetailsData {
  vehicleId: string;
  status: "Open" | "Pending" | "Completed";
  description: string;
  assignedTo: string;
}

export interface WorkOrderSchedulingData {
  issueDate: Date | null;
  issueTime: string;
  issuedBy: string;
  scheduledStartDate: Date | null;
  scheduledStartTime: string;
  actualStartDate: Date | null;
  actualStartTime: string;
  expectedCompletionDate: Date | null;
  expectedCompletionTime: string;
  actualCompletionDate: Date | null;
  actualCompletionTime: string;
  sendReminder: boolean;
}

export interface WorkOrderOdometerData {
  startOdometer: number | null;
  useStartOdometer: boolean;
}

export interface WorkOrderFormData {
  details: WorkOrderDetailsData;
  scheduling: WorkOrderSchedulingData;
  odometer: WorkOrderOdometerData;
  invoiceNumber: string | number;
  poNumber: string | number;
  labels: string[];
  vendor: string;
}

export enum WorkOrderFormSection {
  DETAILS = "details",
  SCHEDULING = "scheduling",
  ODOMETER = "odometer",
}

export const initialFormData: WorkOrderFormData = {
  details: { vehicleId: "", status: "Open", description: "", assignedTo: "" },
  scheduling: {
    issueDate: new Date(),
    issueTime: "",
    issuedBy: "",
    scheduledStartDate: new Date(),
    scheduledStartTime: "",
    actualStartDate: new Date(),
    actualStartTime: "",
    expectedCompletionDate: new Date(),
    expectedCompletionTime: "",
    actualCompletionDate: new Date(),
    actualCompletionTime: "",
    sendReminder: false,
  },
  odometer: { startOdometer: null, useStartOdometer: false },
  invoiceNumber: "",
  poNumber: "",
  labels: [],
  vendor: "",
};
