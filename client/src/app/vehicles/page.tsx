"use client";
import React from "react";
import VehicleList from "../../features/vehicle/components/VehicleList";

const Vehicles = () => {
  return (
    <div className="flex justify-center h-screen overflow-hidden">
      <VehicleList />
    </div>
  );
};

export default Vehicles;
