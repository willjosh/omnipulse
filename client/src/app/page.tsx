"use client";

import React, { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuthContext } from "@/features/auth/context/AuthContext";
import {
  useVehicleAssignedData,
  useVehicleStatusData,
} from "@/features/vehicle/hooks/useVehicles";
import { Car, BarChart3 } from "lucide-react";
import {
  useWorkOrder,
  useWorkOrderStatusData,
} from "@/features/work-order/hooks/useWorkOrders";
import { useOpenIssueData } from "@/features/issue/hooks/useIssues";

const Home = () => {
  const router = useRouter();
  const { isAuthenticated, isLoading } = useAuthContext();
  const {
    assignedVehicleCount,
    unassignedVehicleCount,
    isAssignedVehicleDataLoading,
  } = useVehicleAssignedData();

  const {
    activeVehicleCount,
    inactiveVehicleCount,
    maintenanceVehicleCount,
    outOfServiceVehicleCount,
    isVehicleStatusDataLoading,
  } = useVehicleStatusData();

  const { createdCount, inProgressCount, isLoadingworkOrderStatusData } =
    useWorkOrderStatusData();

  const { openIssueCount, isLoadingOpenIssueData } = useOpenIssueData();

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      router.push("/login");
    }
  }, [isAuthenticated, isLoading, router]);

  // Show loading while checking authentication
  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  // Don't render anything if not authenticated (will redirect)
  if (!isAuthenticated) {
    return null;
  }

  return (
    <div className="p-6">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-600 mt-2">
          Welcome to your fleet management dashboard
        </p>
      </div>

      {/* Quick Stats */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        {/* Vehicle Assignment Card - Clickable */}
        <div
          className="bg-white p-6 rounded-lg shadow-sm border border-gray-200 cursor-pointer hover:shadow-md hover:border-blue-300 transition-all duration-200"
          onClick={() => router.push("/vehicles")}
        >
          <div className="flex items-center mb-4">
            <div className="p-2 bg-blue-100 rounded-lg">
              <Car className="w-6 h-6 text-blue-600" />
            </div>
            <div className="ml-4">
              <p className="text-sm font-medium text-gray-600">
                Vehicle Assignment
              </p>
            </div>
          </div>

          {/* Two numbers side by side */}
          <div className="flex justify-between items-center">
            <div className="text-center">
              <p className="text-3xl font-bold text-blue-600">
                {isAssignedVehicleDataLoading ? "..." : assignedVehicleCount}
              </p>
              <p className="text-xs text-gray-500 mt-1">Assigned</p>
            </div>
            <div className="text-center">
              <p className="text-3xl font-bold text-gray-600">
                {isAssignedVehicleDataLoading ? "..." : unassignedVehicleCount}
              </p>
              <p className="text-xs text-gray-500 mt-1">Unassigned</p>
            </div>
          </div>
        </div>

        {/* Vehicle Status Card - Clickable */}
        <div
          className="bg-white p-6 rounded-lg shadow-sm border border-gray-200 cursor-pointer hover:shadow-md hover:border-green-300 transition-all duration-200"
          onClick={() => router.push("/vehicles")}
        >
          <div className="flex items-center mb-4">
            <div className="p-2 bg-green-100 rounded-lg">
              <BarChart3 className="w-6 h-6 text-green-600" />
            </div>
            <div className="ml-4">
              <p className="text-sm font-medium text-gray-600">
                Vehicle Status
              </p>
            </div>
          </div>

          {/* Four numbers in a 2x2 grid */}
          <div className="grid grid-cols-2 gap-3">
            <div className="text-center">
              <p className="text-2xl font-bold text-green-600">
                {isVehicleStatusDataLoading ? "..." : activeVehicleCount}
              </p>
              <p className="text-xs text-gray-500 mt-1">Active</p>
            </div>
            <div className="text-center">
              <p className="text-2xl font-bold text-yellow-600">
                {isVehicleStatusDataLoading ? "..." : maintenanceVehicleCount}
              </p>
              <p className="text-xs text-gray-500 mt-1">Maintenance</p>
            </div>
            <div className="text-center">
              <p className="text-2xl font-bold text-red-600">
                {isVehicleStatusDataLoading ? "..." : outOfServiceVehicleCount}
              </p>
              <p className="text-xs text-gray-500 mt-1">Out of Service</p>
            </div>
            <div className="text-center">
              <p className="text-2xl font-bold text-gray-600">
                {isVehicleStatusDataLoading ? "..." : inactiveVehicleCount}
              </p>
              <p className="text-xs text-gray-500 mt-1">Inactive</p>
            </div>
          </div>
        </div>

        {/* Work Order Status Card - Clickable */}
        <div
          className="bg-white p-6 rounded-lg shadow-sm border border-gray-200 cursor-pointer hover:shadow-md hover:border-purple-300 transition-all duration-200"
          onClick={() => router.push("/work-orders")}
        >
          <div className="flex items-center mb-4">
            <div className="p-2 bg-purple-100 rounded-lg">
              <svg
                className="w-6 h-6 text-purple-600"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"
                />
              </svg>
            </div>
            <div className="ml-4">
              <p className="text-sm font-medium text-gray-600">
                Work Order Status
              </p>
            </div>
          </div>

          {/* Two numbers side by side */}
          <div className="flex justify-between items-center">
            <div className="text-center">
              <p className="text-3xl font-bold text-blue-600">
                {isLoadingworkOrderStatusData ? "..." : createdCount}
              </p>
              <p className="text-xs text-gray-500 mt-1">Created</p>
            </div>
            <div className="text-center">
              <p className="text-3xl font-bold text-orange-600">
                {isLoadingworkOrderStatusData ? "..." : inProgressCount}
              </p>
              <p className="text-xs text-gray-500 mt-1">In Progress</p>
            </div>
          </div>
        </div>

        {/* Open Issues Card - Replace Scheduled Services */}
        <div
          className="bg-white p-6 rounded-lg shadow-sm border border-gray-200 cursor-pointer hover:shadow-md hover:border-yellow-300 transition-all duration-200"
          onClick={() => router.push("/issues")}
        >
          <div className="flex items-center">
            <div className="p-2 bg-yellow-100 rounded-lg">
              <svg
                className="w-6 h-6 text-yellow-600"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z"
                />
              </svg>
            </div>
            <div className="ml-4">
              <p className="text-sm font-medium text-gray-600">Open Issues</p>
              <p className="text-2xl font-semibold text-gray-900">
                {isLoadingOpenIssueData ? "..." : openIssueCount}
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Quick Actions */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
        <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">
            Quick Actions
          </h3>
          <div className="space-y-3">
            <button
              onClick={() => router.push("/vehicles/create")}
              className="w-full text-left p-3 rounded-lg border border-gray-200 hover:bg-gray-50 transition-colors"
            >
              <div className="flex items-center">
                <svg
                  className="w-5 h-5 text-blue-600 mr-3"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M12 6v6m0 0v6m0-6h6m-6 0H6"
                  />
                </svg>
                <span className="text-sm font-medium">Add New Vehicle</span>
              </div>
            </button>
            <button
              onClick={() => router.push("/inspections/new")}
              className="w-full text-left p-3 rounded-lg border border-gray-200 hover:bg-gray-50 transition-colors"
            >
              <div className="flex items-center">
                <svg
                  className="w-5 h-5 text-green-600 mr-3"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
                  />
                </svg>
                <span className="text-sm font-medium">Start Inspection</span>
              </div>
            </button>
            <button
              onClick={() => router.push("/work-orders/new")}
              className="w-full text-left p-3 rounded-lg border border-gray-200 hover:bg-gray-50 transition-colors"
            >
              <div className="flex items-center">
                <svg
                  className="w-5 h-5 text-purple-600 mr-3"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"
                  />
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
                  />
                </svg>
                <span className="text-sm font-medium">Create Work Order</span>
              </div>
            </button>
          </div>
        </div>

        {/* Rest of your existing Quick Actions content... */}
        <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">
            Recent Activity
          </h3>
          <div className="space-y-3">
            <div className="flex items-center p-3 rounded-lg bg-blue-50">
              <div className="w-2 h-2 bg-blue-600 rounded-full mr-3"></div>
              <div>
                <p className="text-sm font-medium text-gray-900">
                  Vehicle inspection completed
                </p>
                <p className="text-xs text-gray-500">2 hours ago</p>
              </div>
            </div>
            <div className="flex items-center p-3 rounded-lg bg-green-50">
              <div className="w-2 h-2 bg-green-600 rounded-full mr-3"></div>
              <div>
                <p className="text-sm font-medium text-gray-900">
                  New vehicle added
                </p>
                <p className="text-xs text-gray-500">4 hours ago</p>
              </div>
            </div>
            <div className="flex items-center p-3 rounded-lg bg-yellow-50">
              <div className="w-2 h-2 bg-yellow-600 rounded-full mr-3"></div>
              <div>
                <p className="text-sm font-medium text-gray-900">
                  Maintenance scheduled
                </p>
                <p className="text-xs text-gray-500">1 day ago</p>
              </div>
            </div>
          </div>
        </div>

        <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">
            Upcoming Tasks
          </h3>
          <div className="space-y-3">
            <div className="flex items-center justify-between p-3 rounded-lg border border-gray-200">
              <div>
                <p className="text-sm font-medium text-gray-900">
                  Vehicle #123 Inspection
                </p>
                <p className="text-xs text-gray-500">Due tomorrow</p>
              </div>
              <span className="text-xs bg-yellow-100 text-yellow-800 px-2 py-1 rounded-full">
                Pending
              </span>
            </div>
            <div className="flex items-center justify-between p-3 rounded-lg border border-gray-200">
              <div>
                <p className="text-sm font-medium text-gray-900">
                  Oil Change - Truck #456
                </p>
                <p className="text-xs text-gray-500">Due in 2 days</p>
              </div>
              <span className="text-xs bg-blue-100 text-blue-800 px-2 py-1 rounded-full">
                Scheduled
              </span>
            </div>
            <div className="flex items-center justify-between p-3 rounded-lg border border-gray-200">
              <div>
                <p className="text-sm font-medium text-gray-900">
                  Safety Check - Van #789
                </p>
                <p className="text-xs text-gray-500">Due in 3 days</p>
              </div>
              <span className="text-xs bg-green-100 text-green-800 px-2 py-1 rounded-full">
                Ready
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Home;
