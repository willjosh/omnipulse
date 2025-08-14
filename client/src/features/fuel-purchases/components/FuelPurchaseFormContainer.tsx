"use client";
import React from "react";
import FuelPurchaseForm from "./FuelPurchaseForm";

interface FuelPurchaseFormContainerProps {
  mode: "create" | "edit";
  fuelPurchaseId?: string;
}

const FuelPurchaseFormContainer: React.FC<FuelPurchaseFormContainerProps> = ({
  mode,
  fuelPurchaseId,
}) => {
  return (
    <div className="min-h-screen bg-gray-50">
      <FuelPurchaseForm mode={mode} fuelPurchaseId={fuelPurchaseId} />
    </div>
  );
};

export default FuelPurchaseFormContainer;
