import {
  InspectionFormItemTypeEnum,
  VehicleConditionEnum,
} from "./inspectionEnum";

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

export interface InspectionItem {
  inspectionFormItemID: number;
  passed: boolean;
  comment?: string | null;
  snapshotItemLabel: string;
  snapshotItemDescription?: string | null;
  snapshotItemInstructions?: string | null;
  snapshotIsRequired: boolean;
  snapshotInspectionFormItemType: InspectionFormItemTypeEnum;
}

export interface InspectionItemWithLabels
  extends Omit<InspectionItem, "snapshotInspectionFormItemType"> {
  snapshotInspectionFormItemType: number;
  snapshotInspectionFormItemTypeLabel: string;
  snapshotInspectionFormItemTypeEnum: InspectionFormItemTypeEnum;
}

export interface VehicleInfo {
  id: number;
  name: string;
  licensePlate: string;
  vin: string;
}

export interface TechnicianInfo {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
}

export interface InspectionFormInfo {
  id: number;
  title: string;
  description?: string | null;
  isActive: boolean;
}

export interface SingleInspection {
  id: number;
  inspectionFormID: number;
  vehicleID: number;
  technicianID: string;
  inspectionStartTime: string;
  inspectionEndTime: string;
  snapshotFormTitle: string;
  snapshotFormDescription?: string | null;
  odometerReading?: number | null;
  inspectionItems: InspectionItem[];
  vehicleCondition: VehicleConditionEnum;
  notes?: string | null;
  vehicle: VehicleInfo;
  technician: TechnicianInfo;
  inspectionForm: InspectionFormInfo;
  createdAt: string;
  updatedAt: string;
}

export interface SingleInspectionWithLabels
  extends Omit<SingleInspection, "vehicleCondition" | "inspectionItems"> {
  vehicleCondition: number;
  vehicleConditionLabel: string;
  vehicleConditionEnum: VehicleConditionEnum;
  inspectionItems: InspectionItemWithLabels[];
}

export interface InspectionFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
}
