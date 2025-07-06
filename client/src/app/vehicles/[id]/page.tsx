"use client";
import React, { useState, useEffect } from "react";
import { useParams, useRouter } from "next/navigation";
import { Edit, ArrowLeft } from "lucide-react";
import TabNavigation from "@/app/_features/shared/TabNavigation";
import PrimaryButton from "@/app/_features/shared/button/PrimaryButton";
import Loading from "@/app/_features/shared/Loading";
import {
  getStatusDot,
  getStatusColor,
  getVehicleIcon,
} from "@/app/_utils/helper";
import OptionButton from "@/app/_features/shared/button/OptionButton";

interface VehicleDetailData {
  id: string;
  name: string;
  year: number;
  make: string;
  model: string;
  vin: string;
  type: string;
  status: string;
  group: string;
  licensePlate: string;
  fuelType: string;
  currentMeter: number;
  details: {
    vehicleName: string;
    telematicsDevice: string;
    trim: string;
    registrationState: string;
    labels: string;
  };
  maintenance: { serviceProgram: string };
  lifecycle: {
    inServiceDate: string | null;
    inServiceOdometer: number | null;
    estimatedServiceLifeMonths: number | null;
    estimatedServiceLifeMeter: number | null;
    estimatedResaleValue: number | null;
    outOfServiceDate: string | null;
    outOfServiceOdometer: number | null;
  };
  financial: {
    purchaseVendor: string;
    purchaseDate: string;
    purchasePrice: number | null;
    odometer: number | null;
    notes: string;
    loanLeaseType: string;
  };
  specifications: {
    width: number | null;
    height: number | null;
    length: number | null;
    curbWeight: number | null;
    towingCapacity: number | null;
    epaCity: string;
    epaHighway: string;
    epaCombined: string;
    engineSummary: string;
    cylinders: number | null;
    fuelTank1Capacity: number | null;
    transmissionType: string;
    transmissionGears: string;
    driveType: string;
  };
}

const mockVehicleData: VehicleDetailData = {
  id: "2100",
  name: "2100 [2016 Ford F-150]",
  year: 2016,
  make: "Ford",
  model: "F-150",
  vin: "1FTFW1EG3GFA31822",
  type: "Pickup Truck",
  status: "Active",
  group: "Sales",
  licensePlate: "1CA7895",
  fuelType: "Petrol",
  currentMeter: 56491,
  details: {
    vehicleName: "2100 [2016 Ford F-150]",
    telematicsDevice: "GPS Tracker 1",
    trim: "XLT",
    registrationState: "QLD",
    labels: "fleet",
  },
  maintenance: { serviceProgram: "Basic Maintenance Program" },
  lifecycle: {
    inServiceDate: "2016-03-15",
    inServiceOdometer: 0,
    estimatedServiceLifeMonths: 96,
    estimatedServiceLifeMeter: 200000,
    estimatedResaleValue: 25000,
    outOfServiceDate: null,
    outOfServiceOdometer: null,
  },
  financial: {
    purchaseVendor: "Ford Dealer",
    purchaseDate: "2016-03-10",
    purchasePrice: 45000,
    odometer: 0,
    notes: "Fleet purchase with extended warranty",
    loanLeaseType: "none",
  },
  specifications: {
    width: 200,
    height: 195,
    length: 579,
    curbWeight: 2100,
    towingCapacity: 5000,
    epaCity: "13.1L/100km",
    epaHighway: "9.8L/100km",
    epaCombined: "11.8L/100km",
    engineSummary: "3.5L V6",
    cylinders: 6,
    fuelTank1Capacity: 98,
    transmissionType: "Automatic",
    transmissionGears: "10-speed",
    driveType: "4WD",
  },
};

const VehicleDetailsPage = () => {
  const params = useParams();
  const router = useRouter();
  const [activeTab, setActiveTab] = useState("details");
  const [vehicleData, setVehicleData] = useState<VehicleDetailData | null>(
    null,
  );

  useEffect(() => {
    setVehicleData(mockVehicleData);
  }, [params.id]);

  if (!vehicleData) {
    return <Loading />;
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
    router.push(`/vehicles/${vehicleData.id}/edit`);
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
            <span className="text-sm text-gray-900">{vehicleData.name}</span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Meter</span>
            <span className="text-sm text-gray-900">
              {vehicleData.currentMeter.toLocaleString()} mi
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Status</span>
            <div className="flex items-center">
              <div
                className={`w-2 h-2 rounded-full mr-2 ${getStatusDot(vehicleData.status)}`}
              ></div>
              <span className="text-sm text-gray-900">
                {vehicleData.status}
              </span>
            </div>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Group</span>
            <span className="text-sm text-blue-600">{vehicleData.group}</span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Operator</span>
            <span className="text-sm text-gray-500">Unassigned</span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Type</span>
            <span className="text-sm text-blue-600">{vehicleData.type}</span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Fuel Type</span>
            <span className="text-sm text-gray-900">
              {vehicleData.fuelType || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              VIN Number
            </span>
            <span className="text-sm text-gray-900 font-mono">
              {vehicleData.vin}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              License Plate
            </span>
            <span className="text-sm text-gray-900">
              {vehicleData.licensePlate}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Year</span>
            <span className="text-sm text-gray-500">{vehicleData.year}</span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Make</span>
            <span className="text-sm text-gray-500">{vehicleData.make}</span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Model</span>
            <span className="text-sm text-gray-500">{vehicleData.model}</span>
          </div>
          <div className="flex justify-between items-center py-3">
            <span className="text-sm font-medium text-gray-600">Trim</span>
            <span className="text-sm text-gray-500">
              {vehicleData.details?.trim || "—"}
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
      <div className="row-span-2 bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900">Engine</h3>
        </div>
        <div className="p-3 space-y-2">
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Summary</span>
            <span className="text-sm text-gray-900">
              {vehicleData.specifications?.engineSummary || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Cylinders</span>
            <span className="text-sm text-gray-900">
              {vehicleData.specifications?.cylinders || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3">
            <span className="text-sm font-medium text-gray-600">Fuel Tank</span>
            <span className="text-sm text-gray-900">
              {vehicleData.specifications?.fuelTank1Capacity
                ? `${vehicleData.specifications.fuelTank1Capacity} L`
                : "—"}
            </span>
          </div>
        </div>
      </div>
      <div className="bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900">Dimensions</h3>
        </div>
        <div className="p-3 space-y-2">
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Length</span>
            <span className="text-sm text-gray-900">
              {vehicleData.specifications?.length
                ? `${vehicleData.specifications.length} cm`
                : "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Width</span>
            <span className="text-sm text-gray-900">
              {vehicleData.specifications?.width
                ? `${vehicleData.specifications.width} cm`
                : "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3">
            <span className="text-sm font-medium text-gray-600">Height</span>
            <span className="text-sm text-gray-900">
              {vehicleData.specifications?.height
                ? `${vehicleData.specifications.height} cm`
                : "—"}
            </span>
          </div>
        </div>
      </div>
      <div className="bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900">Transmission</h3>
        </div>
        <div className="p-3 space-y-2">
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Type</span>
            <span className="text-sm text-gray-900">
              {vehicleData.specifications?.transmissionType || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Gears</span>
            <span className="text-sm text-gray-900">
              {vehicleData.specifications?.transmissionGears || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3">
            <span className="text-sm font-medium text-gray-600">
              Drive Type
            </span>
            <span className="text-sm text-gray-900">
              {vehicleData.specifications?.driveType || "—"}
            </span>
          </div>
        </div>
      </div>
      <div className="bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900">Performance</h3>
        </div>
        <div className="p-3 space-y-2">
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Curb Weight
            </span>
            <span className="text-sm text-gray-900">
              {vehicleData.specifications?.curbWeight
                ? `${vehicleData.specifications.curbWeight} kg`
                : "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3">
            <span className="text-sm font-medium text-gray-600">
              Towing Capacity
            </span>
            <span className="text-sm text-gray-900">
              {vehicleData.specifications?.towingCapacity
                ? `${vehicleData.specifications.towingCapacity} kg`
                : "—"}
            </span>
          </div>
        </div>
      </div>
      <div className="col-span-2 bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900">Fuel Economy</h3>
        </div>
        <div className="p-3">
          <div className="grid grid-cols-3 gap-6">
            <div className="flex justify-between items-center py-3 border-b border-gray-100">
              <span className="text-sm font-medium text-gray-600">City</span>
              <span className="text-sm text-gray-900">
                {vehicleData.specifications?.epaCity || "—"}
              </span>
            </div>
            <div className="flex justify-between items-center py-3 border-b border-gray-100">
              <span className="text-sm font-medium text-gray-600">Highway</span>
              <span className="text-sm text-gray-900">
                {vehicleData.specifications?.epaHighway || "—"}
              </span>
            </div>
            <div className="flex justify-between items-center py-3 border-b border-gray-100">
              <span className="text-sm font-medium text-gray-600">
                Combined
              </span>
              <span className="text-sm text-gray-900">
                {vehicleData.specifications?.epaCombined || "—"}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );

  const renderFinancialTab = () => (
    <div className="grid grid-cols-2 gap-6">
      <div className="row-span-2 bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900">
            Purchase Details
          </h3>
        </div>
        <div className="p-3 space-y-2">
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Purchase Vendor
            </span>
            <span className="text-sm text-gray-900">
              {vehicleData.financial?.purchaseVendor || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Purchase Date
            </span>
            <span className="text-sm text-gray-900">
              {vehicleData.financial?.purchaseDate || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Purchase Price
            </span>
            <span className="text-sm text-gray-900">
              {vehicleData.financial?.purchasePrice
                ? `$${vehicleData.financial.purchasePrice.toLocaleString()}`
                : "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Financing Type
            </span>
            <span className="text-sm text-gray-900 capitalize">
              {vehicleData.financial?.loanLeaseType || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3">
            <span className="text-sm font-medium text-gray-600">
              Odometer at Purchase
            </span>
            <span className="text-sm text-gray-900">
              {vehicleData.financial?.odometer
                ? `${vehicleData.financial.odometer.toLocaleString()} mi`
                : "—"}
            </span>
          </div>
        </div>
      </div>
      <div className="bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900">
            Lifecycle Estimates
          </h3>
        </div>
        <div className="p-3 space-y-2">
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Estimated Resale Value
            </span>
            <span className="text-sm text-gray-900">
              {vehicleData.lifecycle?.estimatedResaleValue
                ? `$${vehicleData.lifecycle.estimatedResaleValue.toLocaleString()}`
                : "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Service Life (Months)
            </span>
            <span className="text-sm text-gray-900">
              {vehicleData.lifecycle?.estimatedServiceLifeMonths || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3">
            <span className="text-sm font-medium text-gray-600">
              Service Life (Miles)
            </span>
            <span className="text-sm text-gray-900">
              {vehicleData.lifecycle?.estimatedServiceLifeMeter
                ? `${vehicleData.lifecycle.estimatedServiceLifeMeter.toLocaleString()} mi`
                : "—"}
            </span>
          </div>
        </div>
      </div>
      {vehicleData.financial?.notes && (
        <div className="bg-white rounded-3xl border border-gray-200">
          <div className="p-4 border-b border-gray-200">
            <h3 className="text-lg font-semibold text-gray-900">Notes</h3>
          </div>
          <div className="p-4">
            <div className="bg-gray-50 rounded-3xl p-4">
              <p className="text-sm text-gray-700">
                {vehicleData.financial.notes}
              </p>
            </div>
          </div>
        </div>
      )}
    </div>
  );

  const renderLifecycleTab = () => (
    <div className="grid grid-cols-3 gap-6">
      <div className="bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900">In-Service</h3>
        </div>
        <div className="p-3 space-y-2">
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              In-Service Date
            </span>
            <span className="text-sm text-gray-900">
              {vehicleData.lifecycle?.inServiceDate || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3">
            <span className="text-sm font-medium text-gray-600">
              In-Service Odometer
            </span>
            <span className="text-sm text-gray-900">
              {vehicleData.lifecycle?.inServiceOdometer
                ? `${vehicleData.lifecycle.inServiceOdometer.toLocaleString()} mi`
                : "—"}
            </span>
          </div>
        </div>
      </div>
      <div className="col-span-2 row-span-2 bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900">
            Vehicle Life Estimates
          </h3>
        </div>
        <div className="p-3 space-y-2">
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Estimated Service Life (Months)
            </span>
            <span className="text-sm text-gray-900">
              {vehicleData.lifecycle?.estimatedServiceLifeMonths || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Estimated Service Life (Meter)
            </span>
            <span className="text-sm text-gray-900">
              {vehicleData.lifecycle?.estimatedServiceLifeMeter
                ? `${vehicleData.lifecycle.estimatedServiceLifeMeter.toLocaleString()} mi`
                : "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3">
            <span className="text-sm font-medium text-gray-600">
              Estimated Resale Value
            </span>
            <span className="text-sm text-gray-900">
              {vehicleData.lifecycle?.estimatedResaleValue
                ? `$${vehicleData.lifecycle.estimatedResaleValue.toLocaleString()}`
                : "—"}
            </span>
          </div>
        </div>
      </div>
      <div className="bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900">
            Out-of-Service
          </h3>
        </div>
        <div className="p-3 space-y-2">
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Out-of-Service Date
            </span>
            <span className="text-sm text-gray-900">
              {vehicleData.lifecycle?.outOfServiceDate || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3">
            <span className="text-sm font-medium text-gray-600">
              Out-of-Service Odometer
            </span>
            <span className="text-sm text-gray-900">
              {vehicleData.lifecycle?.outOfServiceOdometer
                ? `${vehicleData.lifecycle.outOfServiceOdometer.toLocaleString()} mi`
                : "—"}
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
            Service Program
          </h3>
        </div>
        <div className="p-3 space-y-2">
          <div className="flex justify-between items-center py-3">
            <span className="text-sm font-medium text-gray-600">Program</span>
            <span className="text-sm text-gray-900">
              {vehicleData.maintenance?.serviceProgram || "—"}
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
      <div className="col-span-2 bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900">
            Upcoming Maintenance
          </h3>
        </div>
        <div className="p-4 text-center text-gray-500">
          <p className="text-sm">No upcoming maintenance scheduled</p>
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
                {getVehicleIcon("bus")}
              </div>
              <div>
                <h1 className="text-2xl font-bold text-gray-900 mb-1">
                  {vehicleData.name}
                </h1>
                <p className="text-gray-600 mb-2">
                  {vehicleData.type} • {vehicleData.year} {vehicleData.make}{" "}
                  {vehicleData.model} • {vehicleData.vin} •{" "}
                  {vehicleData.licensePlate}
                </p>
                <div className="flex items-center space-x-4 text-sm">
                  <span className="text-gray-600">
                    {vehicleData.currentMeter.toLocaleString()} mi
                  </span>
                  <div
                    className={`flex items-center px-2 py-1 rounded-full text-xs font-medium ${getStatusColor(vehicleData.status)}`}
                  >
                    <div
                      className={`w-1.5 h-1.5 rounded-full mr-1.5 ${getStatusDot(vehicleData.status)}`}
                    ></div>
                    {vehicleData.status}
                  </div>
                  <span className="text-gray-600">{vehicleData.group}</span>
                  <span className="text-gray-500">Unassigned</span>
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
