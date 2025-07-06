import {
  Vehicle,
  VehicleTypeEnum,
  VehicleStatusEnum,
} from "@/app/hooks/Vehicle/vehicleType";
import React from "react";

const getStatusDot = (status: VehicleStatusEnum) => {
  switch (status) {
    case VehicleStatusEnum.ACTIVE:
      return "bg-green-500";
    case VehicleStatusEnum.INACTIVE:
      return "bg-blue-500";
    case VehicleStatusEnum.MAINTENANCE:
      return "bg-orange-500";
    case VehicleStatusEnum.OUT_OF_SERVICE:
      return "bg-red-500";
    default:
      return "bg-gray-500";
  }
};

const getStatusLabel = (status: VehicleStatusEnum) => {
  switch (status) {
    case VehicleStatusEnum.ACTIVE:
      return "Active";
    case VehicleStatusEnum.INACTIVE:
      return "Inactive";
    case VehicleStatusEnum.MAINTENANCE:
      return "In Shop";
    case VehicleStatusEnum.OUT_OF_SERVICE:
      return "Out of Service";
    default:
      return "Unknown";
  }
};

const getVehicleIcon = (type: VehicleTypeEnum) => {
  switch (type) {
    case VehicleTypeEnum.BUS:
      return "🚌";
    case VehicleTypeEnum.CAR:
      return "🚗";
    case VehicleTypeEnum.TRUCK:
      return "🚛";
    case VehicleTypeEnum.VAN:
      return "🚐";
    case VehicleTypeEnum.MOTORCYCLE:
      return "🏍️";
    case VehicleTypeEnum.TRAILER:
      return "🚚";
    default:
      return "🚗";
  }
};

const getVehicleTypeLabel = (type: VehicleTypeEnum) => {
  switch (type) {
    case VehicleTypeEnum.TRUCK:
      return "Truck";
    case VehicleTypeEnum.VAN:
      return "Van";
    case VehicleTypeEnum.CAR:
      return "Car";
    case VehicleTypeEnum.MOTORCYCLE:
      return "Motorcycle";
    case VehicleTypeEnum.BUS:
      return "Bus";
    case VehicleTypeEnum.HEAVY_VEHICLE:
      return "Heavy Vehicle";
    case VehicleTypeEnum.TRAILER:
      return "Trailer";
    case VehicleTypeEnum.OTHER:
      return "Other";
    default:
      return "Unknown";
  }
};

export const vehicleTableColumns = [
  {
    key: "Name",
    header: "Name",
    width: "220px",
    sortable: true,
    render: (vehicle: Vehicle) => (
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
    render: (vehicle: Vehicle) => vehicle.Year,
  },
  { key: "Make", header: "Make", width: "130px", sortable: true },
  { key: "Model", header: "Model", width: "120px", sortable: true },
  {
    key: "VIN",
    header: "VIN",
    width: "180px",
    render: (vehicle: Vehicle) => (
      <span className="text-sm">{vehicle.VIN}</span>
    ),
  },
  {
    key: "Status",
    header: "Status",
    width: "135px",
    sortable: true,
    render: (vehicle: Vehicle) => (
      <div className="flex items-center">
        <div
          className={`size-2 rounded-full mr-2 ${getStatusDot(vehicle.Status)}`}
        ></div>
        <span>{getStatusLabel(vehicle.Status)}</span>
      </div>
    ),
  },
  {
    key: "VehicleType",
    header: "Type",
    width: "100px",
    sortable: true,
    render: (vehicle: Vehicle) => getVehicleTypeLabel(vehicle.VehicleType),
  },
  { key: "VehicleGroupName", header: "Group", width: "100px", sortable: true },
  {
    key: "Mileage",
    header: "Meter",
    width: "120px",
    render: (vehicle: Vehicle) => (
      <span className="text-primary hover:text-blue-800 cursor-pointer underline">
        {vehicle.Mileage.toLocaleString()} mi{" "}
        {/* Fixed: removed FuelType, added 'mi' */}
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
    render: (vehicle: Vehicle) =>
      vehicle.AssignedTechnicianName ? (
        <div className="flex items-center">
          <div className="flex-shrink-0 h-6 w-6">
            <div className="size-6 rounded-full bg-blue-100 flex items-center justify-center">
              <span className="text-xs font-medium text-blue-800">
                {vehicle.AssignedTechnicianName.split(" ")
                  .map(n => n[0])
                  .join("")}{" "}
                {/* Fixed: get initials */}
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
