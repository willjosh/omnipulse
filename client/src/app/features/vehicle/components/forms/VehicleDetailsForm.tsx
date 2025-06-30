"use client";
import React from "react";
import { useVehicleFormStore } from "../../store/VehicleFormStore";

const VehicleDetailsForm: React.FC = () => {
  const { formData, updateDetails } = useVehicleFormStore();
  const details = formData.details;

  const handleInputChange = (
    field: keyof typeof details,
    value: string | number | null,
  ) => {
    updateDetails({ [field]: value });
  };

  const showValidation = useVehicleFormStore(state => state.showValidation);

  const getFieldError = (field: keyof typeof details) => {
    if (!showValidation) return false;
    const value = details[field];
    return value === "" || value === null || value === undefined;
  };

  return (
    <div className="space-y-2">
      <div className="rounded-lg">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">
          New Vehicle Information
        </h2>

        <div className="space-y-6">
          {/* Vehicle Name */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Vehicle Name
            </label>
            <input
              type="text"
              value={details.vehicleName}
              onChange={e => handleInputChange("vehicleName", e.target.value)}
              placeholder="Enter a nickname"
              className={`w-full px-3 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                getFieldError("vehicleName")
                  ? "border-red-500"
                  : "border-gray-300"
              }`}
              required
            />
          </div>

          {/* Telematics Device */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Telematics Device
            </label>
            <select
              value={details.telematicsDevice}
              onChange={e =>
                handleInputChange("telematicsDevice", e.target.value)
              }
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="">Please select</option>
              <option value="gps-tracker-1">GPS Tracker 1</option>
              <option value="gps-tracker-2">GPS Tracker 2</option>
              <option value="obd-device">OBD Device</option>
            </select>
          </div>

          {/* VIN Number */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              VIN Number
            </label>
            <div className="relative">
              <input
                type="text"
                value={details.vin}
                onChange={e => handleInputChange("vin", e.target.value)}
                placeholder="Vehicle Identification Number"
                className={`w-full px-3 py-2 pr-24 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                  getFieldError("vin") ? "border-red-500" : "border-gray-300"
                }`}
                required
              />
            </div>
          </div>

          {/* License Plate */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              License Plate
            </label>
            <input
              type="text"
              value={details.licensePlate}
              onChange={e => handleInputChange("licensePlate", e.target.value)}
              placeholder="License Plate"
              className={`w-full px-3 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                getFieldError("licensePlate")
                  ? "border-red-500"
                  : "border-gray-300"
              }`}
              required
            />
          </div>

          {/* Type and Fuel Type Row */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Type
              </label>
              <select
                value={details.type}
                onChange={e => handleInputChange("type", e.target.value)}
                className={`w-full px-3 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                  getFieldError("type") ? "border-red-500" : "border-gray-300"
                }`}
                required
              >
                <option value="">Please select</option>
                <option value="Bus">Bus</option>
                <option value="Car">Car</option>
                <option value="Truck">Truck</option>
                <option value="Van">Van</option>
                <option value="SUV">SUV</option>
                <option value="Motorcycle">Motorcycle</option>
                <option value="Equipment">Equipment</option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Fuel Type
              </label>
              <select
                value={details.fuelType}
                onChange={e => handleInputChange("fuelType", e.target.value)}
                className={`w-full px-3 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                  getFieldError("fuelType")
                    ? "border-red-500"
                    : "border-gray-300"
                }`}
                required
              >
                <option value="">Please select</option>
                <option value="Petrol">Petrol</option>
                <option value="Diesel">Diesel</option>
                <option value="Electric">Electric</option>
                <option value="Hybrid">Hybrid</option>
              </select>
            </div>
          </div>

          {/* Year, Make, Model Row */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Year
              </label>
              <select
                value={details.year || ""}
                onChange={e =>
                  handleInputChange(
                    "year",
                    e.target.value ? parseInt(e.target.value) : null,
                  )
                }
                className={`w-full px-3 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                  getFieldError("year") ? "border-red-500" : "border-gray-300"
                }`}
                required
              >
                <option value="">Please select</option>
                {Array.from({ length: 30 }, (_, i) => {
                  const year = new Date().getFullYear() - i;
                  return (
                    <option key={year} value={year.toString()}>
                      {year}
                    </option>
                  );
                })}
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Make
              </label>
              <select
                value={details.make}
                onChange={e => handleInputChange("make", e.target.value)}
                className={`w-full px-3 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                  getFieldError("make") ? "border-red-500" : "border-gray-300"
                }`}
                required
              >
                <option value="">Please select</option>
                <option value="Toyota">Toyota</option>
                <option value="Ford">Ford</option>
                <option value="Chevrolet">Chevrolet</option>
                <option value="Honda">Honda</option>
                <option value="Nissan">Nissan</option>
                <option value="GMC">GMC</option>
                <option value="Ram">Ram</option>
                <option value="Hyundai">Hyundai</option>
                <option value="Kia">Kia</option>
                <option value="Volkswagen">Volkswagen</option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Model
              </label>
              <select
                value={details.model}
                onChange={e => handleInputChange("model", e.target.value)}
                className={`w-full px-3 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                  getFieldError("model") ? "border-red-500" : "border-gray-300"
                }`}
                required
              >
                <option value="">Please select</option>
                <option value="Camry">Camry</option>
                <option value="F-150">F-150</option>
                <option value="Silverado">Silverado</option>
                <option value="Accord">Accord</option>
                <option value="Altima">Altima</option>
                <option value="Corolla">Corolla</option>
                <option value="Civic">Civic</option>
                <option value="Escape">Escape</option>
              </select>
            </div>
          </div>

          {/* Trim and Registration State Row */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Trim
              </label>
              <input
                type="text"
                value={details.trim}
                onChange={e => handleInputChange("trim", e.target.value)}
                placeholder="e.g. SR, LE, XLE, etc."
                className={`w-full px-3 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                  getFieldError("trim") ? "border-red-500" : "border-gray-300"
                }`}
                required
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Registration State/Province
              </label>
              <select
                value={details.registrationState}
                onChange={e =>
                  handleInputChange("registrationState", e.target.value)
                }
                className={`w-full px-3 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                  getFieldError("registrationState")
                    ? "border-red-500"
                    : "border-gray-300"
                }`}
                required
              >
                <option value="">Please select</option>
                <option value="NSW">New South Wales (NSW)</option>
                <option value="VIC">Victoria (VIC)</option>
                <option value="QLD">Queensland (QLD)</option>
                <option value="WA">Western Australia (WA)</option>
                <option value="SA">South Australia (SA)</option>
                <option value="TAS">Tasmania (TAS)</option>
                <option value="ACT">Australian Capital Territory (ACT)</option>
                <option value="NT">Northern Territory (NT)</option>
              </select>
            </div>
          </div>

          {/* Labels */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Labels
            </label>
            <select
              value={details.labels}
              onChange={e => handleInputChange("labels", e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="">Please select</option>
              <option value="fleet">Fleet Vehicle</option>
              <option value="company">Company Car</option>
              <option value="rental">Rental</option>
              <option value="personal">Personal Use</option>
              <option value="maintenance">Under Maintenance</option>
            </select>
          </div>
        </div>
      </div>
    </div>
  );
};

export default VehicleDetailsForm;
