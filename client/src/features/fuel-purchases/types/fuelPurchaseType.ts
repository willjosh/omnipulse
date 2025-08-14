export interface FuelPurchase {
  id: number;
  vehicleId: number;
  vehicleName?: string;
  purchasedByUserId: string;
  purchaseDate: string;
  odometerReading: number;
  volume: number;
  pricePerUnit: number;
  totalCost: number;
  fuelStation: string;
  receiptNumber: string;
  notes?: string | null;
}

export interface FuelPurchaseWithLabels extends FuelPurchase {}

export interface CreateFuelPurchaseCommand {
  vehicleId: number;
  purchasedByUserId: string;
  purchaseDate: string;
  odometerReading: number;
  volume: number;
  pricePerUnit: number;
  totalCost: number;
  fuelStation: string;
  receiptNumber: string;
  notes?: string | null;
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
}

export interface FuelPurchaseFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
}
