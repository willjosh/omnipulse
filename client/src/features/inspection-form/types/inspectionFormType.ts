export interface InspectionForm {
  id: number;
  title: string;
  description?: string | null;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  inspectionCount: number;
  inspectionFormItemCount: number;
}

export interface CreateInspectionFormCommand {
  title: string;
  description?: string | null;
  isActive: boolean;
}

export interface UpdateInspectionFormCommand
  extends CreateInspectionFormCommand {
  inspectionFormID: number;
}

export interface InspectionFormFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
}
