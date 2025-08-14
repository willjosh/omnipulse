import {
  getStatusDot,
  getVehicleIcon,
  getVehicleTypeLabel,
} from "@/features/vehicle/utils/vehicleEnumHelper";
import { ServiceProgramVehicleWithDetails } from "@/features/service-program/api/serviceProgramVehicleApi";
import React from "react";

export const serviceProgramVehicleTableColumns = [
  {
    key: "vehiclename",
    header: "Vehicle Name",
    sortable: true,
    render: (spVehicle: ServiceProgramVehicleWithDetails) => (
      <div className="flex items-center">
        <div className="flex-shrink-0">
          <div className="size-8 rounded bg-gray-100 flex items-center justify-center text-sm">
            {getVehicleIcon(spVehicle.vehicle.vehicleType)}
          </div>
        </div>
        <div className="ml-3">
          <div className="text-sm font-medium text-gray-900">
            {spVehicle.vehicle.name}
          </div>
          <div className="text-xs text-gray-500">
            {spVehicle.vehicle.year} {spVehicle.vehicle.make}{" "}
            {spVehicle.vehicle.model}
          </div>
        </div>
      </div>
    ),
  },
  {
    key: "group",
    header: "Group",
    sortable: false,
    render: (spVehicle: ServiceProgramVehicleWithDetails) => (
      <span className="text-sm text-gray-900">
        {spVehicle.vehicle.vehicleGroupName || "No Group"}
      </span>
    ),
  },
  {
    key: "status",
    header: "Status",
    sortable: false,
    render: (spVehicle: ServiceProgramVehicleWithDetails) => (
      <div className="flex items-center">
        <div
          className={`size-2 rounded-full mr-2 ${getStatusDot(spVehicle.vehicle.status)}`}
        ></div>
        <span className="text-sm">{spVehicle.vehicle.statusLabel}</span>
      </div>
    ),
  },
  {
    key: "odometer",
    header: "Odometer",
    sortable: false,
    render: (spVehicle: ServiceProgramVehicleWithDetails) => (
      <span className="text-sm text-primary hover:text-blue-800 cursor-pointer underline">
        {spVehicle.vehicle.mileage.toLocaleString()} mi
      </span>
    ),
  },
  {
    key: "type",
    header: "Type",
    sortable: false,
    render: (spVehicle: ServiceProgramVehicleWithDetails) => (
      <span className="text-sm text-gray-600">
        {getVehicleTypeLabel(spVehicle.vehicle.vehicleType)}
      </span>
    ),
  },
  {
    key: "licenseplate",
    header: "License Plate",
    sortable: false,
    render: (spVehicle: ServiceProgramVehicleWithDetails) => (
      <span className="text-sm text-gray-900">
        {spVehicle.vehicle.licensePlate || "—"}
      </span>
    ),
  },
  {
    key: "assignedtechnician",
    header: "Operator",
    sortable: false,
    render: (spVehicle: ServiceProgramVehicleWithDetails) =>
      spVehicle.vehicle.assignedTechnicianName ? (
        <div className="flex items-center">
          <div className="flex-shrink-0 h-6 w-6">
            <div className="size-6 rounded-full bg-blue-100 flex items-center justify-center">
              <span className="text-xs font-medium text-blue-800">
                {spVehicle.vehicle.assignedTechnicianName
                  .split(" ")
                  .map((n: string) => n[0])
                  .join("")}
              </span>
            </div>
          </div>
          <span className="ml-2 text-sm">
            {spVehicle.vehicle.assignedTechnicianName}
          </span>
        </div>
      ) : (
        <span className="text-gray-400 italic text-sm">Unassigned</span>
      ),
  },
  {
    key: "addedat",
    header: "Added to Program",
    sortable: true,
    render: (spVehicle: ServiceProgramVehicleWithDetails) => (
      <span className="text-sm text-gray-600">
        {new Date(spVehicle.addedAt).toLocaleDateString()}
      </span>
    ),
  },
];
