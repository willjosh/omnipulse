import { VehicleConditionEnum } from "./inspectionEnum";

export interface Inspection {
  id: number;
  inspectionFormID: number;
  vehicleID: number;
  technicianID: string;
  inspectionStartTime: string;
  inspectionEndTime: string;
  odometerReading?: number | null;
  vehicleCondition: VehicleConditionEnum;
  notes?: string | null;
  snapshotFormTitle: string;
  snapshotFormDescription?: string | null;
  inspectionFormName: string;
  vehicleName: string;
  technicianName: string;
  inspectionItemsCount: number;
  passedItemsCount: number;
  failedItemsCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface InspectionWithLabels
  extends Omit<Inspection, "vehicleCondition"> {
  vehicleCondition: number;
  vehicleConditionLabel: string;
  vehicleConditionEnum: VehicleConditionEnum;
}

export interface CreateInspectionPassFailItemCommand {
  inspectionFormItemID: number;
  passed: boolean;
  comment?: string | null;
}

export interface CreateInspectionCommand {
  inspectionFormID: number;
  vehicleID: number;
  technicianID: string;
  inspectionStartTime: string;
  inspectionEndTime: string;
  odometerReading?: number | null;
  vehicleCondition: VehicleConditionEnum;
  notes?: string | null;
  inspectionItems: CreateInspectionPassFailItemCommand[];
}

export interface InspectionFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
}
