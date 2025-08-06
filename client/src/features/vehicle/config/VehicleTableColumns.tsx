import {
  getStatusDot,
  getVehicleIcon,
  getVehicleTypeLabel,
} from "@/features/vehicle/utils/vehicleEnumHelper";
import { VehicleWithLabels } from "@/features/vehicle/types/vehicleType";
import React from "react";

export const vehicleTableColumns = [
  {
    key: "name",
    header: "Name",
    sortable: true,
    render: (vehicle: VehicleWithLabels) => (
      <div className="flex items-center">
        <div className="flex-shrink-0">
          <div className="size-8 rounded bg-gray-100 flex items-center justify-center text-sm">
            {getVehicleIcon(vehicle.vehicleType)}
          </div>
        </div>
        <div className="ml-3">
          <div className="text-sm font-medium text-gray-900">
            {vehicle.name}
          </div>
        </div>
      </div>
    ),
  },
  {
    key: "year",
    header: "Year",
    sortable: true,
    render: (vehicle: VehicleWithLabels) => vehicle.year,
  },
  { key: "make", header: "Make", sortable: true },
  { key: "model", header: "Model", sortable: true },
  {
    key: "vin",
    header: "VIN",
    sortable: false,
    render: (vehicle: VehicleWithLabels) => (
      <span className="text-sm">{vehicle.vin}</span>
    ),
  },
  {
    key: "status",
    header: "Status",
    sortable: true,
    render: (vehicle: VehicleWithLabels) => (
      <div className="flex items-center">
        <div
          className={`size-2 rounded-full mr-2 ${getStatusDot(vehicle.status)}`}
        ></div>
        <span>{vehicle.statusLabel}</span>
      </div>
    ),
  },
  {
    key: "vehicleType",
    header: "Type",
    sortable: false,
    render: (vehicle: VehicleWithLabels) =>
      getVehicleTypeLabel(vehicle.vehicleType),
  },
  { key: "vehicleGroupName", header: "Group", sortable: false },
  {
    key: "mileage",
    header: "Meter",
    sortable: true,
    render: (vehicle: VehicleWithLabels) => (
      <span className="text-primary hover:text-blue-800 cursor-pointer underline">
        {vehicle.mileage.toLocaleString()} mi{" "}
      </span>
    ),
  },
  { key: "licensePlate", header: "License Plate", sortable: false },
  {
    key: "assignedTechnicianName",
    header: "Operator",
    sortable: false,
    render: (vehicle: VehicleWithLabels) =>
      vehicle.assignedTechnicianName ? (
        <div className="flex items-center">
          <div className="flex-shrink-0 h-6 w-6">
            <div className="size-6 rounded-full bg-blue-100 flex items-center justify-center">
              <span className="text-xs font-medium text-blue-800">
                {vehicle.assignedTechnicianName
                  .split(" ")
                  .map((n: string) => n[0])
                  .join("")}{" "}
              </span>
            </div>
          </div>
          <span className="ml-2">{vehicle.assignedTechnicianName}</span>
        </div>
      ) : (
        <span className="text-gray-400 italic">Unassigned</span>
      ),
  },
];
