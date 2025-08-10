"use client";

import React, { useState } from "react";
import { useParams, useRouter } from "next/navigation";
import {
  useInspectionForm,
  useDeactivateInspectionForm,
} from "@/features/inspection-form/hooks/useInspectionForms";
import {
  useInspectionFormItems,
  useDeactivateInspectionFormItem,
} from "@/features/inspection-form/hooks/useInspectionFormItems";
import { PrimaryButton } from "@/components/ui/Button";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import { getInspectionFormItemTypeLabel } from "@/features/inspection-form/utils/inspectionFormEnumHelper";
import {
  Edit as EditIcon,
  Archive as ArchiveIcon,
} from "@/components/ui/Icons";
import { Plus, ArrowLeft } from "lucide-react";
import { ConfirmModal } from "@/components/ui/Modal";
import Loading from "@/components/ui/Feedback/Loading";

export default function InspectionFormDetailsPage() {
  const params = useParams();
  const router = useRouter();
  const notify = useNotification();
  const inspectionFormId = Number(params.id);

  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    inspectionForm: any | null;
  }>({ isOpen: false, inspectionForm: null });

  const [itemConfirmModal, setItemConfirmModal] = useState<{
    isOpen: boolean;
    item: any | null;
  }>({ isOpen: false, item: null });

  const { mutate: deactivateInspectionForm, isPending: isDeactivating } =
    useDeactivateInspectionForm();

  const {
    mutate: deactivateInspectionFormItem,
    isPending: isDeactivatingItem,
  } = useDeactivateInspectionFormItem();

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
    return <Loading />;
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

  const handleEdit = () => {
    router.push(`/inspection-forms/${inspectionFormId}/edit`);
  };

  const handleBack = () => {
    router.push("/inspection-forms");
  };

  const handleAddItem = () => {
    router.push(`/inspection-forms/${inspectionFormId}/items/new`);
  };

  const handleArchive = () => {
    setConfirmModal({ isOpen: true, inspectionForm });
  };

  const handleConfirmArchive = async () => {
    if (!confirmModal.inspectionForm) return;

    deactivateInspectionForm(confirmModal.inspectionForm.id, {
      onSuccess: () => {
        notify("Inspection form archived successfully", "success");
        setConfirmModal({ isOpen: false, inspectionForm: null });
        router.push("/inspection-forms");
      },
      onError: () => {
        notify("Failed to archive inspection form", "error");
      },
    });
  };

  const handleCancelArchive = () => {
    setConfirmModal({ isOpen: false, inspectionForm: null });
  };

  const handleDeactivateItem = (item: any) => {
    setItemConfirmModal({ isOpen: true, item });
  };

  const handleConfirmDeactivateItem = async () => {
    if (!itemConfirmModal.item) return;

    deactivateInspectionFormItem(
      { inspectionFormId, itemId: itemConfirmModal.item.id },
      {
        onSuccess: () => {
          notify("Inspection item deactivated successfully", "success");
          setItemConfirmModal({ isOpen: false, item: null });
        },
        onError: () => {
          notify("Failed to deactivate inspection item", "error");
        },
      },
    );
  };

  const handleCancelDeactivateItem = () => {
    setItemConfirmModal({ isOpen: false, item: null });
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="bg-white">
        <div className="px-6 py-4">
          <div className="flex items-center space-x-4 mb-4">
            <button
              onClick={handleBack}
              className="flex items-center text-gray-600 hover:text-blue-500"
            >
              <ArrowLeft className="w-4 h-4 mr-1" />
              <span className="text-sm">Inspection Forms</span>
            </button>
          </div>
          <div className="flex items-start justify-between">
            <div className="flex-1">
              <h1 className="text-2xl font-bold text-gray-900 mb-1">
                {inspectionForm.title}
              </h1>
              {inspectionForm.description && (
                <p className="text-gray-600 mb-2">
                  {inspectionForm.description}
                </p>
              )}
            </div>
            <div className="flex items-center space-x-3">
              <PrimaryButton
                onClick={handleArchive}
                disabled={isDeactivating}
                className="bg-red-600 hover:bg-red-700 text-white border-red-600"
              >
                <ArchiveIcon />
                Archive
              </PrimaryButton>
              <PrimaryButton onClick={handleEdit}>
                <EditIcon />
                Edit
              </PrimaryButton>
            </div>
          </div>
        </div>
      </div>

      <div className="px-6 mt-4 mb-8">
        {/* Inspection Items Section */}
        <div className="bg-white rounded-3xl border border-gray-200 p-6">
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
            </div>
          ) : (
            <div className="space-y-4">
              {inspectionFormItems?.map((item: any, index: number) => (
                <div
                  key={item.id}
                  className="border border-gray-200 rounded-3xl p-4 hover:border-gray-300 transition-colors"
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
                        onClick={() => handleDeactivateItem(item)}
                        className="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                        title="Deactivate item"
                      >
                        <ArchiveIcon />
                      </button>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>

      <ConfirmModal
        isOpen={confirmModal.isOpen}
        onClose={handleCancelArchive}
        title="Archive Inspection Form"
        message={`Are you sure you want to archive "${confirmModal.inspectionForm?.title}"? This action cannot be undone.`}
        confirmText={isDeactivating ? "Archiving..." : "Archive"}
        cancelText="Cancel"
        onConfirm={handleConfirmArchive}
      />

      <ConfirmModal
        isOpen={itemConfirmModal.isOpen}
        onClose={handleCancelDeactivateItem}
        title="Deactivate Inspection Item"
        message={`Are you sure you want to deactivate "${itemConfirmModal.item?.itemLabel}"? This action cannot be undone.`}
        confirmText={isDeactivatingItem ? "Deactivating..." : "Deactivate"}
        cancelText="Cancel"
        onConfirm={handleConfirmDeactivateItem}
      />
    </div>
  );
}
