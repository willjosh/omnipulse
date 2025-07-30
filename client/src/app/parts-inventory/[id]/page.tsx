"use client";
import React, { useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { Edit, ArrowLeft, Package } from "lucide-react";
import { TabNavigation } from "@/components/ui/Tabs";
import { PrimaryButton, OptionButton } from "@/components/ui/Button";
import { Loading } from "@/components/ui/Feedback";
import { useInventoryItem } from "@/features/inventory-item/hooks/useInventoryItems";
import InventoryModal from "@/features/inventory-item/components/InventoryModal";

const InventoryItemDetailsPage = () => {
  const params = useParams();
  const router = useRouter();
  const [activeTab, setActiveTab] = useState("details");
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const inventoryItemId = parseInt(params.id as string);
  const { inventoryItem, isPending: isLoadingInventoryItem } =
    useInventoryItem(inventoryItemId);

  if (isLoadingInventoryItem) {
    return <Loading />;
  }

  if (!inventoryItem) {
    return (
      <div className="min-h-screen max-w-7xl shadow border-b border-gray-200 bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-xl font-semibold text-gray-900 mb-2">
            Inventory Item Not Found
          </h2>
          <p className="text-gray-600 mb-4">
            {"The inventory item you're looking for doesn't exist."}
          </p>
          <PrimaryButton onClick={() => router.push("/parts-inventory")}>
            Back to Parts Inventory
          </PrimaryButton>
        </div>
      </div>
    );
  }

  const tabs = [
    { key: "details", label: "Details", count: undefined },
    { key: "specifications", label: "Specifications", count: undefined },
    { key: "tracking", label: "Tracking", count: undefined },
  ];

  const handleEdit = () => {
    setIsEditModalOpen(true);
  };

  const handleBack = () => {
    router.push("/parts-inventory");
  };

  const formatCurrency = (value: number | null | undefined) => {
    if (value === null || value === undefined) return "—";
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
    }).format(value);
  };

  const formatWeight = (value: number | null | undefined) => {
    if (value === null || value === undefined) return "—";
    return `${value} kg`;
  };

  const renderDetailTab = () => (
    <div className="grid grid-cols-2 gap-6">
      <div className="bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-semibold text-gray-900">
              Basic Information
            </h2>
            <button className="text-sm text-gray-500">All Fields</button>
          </div>
        </div>
        <div className="p-3 space-y-2">
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Item Number
            </span>
            <span className="text-sm text-gray-900 font-mono">
              {inventoryItem.itemNumber}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Item Name</span>
            <span className="text-sm text-gray-900">
              {inventoryItem.itemName}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Description
            </span>
            <span className="text-sm text-gray-900">
              {inventoryItem.description || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Category</span>
            <span className="text-sm text-blue-600">
              {inventoryItem.categoryLabel || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Status</span>
            <div className="flex items-center">
              <div
                className={`w-2 h-2 rounded-full mr-2 ${inventoryItem.isActive ? "bg-green-500" : "bg-red-500"}`}
              ></div>
              <span className="text-sm text-gray-900">
                {inventoryItem.isActive ? "Active" : "Inactive"}
              </span>
            </div>
          </div>
        </div>
      </div>

      <div className="bg-white rounded-3xl border border-gray-200">
        <div className="p-4 border-b border-gray-200">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-semibold text-gray-900">
              Product Details
            </h2>
          </div>
        </div>
        <div className="p-3 space-y-2">
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Manufacturer
            </span>
            <span className="text-sm text-gray-900">
              {inventoryItem.manufacturer || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Manufacturer Part Number
            </span>
            <span className="text-sm text-gray-900 font-mono">
              {inventoryItem.manufacturerPartNumber || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">
              Universal Product Code
            </span>
            <span className="text-sm text-gray-900 font-mono">
              {inventoryItem.universalProductCode || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Supplier</span>
            <span className="text-sm text-gray-900">
              {inventoryItem.supplier || "—"}
            </span>
          </div>
          <div className="flex justify-between items-center py-3 border-b border-gray-100">
            <span className="text-sm font-medium text-gray-600">Weight</span>
            <span className="text-sm text-gray-900">
              {formatWeight(inventoryItem.weightKG)}
            </span>
          </div>
        </div>
      </div>
    </div>
  );

  const renderSpecificationTab = () => (
    <div className="bg-white rounded-3xl border border-gray-200">
      <div className="p-4 border-b border-gray-200">
        <h2 className="text-lg font-semibold text-gray-900">
          Cost Information
        </h2>
      </div>
      <div className="p-3 space-y-2">
        <div className="flex justify-between items-center py-3 border-b border-gray-100">
          <span className="text-sm font-medium text-gray-600">Unit Cost</span>
          <span className="text-sm text-gray-900 font-semibold">
            {formatCurrency(inventoryItem.unitCost)}
          </span>
        </div>
        <div className="flex justify-between items-center py-3 border-b border-gray-100">
          <span className="text-sm font-medium text-gray-600">
            Cost Measurement Unit
          </span>
          <span className="text-sm text-gray-900">
            {inventoryItem.unitCostMeasurementUnitLabel || "—"}
          </span>
        </div>
      </div>
    </div>
  );

  const renderTrackingTab = () => (
    <div className="bg-white rounded-3xl border border-gray-200 p-8 text-center">
      <p className="text-gray-500">Inventory tracking features coming soon</p>
    </div>
  );

  const renderTab = () => {
    switch (activeTab) {
      case "details":
        return renderDetailTab();
      case "specifications":
        return renderSpecificationTab();
      case "tracking":
        return renderTrackingTab();
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
              <span className="text-sm">Parts Inventory</span>
            </button>
          </div>
          <div className="flex items-start justify-between">
            <div className="flex items-start space-x-4">
              <div className="w-22 h-22 bg-gray-100 rounded-3xl flex items-center justify-center">
                <Package className="w-8 h-8 text-gray-600" />
              </div>
              <div>
                <h1 className="text-2xl font-bold text-gray-900 mb-1">
                  {inventoryItem.itemName}
                </h1>
                <p className="text-gray-600 mb-2">
                  {inventoryItem.itemNumber} •{" "}
                  {inventoryItem.categoryLabel || "No Category"} •{" "}
                  {inventoryItem.manufacturer || "No Manufacturer"}
                </p>
                <div className="flex items-center space-x-4 text-sm">
                  <span className="text-gray-600">
                    {formatCurrency(inventoryItem.unitCost)}
                  </span>
                  <div
                    className={`flex items-center px-2 py-1 rounded-full text-xs font-medium ${inventoryItem.isActive ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"}`}
                  >
                    <div
                      className={`w-1.5 h-1.5 rounded-full mr-1.5 ${inventoryItem.isActive ? "bg-green-500" : "bg-red-500"}`}
                    ></div>
                    {inventoryItem.isActive ? "Active" : "Inactive"}
                  </div>
                  {inventoryItem.supplier && (
                    <span className="text-gray-600">
                      {inventoryItem.supplier}
                    </span>
                  )}
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

      {/* Edit Modal */}
      <InventoryModal
        isOpen={isEditModalOpen}
        mode="edit"
        item={inventoryItem}
        onClose={() => setIsEditModalOpen(false)}
      />
    </div>
  );
};

export default InventoryItemDetailsPage;
