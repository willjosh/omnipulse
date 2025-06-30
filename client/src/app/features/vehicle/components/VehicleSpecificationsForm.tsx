"use client";
import React from "react";
import { useVehicleFormStore } from "../store/VehicleFormStore";

const VehicleSpecificationsForm: React.FC = () => {
  const { formData, updateSpecifications } = useVehicleFormStore();
  const specs = formData.specifications;

  const handleInputChange = (
    field: keyof typeof specs,
    value: string | number | null,
  ) => {
    updateSpecifications({ [field]: value });
  };

  return (
    <div className="space-y-8">
      {/* Dimensions */}
      <div className="bg-white rounded-lg p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-6">Dimensions</h2>

        <div className="space-y-6">
          {/* Width, Height Row */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Width
              </label>
              <div className="relative">
                <input
                  type="number"
                  step="0.1"
                  value={specs.width || ""}
                  onChange={e =>
                    handleInputChange(
                      "width",
                      e.target.value ? parseFloat(e.target.value) : null,
                    )
                  }
                  placeholder="Width"
                  className="w-full px-3 py-2 pr-12 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
                <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 text-sm">
                  cm
                </span>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Height
              </label>
              <div className="relative">
                <input
                  type="number"
                  step="0.1"
                  value={specs.height || ""}
                  onChange={e =>
                    handleInputChange(
                      "height",
                      e.target.value ? parseFloat(e.target.value) : null,
                    )
                  }
                  placeholder="Height"
                  className="w-full px-3 py-2 pr-12 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
                <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 text-sm">
                  cm
                </span>
              </div>
            </div>
          </div>

          {/* Length, Interior Volume Row */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Length
              </label>
              <div className="relative">
                <input
                  type="number"
                  step="0.1"
                  value={specs.length || ""}
                  onChange={e =>
                    handleInputChange(
                      "length",
                      e.target.value ? parseFloat(e.target.value) : null,
                    )
                  }
                  placeholder="Length"
                  className="w-full px-3 py-2 pr-12 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
                <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 text-sm">
                  cm
                </span>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Interior Volume
              </label>
              <div className="relative">
                <input
                  type="number"
                  step="0.1"
                  value={specs.interiorVolume || ""}
                  onChange={e =>
                    handleInputChange(
                      "interiorVolume",
                      e.target.value ? parseFloat(e.target.value) : null,
                    )
                  }
                  placeholder="Interior Volume"
                  className="w-full px-3 py-2 pr-8 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
                <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 text-sm">
                  L
                </span>
              </div>
            </div>
          </div>

          {/* Passenger Volume, Cargo Volume Row */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Passenger Volume
              </label>
              <div className="relative">
                <input
                  type="number"
                  step="0.1"
                  value={specs.passengerVolume || ""}
                  onChange={e =>
                    handleInputChange(
                      "passengerVolume",
                      e.target.value ? parseFloat(e.target.value) : null,
                    )
                  }
                  placeholder="Passenger Volume"
                  className="w-full px-3 py-2 pr-8 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
                <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 text-sm">
                  L
                </span>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Cargo Volume
              </label>
              <div className="relative">
                <input
                  type="number"
                  step="0.1"
                  value={specs.cargoVolume || ""}
                  onChange={e =>
                    handleInputChange(
                      "cargoVolume",
                      e.target.value ? parseFloat(e.target.value) : null,
                    )
                  }
                  placeholder="Cargo Volume"
                  className="w-full px-3 py-2 pr-8 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
                <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 text-sm">
                  L
                </span>
              </div>
            </div>
          </div>

          {/* Ground Clearance, Bed Length Row */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Ground Clearance
              </label>
              <div className="relative">
                <input
                  type="number"
                  step="0.1"
                  value={specs.groundClearance || ""}
                  onChange={e =>
                    handleInputChange(
                      "groundClearance",
                      e.target.value ? parseFloat(e.target.value) : null,
                    )
                  }
                  placeholder="Ground Clearance"
                  className="w-full px-3 py-2 pr-12 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
                <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 text-sm">
                  cm
                </span>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Bed Length
              </label>
              <div className="relative">
                <input
                  type="number"
                  step="0.1"
                  value={specs.bedLength || ""}
                  onChange={e =>
                    handleInputChange(
                      "bedLength",
                      e.target.value ? parseFloat(e.target.value) : null,
                    )
                  }
                  placeholder="Bed Length"
                  className="w-full px-3 py-2 pr-12 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
                <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 text-sm">
                  cm
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Weight */}
      <div className="bg-white rounded-lg p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-6">Weight</h2>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Curb Weight
            </label>
            <div className="relative">
              <input
                type="number"
                step="0.1"
                value={specs.curbWeight || ""}
                onChange={e =>
                  handleInputChange(
                    "curbWeight",
                    e.target.value ? parseFloat(e.target.value) : null,
                  )
                }
                placeholder="Curb Weight"
                className="w-full px-3 py-2 pr-12 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
              <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 text-sm">
                kg
              </span>
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Gross Vehicle Weight Rating
            </label>
            <div className="relative">
              <input
                type="number"
                step="0.1"
                value={specs.grossVehicleWeightRating || ""}
                onChange={e =>
                  handleInputChange(
                    "grossVehicleWeightRating",
                    e.target.value ? parseFloat(e.target.value) : null,
                  )
                }
                placeholder="Gross Vehicle Weight Rating"
                className="w-full px-3 py-2 pr-12 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
              <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 text-sm">
                kg
              </span>
            </div>
          </div>
        </div>
      </div>

      {/* Performance */}
      <div className="bg-white rounded-lg p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-6">
          Performance
        </h2>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Towing Capacity
            </label>
            <div className="relative">
              <input
                type="number"
                step="0.1"
                value={specs.towingCapacity || ""}
                onChange={e =>
                  handleInputChange(
                    "towingCapacity",
                    e.target.value ? parseFloat(e.target.value) : null,
                  )
                }
                placeholder="Towing Capacity"
                className="w-full px-3 py-2 pr-12 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
              <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 text-sm">
                kg
              </span>
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Max Payload
            </label>
            <div className="relative">
              <input
                type="number"
                step="0.1"
                value={specs.maxPayload || ""}
                onChange={e =>
                  handleInputChange(
                    "maxPayload",
                    e.target.value ? parseFloat(e.target.value) : null,
                  )
                }
                placeholder="Max Payload"
                className="w-full px-3 py-2 pr-12 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
              <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 text-sm">
                kg
              </span>
            </div>
          </div>
        </div>
      </div>

      {/* Fuel Economy */}
      <div className="bg-white rounded-lg p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-6">
          Fuel Economy
        </h2>

        <div className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                EPA City
              </label>
              <input
                type="text"
                value={specs.epaCity}
                onChange={e => handleInputChange("epaCity", e.target.value)}
                placeholder="EPA City Rating"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                EPA Highway
              </label>
              <input
                type="text"
                value={specs.epaHighway}
                onChange={e => handleInputChange("epaHighway", e.target.value)}
                placeholder="EPA Highway Rating"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              EPA Combined
            </label>
            <input
              type="text"
              value={specs.epaCombined}
              onChange={e => handleInputChange("epaCombined", e.target.value)}
              placeholder="EPA Combined Rating"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
        </div>
      </div>

      {/* Engine */}
      <div className="bg-white rounded-lg p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-6">Engine</h2>

        <div className="space-y-6">
          {/* Engine Summary, Brand, Aspiration Row */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Engine Summary
              </label>
              <input
                type="text"
                value={specs.engineSummary}
                onChange={e =>
                  handleInputChange("engineSummary", e.target.value)
                }
                placeholder="Engine Summary"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Engine Brand
              </label>
              <input
                type="text"
                value={specs.engineBrand}
                onChange={e => handleInputChange("engineBrand", e.target.value)}
                placeholder="Engine Brand"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Aspiration
              </label>
              <select
                value={specs.aspiration}
                onChange={e => handleInputChange("aspiration", e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              >
                <option value="">Please select</option>
                <option value="naturally-aspirated">Naturally Aspirated</option>
                <option value="turbocharged">Turbocharged</option>
                <option value="supercharged">Supercharged</option>
                <option value="twin-turbo">Twin Turbo</option>
              </select>
            </div>
          </div>

          {/* Block Type, Bore, Cam Type */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Block Type
              </label>
              <select
                value={specs.blockType}
                onChange={e => handleInputChange("blockType", e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              >
                <option value="">Please select</option>
                <option value="inline">Inline</option>
                <option value="v-type">V-Type</option>
                <option value="flat">Flat/Boxer</option>
                <option value="w-type">W-Type</option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Bore
              </label>
              <div className="relative">
                <input
                  type="number"
                  step="0.1"
                  value={specs.bore || ""}
                  onChange={e =>
                    handleInputChange(
                      "bore",
                      e.target.value ? parseFloat(e.target.value) : null,
                    )
                  }
                  placeholder="Bore"
                  className="w-full px-3 py-2 pr-12 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
                <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 text-sm">
                  cm
                </span>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Cam Type
              </label>
              <select
                value={specs.camType}
                onChange={e => handleInputChange("camType", e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              >
                <option value="">Please select</option>
                <option value="ohv">OHV (Overhead Valve)</option>
                <option value="ohc">OHC (Overhead Cam)</option>
                <option value="sohc">SOHC (Single Overhead Cam)</option>
                <option value="dohc">DOHC (Dual Overhead Cam)</option>
              </select>
            </div>
          </div>

          {/* More Engine Fields */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Compression
              </label>
              <input
                type="text"
                value={specs.compression}
                onChange={e => handleInputChange("compression", e.target.value)}
                placeholder="Compression Ratio"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Cylinders
              </label>
              <input
                type="number"
                value={specs.cylinders || ""}
                onChange={e =>
                  handleInputChange(
                    "cylinders",
                    e.target.value ? parseInt(e.target.value) : null,
                  )
                }
                placeholder="Number of Cylinders"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Displacement
              </label>
              <input
                type="text"
                value={specs.displacement}
                onChange={e =>
                  handleInputChange("displacement", e.target.value)
                }
                placeholder="Engine Displacement"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Fuel Induction
              </label>
              <select
                value={specs.fuelInduction}
                onChange={e =>
                  handleInputChange("fuelInduction", e.target.value)
                }
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              >
                <option value="">Please select</option>
                <option value="fuel-injection">Fuel Injection</option>
                <option value="direct-injection">Direct Injection</option>
                <option value="carburetor">Carburetor</option>
                <option value="turbo-direct-injection">
                  Turbo Direct Injection
                </option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Max HP
              </label>
              <input
                type="number"
                value={specs.maxHp || ""}
                onChange={e =>
                  handleInputChange(
                    "maxHp",
                    e.target.value ? parseInt(e.target.value) : null,
                  )
                }
                placeholder="Maximum Horsepower"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Max Torque
              </label>
              <input
                type="text"
                value={specs.maxTorque}
                onChange={e => handleInputChange("maxTorque", e.target.value)}
                placeholder="Maximum Torque"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Redline RPM
              </label>
              <input
                type="number"
                value={specs.redlineRpm || ""}
                onChange={e =>
                  handleInputChange(
                    "redlineRpm",
                    e.target.value ? parseInt(e.target.value) : null,
                  )
                }
                placeholder="Redline RPM"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Stroke
              </label>
              <input
                type="text"
                value={specs.stroke}
                onChange={e => handleInputChange("stroke", e.target.value)}
                placeholder="Engine Stroke"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Valves
              </label>
              <input
                type="text"
                value={specs.valves}
                onChange={e => handleInputChange("valves", e.target.value)}
                placeholder="Number of Valves"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>
          </div>
        </div>
      </div>

      {/* Transmission */}
      <div className="bg-white rounded-lg p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-6">
          Transmission
        </h2>

        <div className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Transmission Summary
              </label>
              <input
                type="text"
                value={specs.transmissionSummary}
                onChange={e =>
                  handleInputChange("transmissionSummary", e.target.value)
                }
                placeholder="Transmission Summary"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Transmission Brand
              </label>
              <input
                type="text"
                value={specs.transmissionBrand}
                onChange={e =>
                  handleInputChange("transmissionBrand", e.target.value)
                }
                placeholder="Transmission Brand"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Transmission Type
              </label>
              <select
                value={specs.transmissionType}
                onChange={e =>
                  handleInputChange("transmissionType", e.target.value)
                }
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              >
                <option value="">Please select</option>
                <option value="manual">Manual</option>
                <option value="automatic">Automatic</option>
                <option value="cvt">CVT</option>
                <option value="dual-clutch">Dual Clutch</option>
              </select>
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Transmission Gears
            </label>
            <input
              type="text"
              value={specs.transmissionGears}
              onChange={e =>
                handleInputChange("transmissionGears", e.target.value)
              }
              placeholder="Number of Gears"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
        </div>
      </div>

      {/* Wheels & Tires */}
      <div className="bg-white rounded-lg p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-6">
          Wheels & Tires
        </h2>

        <div className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Drive Type
              </label>
              <select
                value={specs.driveType}
                onChange={e => handleInputChange("driveType", e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              >
                <option value="">Please select</option>
                <option value="fwd">FWD (Front Wheel Drive)</option>
                <option value="rwd">RWD (Rear Wheel Drive)</option>
                <option value="awd">AWD (All Wheel Drive)</option>
                <option value="4wd">4WD (Four Wheel Drive)</option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Brake System
              </label>
              <select
                value={specs.brakeSystem}
                onChange={e => handleInputChange("brakeSystem", e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              >
                <option value="">Please select</option>
                <option value="disc-disc">Disc/Disc</option>
                <option value="disc-drum">Disc/Drum</option>
                <option value="drum-drum">Drum/Drum</option>
                <option value="abs">ABS</option>
                <option value="abs-ebd">ABS with EBD</option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Front Track Width
              </label>
              <input
                type="text"
                value={specs.frontTrackWidth}
                onChange={e =>
                  handleInputChange("frontTrackWidth", e.target.value)
                }
                placeholder="Front Track Width"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Rear Track Width
              </label>
              <input
                type="text"
                value={specs.rearTrackWidth}
                onChange={e =>
                  handleInputChange("rearTrackWidth", e.target.value)
                }
                placeholder="Rear Track Width"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Wheelbase
              </label>
              <div className="relative">
                <input
                  type="number"
                  step="0.1"
                  value={specs.wheelbase || ""}
                  onChange={e =>
                    handleInputChange(
                      "wheelbase",
                      e.target.value ? parseFloat(e.target.value) : null,
                    )
                  }
                  placeholder="Wheelbase"
                  className="w-full px-3 py-2 pr-12 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
                <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 text-sm">
                  cm
                </span>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Front Wheel Diameter
              </label>
              <input
                type="text"
                value={specs.frontWheelDiameter}
                onChange={e =>
                  handleInputChange("frontWheelDiameter", e.target.value)
                }
                placeholder="Front Wheel Diameter"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Rear Wheel Diameter
              </label>
              <input
                type="text"
                value={specs.rearWheelDiameter}
                onChange={e =>
                  handleInputChange("rearWheelDiameter", e.target.value)
                }
                placeholder="Rear Wheel Diameter"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Rear Axle
              </label>
              <input
                type="text"
                value={specs.rearAxle}
                onChange={e => handleInputChange("rearAxle", e.target.value)}
                placeholder="Rear Axle"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Front Tire Type
              </label>
              <input
                type="text"
                value={specs.frontTireType}
                onChange={e =>
                  handleInputChange("frontTireType", e.target.value)
                }
                placeholder="Front Tire Type"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Front Tire PSI
              </label>
              <input
                type="text"
                value={specs.frontTirePsi}
                onChange={e =>
                  handleInputChange("frontTirePsi", e.target.value)
                }
                placeholder="Front Tire PSI"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Rear Tire Type
              </label>
              <input
                type="text"
                value={specs.rearTireType}
                onChange={e =>
                  handleInputChange("rearTireType", e.target.value)
                }
                placeholder="Rear Tire Type"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Rear Tire PSI
              </label>
              <input
                type="text"
                value={specs.rearTirePsi}
                onChange={e => handleInputChange("rearTirePsi", e.target.value)}
                placeholder="Rear Tire PSI"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>
          </div>
        </div>
      </div>

      {/* Fuel */}
      <div className="bg-white rounded-lg p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-6">Fuel</h2>

        <div className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Fuel Quality
              </label>
              <input
                type="text"
                value={specs.fuelQuality}
                onChange={e => handleInputChange("fuelQuality", e.target.value)}
                placeholder="Fuel Quality"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Fuel Tank 1 Capacity
              </label>
              <div className="relative">
                <input
                  type="number"
                  step="0.1"
                  value={specs.fuelTank1Capacity || ""}
                  onChange={e =>
                    handleInputChange(
                      "fuelTank1Capacity",
                      e.target.value ? parseFloat(e.target.value) : null,
                    )
                  }
                  placeholder="Tank 1 Capacity"
                  className="w-full px-3 py-2 pr-16 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
                <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 text-sm">
                  Liters
                </span>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Fuel Tank 2 Capacity
              </label>
              <div className="relative">
                <input
                  type="number"
                  step="0.1"
                  value={specs.fuelTank2Capacity || ""}
                  onChange={e =>
                    handleInputChange(
                      "fuelTank2Capacity",
                      e.target.value ? parseFloat(e.target.value) : null,
                    )
                  }
                  placeholder="Tank 2 Capacity"
                  className="w-full px-3 py-2 pr-16 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
                <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 text-sm">
                  Liters
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Oil */}
      <div className="bg-white rounded-lg p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-6">Oil</h2>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Oil Capacity
          </label>
          <input
            type="text"
            value={specs.oilCapacity}
            onChange={e => handleInputChange("oilCapacity", e.target.value)}
            placeholder="Oil Capacity"
            className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          />
        </div>
      </div>
    </div>
  );
};

export default VehicleSpecificationsForm;
