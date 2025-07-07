"use client";
import React, { useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { Edit, ArrowLeft } from "lucide-react";
import TabNavigation from "@/app/_features/shared/TabNavigation";
import PrimaryButton from "@/app/_features/shared/button/PrimaryButton";
import Loading from "@/app/_features/shared/Loading";
import {
  getStatusDot,
  getStatusColor,
  getVehicleIcon,
} from "@/app/_utils/vehicleEnumHelper";
import OptionButton from "@/app/_features/shared/button/OptionButton";
import { useVehicles } from "@/app/hooks/Vehicle/useVehicles";

const VehicleDetailsPage = () => {
  const params = useParams();
  const router = useRouter();
  const [activeTab, setActiveTab] = useState("details");

  // Extract id from params and use it to fetch vehicle data
  const vehicleId = params.id as string;
  const { vehicle, isLoadingVehicle } = useVehicles(undefined, vehicleId);

  if (isLoadingVehicle) {
    return <Loading />;
  }

  if (!vehicle) {
    return (
      <div className="min-h-screen max-w-7xl shadow border-b border-gray-200 bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-xl font-semibold text-gray-900 mb-2">
            Vehicle Not Found
          </h2>
          <p className="text-gray-600 mb-4">
            {"The vehicle you're looking for doesn't exist."}
          </p>
          <PrimaryButton onClick={() => router.push("/vehicles")}>
            Back to Vehicles
          </PrimaryButton>
        </div>
      </div>
    );
  }

  const tabs = [
    { key: "details", label: "Details", count: undefined },
    { key: "specifications", label: "Specifications", count: undefined },
    { key: "financial", label: "Financial", count: undefined },
    { key: "lifecycle", label: "Lifecycle", count: undefined },
    { key: "maintenance", label: "Maintenance", count: undefined },
    { key: "more", label: "More", count: undefined },
  ];

  const handleEdit = () => {
    router.push(`/vehicles/${vehicle.id}/edit`);
  };

  const handleBack = () => {
    router.push("/vehicles");
  };

  const renderDetailTab = () => (
    <div className="grid grid-cols-2 gap-6">
      <div className="bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-semibold text-gray-900">Details</h2>
            <button className="text-sm text-gray-500">All Fields</button>
          </div>
        </div>
        <div className="p-3 space-y-2">
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Name</span>
            <span className="text-sm text-gray-900">{vehicle.Name}</span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Mileage</span>
            <span className="text-sm text-gray-900">
              {vehicle.Mileage?.toLocaleString() || "—"} mi
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Engine Hours
            </span>
            <span className="text-sm text-gray-900">
              {vehicle.EngineHours || "—"} hrs
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Status</span>
            <div className="flex items-center">
              <div
                className={`w-2 h-2 rounded-full mr-2 ${getStatusDot(vehicle.Status)}`}
              ></div>
              <span className="text-sm text-gray-900">{vehicle.Status}</span>
            </div>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Group</span>
            <span className="text-sm text-blue-600">
              {vehicle.VehicleGroupName}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Assigned Technician
            </span>
            <span className="text-sm text-gray-500">
              {vehicle.AssignedTechnicianName || "Unassigned"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Type</span>
            <span className="text-sm text-blue-600">
              {vehicle.VehicleTypeLabel}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Fuel Type</span>
            <span className="text-sm text-gray-900">
              {vehicle.FuelTypeLabel || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              VIN Number
            </span>
            <span className="text-sm text-gray-900 font-mono">
              {vehicle.VIN}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              License Plate
            </span>
            <span className="text-sm text-gray-900">
              {vehicle.LicensePlate}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              License Plate Expiration
            </span>
            <span className="text-sm text-gray-900">
              {vehicle.LicensePlateExpirationDate}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Year</span>
            <span className="text-sm text-gray-500">{vehicle.Year}</span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Make</span>
            <span className="text-sm text-gray-500">{vehicle.Make}</span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Model</span>
            <span className="text-sm text-gray-500">{vehicle.Model}</span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Trim</span>
            <span className="text-sm text-gray-500">{vehicle.Trim || "—"}</span>
          </div>
          <div className="flex justify-between items-center py-3">
            <span className="text-sm font-medium text-gray-600">Location</span>
            <span className="text-sm text-gray-500">
              {vehicle.Location || "—"}
            </span>
          </div>
        </div>
      </div>
      <div className="space-y-6">
        <div className="bg-white rounded-3xl border border-gray-200">
          <div className="p-4 border-b border-gray-200">
            <div className="flex items-center justify-between">
              <h3 className="text-lg font-semibold text-gray-900">
                Service Reminders
              </h3>
              <div className="flex items-center space-x-4 text-sm text-primary hover:text-blue-600">
                <button>+ Add Service Reminder</button>
                <button>View All</button>
              </div>
            </div>
          </div>
          <div className="p-4 text-center text-gray-500">
            <p className="text-sm">
              There are no Service Reminders due soon for this Vehicle
            </p>
          </div>
        </div>
        <div className="bg-white rounded-3xl border border-gray-200">
          <div className="p-4 border-b border-gray-200">
            <div className="flex items-center justify-between">
              <h3 className="text-lg font-semibold text-gray-900">
                Renewal Reminders
              </h3>
              <div className="flex items-center space-x-4 text-sm text-primary hover:text-blue-600">
                <button>+ Add Renewal Reminder</button>
                <button>View All</button>
              </div>
            </div>
          </div>
          <div className="p-4 text-center text-gray-500">
            <p className="text-sm">
              There are no Renewal Reminders due soon for this Vehicle
            </p>
          </div>
        </div>
      </div>
    </div>
  );

  const renderSpecificationTab = () => (
    <div className="grid grid-cols-3 gap-6">
      <div className="bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900">Engine</h3>
        </div>
        <div className="p-3 space-y-2">
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Fuel Capacity
            </span>
            <span className="text-sm text-gray-900">
              {vehicle.FuelCapacity ? `${vehicle.FuelCapacity} L` : "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3">
            <span className="text-sm font-medium text-gray-600">Fuel Type</span>
            <span className="text-sm text-gray-900">
              {vehicle.FuelType || "—"}
            </span>
          </div>
        </div>
      </div>
      <div className="col-span-2 bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900">
            Vehicle Information
          </h3>
        </div>
        <div className="p-3 space-y-2">
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Vehicle Type
            </span>
            <span className="text-sm text-gray-900">{vehicle.VehicleType}</span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Make & Model
            </span>
            <span className="text-sm text-gray-900">
              {vehicle.Year} {vehicle.Make} {vehicle.Model}
            </span>
          </div>
          <div className="flex justify-between items-center py-3">
            <span className="text-sm font-medium text-gray-600">Trim</span>
            <span className="text-sm text-gray-900">{vehicle.Trim || "—"}</span>
          </div>
        </div>
      </div>
    </div>
  );

  const renderFinancialTab = () => (
    <div className="grid grid-cols-2 gap-6">
      <div className="bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900">
            Purchase Details
          </h3>
        </div>
        <div className="p-3 space-y-2">
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Purchase Date
            </span>
            <span className="text-sm text-gray-900">
              {vehicle.PurchaseDate || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3">
            <span className="text-sm font-medium text-gray-600">
              Purchase Price
            </span>
            <span className="text-sm text-gray-900">
              {vehicle.PurchasePrice
                ? `$${vehicle.PurchasePrice.toLocaleString()}`
                : "—"}
            </span>
          </div>
        </div>
      </div>
    </div>
  );

  const renderLifecycleTab = () => (
    <div className="grid grid-cols-3 gap-6">
      <div className="bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900">
            Lifecycle Information
          </h3>
        </div>
        <div className="p-3 space-y-2">
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Status</span>
            <span className="text-sm text-gray-900">{vehicle.StatusLabel}</span>
          </div>
          <div className="flex justify-between items-center py-3">
            <span className="text-sm font-medium text-gray-600">
              Current Mileage
            </span>
            <span className="text-sm text-gray-900">
              {vehicle.Mileage ? `${vehicle.Mileage.toLocaleString()} mi` : "—"}
            </span>
          </div>
        </div>
      </div>
    </div>
  );

  const renderMaintenanceTab = () => (
    <div className="grid grid-cols-2 gap-6">
      <div className="bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900">
            Maintenance Information
          </h3>
        </div>
        <div className="p-3 space-y-2">
          <div className="flex justify-between items-center py-3">
            <span className="text-sm font-medium text-gray-600">
              Engine Hours
            </span>
            <span className="text-sm text-gray-900">
              {vehicle.EngineHours || "—"} hrs
            </span>
          </div>
        </div>
      </div>
      <div className="bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900">
            Recent Service
          </h3>
        </div>
        <div className="p-4 text-center text-gray-500">
          <p className="text-sm">No recent service records</p>
        </div>
      </div>
    </div>
  );

  const renderTab = () => {
    switch (activeTab) {
      case "details":
        return renderDetailTab();
      case "specifications":
        return renderSpecificationTab();
      case "financial":
        return renderFinancialTab();
      case "lifecycle":
        return renderLifecycleTab();
      case "maintenance":
        return renderMaintenanceTab();
      case "more":
        return (
          <div className="bg-white rounded-3xl border border-gray-200 p-8 text-center">
            <p className="text-gray-500">Future implementation</p>
          </div>
        );
      default:
        return renderDetailTab();
    }
  };

  return (
    <div className="min-h-screen max-w-7xl shadow border-b border-gray-200 bg-gray-50">
      <div className="bg-white">
        <div className="px-6 py-4">
          <div className="flex items-center space-x-4 mb-4">
            <button
              onClick={handleBack}
              className="flex items-center text-gray-600 hover:text-gray-900"
            >
              <ArrowLeft className="w-4 h-4 mr-1" />
              <span className="text-sm">Vehicles</span>
            </button>
          </div>
          <div className="flex items-start justify-between">
            <div className="flex items-start space-x-4">
              <div className="w-22 h-22 bg-gray-100 rounded-3xl flex items-center justify-center">
                {getVehicleIcon(vehicle.VehicleType)}
              </div>
              <div>
                <h1 className="text-2xl font-bold text-gray-900 mb-1">
                  {vehicle.Name}
                </h1>
                <p className="text-gray-600 mb-2">
                  {vehicle.VehicleType} • {vehicle.Year} {vehicle.Make}{" "}
                  {vehicle.Model} • {vehicle.VIN} • {vehicle.LicensePlate}
                </p>
                <div className="flex items-center space-x-4 text-sm">
                  <span className="text-gray-600">
                    {vehicle.Mileage?.toLocaleString() || "—"} mi
                  </span>
                  <div
                    className={`flex items-center px-2 py-1 rounded-full text-xs font-medium ${getStatusColor(vehicle.Status)}`}
                  >
                    <div
                      className={`w-1.5 h-1.5 rounded-full mr-1.5 ${getStatusDot(vehicle.Status)}`}
                    ></div>
                    {vehicle.Status}
                  </div>
                  <span className="text-gray-600">
                    {vehicle.VehicleGroupName}
                  </span>
                  <span className="text-gray-500">
                    {vehicle.AssignedTechnicianName || "Unassigned"}
                  </span>
                </div>
              </div>
            </div>
            <div className="flex items-center space-x-3">
              <OptionButton className="w-5 h-5" />
              <PrimaryButton onClick={handleEdit}>
                <Edit className="w-4 h-4 mr-2" />
                Edit
              </PrimaryButton>
            </div>
          </div>
        </div>
        <div className="w-full px-6">
          <TabNavigation
            tabs={tabs}
            activeTab={activeTab}
            onTabChange={setActiveTab}
          />
        </div>
        <div className="p-6">{renderTab()}</div>
      </div>
    </div>
  );
};

export default VehicleDetailsPage;
