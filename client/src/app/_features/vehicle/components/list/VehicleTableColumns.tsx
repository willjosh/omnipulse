import {
  getStatusDot,
  getVehicleIcon,
  getVehicleTypeLabel,
} from "@/app/_utils/vehicleEnumHelper";
import { VehicleWithLabels } from "@/app/_hooks/vehicle/vehicleType";
import { VehicleStatusEnum } from "@/app/_hooks/vehicle/vehicleEnum";
import React from "react";

export const vehicleTableColumns = [
  {
    key: "Name",
    header: "Name",
    sortable: false,
    render: (vehicle: VehicleWithLabels) => (
      <div className="flex items-center">
        <div className="flex-shrink-0">
          <div className="size-8 rounded bg-gray-100 flex items-center justify-center text-sm">
            {getVehicleIcon(vehicle.VehicleType)}
          </div>
        </div>
        <div className="ml-3">
          <div className="text-sm font-medium text-gray-900">
            {vehicle.Name}
          </div>
        </div>
      </div>
    ),
  },
  {
    key: "Year",
    header: "Year",
    sortable: false,
    render: (vehicle: VehicleWithLabels) => vehicle.Year,
  },
  { key: "Make", header: "Make", sortable: false },
  { key: "Model", header: "Model", sortable: false },
  {
    key: "VIN",
    header: "VIN",
    render: (vehicle: VehicleWithLabels) => (
      <span className="text-sm">{vehicle.VIN}</span>
    ),
  },
  {
    key: "Status",
    header: "Status",
    sortable: false,
    render: (vehicle: VehicleWithLabels) => (
      <div className="flex items-center">
        <div
          className={`size-2 rounded-full mr-2 ${getStatusDot(vehicle.Status)}`}
        ></div>
        <span>{vehicle.StatusLabel}</span>
      </div>
    ),
  },
  {
    key: "VehicleType",
    header: "Type",
    sortable: false,
    render: (vehicle: VehicleWithLabels) =>
      getVehicleTypeLabel(vehicle.VehicleType),
  },
  { key: "VehicleGroupName", header: "Group", sortable: false },
  {
    key: "Mileage",
    header: "Meter",
    render: (vehicle: VehicleWithLabels) => (
      <span className="text-primary hover:text-blue-800 cursor-pointer underline">
        {vehicle.Mileage.toLocaleString()} mi{" "}
      </span>
    ),
  },
  { key: "LicensePlate", header: "License Plate", sortable: false },
  {
    key: "AssignedTechnicianName",
    header: "Operator",
    render: (vehicle: VehicleWithLabels) =>
      vehicle.AssignedTechnicianName ? (
        <div className="flex items-center">
          <div className="flex-shrink-0 h-6 w-6">
            <div className="size-6 rounded-full bg-blue-100 flex items-center justify-center">
              <span className="text-xs font-medium text-blue-800">
                {vehicle.AssignedTechnicianName.split(" ")
                  .map(n => n[0])
                  .join("")}{" "}
              </span>
            </div>
          </div>
          <span className="ml-2">{vehicle.AssignedTechnicianName}</span>
        </div>
      ) : (
        <span className="text-gray-400 italic">Unassigned</span>
      ),
  },
];
