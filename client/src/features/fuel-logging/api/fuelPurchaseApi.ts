import { agent } from "@/lib/axios/agent";
import {
  FuelPurchase,
  FuelPurchaseWithLabels,
  FuelPurchaseFilter,
  CreateFuelPurchaseCommand,
  UpdateFuelPurchaseCommand,
} from "../types/fuelPurchaseType";
import {
  FuelTypeEnum,
  PaymentMethodEnum,
  FuelPurchaseStatusEnum,
} from "../types/fuelPurchaseEnum";
import {
  getFuelTypeLabel,
  getPaymentMethodLabel,
  getFuelPurchaseStatusLabel,
} from "../utils/fuelPurchaseEnumHelper";

export const convertFuelPurchaseData = (
  fuelPurchase: FuelPurchase,
): FuelPurchaseWithLabels => ({
  ...fuelPurchase,
  fuelType: fuelPurchase.fuelType as number,
  fuelTypeLabel: getFuelTypeLabel(fuelPurchase.fuelType),
  fuelTypeEnum: fuelPurchase.fuelType as FuelTypeEnum,
  paymentMethod: fuelPurchase.paymentMethod as number,
  paymentMethodLabel: getPaymentMethodLabel(fuelPurchase.paymentMethod),
  paymentMethodEnum: fuelPurchase.paymentMethod as PaymentMethodEnum,
  status: fuelPurchase.status as number,
  statusLabel: getFuelPurchaseStatusLabel(fuelPurchase.status),
  statusEnum: fuelPurchase.status as FuelPurchaseStatusEnum,
});

export const fuelPurchaseApi = {
  getFuelPurchases: async (filter: FuelPurchaseFilter = {}) => {
    const queryParams = new URLSearchParams();
    if (filter.PageNumber)
      queryParams.append("PageNumber", filter.PageNumber.toString());
    if (filter.PageSize)
      queryParams.append("PageSize", filter.PageSize.toString());
    if (filter.Search) queryParams.append("Search", filter.Search);
    if (filter.SortBy) queryParams.append("SortBy", filter.SortBy);
    if (filter.SortDescending !== undefined)
      queryParams.append("SortDescending", filter.SortDescending.toString());
    if (filter.VehicleId)
      queryParams.append("VehicleId", filter.VehicleId.toString());
    if (filter.StartDate) queryParams.append("StartDate", filter.StartDate);
    if (filter.EndDate) queryParams.append("EndDate", filter.EndDate);
    if (filter.FuelType !== undefined)
      queryParams.append("FuelType", filter.FuelType.toString());
    if (filter.Status !== undefined)
      queryParams.append("Status", filter.Status.toString());

    const queryString = queryParams.toString();
    const { data } = await agent.get<{
      items: FuelPurchase[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>(`/api/FuelPurchases${queryString ? `?${queryString}` : ""}`);
    return data;
  },

  getFuelPurchase: async (id: number) => {
    const { data } = await agent.get<FuelPurchase>(`/api/FuelPurchases/${id}`);
    return data;
  },

  createFuelPurchase: async (command: CreateFuelPurchaseCommand) => {
    const { data } = await agent.post("/api/FuelPurchases", command);
    return data;
  },

  updateFuelPurchase: async (command: UpdateFuelPurchaseCommand) => {
    const { data } = await agent.put(
      `/api/FuelPurchases/${command.fuelPurchaseId}`,
      command,
    );
    return data;
  },

  archiveFuelPurchase: async (id: number) => {
    const { data } = await agent.patch(`/api/FuelPurchases/${id}/archive`);
    return data;
  },

  deleteFuelPurchase: async (id: number) => {
    const { data } = await agent.delete(`/api/FuelPurchases/${id}`);
    return data;
  },

  getFuelPurchasesByVehicle: async (
    vehicleId: number,
    filter: FuelPurchaseFilter = {},
  ) => {
    const updatedFilter = { ...filter, VehicleId: vehicleId };
    return await fuelPurchaseApi.getFuelPurchases(updatedFilter);
  },

  validateOdometerReading: async (
    vehicleId: number,
    odometerReading: number,
  ) => {
    const { data } = await agent.get<boolean>(
      `/api/FuelPurchases/validate-odometer?vehicleId=${vehicleId}&odometerReading=${odometerReading}`,
    );
    return data;
  },

  validateReceiptNumber: async (receiptNumber: string) => {
    const { data } = await agent.get<boolean>(
      `/api/FuelPurchases/validate-receipt?receiptNumber=${receiptNumber}`,
    );
    return data;
  },
};
