"use client";
import React from "react";
import { useVehicleFormStore } from "../store/VehicleFormStore";

const VehicleMaintenanceForm: React.FC = () => {
  const { formData, updateMaintenance } = useVehicleFormStore();
  const maintenance = formData.maintenance;

  const handleInputChange = (
    field: keyof typeof maintenance,
    value: string,
  ) => {
    updateMaintenance({ [field]: value });
  };

  return (
    <div className="space-y-8">
      {/* Maintenance Schedule */}
      <div className="bg-white rounded-lg p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-6">
          Maintenance Schedule
        </h2>

        <div className="space-y-6">
          {/* Service Program */}
          <div>
            <h3 className="text-base font-medium text-gray-900 mb-2">
              Choose a Service Program
            </h3>
            <p className="text-sm text-gray-600 mb-6">
              Service Programs automatically manage Service Reminders for
              Vehicles that share common preventative maintenance needs.
            </p>

            <div className="space-y-4">
              <div className="flex items-start">
                <input
                  type="radio"
                  id="service-none"
                  name="serviceProgram"
                  value=""
                  checked={maintenance.serviceProgram === ""}
                  onChange={e =>
                    handleInputChange("serviceProgram", e.target.value)
                  }
                  className="mt-1 h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300"
                />
                <div className="ml-3">
                  <label
                    htmlFor="service-none"
                    className="text-sm font-medium text-gray-900"
                  >
                    None
                  </label>
                  <p className="text-sm text-gray-600">
                    No Service Reminders will be created
                  </p>
                </div>
              </div>

              {/* Choose existing Service Program */}
              <div className="border-2 border-green-200 rounded-lg p-4 bg-green-50">
                <div className="flex items-start">
                  <input
                    type="radio"
                    id="service-existing"
                    name="serviceProgram"
                    value="existing"
                    checked={maintenance.serviceProgram === "existing"}
                    onChange={e =>
                      handleInputChange("serviceProgram", e.target.value)
                    }
                    className="mt-1 h-4 w-4 text-green-600 focus:ring-green-500 border-gray-300"
                  />
                  <div className="ml-3 flex-1">
                    <label
                      htmlFor="service-existing"
                      className="text-sm font-medium text-gray-900"
                    >
                      Choose an existing Service Program
                    </label>

                    {maintenance.serviceProgram === "existing" && (
                      <div className="mt-3">
                        <select
                          value={
                            maintenance.serviceProgram === "existing"
                              ? ""
                              : maintenance.serviceProgram
                          }
                          onChange={e =>
                            handleInputChange(
                              "serviceProgram",
                              e.target.value || "existing",
                            )
                          }
                          className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-green-500 focus:border-transparent"
                        >
                          <option value="">Please select</option>
                          <option value="basic-maintenance">
                            Basic Maintenance Program
                          </option>
                          <option value="heavy-duty">
                            Heavy Duty Vehicle Program
                          </option>
                          <option value="light-vehicle">
                            Light Vehicle Program
                          </option>
                          <option value="commercial-fleet">
                            Commercial Fleet Program
                          </option>
                        </select>
                      </div>
                    )}
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default VehicleMaintenanceForm;
