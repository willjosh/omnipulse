"use client";
import React from "react";
import { useVehicleFormStore } from "../../store/VehicleFormStore";

const VehicleFinancialForm: React.FC = () => {
  const { formData, updateFinancial } = useVehicleFormStore();
  const financial = formData.financial;

  const handleInputChange = (
    field: keyof typeof financial,
    value: string | number | null,
  ) => {
    updateFinancial({ [field]: value });
  };

  const today = new Date().toISOString().split("T")[0];

  return (
    <div className="space-y-8">
      {/* Purchase Details */}
      <div className="bg-white rounded-lg p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-6">
          Purchase Details
        </h2>

        <div className="space-y-6">
          {/* Purchase Vendor */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Purchase Vendor
            </label>
            <select
              value={financial.purchaseVendor}
              onChange={e =>
                handleInputChange("purchaseVendor", e.target.value)
              }
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="">Please select</option>
              <option value="toyota-dealer">Toyota Dealer</option>
              <option value="ford-dealer">Ford Dealer</option>
              <option value="chevrolet-dealer">Chevrolet Dealer</option>
              <option value="honda-dealer">Honda Dealer</option>
              <option value="private-seller">Private Seller</option>
              <option value="auction">Auction</option>
              <option value="lease-company">Lease Company</option>
            </select>
          </div>

          {/* Purchase Date, Purchase Price Row */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Purchase Date
              </label>
              <input
                type="date"
                value={financial.purchaseDate || today}
                onChange={e =>
                  handleInputChange("purchaseDate", e.target.value)
                }
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Purchase Price
              </label>
              <div className="relative">
                <span className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-500">
                  $
                </span>
                <input
                  type="number"
                  step="0.01"
                  value={financial.purchasePrice || ""}
                  onChange={e =>
                    handleInputChange(
                      "purchasePrice",
                      e.target.value ? parseFloat(e.target.value) : null,
                    )
                  }
                  placeholder="0.00"
                  className="w-full pl-8 pr-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>
            </div>
          </div>

          {/* Odometer */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Odometer
            </label>
            <div className="relative">
              <input
                type="number"
                value={financial.odometer || ""}
                onChange={e =>
                  handleInputChange(
                    "odometer",
                    e.target.value ? parseInt(e.target.value) : null,
                  )
                }
                placeholder="Odometer reading at purchase"
                className="w-full px-3 py-2 pr-12 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
              <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 text-sm">
                km
              </span>
            </div>
          </div>

          {/* Notes */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Notes
            </label>
            <textarea
              value={financial.notes}
              onChange={e => handleInputChange("notes", e.target.value)}
              placeholder="Additional purchase details or notes"
              rows={4}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent resize-vertical"
            />
          </div>
        </div>
      </div>

      {/* Loan/Lease  */}
      <div className="bg-white rounded-lg p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-6">Loan/Lease</h2>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {/* Loan */}
          <div className="border border-gray-200 rounded-lg p-4 hover:border-gray-300 transition-colors">
            <label className="flex flex-col items-center cursor-pointer">
              <input
                type="radio"
                name="loanLeaseType"
                value="loan"
                checked={financial.loanLeaseType === "loan"}
                onChange={e =>
                  handleInputChange("loanLeaseType", e.target.value as "loan")
                }
                className="mb-3 h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300"
              />
              <div className="text-center">
                <div className="font-medium text-gray-900 mb-1">Loan</div>
                <div className="text-sm text-gray-600">
                  This vehicle is associated with a loan
                </div>
              </div>
            </label>
          </div>

          {/* Lease */}
          <div className="border border-gray-200 rounded-lg p-4 hover:border-gray-300 transition-colors">
            <label className="flex flex-col items-center cursor-pointer">
              <input
                type="radio"
                name="loanLeaseType"
                value="lease"
                checked={financial.loanLeaseType === "lease"}
                onChange={e =>
                  handleInputChange("loanLeaseType", e.target.value as "lease")
                }
                className="mb-3 h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300"
              />
              <div className="text-center">
                <div className="font-medium text-gray-900 mb-1">Lease</div>
                <div className="text-sm text-gray-600">
                  This vehicle is being leased
                </div>
              </div>
            </label>
          </div>

          <div className="border border-gray-200 rounded-lg p-4 hover:border-gray-300 transition-colors bg-green-50">
            <label className="flex flex-col items-center cursor-pointer">
              <input
                type="radio"
                name="loanLeaseType"
                value="none"
                checked={financial.loanLeaseType === "none"}
                onChange={e =>
                  handleInputChange("loanLeaseType", e.target.value as "none")
                }
                className="mb-3 h-4 w-4 text-green-600 focus:ring-green-500 border-gray-300"
              />
              <div className="text-center">
                <div className="flex items-center justify-center mb-1">
                  <span className="w-2 h-2 bg-green-500 rounded-full mr-2"></span>
                  <span className="font-medium text-gray-900">None</span>
                </div>
                <div className="text-sm text-gray-600">
                  This vehicle is not being financed
                </div>
              </div>
            </label>
          </div>
        </div>

        {financial.loanLeaseType === "loan" && (
          <div className="mt-6 p-4 bg-blue-50 rounded-lg">
            <p className="text-sm text-blue-800">
              Loan details form would appear here when implemented.
            </p>
          </div>
        )}

        {financial.loanLeaseType === "lease" && (
          <div className="mt-6 p-4 bg-blue-50 rounded-lg">
            <p className="text-sm text-blue-800">
              Lease details form would appear here when implemented.
            </p>
          </div>
        )}
      </div>
    </div>
  );
};

export default VehicleFinancialForm;
