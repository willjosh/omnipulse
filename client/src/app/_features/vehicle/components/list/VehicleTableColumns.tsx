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
    width: "220px",
    sortable: true,
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
    width: "80px",
    sortable: true,
    render: (vehicle: VehicleWithLabels) => vehicle.Year,
  },
  { key: "Make", header: "Make", width: "130px", sortable: true },
  { key: "Model", header: "Model", width: "120px", sortable: true },
  {
    key: "VIN",
    header: "VIN",
    width: "180px",
    render: (vehicle: VehicleWithLabels) => (
      <span className="text-sm">{vehicle.VIN}</span>
    ),
  },
  {
    key: "Status",
    header: "Status",
    width: "135px",
    sortable: true,
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
    width: "100px",
    sortable: true,
    render: (vehicle: VehicleWithLabels) =>
      getVehicleTypeLabel(vehicle.VehicleType),
  },
  { key: "VehicleGroupName", header: "Group", width: "100px", sortable: true },
  {
    key: "Mileage",
    header: "Meter",
    width: "120px",
    render: (vehicle: VehicleWithLabels) => (
      <span className="text-primary hover:text-blue-800 cursor-pointer underline">
        {vehicle.Mileage.toLocaleString()} mi{" "}
      </span>
    ),
  },
  {
    key: "LicensePlate",
    header: "License Plate",
    width: "140px",
    sortable: true,
  },
  {
    key: "AssignedTechnicianName",
    header: "Operator",
    width: "160px",
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
