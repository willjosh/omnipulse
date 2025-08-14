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
    <div className="space-y-6 p-6">
      {/* Header */}
      <div className="flex flex-col space-y-4 sm:flex-row sm:items-center sm:justify-between sm:space-y-0">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Vehicle Status</h1>
          <p className="mt-1 text-sm text-gray-500">
            Manage and view vehicle status types
          </p>
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
