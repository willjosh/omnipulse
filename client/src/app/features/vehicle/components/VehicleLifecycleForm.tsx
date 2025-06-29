"use client";
import React from "react";
import { useVehicleFormStore } from "../store/VehicleFormStore";

const VehicleLifecycleForm: React.FC = () => {
  const { formData, updateLifecycle } = useVehicleFormStore();
  const lifecycle = formData.lifecycle;

  const handleInputChange = (
    field: keyof typeof lifecycle,
    value: string | number | null,
  ) => {
    updateLifecycle({ [field]: value });
  };

  const today = new Date().toISOString().split("T")[0];

  return (
    <div className="space-y-8">
      {/* In-Service */}
      <div className="bg-white rounded-lg p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-6">In-Service</h2>

        <div className="space-y-6">
          {/* In-Service Date */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              In-Service Date
            </label>
            <input
              type="date"
              value={lifecycle.inServiceDate || today}
              onChange={e => handleInputChange("inServiceDate", e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
            <p className="text-xs text-gray-600 mt-1">
              Date vehicle entered active fleet service
            </p>
          </div>

          {/* In-Service Odometer */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              In-Service Odometer
            </label>
            <input
              type="number"
              value={lifecycle.inServiceOdometer || ""}
              onChange={e =>
                handleInputChange(
                  "inServiceOdometer",
                  e.target.value ? parseInt(e.target.value) : null,
                )
              }
              placeholder="Odometer reading"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
            <p className="text-xs text-gray-600 mt-1">
              Odometer reading on in-service date
            </p>
          </div>
        </div>
      </div>

      {/* Vehicle Life Estimates */}
      <div className="bg-white rounded-lg p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-6">
          Vehicle Life Estimates
        </h2>

        <div className="space-y-6">
          {/* Estimated Service Life */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Estimated Service Life in Months
            </label>
            <input
              type="number"
              value={lifecycle.estimatedServiceLifeMonths || ""}
              onChange={e =>
                handleInputChange(
                  "estimatedServiceLifeMonths",
                  e.target.value ? parseInt(e.target.value) : null,
                )
              }
              placeholder="Number of months"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
            <p className="text-xs text-gray-600 mt-1">
              Number of months vehicle is expected to be in active fleet service
            </p>
          </div>

          {/* Estimated Service Life */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Estimated Service Life in Meter
            </label>
            <input
              type="number"
              value={lifecycle.estimatedServiceLifeMeter || ""}
              onChange={e =>
                handleInputChange(
                  "estimatedServiceLifeMeter",
                  e.target.value ? parseInt(e.target.value) : null,
                )
              }
              placeholder="Meter value"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
            <p className="text-xs text-gray-600 mt-1">
              Primary meter value vehicle is expected to use/run before retiring
              from fleet service
            </p>
          </div>

          {/* Estimated Resale Value */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Estimated Resale Value
            </label>
            <div className="relative">
              <span className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-500">
                $
              </span>
              <input
                type="number"
                step="0.01"
                value={lifecycle.estimatedResaleValue || ""}
                onChange={e =>
                  handleInputChange(
                    "estimatedResaleValue",
                    e.target.value ? parseFloat(e.target.value) : null,
                  )
                }
                placeholder="0.00"
                className="w-full pl-8 pr-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>
            <p className="text-xs text-gray-600 mt-1">
              Amount expected to be recuperated after retirement and
              sale/disposal (less any associated costs)
            </p>
          </div>
        </div>
      </div>

      {/* Out-of-Service Section */}
      <div className="bg-white rounded-lg p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-6">
          Out-of-Service
        </h2>

        <div className="space-y-6">
          {/* Out-of-Service Date */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Out-of-Service Date
            </label>
            <input
              type="date"
              value={lifecycle.outOfServiceDate || today}
              onChange={e =>
                handleInputChange("outOfServiceDate", e.target.value)
              }
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
            <p className="text-xs text-gray-600 mt-1">
              Date vehicle was retired from fleet service
            </p>
          </div>

          {/* Out-of-Service Odometer */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Out-of-Service Odometer
            </label>
            <input
              type="number"
              value={lifecycle.outOfServiceOdometer || ""}
              onChange={e =>
                handleInputChange(
                  "outOfServiceOdometer",
                  e.target.value ? parseInt(e.target.value) : null,
                )
              }
              placeholder="Final odometer reading"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
            <p className="text-xs text-gray-600 mt-1">
              Final odometer reading on out-of-service date
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default VehicleLifecycleForm;
