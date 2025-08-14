"use client";
import React from "react";
import { useParams, useRouter } from "next/navigation";
import { Edit, ArrowLeft, Car, Calendar, Receipt, Fuel } from "lucide-react";
import { PrimaryButton } from "@/components/ui/Button";
import { Loading } from "@/components/ui/Feedback";
import { useFuelPurchase } from "@/features/fuel-purchases/hooks/useFuelPurchases";

const FuelPurchaseDetailsPage = () => {
  const params = useParams();
  const router = useRouter();

  const fuelPurchaseId = params.id as string;
  const { fuelPurchase, isPending, isError } = useFuelPurchase(fuelPurchaseId);

  if (isPending) {
    return (
      <div className="flex justify-center items-center mt-60">
        <Loading />
      </div>
    );
  }

  if (isError || !fuelPurchase) {
    return (
      <div className="min-h-screen max-w-7xl shadow border-b border-gray-200 bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-xl font-semibold text-gray-900 mb-2">
            Fuel Purchase Not Found
          </h2>
          <p className="text-gray-600 mb-4">
            The fuel purchase you&apos;re looking for doesn&apos;t exist.
          </p>
          <PrimaryButton onClick={() => router.push("/fuel-purchases")}>
            Back to Fuel Purchases
          </PrimaryButton>
        </div>
      </div>
    );
  }

  const handleEdit = () => {
    router.push(`/fuel-purchases/${fuelPurchaseId}/edit`);
  };

  const handleBack = () => {
    router.push("/fuel-purchases");
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString("en-US", {
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
    }).format(amount);
  };

  const formatNumber = (num: number, decimals: number = 2) => {
    return num.toLocaleString("en-US", {
      minimumFractionDigits: decimals,
      maximumFractionDigits: decimals,
    });
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="bg-white border-b border-gray-200">
        <div className="px-6 py-4">
          <div className="flex items-center space-x-4 mb-4">
            <button
              onClick={handleBack}
              className="flex items-center text-gray-600 hover:text-blue-500"
            >
              <ArrowLeft className="w-4 h-4 mr-1" />
              <span className="text-sm">Fuel Purchases</span>
            </button>
          </div>

          <div className="flex items-start justify-between">
            <div className="flex items-start space-x-4">
              <div className="w-16 h-16 bg-blue-100 rounded-lg flex items-center justify-center">
                <Fuel className="w-8 h-8 text-blue-600" />
              </div>
              <div>
                <h1 className="text-2xl font-bold text-gray-900 mb-1">
                  Fuel Purchase #{fuelPurchase.receiptNumber}
                </h1>
                <p className="text-gray-600 mb-2">
                  {fuelPurchase.vehicleName ||
                    `Vehicle ID: ${fuelPurchase.vehicleId}`}{" "}
                  • {fuelPurchase.fuelStation}
                </p>
                <div className="flex items-center space-x-4 text-sm">
                  <span className="text-gray-600 flex items-center">
                    <Calendar className="w-4 h-4 mr-1" />
                    {formatDate(fuelPurchase.purchaseDate)}
                  </span>
                  <span className="text-gray-600">
                    {formatNumber(fuelPurchase.volume)} L
                  </span>
                  <span className="text-green-600 font-medium">
                    {formatCurrency(fuelPurchase.totalCost)}
                  </span>
                </div>
              </div>
            </div>

            <div className="flex items-center space-x-3">
              <PrimaryButton onClick={handleEdit}>
                <Edit className="w-4 h-4 mr-2" />
                Edit
              </PrimaryButton>
            </div>
          </div>
        </div>
      </div>

      <div className="max-w-7xl mx-auto p-6">
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {/* Purchase Information */}
          <div className="bg-white rounded-lg border border-gray-200">
            <div className="p-4 border-b border-gray-200">
              <h2 className="text-lg font-semibold text-gray-900 flex items-center">
                <Receipt className="w-5 h-5 mr-2" />
                Purchase Information
              </h2>
            </div>
            <div className="p-4 space-y-4">
              <div className="flex justify-between items-center py-2 border-b border-gray-100">
                <span className="text-sm font-medium text-gray-600">
                  Receipt Number
                </span>
                <span className="text-sm text-gray-900 font-mono">
                  {fuelPurchase.receiptNumber}
                </span>
              </div>
              <div className="flex justify-between items-center py-2 border-b border-gray-100">
                <span className="text-sm font-medium text-gray-600">
                  Purchase Date
                </span>
                <span className="text-sm text-gray-900">
                  {formatDate(fuelPurchase.purchaseDate)}
                </span>
              </div>
              <div className="flex justify-between items-center py-2 border-b border-gray-100">
                <span className="text-sm font-medium text-gray-600">
                  Fuel Station
                </span>
                <span className="text-sm text-gray-900">
                  {fuelPurchase.fuelStation}
                </span>
              </div>
              <div className="flex justify-between items-center py-2 border-b border-gray-100">
                <span className="text-sm font-medium text-gray-600">
                  Purchased By
                </span>
                <span className="text-sm text-gray-900">
                  {fuelPurchase.purchasedByUserId}
                </span>
              </div>
              {fuelPurchase.notes && (
                <div className="py-2">
                  <span className="text-sm font-medium text-gray-600 block mb-1">
                    Notes
                  </span>
                  <span className="text-sm text-gray-900 block bg-gray-50 p-2 rounded">
                    {fuelPurchase.notes}
                  </span>
                </div>
              )}
            </div>
          </div>

          {/* Vehicle & Fuel Details */}
          <div className="bg-white rounded-lg border border-gray-200">
            <div className="p-4 border-b border-gray-200">
              <h2 className="text-lg font-semibold text-gray-900 flex items-center">
                <Car className="w-5 h-5 mr-2" />
                Vehicle & Fuel Details
              </h2>
            </div>
            <div className="p-4 space-y-4">
              <div className="flex justify-between items-center py-2 border-b border-gray-100">
                <span className="text-sm font-medium text-gray-600">
                  Vehicle
                </span>
                <span className="text-sm text-gray-900">
                  {fuelPurchase.vehicleName ||
                    `Vehicle ID: ${fuelPurchase.vehicleId}`}
                </span>
              </div>
              <div className="flex justify-between items-center py-2 border-b border-gray-100">
                <span className="text-sm font-medium text-gray-600">
                  Odometer Reading
                </span>
                <span className="text-sm text-gray-900">
                  {formatNumber(fuelPurchase.odometerReading, 1)} km
                </span>
              </div>
              <div className="flex justify-between items-center py-2 border-b border-gray-100">
                <span className="text-sm font-medium text-gray-600">
                  Volume
                </span>
                <span className="text-sm text-gray-900">
                  {formatNumber(fuelPurchase.volume)} L
                </span>
              </div>
              <div className="flex justify-between items-center py-2 border-b border-gray-100">
                <span className="text-sm font-medium text-gray-600">
                  Price per Unit
                </span>
                <span className="text-sm text-gray-900">
                  {formatCurrency(fuelPurchase.pricePerUnit)}/L
                </span>
              </div>
              <div className="flex justify-between items-center py-2 bg-green-50 px-3 rounded">
                <span className="text-sm font-semibold text-green-800">
                  Total Cost
                </span>
                <span className="text-lg font-bold text-green-800">
                  {formatCurrency(fuelPurchase.totalCost)}
                </span>
              </div>
            </div>
          </div>
        </div>

        {/* Summary Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mt-6">
          <div className="bg-white rounded-lg border border-gray-200 p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600">Volume</p>
                <p className="text-2xl font-bold text-gray-900">
                  {formatNumber(fuelPurchase.volume)} L
                </p>
              </div>
              <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
                <Fuel className="w-6 h-6 text-blue-600" />
              </div>
            </div>
          </div>

          <div className="bg-white rounded-lg border border-gray-200 p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600">Price/Unit</p>
                <p className="text-2xl font-bold text-gray-900">
                  {formatCurrency(fuelPurchase.pricePerUnit)}
                </p>
              </div>
              <div className="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
                <span className="text-green-600 font-bold text-lg">$</span>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-lg border border-gray-200 p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600">Total Cost</p>
                <p className="text-2xl font-bold text-green-600">
                  {formatCurrency(fuelPurchase.totalCost)}
                </p>
              </div>
              <div className="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
                <Receipt className="w-6 h-6 text-green-600" />
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default FuelPurchaseDetailsPage;
