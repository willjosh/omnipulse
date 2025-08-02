import {
  FuelTypeEnum,
  PaymentMethodEnum,
  FuelPurchaseStatusEnum,
} from "./fuelPurchaseEnum";

export interface FuelPurchase {
  id: number;
  vehicleId: number;
  vehicleName?: string;
  purchasedByUserId: string;
  purchasedByUserName?: string;
  purchaseDate: string;
  odometerReading: number;
  volume: number;
  pricePerUnit: number;
  totalCost: number;
  fuelStation: string;
  receiptNumber: string;
  notes?: string | null;
  fuelType?: FuelTypeEnum;
  paymentMethod?: PaymentMethodEnum;
  status: FuelPurchaseStatusEnum;
  createdAt?: string;
  updatedAt?: string;
}

export interface FuelPurchaseWithLabels
  extends Omit<FuelPurchase, "fuelType" | "paymentMethod" | "status"> {
  fuelType: number;
  fuelTypeLabel: string;
  fuelTypeEnum: FuelTypeEnum;
  paymentMethod: number;
  paymentMethodLabel: string;
  paymentMethodEnum: PaymentMethodEnum;
  status: number;
  statusLabel: string;
  statusEnum: FuelPurchaseStatusEnum;
}

export interface CreateFuelPurchaseCommand {
  vehicleId: number;
  purchasedByUserId: string;
  purchaseDate: string;
  odometerReading: number;
  volume: number;
  pricePerUnit: number;
  fuelStation: string;
  receiptNumber: string;
  notes?: string | null;
  fuelType?: FuelTypeEnum;
  paymentMethod?: PaymentMethodEnum;
}

export interface UpdateFuelPurchaseCommand {
  fuelPurchaseId: number;
  vehicleId: number;
  purchasedByUserId: string;
  purchaseDate: string;
  odometerReading: number;
  volume: number;
  pricePerUnit: number;
  fuelStation: string;
  receiptNumber: string;
  notes?: string | null;
  fuelType?: FuelTypeEnum;
  paymentMethod?: PaymentMethodEnum;
}

export interface FuelPurchaseFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
  VehicleId?: number;
  StartDate?: string;
  EndDate?: string;
  FuelType?: FuelTypeEnum;
  Status?: FuelPurchaseStatusEnum;
}
