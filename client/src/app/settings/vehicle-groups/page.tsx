"use client";
import React from "react";
import { VehicleGroupList } from "@/features/vehicle-group/components/VehicleGroupList";

const VehicleGroupsPage = () => {
  return (
    <div className="flex justify-center h-screen overflow-hidden">
      <VehicleGroupList />
    </div>
  );
};

export default VehicleGroupsPage;
