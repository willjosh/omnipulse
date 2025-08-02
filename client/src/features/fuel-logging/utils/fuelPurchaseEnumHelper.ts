import {
  FuelTypeEnum,
  PaymentMethodEnum,
  FuelPurchaseStatusEnum,
} from "@/features/fuel-logging/types/fuelPurchaseEnum";

export function getFuelTypeLabel(fuelType?: FuelTypeEnum | null): string {
  switch (fuelType) {
    case FuelTypeEnum.GASOLINE:
      return "Gasoline";
    case FuelTypeEnum.DIESEL:
      return "Diesel";
    case FuelTypeEnum.ELECTRIC:
      return "Electric";
    case FuelTypeEnum.HYBRID:
      return "Hybrid";
    case FuelTypeEnum.PROPANE:
      return "Propane";
    case FuelTypeEnum.NATURAL_GAS:
      return "Natural Gas";
    default:
      return "Unknown";
  }
}

export function getPaymentMethodLabel(
  paymentMethod?: PaymentMethodEnum | null,
): string {
  switch (paymentMethod) {
    case PaymentMethodEnum.CASH:
      return "Cash";
    case PaymentMethodEnum.CREDIT_CARD:
      return "Credit Card";
    case PaymentMethodEnum.DEBIT_CARD:
      return "Debit Card";
    case PaymentMethodEnum.FLEET_CARD:
      return "Fleet Card";
    case PaymentMethodEnum.INVOICE:
      return "Invoice";
    default:
      return "Unknown";
  }
}

export function getFuelPurchaseStatusLabel(
  status?: FuelPurchaseStatusEnum | null,
): string {
  switch (status) {
    case FuelPurchaseStatusEnum.ACTIVE:
      return "Active";
    case FuelPurchaseStatusEnum.ARCHIVED:
      return "Archived";
    default:
      return "Unknown";
  }
}
