import { Technician } from "@/app/_hooks/technician/technicianType";
import React from "react";

export const technicianTableColumns = [
  {
    key: "FirstName",
    header: "Name",
    sortable: true,
    render: (technician: Technician) => (
      <div className="flex items-center">
        <div className="flex-shrink-0">
          <div className="size-8 rounded-full bg-blue-100 flex items-center justify-center">
            <span className="text-xs font-medium text-blue-800">
              {technician.FirstName[0]}
              {technician.LastName[0]}
            </span>
          </div>
        </div>
        <div className="ml-3">
          <div className="text-sm font-medium text-gray-900">
            {technician.FirstName} {technician.LastName}
          </div>
        </div>
      </div>
    ),
  },
  {
    key: "Email",
    header: "Email",
    sortable: false,
    render: (technician: Technician) => (
      <span className="text-sm text-gray-600">{technician.Email}</span>
    ),
  },
  {
    key: "HireDate",
    header: "Hire Date",
    sortable: false,
    render: (technician: Technician) => {
      const hireDate = new Date(technician.HireDate);
      return (
        <span className="text-sm text-gray-900">
          {hireDate.toLocaleDateString()}
        </span>
      );
    },
  },
  {
    key: "IsActive",
    header: "Status",
    sortable: false,
    render: (technician: Technician) => (
      <div className="flex items-center">
        <div
          className={`size-2 rounded-full mr-2 ${
            technician.IsActive ? "bg-green-400" : "bg-red-400"
          }`}
        ></div>
        <span className="text-sm">
          {technician.IsActive ? "Active" : "Inactive"}
        </span>
      </div>
    ),
  },
  {
    key: "UserType",
    header: "User Type",
    sortable: false,
    render: (technician: Technician) => (
      <span className="text-sm text-gray-900">Technician</span>
    ),
  },
];
