"use client";

import React from "react";
import { useParams, useRouter } from "next/navigation";
import { useInspectionForm } from "@/features/inspection-form/hooks/useInspectionForms";
import { useInspectionFormItems } from "@/features/inspection-form/hooks/useInspectionFormItems";
import InspectionFormHeader from "@/features/inspection-form/components/InspectionFormHeader";
import { PrimaryButton } from "@/components/ui/Button";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import { getInspectionFormItemTypeLabel } from "@/features/inspection-form/utils/inspectionFormEnumHelper";
import { Edit as EditIcon } from "@/components/ui/Icons";
import { Trash2, Plus } from "lucide-react";

export default function InspectionFormDetailsPage() {
  const params = useParams();
  const router = useRouter();
  const notify = useNotification();
  const inspectionFormId = Number(params.id);

  const {
    inspectionForm,
    isPending: isFormLoading,
    isError: isFormError,
  } = useInspectionForm(inspectionFormId);

  const {
    inspectionFormItems,
    isPending: isItemsLoading,
    isError: isItemsError,
  } = useInspectionFormItems(inspectionFormId);

  if (isFormLoading || isItemsLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (isFormError || !inspectionForm) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <h2 className="text-2xl font-bold text-gray-900 mb-4">
            Form Not Found
          </h2>
          <p className="text-gray-600 mb-6">
            The inspection form you&apos;re looking for doesn&apos;t exist.
          </p>
          <PrimaryButton onClick={() => router.push("/inspection-forms")}>
            Back to Forms
          </PrimaryButton>
        </div>
      </div>
    );
  }

  const breadcrumbs = [
    { label: "Inspection Forms", href: "/inspection-forms" },
  ];

  const handleEdit = () => {
    router.push(`/inspection-forms/${inspectionFormId}/edit`);
  };

  const handleAddItem = () => {
    router.push(`/inspection-forms/${inspectionFormId}/items/new`);
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <InspectionFormHeader
        title={inspectionForm.title}
        breadcrumbs={breadcrumbs}
        actions={
          <PrimaryButton onClick={handleEdit}>
            <EditIcon /> Edit
          </PrimaryButton>
        }
      />

      <div className="px-6 pb-12 mt-4 max-w-6xl mx-auto">
        {/* Form Details Section */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-8">
          <h2 className="text-xl font-semibold text-gray-900 mb-4">
            Form Details
          </h2>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Title
              </label>
              <p className="text-gray-900">{inspectionForm.title}</p>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Status
              </label>
              <span
                className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                  inspectionForm.isActive
                    ? "bg-green-100 text-green-800"
                    : "bg-red-100 text-red-800"
                }`}
              >
                {inspectionForm.isActive ? "Active" : "Inactive"}
              </span>
            </div>

            {inspectionForm.description && (
              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Description
                </label>
                <p className="text-gray-900">{inspectionForm.description}</p>
              </div>
            )}

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Total Inspections
              </label>
              <p className="text-gray-900">{inspectionForm.inspectionCount}</p>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Checklist Items
              </label>
              <p className="text-gray-900">
                {inspectionForm.inspectionFormItemCount}
              </p>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Created
              </label>
              <p className="text-gray-900">
                {new Date(inspectionForm.createdAt).toLocaleDateString()}
              </p>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Last Updated
              </label>
              <p className="text-gray-900">
                {new Date(inspectionForm.updatedAt).toLocaleDateString()}
              </p>
            </div>
          </div>
        </div>

        {/* Inspection Items Section */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
          <div className="flex items-center justify-between mb-6">
            <div>
              <h2 className="text-xl font-semibold text-gray-900">
                Inspection Items
              </h2>
              <p className="text-sm text-gray-600 mt-1">
                {inspectionFormItems?.length || 0} items in this form
              </p>
            </div>
            <PrimaryButton onClick={handleAddItem}>
              <Plus size={16} />
              Add Item
            </PrimaryButton>
          </div>

          {isItemsError ? (
            <div className="text-center py-8">
              <p className="text-red-600">Failed to load inspection items</p>
            </div>
          ) : inspectionFormItems?.length === 0 ? (
            <div className="text-center py-8">
              <p className="text-gray-500 mb-4">No inspection items found</p>
              <div className="flex justify-center">
                <PrimaryButton onClick={handleAddItem}>
                  <Plus size={16} />
                  Add Item
                </PrimaryButton>
              </div>
            </div>
          ) : (
            <div className="space-y-4">
              {inspectionFormItems?.map((item: any, index: number) => (
                <div
                  key={item.id}
                  className="border border-gray-200 rounded-lg p-4 hover:border-gray-300 transition-colors"
                >
                  <div className="flex justify-between items-start">
                    <div className="flex-1">
                      <div className="flex items-center gap-2 mb-2">
                        <span className="font-medium text-gray-900">
                          {item.itemLabel}
                          {item.isRequired && (
                            <span className="text-red-500 ml-1">*</span>
                          )}
                        </span>
                        <div className="flex items-center gap-1 text-sm text-gray-600">
                          <span className="bg-gray-100 text-gray-700 px-2 py-1 rounded-full text-xs">
                            {getInspectionFormItemTypeLabel(
                              item.inspectionFormItemTypeEnum,
                            )}
                          </span>
                        </div>
                      </div>

                      {item.itemDescription && (
                        <p className="text-sm text-gray-600 mb-2">
                          {item.itemDescription}
                        </p>
                      )}

                      {item.itemInstructions && (
                        <div className="mb-2">
                          <span className="text-xs font-medium text-gray-500 uppercase tracking-wide">
                            Instructions:
                          </span>
                          <p className="text-sm text-gray-600 mt-1">
                            {item.itemInstructions}
                          </p>
                        </div>
                      )}

                      <div className="flex items-center gap-4 text-xs text-gray-500">
                        <span>Required: {item.isRequired ? "Yes" : "No"}</span>
                        <span>
                          Type:{" "}
                          {getInspectionFormItemTypeLabel(
                            item.inspectionFormItemTypeEnum,
                          )}
                        </span>
                        <span>ID: #{item.id}</span>
                      </div>
                    </div>

                    <div className="flex gap-2 ml-4">
                      <button
                        onClick={() =>
                          router.push(
                            `/inspection-forms/${inspectionFormId}/items/${item.id}/edit`,
                          )
                        }
                        className="p-2 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                        title="Edit item"
                      >
                        <EditIcon />
                      </button>
                      <button
                        onClick={() => {
                          // TODO: Implement delete functionality
                          notify(
                            "Delete functionality not implemented yet",
                            "info",
                          );
                        }}
                        className="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                        title="Delete item"
                      >
                        <Trash2 size={16} />
                      </button>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
