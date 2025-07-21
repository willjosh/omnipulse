"use client";
import React from "react";
import { VehicleGroupList } from "@/app/_features/shared/vehicle-groups";

const VehicleGroupsPage = () => {
  return (
    <div className="flex justify-center h-screen overflow-hidden">
      <VehicleGroupList />
    </div>
  );
};

export default VehicleGroupsPage;
