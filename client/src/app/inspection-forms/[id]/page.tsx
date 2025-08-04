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
import { Archive, Plus } from "lucide-react";

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
        description={inspectionForm.description || undefined}
        showDescription={true}
        breadcrumbs={breadcrumbs}
        actions={
          <PrimaryButton onClick={handleEdit}>
            <EditIcon /> Edit
          </PrimaryButton>
        }
      />

      <div className="px-6 pb-12 mt-4 max-w-6xl mx-auto">
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
                    <div className="flex-1 min-w-0 pr-4 overflow-hidden">
                      <div className="flex items-center gap-2 mb-2">
                        <span className="font-medium text-gray-900">
                          {item.itemLabel}
                          {item.isRequired && (
                            <span className="text-red-500 ml-1">*</span>
                          )}
                        </span>
                      </div>

                      <div className="text-sm text-gray-600 mb-2">
                        <div className="flex items-baseline mb-1">
                          <span className="text-xs font-medium text-gray-500 uppercase tracking-wide whitespace-nowrap flex-shrink-0 w-24">
                            Description:
                          </span>
                          <div className="ml-2 break-words overflow-hidden">
                            {item.itemDescription || "-"}
                          </div>
                        </div>
                        <div className="flex items-baseline">
                          <span className="text-xs font-medium text-gray-500 uppercase tracking-wide whitespace-nowrap flex-shrink-0 w-24">
                            Instructions:
                          </span>
                          <div className="ml-2 break-words overflow-hidden">
                            {item.itemInstructions || "-"}
                          </div>
                        </div>
                      </div>
                    </div>

                    <div className="flex items-center gap-2 flex-shrink-0">
                      <span className="bg-gray-100 text-gray-700 px-2 py-1 rounded-full text-xs">
                        {getInspectionFormItemTypeLabel(
                          item.inspectionFormItemTypeEnum,
                        )}
                      </span>
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
                          // TODO: Implement deactivate functionality
                          notify(
                            "Deactivate functionality not implemented yet",
                            "info",
                          );
                        }}
                        className="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                        title="Deactivate item"
                      >
                        <Archive size={16} />
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
