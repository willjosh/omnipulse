import React from "react";
import { VehicleListItem } from "../../types/VehicleListTypes";
import { getVehicleIcon, getStatusDot } from "@/app/_utils/helper";

export const vehicleTableColumns = [
  {
    key: "name",
    header: "Name",
    width: "220px",
    sortable: true,
    render: (vehicle: VehicleListItem) => (
      <div className="flex items-center">
        <div className="flex-shrink-0">
          <div className="size-8 rounded bg-gray-100 flex items-center justify-center text-sm">
            {getVehicleIcon(vehicle.type)}
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
  { key: "year", header: "Year", width: "80px", sortable: true },
  { key: "make", header: "Make", width: "130px", sortable: true },
  { key: "model", header: "Model", width: "120px", sortable: true },
  {
    key: "vin",
    header: "VIN",
    width: "180px",
    render: (vehicle: VehicleListItem) => (
      <span className="text-sm">{vehicle.vin}</span>
    ),
  },
  {
    key: "status",
    header: "Status",
    width: "135px",
    sortable: true,
    render: (vehicle: VehicleListItem) => (
      <div className="flex items-center">
        <div
          className={`size-2 rounded-full mr-2 ${getStatusDot(vehicle.status)}`}
        ></div>
        <span>{vehicle.status}</span>
      </div>
    ),
  },
  { key: "type", header: "Type", width: "100px", sortable: true },
  { key: "group", header: "Group", width: "100px", sortable: true },
  {
    key: "currentMeter",
    header: "Meter",
    width: "120px",
    render: (vehicle: VehicleListItem) => (
      <span className="text-primary hover:text-blue-800 cursor-pointer underline">
        {vehicle.currentMeter.toLocaleString()} {vehicle.meterUnit}
      </span>
    ),
  },
  {
    key: "licensePlate",
    header: "License Plate",
    width: "140px",
    sortable: true,
  },
  {
    key: "assignedOperator",
    header: "Operator",
    width: "160px",
    render: (vehicle: VehicleListItem) =>
      vehicle.assignedOperator ? (
        <div className="flex items-center">
          <div className="flex-shrink-0 h-6 w-6">
            <div className="size-6 rounded-full bg-blue-100 flex items-center justify-center">
              <span className="text-xs font-medium text-blue-800">
                {vehicle.assignedOperator
                  .split(" ")
                  .map(name => name[0])
                  .join("")}
              </span>
            </div>
          </div>
          <span className="ml-2">{vehicle.assignedOperator}</span>
        </div>
      ) : (
        <span className="text-gray-400 italic">Unassigned</span>
      ),
  },
];
