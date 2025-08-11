import { agent } from "@/lib/axios/agent";
import {
  Inspection,
  InspectionWithLabels,
  SingleInspection,
  SingleInspectionWithLabels,
  CreateInspectionCommand,
  InspectionFilter,
} from "../types/inspectionType";
import {
  VehicleConditionEnum,
  InspectionFormItemTypeEnum,
} from "../types/inspectionEnum";
import {
  getVehicleConditionLabel,
  getInspectionFormItemTypeLabel,
} from "../utils/inspectionEnumHelper";

export const convertInspectionData = (
  inspection: Inspection,
): InspectionWithLabels => ({
  ...inspection,
  vehicleCondition: inspection.vehicleCondition as number,
  vehicleConditionLabel: getVehicleConditionLabel(inspection.vehicleCondition),
  vehicleConditionEnum: inspection.vehicleCondition as VehicleConditionEnum,
});

export const convertSingleInspectionData = (
  inspection: SingleInspection,
): SingleInspectionWithLabels => ({
  ...inspection,
  vehicleCondition: inspection.vehicleCondition as number,
  vehicleConditionLabel: getVehicleConditionLabel(inspection.vehicleCondition),
  vehicleConditionEnum: inspection.vehicleCondition as VehicleConditionEnum,
  inspectionItems: inspection.inspectionItems.map(item => ({
    ...item,
    snapshotInspectionFormItemType:
      item.snapshotInspectionFormItemType as number,
    snapshotInspectionFormItemTypeLabel: getInspectionFormItemTypeLabel(
      item.snapshotInspectionFormItemType,
    ),
    snapshotInspectionFormItemTypeEnum:
      item.snapshotInspectionFormItemType as InspectionFormItemTypeEnum,
  })),
});

export const inspectionApi = {
  getInspections: async (filter: InspectionFilter = {}) => {
    const queryParams = new URLSearchParams();
    if (filter.PageNumber)
      queryParams.append("PageNumber", filter.PageNumber.toString());
    if (filter.PageSize)
      queryParams.append("PageSize", filter.PageSize.toString());
    if (filter.Search) queryParams.append("Search", filter.Search);
    if (filter.SortBy) queryParams.append("SortBy", filter.SortBy);
    if (filter.SortDescending !== undefined)
      queryParams.append("SortDescending", filter.SortDescending.toString());
    const queryString = queryParams.toString();
    const { data } = await agent.get<{
      items: Inspection[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>(`/api/Inspections${queryString ? `?${queryString}` : ""}`);
    return data;
  },

  getInspection: async (id: number) => {
    const { data } = await agent.get<SingleInspection>(
      `/api/Inspections/${id}`,
    );
    return data;
  },

  createInspection: async (command: CreateInspectionCommand) => {
    const { data } = await agent.post("/api/Inspections", command);
    return data;
  },
};
