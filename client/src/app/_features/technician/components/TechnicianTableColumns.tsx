import { Technician } from "@/app/_hooks/technician/technicianType";
import React from "react";

export const technicianTableColumns = [
  {
    key: "firstName",
    header: "Name",
    sortable: true,
    render: (technician: Technician) => (
      <div className="flex items-center">
        <div className="flex-shrink-0">
          <div className="size-8 rounded-full bg-blue-100 flex items-center justify-center">
            <span className="text-xs font-medium text-blue-800">
              {technician.firstName[0]}
              {technician.lastName[0]}
            </span>
          </div>
        </div>
        <div className="ml-3">
          <div className="text-sm font-medium text-gray-900">
            {technician.firstName} {technician.lastName}
          </div>
        </div>
      </div>
    ),
  },
  {
    key: "email",
    header: "Email",
    sortable: false,
    render: (technician: Technician) => (
      <span className="text-sm text-gray-600">{technician.email}</span>
    ),
  },
  {
    key: "hireDate",
    header: "Hire Date",
    sortable: true,
    render: (technician: Technician) => {
      const hireDate = new Date(technician.hireDate);
      return (
        <span className="text-sm text-gray-900">
          {hireDate.toLocaleDateString()}
        </span>
      );
    },
  },
  {
    key: "isActive",
    header: "Status",
    sortable: false,
    render: (technician: Technician) => (
      <div className="flex items-center">
        <div
          className={`size-2 rounded-full mr-2 ${
            technician.isActive ? "bg-green-400" : "bg-red-400"
          }`}
        ></div>
        <span className="text-sm">
          {technician.isActive ? "Active" : "Inactive"}
        </span>
      </div>
    ),
  },
  {
    key: "userType",
    header: "User Type",
    sortable: false,
    render: (_technician: Technician) => (
      <span className="text-sm text-gray-900">Technician</span>
    ),
  },
];
