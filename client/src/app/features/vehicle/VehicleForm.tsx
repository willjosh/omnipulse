"use client";
import React, { useState } from "react";

interface VehicleFormData {
  vehicleName: string;
  telematics: string;
  vin: string;
  licensePlate: string;
  type: string;
  fuelType: string;
  year: string;
  make: string;
  model: string;
  trim: string;
  registrationState: string;
  labels: string;
}

const NewVehicleForm: React.FC = () => {
  const [formData, setFormData] = useState<VehicleFormData>({
    vehicleName: "",
    telematics: "",
    vin: "",
    licensePlate: "",
    type: "Car",
    fuelType: "",
    year: "",
    make: "",
    model: "",
    trim: "",
    registrationState: "",
    labels: "",
  });

  const handleInputChange = (field: keyof VehicleFormData, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log("Vehicle data submitted:", formData);
    // Add new vehicle logic not yet implemented
  };

  const handleCancel = () => {
    setFormData({
      vehicleName: "",
      telematics: "",
      vin: "",
      licensePlate: "",
      type: "Car",
      fuelType: "",
      year: "",
      make: "",
      model: "",
      trim: "",
      registrationState: "",
      labels: "",
    });
  };

  return (
    <div className="max-w-4xl mx-auto my-16 rounded-2xl p-6 bg-white">
      <form onSubmit={handleSubmit} className="space-y-2">
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
                value={formData.vehicleName}
                onChange={e => handleInputChange("vehicleName", e.target.value)}
                placeholder="Enter a nickname"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                required
              />
            </div>

            {/* Telematics Device */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Telematics Device
              </label>
              <select
                value={formData.telematics}
                onChange={e => handleInputChange("telematics", e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                required
              >
                <option value="">Please select</option>
                <option value="gps-tracker-1">GPS Tracker 1</option>
                <option value="gps-tracker-2">GPS Tracker 2</option>
                <option value="obd-device">OBD Device</option>
              </select>
              <p className="text-xs text-gray-600 mt-1">
                Link this vehicle with one of your Telematics Devices to begin
                automatically collecting data.
              </p>
            </div>

            {/* VIN and License Plate Row */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  VIN#
                </label>
                <div className="relative">
                  <input
                    type="text"
                    value={formData.vin}
                    onChange={e => handleInputChange("vin", e.target.value)}
                    placeholder="Vehicle Identification Number"
                    className="w-full px-3 py-2 pr-10 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    required
                  />
                  <button
                    type="button"
                    className="absolute right-2 top-1/2 transform -translate-y-1/2 p-1 text-gray-400 hover:text-gray-600"
                  ></button>
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  License Plate
                </label>
                <input
                  type="text"
                  value={formData.licensePlate}
                  onChange={e =>
                    handleInputChange("licensePlate", e.target.value)
                  }
                  placeholder="ex: ABC-1234"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  required
                />
              </div>
            </div>

            {/* Type and Fuel Type Row */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Type
                </label>
                <select
                  value={formData.type}
                  onChange={e => handleInputChange("type", e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  required
                >
                  <option value="Car">Car</option>
                  <option value="Truck">Truck</option>
                  <option value="Van">Van</option>
                  <option value="SUV">SUV</option>
                  <option value="Motorcycle">Motorcycle</option>
                  <option value="Equipment">Equipment</option>
                </select>
                <p className="text-xs text-gray-600 mt-1">
                  Categorize this vehicle
                </p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Fuel Type
                </label>
                <select
                  value={formData.fuelType}
                  onChange={e => handleInputChange("fuelType", e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
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
                  value={formData.year}
                  onChange={e => handleInputChange("year", e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
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
                <p className="text-xs text-gray-600 mt-1">
                  e.g. 1998, 2012, etc.
                </p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Make
                </label>
                <select
                  value={formData.make}
                  onChange={e => handleInputChange("make", e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
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
                <p className="text-xs text-gray-600 mt-1">
                  e.g. Toyota, GMC, Chevrolet, etc.
                </p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Model
                </label>
                <select
                  value={formData.model}
                  onChange={e => handleInputChange("model", e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
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
                <p className="text-xs text-gray-600 mt-1">
                  e.g. 4Runner, Yukon, Silverado, etc.
                </p>
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
                  value={formData.trim}
                  onChange={e => handleInputChange("trim", e.target.value)}
                  placeholder="ex: SR, LE, XLE, etc."
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Registration State/Province
                </label>
                <select
                  value={formData.registrationState}
                  onChange={e =>
                    handleInputChange("registrationState", e.target.value)
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  required
                >
                  <option value="">Please select</option>
                  <option value="NSW">New South Wales (NSW)</option>
                  <option value="VIC">Victoria (VIC)</option>
                  <option value="QLD">Queensland (QLD)</option>
                  <option value="WA">Western Australia (WA)</option>
                  <option value="SA">South Australia (SA)</option>
                  <option value="TAS">Tasmania (TAS)</option>
                  <option value="ACT">
                    Australian Capital Territory (ACT)
                  </option>
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
                value={formData.labels}
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

        {/* Form Actions */}
        <div className="flex justify-end space-x-4 pt-2 border-gray-200">
          <button
            type="button"
            onClick={handleCancel}
            className="px-6 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            Cancel
          </button>
          <button
            type="submit"
            className="px-6 py-2 text-sm font-medium text-white bg-[var(--primary-color)] rounded-lg hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            Add Vehicle
          </button>
        </div>
      </form>
    </div>
  );
};

export default NewVehicleForm;
