"use client";
import React from "react";
import TechnicianForm from "./TechnicianForm";

interface TechnicianFormContainerProps {
  mode: "create" | "edit";
  technicianId?: string;
}

const TechnicianFormContainer: React.FC<TechnicianFormContainerProps> = ({
  mode,
  technicianId,
}) => {
  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-2xl mx-auto py-8 px-4">
        <div className="bg-white rounded-lg shadow-lg p-6">
          <div className="mb-6">
            <h1 className="text-2xl font-bold text-gray-900">
              {mode === "create" ? "Add New Technician" : "Edit Technician"}
            </h1>
            <p className="text-gray-600 mt-1">
              {mode === "create"
                ? "Create a new technician profile"
                : "Update technician information"}
            </p>
          </div>
          <TechnicianForm mode={mode} technicianId={technicianId} />
        </div>
      </div>
    </div>
  );
};

export default TechnicianFormContainer;
