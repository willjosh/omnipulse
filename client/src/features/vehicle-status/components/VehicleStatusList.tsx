"use client";

import React, { useState } from "react";
import { useVehicleStatuses } from "../hooks/useVehicleStatus";
import { VehicleStatus } from "../types/vehicleStatusType";
import { Settings, Info } from "lucide-react";
import { Loading } from "@/components/ui/Feedback";
import { DataTable } from "@/components/ui/Table";
import { vehicleStatusTableColumns } from "@/features/vehicle-status/components/VehicleStatusTableColumns";

export const VehicleStatusList: React.FC = () => {
  const { vehicleStatuses, isPending, isError, error } = useVehicleStatuses();

  if (isPending) {
    return <Loading />;
  }

  if (isError) {
    return (
      <div className="text-center py-8">
        <p className="text-red-500">
          Error loading vehicle statuses: {error?.message}
        </p>
      </div>
    );
  }

  return (
    <div className="ml-6 mt-6 space-y-6">
      {/* Header */}
      <div className="flex flex-col space-y-4 sm:flex-row sm:items-center sm:justify-between sm:space-y-0">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Vehicle Status</h1>
          <p className="mt-1 text-sm text-gray-500">
            Manage and view vehicle status types
          </p>
        </div>
      </div>

      {/* Information Banner */}
      <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
        <div className="flex">
          <Info className="w-5 h-5 text-blue-600 mt-0.5 mr-3 flex-shrink-0" />
          <div className="text-sm text-blue-700">
            <p className="font-medium">Predefined Vehicle Statuses</p>
            <p className="mt-1">
              These vehicle statuses are predefined system values based on your
              fleet management needs. Custom status creation will be available
              when the backend API is implemented.
            </p>
          </div>
        </div>
      </div>

      <DataTable<VehicleStatus>
        data={vehicleStatuses || []}
        columns={vehicleStatusTableColumns}
        actions={[]}
        showActions={false}
        loading={isPending}
        fixedLayout={false}
        getItemId={status =>
          status.id ? status.id.toString() : `temp-${Date.now()}`
        }
        emptyState={
          <div className="text-center py-8">
            <Settings className="w-12 h-12 text-gray-400 mx-auto mb-4" />
            <p className="text-gray-500">No vehicle statuses found</p>
          </div>
        }
      />
    </div>
  );
};
