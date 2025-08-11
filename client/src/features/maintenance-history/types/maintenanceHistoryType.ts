export interface MaintenanceHistory {
  maintenanceHistoryID: number;
  vehicleID: number;
  vehicleName: string;
  workOrderID: number;
  serviceTaskID: number[];
  serviceTaskName: string[];
  technicianID: string;
  technicianName: string;
  serviceDate: string;
  mileageAtService: number;
  cost: number;
  labourHours: number;
  notes?: string | null;
}

export interface MaintenanceHistoryWithFormattedDates
  extends MaintenanceHistory {
  serviceDateFormatted: string | null;
}

export interface CreateMaintenanceHistoryCommand {
  workOrderID: number;
  serviceDate: string;
  mileageAtService: number;
  description?: string | null;
  cost: number;
  labourHours: number;
  notes?: string | null;
}

export interface MaintenanceHistoryFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
}
