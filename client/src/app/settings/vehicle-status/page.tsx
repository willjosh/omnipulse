"use client";
import React from "react";
import { VehicleStatusList } from "@/features/vehicle-status/components/VehicleStatusList";

const VehicleStatusPage = () => {
  return (
    <div className="flex justify-center h-screen overflow-hidden">
      <VehicleStatusList />
    </div>
  );
};

export default VehicleStatusPage;
