"use client";
import React from "react";
import { VehicleStatusList } from "@/app/_features/shared/vehicle-status";

const VehicleStatusPage = () => {
  return (
    <div className="flex justify-center h-screen overflow-hidden">
      <VehicleStatusList />
    </div>
  );
};

export default VehicleStatusPage;
