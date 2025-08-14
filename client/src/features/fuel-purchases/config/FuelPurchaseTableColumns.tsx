import { FuelPurchaseWithLabels } from "../types/fuelPurchaseType";
import React from "react";
import { formatEmptyValueWithUnknown } from "@/utils/emptyValueUtils";

export const fuelPurchaseTableColumns = [
  {
    key: "vehicleName",
    header: "Vehicle",
    sortable: true,
    render: (fuelPurchase: FuelPurchaseWithLabels) => (
      <div className="flex items-center">
        <div className="flex-shrink-0">
          <div className="size-8 rounded bg-blue-100 flex items-center justify-center text-sm">
            🚛
          </div>
        </div>
        <div className="ml-3">
          <div className="text-sm font-medium text-gray-900">
            {fuelPurchase.vehicleName || `Vehicle ${fuelPurchase.vehicleId}`}
          </div>
        </div>
      </div>
    ),
  },
  {
    key: "purchaseDate",
    header: "Purchase Date",
    sortable: true,
    render: (fuelPurchase: FuelPurchaseWithLabels) => (
      <div className="text-sm text-gray-900">
        {new Date(fuelPurchase.purchaseDate).toLocaleDateString()}
      </div>
    ),
  },
  {
    key: "fuelStation",
    header: "Fuel Station",
    sortable: true,
    render: (fuelPurchase: FuelPurchaseWithLabels) => (
      <div className="text-sm text-gray-900">{fuelPurchase.fuelStation}</div>
    ),
  },
  {
    key: "volume",
    header: "Volume (L)",
    sortable: true,
    render: (fuelPurchase: FuelPurchaseWithLabels) => (
      <div className="text-sm text-gray-900">
        {fuelPurchase.volume.toFixed(2)}
      </div>
    ),
  },
  {
    key: "pricePerUnit",
    header: "Price/Unit",
    sortable: true,
    render: (fuelPurchase: FuelPurchaseWithLabels) => (
      <div className="text-sm text-gray-900">
        ${fuelPurchase.pricePerUnit.toFixed(2)}
      </div>
    ),
  },
  {
    key: "totalCost",
    header: "Total Cost",
    sortable: true,
    render: (fuelPurchase: FuelPurchaseWithLabels) => (
      <div className="text-sm font-medium text-gray-900">
        ${fuelPurchase.totalCost.toFixed(2)}
      </div>
    ),
  },
  {
    key: "odometerReading",
    header: "Odometer",
    sortable: true,
    render: (fuelPurchase: FuelPurchaseWithLabels) => (
      <div className="text-sm text-gray-900">
        {fuelPurchase.odometerReading.toLocaleString()} km
      </div>
    ),
  },
  {
    key: "receiptNumber",
    header: "Receipt #",
    sortable: true,
    render: (fuelPurchase: FuelPurchaseWithLabels) => (
      <div className="text-sm text-gray-900 font-mono">
        {fuelPurchase.receiptNumber}
      </div>
    ),
  },
];
