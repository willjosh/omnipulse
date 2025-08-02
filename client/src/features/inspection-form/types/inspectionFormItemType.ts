import { InspectionFormItemTypeEnum } from "./inspectionFormEnum";

export interface InspectionFormItem {
  id: number;
  inspectionFormID: number;
  itemLabel: string;
  itemDescription?: string | null;
  itemInstructions?: string | null;
  isRequired: boolean;
  inspectionFormItemTypeEnum: InspectionFormItemTypeEnum;
  createdAt: string;
  updatedAt: string;
}

export interface InspectionFormItemWithLabels
  extends Omit<InspectionFormItem, "inspectionFormItemTypeEnum"> {
  inspectionFormItemType: number;
  inspectionFormItemTypeLabel: string;
  inspectionFormItemTypeEnum: InspectionFormItemTypeEnum;
}

export interface CreateInspectionFormItemCommand {
  inspectionFormID: number;
  itemLabel: string;
  itemDescription?: string | null;
  itemInstructions?: string | null;
  inspectionFormItemTypeEnum: InspectionFormItemTypeEnum;
  isRequired: boolean;
}

export interface UpdateInspectionFormItemCommand
  extends Omit<
    CreateInspectionFormItemCommand,
    "inspectionFormID" | "inspectionFormItemTypeEnum"
  > {
  inspectionFormItemID: number;
}

export interface InspectionFormItemFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
}
