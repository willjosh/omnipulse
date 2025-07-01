import React from "react";
import { VehicleListItem } from "../../types/VehicleListTypes";

const getStatusDot = (status: string) => {
  switch (status) {
    case "Active":
      return "bg-green-500";
    case "Inactive":
      return "bg-blue-500";
    case "In Shop":
      return "bg-orange-500";
    case "Out of Service":
      return "bg-red-500";
    default:
      return "bg-gray-500";
  }
};

const getVehicleIcon = (type: string) => {
  switch (type.toLowerCase()) {
    case "city bus":
    case "tour bus":
    case "school bus":
      return "🚌";
    case "minibus":
      return "🚐";
    default:
      return "🚗";
  }
};

export const vehicleTableColumns = [
  {
    key: "name",
    header: "Name",
    sortable: true,
    render: (vehicle: VehicleListItem) => (
      <div className="flex items-center">
        <div className="flex-shrink-0 h-8 w-8">
          <div className="h-8 w-8 rounded bg-gray-100 flex items-center justify-center text-sm">
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
  { key: "year", header: "Year", sortable: true },
  { key: "make", header: "Make", sortable: true },
  { key: "model", header: "Model", sortable: true },
  {
    key: "vin",
    header: "VIN",
    render: (vehicle: VehicleListItem) => (
      <span className="font-mono">{vehicle.vin}</span>
    ),
  },
  {
    key: "status",
    header: "Status",
    sortable: true,
    render: (vehicle: VehicleListItem) => (
      <div className="flex items-center">
        <div
          className={`h-2 w-2 rounded-full mr-2 ${getStatusDot(vehicle.status)}`}
        ></div>
        <span>{vehicle.status}</span>
      </div>
    ),
  },
  { key: "type", header: "Type", sortable: true },
  { key: "group", header: "Group", sortable: true },
  {
    key: "currentMeter",
    header: "Current Meter",
    render: (vehicle: VehicleListItem) => (
      <span className="text-blue-600 hover:text-blue-800 cursor-pointer underline">
        {vehicle.currentMeter.toLocaleString()} {vehicle.meterUnit}
      </span>
    ),
  },
  { key: "licensePlate", header: "License Plate", sortable: true },
  {
    key: "assignedOperator",
    header: "Assigned Operator",
    render: (vehicle: VehicleListItem) =>
      vehicle.assignedOperator ? (
        <div className="flex items-center">
          <div className="flex-shrink-0 h-6 w-6">
            <div className="h-6 w-6 rounded-full bg-blue-100 flex items-center justify-center">
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
